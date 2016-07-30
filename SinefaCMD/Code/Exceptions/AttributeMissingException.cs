using System;
using System.Runtime.Serialization;

namespace SinefaCMD
{
    [Serializable]
    public class AttributeMissingException : Exception
    {
        public AttributeMissingException()
        {
        }

        public AttributeMissingException(string message) : base(message)
        {
        }

        public AttributeMissingException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AttributeMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
