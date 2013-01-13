using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Southpaw.Runtime.Clientside
{
    [Imported(IsRealType = true)]
    public class ValidationResults
    {
        [IntrinsicProperty]
        public bool IsError { get { return false; } }
        [IntrinsicProperty]
        public JsDictionary<string, List<string>> ErrorsByProperty { get; set; }
        [IntrinsicProperty]
        public List<string> ErrorMessages { get; set; }
        public ValidationResults AddError(string propertyName, string errorMessage)
        {
            return this;
        }
        public ValidationResults Clear()
        {
            return this;
        }
    }
}
