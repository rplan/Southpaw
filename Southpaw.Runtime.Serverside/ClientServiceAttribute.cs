using System;

namespace Southpaw.Runtime.Serverside
{
    public class ClientServiceAttribute : Attribute
    {
        public ClientServiceAttribute(Type returnType)
        {
            ReturnType = returnType;
        }

        private Type _returnType;
        public Type ReturnType
        {
            get { return _returnType; }
            set { _returnType = value; }
        }
    }
}