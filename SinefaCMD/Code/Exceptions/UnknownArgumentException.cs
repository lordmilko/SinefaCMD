using System;
using System.Runtime.Serialization;

namespace SinefaCMD
{
    [Serializable]
    public class UnknownArgumentException : Exception
    {
        public UnknownArgumentException()
        {
        }

        public UnknownArgumentException(string message) : base(message)
        {
        }

        public UnknownArgumentException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnknownArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
