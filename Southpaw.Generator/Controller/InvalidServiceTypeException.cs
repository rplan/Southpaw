using System;
using System.Runtime.Serialization;

namespace Southpaw.Generator.Controller
{
    [Serializable]
    public class InvalidServiceTypeException : Exception
    {
        public InvalidServiceTypeException()
        {
        }

        public InvalidServiceTypeException(string message) : base(message)
        {
        }

        public InvalidServiceTypeException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidServiceTypeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}