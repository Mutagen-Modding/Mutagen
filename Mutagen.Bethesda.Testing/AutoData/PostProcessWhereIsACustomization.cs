using AutoFixture;
using AutoFixture.Kernel;

namespace Mutagen.Bethesda.Testing.AutoData;

public class PostProcessWhereIsACustomization<T> : ICustomization
    where T : class
{
    readonly PostProcessSpecimensBehavior _behavior;

    public PostProcessWhereIsACustomization(Action<T> action)
    {
        _behavior = new PostProcessSpecimensBehavior(action);
    }

    void ICustomization.Customize(IFixture fixture)
    {
        fixture.Behaviors.Add(_behavior);
    }

    class PostProcessSpecimensBehavior : ISpecimenBuilderTransformation
    {
        readonly Action<object> _applyActionIfSpecimenIsOfTypeT;

        public PostProcessSpecimensBehavior(Action<T> action)
        {
            _applyActionIfSpecimenIsOfTypeT = x =>
            {
                var asT = x as T;
                if (asT != null)
                    action(asT);
            };
        }

        ISpecimenBuilderNode ISpecimenBuilderTransformation.Transform(ISpecimenBuilder builder)
        {
            return new SpecimenBuilderPostProcessor(builder, _applyActionIfSpecimenIsOfTypeT);
        }

        class SpecimenBuilderPostProcessor : ISpecimenBuilderNode
        {
            readonly ISpecimenBuilder _builder;
            readonly Action<object> _postProcess;

            public SpecimenBuilderPostProcessor(ISpecimenBuilder builder, Action<object> initializationAction)
            {
                _postProcess = initializationAction;
                _builder = builder;
            }

            object ISpecimenBuilder.Create(object request, ISpecimenContext context)
            {
                var specimen = _builder.Create(request, context);
                _postProcess(specimen);
                return specimen;
            }

            ISpecimenBuilderNode ISpecimenBuilderNode.Compose(
                System.Collections.Generic.IEnumerable<ISpecimenBuilder> builders)
            {
                return new SpecimenBuilderPostProcessor(new CompositeSpecimenBuilder(builders), _postProcess);
            }

            System.Collections.Generic.IEnumerator<ISpecimenBuilder>
                System.Collections.Generic.IEnumerable<ISpecimenBuilder>.GetEnumerator()
            {
                yield return _builder;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                yield return _builder;
            }
        }
    }
}