using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace FracturedAquifers
{
    public class FractureChargePlacementValidator : IBlockObjectValidator
    {

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
                if (SourceLocator.HasAllowedSourceAtFoundation(_blockService, block))
                {
                    errorMessage = null;
                    return true;
                }
            }

            errorMessage = "Must be built on a water source, seep, badwater source, badwater seep, or aquifer.";
            return false;
        }
    }
}
