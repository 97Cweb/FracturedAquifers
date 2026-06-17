using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace FracturedAquifers
{
    public static class SourceLocator
    {
        public static readonly HashSet<string> AllowedSourceTemplates = new()
        {
            "Aquifer",
            "WaterSource",
            "WaterSeep",
            "BadwaterSource",
            "BadwaterSeep"
        };

        public static readonly HashSet<string> WaterSourceOnly = new()
        {
            "WaterSource"
        };

        public static bool HasAllowedSourceAtFoundation(
            IBlockService blockService,
            Block foundationBlock)
        {
            return HasTemplateAtFoundation(
                blockService,
                foundationBlock,
                AllowedSourceTemplates);
        }

        public static bool HasWaterSourceAtFoundation(
            IBlockService blockService,
            Block foundationBlock)
        {
            return HasTemplateAtFoundation(
                blockService,
                foundationBlock,
                WaterSourceOnly);
        }

        private static bool HasTemplateAtFoundation(
            IBlockService blockService,
            Block foundationBlock,
            HashSet<string> allowedTemplates)
        {
            Vector3Int c = foundationBlock.Coordinates;

            return HasTemplateAt(blockService, c, allowedTemplates)
                   || HasTemplateAt(blockService, c + Vector3Int.back, allowedTemplates);
        }

        private static bool HasTemplateAt(
            IBlockService blockService,
            Vector3Int coordinates,
            HashSet<string> allowedTemplates)
        {
            foreach (BlockObject obj in blockService.GetObjectsAt(coordinates))
            {
                TemplateSpec template = obj.GetComponent<TemplateSpec>();

                if (template == null)
                {
                    continue;
                }

                foreach (string allowed in allowedTemplates)
                {
                    if (template.IsNamed(allowed))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public static BlockObject FindAllowedSourceNearBlockObject(
            IBlockService blockService,
            BlockObject blockObject)
        {
            foreach (Block block in blockObject.PositionedBlocks.GetFoundationBlocks())
            {
                BlockObject source = FindTemplateAtFoundation(
                    blockService,
                    block,
                    AllowedSourceTemplates);

                if (source != null)
                {
                    return source;
                }
            }

            return null;
        }

        private static BlockObject FindTemplateAtFoundation(
            IBlockService blockService,
            Block foundationBlock,
            HashSet<string> allowedTemplates)
        {
            Vector3Int c = foundationBlock.Coordinates;

            Vector3Int[] coordinatesToCheck =
            {
                c,
                c + Vector3Int.back
            };

            foreach (Vector3Int coordinates in coordinatesToCheck)
            {
                foreach (BlockObject obj in blockService.GetObjectsAt(coordinates))
                {
                    TemplateSpec template = obj.GetComponent<TemplateSpec>();

                    if (template == null)
                    {
                        continue;
                    }

                    foreach (string allowed in allowedTemplates)
                    {
                        if (template.IsNamed(allowed))
                        {
                            return obj;
                        }
                    }
                }
            }

            return null;
        }
    }
}
