using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
