using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace FracturedAquifers
{
    public class FractureChargePlacementValidator : IBlockObjectValidator
    {
        private static readonly HashSet<string> AllowedTemplates = new()
        {
            "Aquifer",
            "WaterSource",
            "WaterSeep",
            "BadwaterSource",
            "BadwaterSeep"
        };

        private readonly IBlockService _blockService;

        public FractureChargePlacementValidator(IBlockService blockService)
        {
            _blockService = blockService;
        }

        public bool IsValid(BlockObject blockObject, out string errorMessage)
        {
            if (blockObject.GetComponent<FractureChargeSpec>() == null)
            {
                errorMessage = null;
                return true;
            }

            foreach (Block block in blockObject.PositionedBlocks.GetFoundationBlocks())
            {
                Debug.Log("[FracturedAquifers] Checking coord " + block.Coordinates);
                if (HasAllowedSourceAt(block.Coordinates))
                {
                    errorMessage = null;
                    return true;
                }

                if (HasAllowedSourceAt(block.Coordinates + Vector3Int.back))
                {
                    errorMessage = null;
                    return true;
                }
            }

            errorMessage = "Must be built on a water source, seep, badwater source, badwater seep, or aquifer.";
            return false;
        }

        private bool HasAllowedSourceAt(Vector3Int coordinates)
        {
            foreach (BlockObject obj in _blockService.GetObjectsAt(coordinates))
            {
                TemplateSpec template = obj.GetComponent<TemplateSpec>();

                if (template == null)
                {
                    continue;
                }

                foreach (string allowed in AllowedTemplates)
                {
                    if (template.IsNamed(allowed))
                    {
                        Debug.Log("[FracturedAquifers] Found " + template.TemplateName + " at " + coordinates);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
