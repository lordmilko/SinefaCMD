using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SinefaCMD
{
    [Serializable]
    public class ArgumentMissingException : Exception
    {
        public ArgumentMissingException()
        {
        }

        public ArgumentMissingException(string message) : base(message)
        {
        }

        public ArgumentMissingException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ArgumentMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
