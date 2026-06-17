using Timberborn.BlueprintSystem;

namespace FracturedAquifers
{
    public enum ChargeDistribution
    {
        BellCurve,
        SkewLeft,
        SkewRight,
        Bimodal,
        U
    }
    public record FractureChargeSpec : ComponentSpec
    {
        [Serialize]
        public float MinMultiplier { get; init; }

        [Serialize]
        public float MaxMultiplier { get; init; }

        [Serialize]
        public ChargeDistribution Distribution { get; init; }
    }
}
