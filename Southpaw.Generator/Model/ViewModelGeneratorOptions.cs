using System;
using System.Collections.Generic;

namespace Southpaw.Generator.Model
{
    public class ViewModelGeneratorOptions
    {
        public Tuple<string, string> NamespaceSubstitution { get; set; }
        public Dictionary<Type, string> ValidationAttributeMap { get; set; }
    }
}