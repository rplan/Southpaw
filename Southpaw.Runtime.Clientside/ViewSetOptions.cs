using System.Runtime.CompilerServices;

namespace Southpaw.Runtime.Clientside
{
    [Imported]
    public class ViewSetOptions
    {
        /// <summary>
        /// Events don't fire when IsSilent is true
        /// </summary>
        public bool IsSilent;
        /// <summary>
        /// Whether or not to perform validation
        /// </summary>
        public bool Validate;
    }
}