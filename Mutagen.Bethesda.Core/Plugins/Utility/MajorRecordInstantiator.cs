using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mutagen.Bethesda.Plugins.Utility
{    
    /// <summary>
    /// A static class encapsulating the job of creating a new Major Record in a generic context
    /// </summary>
    /// <typeparam name="TMajor">
    /// Type of Major Record to instantiate.  Can be the direct class, or one of its interfaces.
    /// </typeparam>
    public static class MajorRecordInstantiator<TMajor>
        where TMajor : IMajorRecordGetter
    {
        /// <summary>
        /// Function to call to retrieve a new Major Record of type T
        /// </summary>
        public static readonly Func<FormKey, GameRelease, TMajor> Activator;

        static MajorRecordInstantiator()
        {
            if (!LoquiRegistration.TryGetRegister(typeof(TMajor), out var regis))
            {
                throw new ArgumentException();
            }

            var ctorInfo = regis.ClassType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(c =>
                {
                    var param = c.GetParameters();
                    if (param.Length != 2) return false;
                    if (param[0].ParameterType != typeof(FormKey)) return false;
                    if (param[1].ParameterType != typeof(GameRelease)) return false;
                    return true;
                })
                .First();
            var paramInfo = ctorInfo.GetParameters();
            ParameterExpression formKey = Expression.Parameter(typeof(FormKey), "formKey");
            ParameterExpression gameRelease = Expression.Parameter(typeof(GameRelease), "gameRelease");
            NewExpression newExp = Expression.New(ctorInfo, formKey, gameRelease);
            LambdaExpression lambda = Expression.Lambda(typeof(Func<FormKey, GameRelease, TMajor>), newExp, formKey, gameRelease);
            Activator = (Func<FormKey, GameRelease, TMajor>)lambda.Compile();
        }
    }
}
