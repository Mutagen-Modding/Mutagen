using Loqui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Mutagen.Bethesda.Internals
{    
    /// <summary>
    /// A static class encapsulating the job of creating a new Major Record in a generic context
    /// </summary>
    /// <typeparam name="TMajor">
    /// Type of Major Record to instantiate.  Can be the direct class, or one of its interfaces.
    /// </typeparam>
    public static class MajorRecordInstantiator<TMajor>
        where TMajor : IMajorRecordCommon
    {
        /// <summary>
        /// Function to call to retrieve a new Major Record of type T
        /// </summary>
        public static readonly MajorRecordActivator Activator;

        /// <summary>
        /// Constructs a new Major Record of type T with the given FormKey
        /// </summary>
        /// <param name="formKey">FormKey to give the new Major Record</param>
        /// <returns>New Major Record of type T with given FormKey</returns>
        public delegate TMajor MajorRecordActivator(FormKey formKey);

        static MajorRecordInstantiator()
        {
            if (!LoquiRegistration.TryGetRegister(typeof(TMajor), out var regis))
            {
                throw new ArgumentException();
            }

            var ctorInfo = regis.ClassType.GetConstructors()
                .Where(c => c.GetParameters().Length == 1)
                .Where(c => c.GetParameters()[0].ParameterType == typeof(FormKey))
                .First();
            var paramInfo = ctorInfo.GetParameters();
            ParameterExpression param = Expression.Parameter(typeof(FormKey), "formKey");
            NewExpression newExp = Expression.New(ctorInfo, param);
            LambdaExpression lambda = Expression.Lambda(typeof(MajorRecordActivator), newExp, param);
            Activator = (MajorRecordActivator)lambda.Compile();
        }
    }
}
