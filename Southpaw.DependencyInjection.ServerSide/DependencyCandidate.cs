using System;
using System.Collections.Generic;

namespace Southpaw.DependencyInjection.ServerSide
{
    class DependencyCandidate
    {
        public Dictionary<string, string> Variables { get; set; }
        public string TypeName { get; set; }
    }
}