using System.Reflection;
using HarmonyLib;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.TemplateSystem;
using Random = UnityEngine.Random;
using UnityEngine;

namespace FracturedAquifers
{
    [HarmonyPatch]
    public static class FractureChargeDetonationPatch
    {
        private static MethodBase TargetMethod()
        {
            var dynamiteType = AccessTools.TypeByName("Timberborn.Explosions.Dynamite");
            return AccessTools.DeclaredMethod(dynamiteType, "Detonate");
        }

        private static bool Prefix(object __instance)
        {
            var blockObject = Traverse.Create(__instance)
                .Field("_blockObject")
                .GetValue<BlockObject>();

            if (blockObject == null)
            {
                Debug.Log("[FracturedAquifers] Detonate patch: no blockObject");
                return true;
            }

            if (blockObject.GetComponent<FractureChargeSpec>() == null)
            {
                return true;  //normal dynamite
            }

            Debug.Log("[FracturedAquifers] Fracture Charge detonated at " + blockObject.Coordinates);


            var blockService = Traverse.Create(__instance)
                .Field("_blockService")
                .GetValue<IBlockService>();

            BlockObject source = SourceLocator.FindAllowedSourceNearBlockObject(
                blockService,
                blockObject);

            if (source == null)
            {
                Debug.LogWarning("[FracturedAquifers] No source found near detonated Fracture Charge.");
            }
            else
            {
                TryModifySourceStrength(source, blockObject);
            }


            var entityService = Traverse.Create(__instance)
                .Field("_entityService")
                .GetValue<Timberborn.EntitySystem.EntityService>();
            
            Traverse.Create(__instance)
              .Method("PlayEffects")
              .GetValue();

            DisableDeconstruction(__instance);

            entityService.Delete((BaseComponent)__instance);

            return false;
        }

        private static void TryModifySourceStrength(BlockObject source, BlockObject charge)
        {
            object waterSource = null;

            foreach (var component in source.AllComponents)
            {
                if (component.GetType().FullName == "Timberborn.WaterSourceSystem.WaterSource")
                {
                    waterSource = component;
                    break;
                }
            }

            var template = source.GetComponent<TemplateSpec>();

            if (waterSource == null)
            {
                Debug.LogWarning("[FracturedAquifers] " + template?.TemplateName + " has no WaterSource component yet.");
                return;
            }

            float oldStrength = Traverse.Create(waterSource)
                .Property("SpecifiedStrength")
                .GetValue<float>();

            var chargeSpec = charge.GetComponent<FractureChargeSpec>();

            float min = chargeSpec.MinMultiplier;
            float max = chargeSpec.MaxMultiplier;
            float multiplier = 1.0f;

            switch(chargeSpec.Distribution)
            {
                case ChargeDistribution.BellCurve:
                    multiplier = BellCurve(min,max);
                    break;

                case ChargeDistribution.SkewLeft:
                    multiplier = SkewLeft(min,max);
                    break;

                case ChargeDistribution.SkewRight:
                    multiplier = SkewRight(min,max);
                    break;
                case ChargeDistribution.Bimodal:
                    multiplier = Bimodal(min,max);
                    break;
                case ChargeDistribution.U:
                    multiplier = U(min,max);
                    break;
            }
            float newStrength = oldStrength * multiplier;

            Traverse.Create(waterSource)
                .Method("SetSpecifiedStrength", newStrength)
                .GetValue();

            Debug.Log($"[FracturedAquifers] {template?.TemplateName} strength {oldStrength} -> {newStrength} x{multiplier}");
        }

        private static void DisableDeconstruction(object dynamite)
        {
            var baseComponent = (BaseComponent)dynamite;

            foreach (var component in baseComponent.AllComponents)
            {
                if (component.GetType().Name == "Deconstructible")
                {
                    Traverse.Create(component)
                        .Method("DisableDeconstruction")
                        .GetValue();

                    Debug.Log("[FracturedAquifers] Disabled deconstruction recovery.");
                    return;
                }
            }

            Debug.LogWarning("[FracturedAquifers] No Deconstructible component found.");
        }

        private static float BellCurve(float min, float max)
        {
            float t = (Random.value + Random.value) / 2f;
            return Mathf.Lerp(min, max, t);
        }

        private static float SkewLeft(float min, float max)
        {
            float t = Random.value;
            t = t * t;
            return Mathf.Lerp(min, max, t);
        }

        private static float SkewRight(float min, float max)
        {
            float t = Random.value;
            t = 1f - (t * t);
            return Mathf.Lerp(min, max, t);
        }

        private static float Bimodal(float min, float max)
        {
            float centre = (min + max)/2f;
            float spread = (max - min);
            float minChange = 0.1f*spread;

            if(Random.value < 0.5f)
            {
                return BellCurve(min, centre-minChange);
            }
            return BellCurve(centre + minChange, max);
        }
        private static float U(float min, float max)
        {
            float t = Random.value;

            if (t < 0.5f)
                t = t * t;
            else
                t = 1f - (1f - t) * (1f - t);

            return Mathf.Lerp(min,max,t);
        }
    }
}
