using System;
using System.Runtime.CompilerServices;

namespace Southpaw.DependencyInjection.ClientSide
{
    [Imported]
    [NonScriptable]
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class DependencyDefinitionAttribute : Attribute
    {
        public DependencyDefinitionAttribute() { }

        public DependencyDefinitionAttribute(Type t)
        {
            ForType = t;
        }
        public DependencyDefinitionAttribute(Type t, string variables)
        {
            ForType = t;
            Variables = variables;
        }
        /// <summary>
        /// The type for which the dependency is specified - usually, an interface
        /// </summary>
        /// <note>Read-only due to how this property is retrieved to generate the dependency config</note>
        public Type ForType { get; private set; }
        /// <summary>
        /// A query string-formatted specifications of the variables under which class
        /// should be returned as a dependency when <see cref="ForType"/> is specified
        /// </summary>
        /// <note>Read-only due to how this property is retrieved to generate the dependency config</note>
        public string Variables { get; private set; }
    }

    [Imported]
    public class Dependencies
    {
        public T GetInstance<T>()
        {
            return default(T);
        }
    }
}
