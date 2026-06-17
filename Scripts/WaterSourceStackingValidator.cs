using Timberborn.BlockSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace FracturedAquifers
{
    public class WaterSourceStackingValidator : IBlockObjectValidator
    {
        private readonly IBlockService _blockService;

        public WaterSourceStackingValidator(IBlockService blockService)
        {
            _blockService = blockService;
        }

        public bool IsValid(BlockObject blockObject, out string errorMessage)
        {
            if (blockObject.GetComponent<FractureChargeSpec>() != null)
            {
                errorMessage = null;
                return true;
            }

            TemplateSpec template = blockObject.GetComponent<TemplateSpec>();

            if (template != null &&
                (template.IsNamed("Rubble") || template.IsNamed("RecoveredGoodStack")))
            {
                errorMessage = null;
                return true;
            }

            foreach (Block block in blockObject.PositionedBlocks.GetFoundationBlocks())
            {
                if (SourceLocator.HasWaterSourceAtFoundation(_blockService, block))
                {
                    errorMessage = "Only Fracture Charges can be built on water sources.";
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }
    }
}
