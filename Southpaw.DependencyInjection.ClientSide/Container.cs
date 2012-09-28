using System;
using System.Runtime.CompilerServices;

namespace Southpaw.DependencyInjection.ClientSide
{
    [Imported]
    [ScriptName("$DI")]
    public class Container
    {
        [IgnoreGenericArguments]
        [InlineCode("$DI.g({T}.get_fullName())")]
        public T Get<T>()
            where T : class
        {
            return null;
        }

        [IgnoreGenericArguments]
        [InlineCode("$DI.g({T}.get_fullName(), {*constructorParams})")]
        public T Get<T>(params object[] constructorParams)
            where T : class
        {
            return null;
        }

        [InlineCode("$DI.r({TTarget}.get_fullName(), {TImplementation})")]
        public void Register<TTarget, TImplementation>() { }
    }
}
