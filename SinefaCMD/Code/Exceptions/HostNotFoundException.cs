using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SinefaCMD
{
    [Serializable]
    public class HostNotFoundException : Exception
    {
        public HostNotFoundException()
        {
        }

        public HostNotFoundException(string message) : base(message)
        {
        }

        public HostNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected HostNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
