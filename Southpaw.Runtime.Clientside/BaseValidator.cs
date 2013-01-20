using System.Runtime.CompilerServices;

namespace Southpaw.Runtime.Clientside.Validation
{
    public class BaseValidator
    {
        /// <summary>
        /// The main Validation method.
        /// </summary>
        /// <param name="o">the object to validate</param>
        /// <param name="parameters">validator-specific parameters</param>
        /// <returns>the error message string, or null if the object is valid</returns>
        public virtual string Validate(object o, ValidatorOptions parameters)
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
    public class RangeValidator : BaseValidator { }

    [Imported(IsRealType = false)]
    public class RegexValidatorOptions : ValidatorOptions
    {
        [IntrinsicProperty]
        public string Pattern { get; set; }     
    }

    [Imported(IsRealType = true)]
    public class RegexValidator : BaseValidator { }

    [Imported(IsRealType = false)]
    public class RequiredValidatorOptions : ValidatorOptions
    {
        [IntrinsicProperty]
        public bool AllowEmptyStrings { get; set; }     
    }

    [Imported(IsRealType = true)]
    public class RequiredValidator : BaseValidator { }

    [Imported(IsRealType = false)]
    public class LengthValidatorOptions : ValidatorOptions
    {
        [IntrinsicProperty]
        public int MinimumLength { get; set; }     
        [IntrinsicProperty]
        public int MaximumLength { get; set; }     
    }

    [Imported(IsRealType = true)]
    public class LengthValidator : BaseValidator { }
}

namespace Southpaw.Runtime.Clientside.Validation.Type
{
    [Imported(IsRealType = false)]
    public class TypeValidatorOptions : ValidatorOptions { }
    [Imported(IsRealType = true)]
    public class IntValidator : BaseValidator { }
    [Imported(IsRealType = true)]
    public class DateValidator : BaseValidator { }
    [Imported(IsRealType = true)]
    public class FloatValidator : BaseValidator { }
    [Imported(IsRealType = true)]
    public class BoolValidator : BaseValidator { }
    [Imported(IsRealType = true)]
    public class CharValidator : BaseValidator { }
}