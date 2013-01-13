using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Southpaw.Runtime.Clientside.Validation
{
    public class BaseValidator
    {
        public string Validate(object o, ValidatorOptions parameters)
        {
            return null;
        }
    }

    [Imported(IsRealType = false)]
    public class ValidatorOptions
    {
        [IntrinsicProperty]
        public string Property { get; set; }     
        [IntrinsicProperty]
        public string ErrorMessage { get; set; }     
    }

    [Imported(IsRealType = false)]
    public class RangeValidatorOptions : ValidatorOptions
    {
        [IntrinsicProperty]
        public int Minimum { get; set; }     
        [IntrinsicProperty]
        public int Maximum { get; set; }     
    }

    [Imported(IsRealType = true)]
    public class RangeValidator : BaseValidator
    {
    }

    [Imported(IsRealType = false)]
    public class RegexValidatorOptions : ValidatorOptions
    {
        [IntrinsicProperty]
        public Regex Pattern { get; set; }     
    }

    [Imported(IsRealType = true)]
    public class RegexValidator : BaseValidator
    {
    }

    [Imported(IsRealType = false)]
    public class RequiredValidatorOptions : ValidatorOptions
    {
        [IntrinsicProperty]
        public bool AllowEmptyStrings { get; set; }     
    }

    [Imported(IsRealType = true)]
    public class RequiredValidator : BaseValidator
    {
    }

    [Imported(IsRealType = false)]
    public class LengthValidatorOptions : ValidatorOptions
    {
        [IntrinsicProperty]
        public int MinimumLength { get; set; }     
        [IntrinsicProperty]
        public int MaximumLength { get; set; }     
    }

    [Imported(IsRealType = true)]
    public class LengthValidator : BaseValidator
    {
    }
}