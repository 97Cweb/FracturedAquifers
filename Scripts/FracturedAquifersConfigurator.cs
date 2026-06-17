using Bindito.Core;

using HarmonyLib;

using UnityEngine;

using Timberborn.BlockSystem;
using Timberborn.TemplateInstantiation;

namespace FracturedAquifers
{
    [Context("Game")]
    public class FracturedAquifersConfigurator : Configurator
    {
        protected override void Configure()
        {

            var harmony = new Harmony("97cweb.FracturedAquifers");
            harmony.PatchAll();

            Debug.Log("[FracturedAquifers] Harmony PatchAll called.");


            MultiBind<IBlockObjectValidator>()
                .To<FractureChargePlacementValidator>()
                .AsSingleton();

            MultiBind<IBlockObjectValidator>()
                .To<WaterSourceStackingValidator>()
                .AsSingleton();

            MultiBind<TemplateModule>()
                .ToProvider(ProvideTemplateModule)
                .AsSingleton(); 
        }

        private static TemplateModule ProvideTemplateModule()
        {
            var builder = new TemplateModule.Builder();

            builder.AddDecorator<FractureChargeSpec, FractureChargeSpec>();

            return builder.Build();
        }

        
    }
}
