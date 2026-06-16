using Bindito.Core;
using Timberborn.BlockSystem;
using Timberborn.TemplateInstantiation;

namespace FracturedAquifers
{
    [Context("Game")]
    public class FracturedAquifersConfigurator : Configurator
    {
        protected override void Configure()
        {
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
