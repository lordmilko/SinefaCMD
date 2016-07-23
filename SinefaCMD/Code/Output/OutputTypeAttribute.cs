using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinefaCMD.Output
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    sealed class OutputTypeAttribute : Attribute
    {
        public System.Type Type { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputType" /> class.
        /// </summary>
        /// <param name="type">A type that implements <see cref="IOutput"/> </param>
        public OutputTypeAttribute(System.Type type)
        {
            if (!typeof (IOutput).IsAssignableFrom(type))
                throw new ArgumentException($"{type.Name} does not implement interface IOutput", nameof(type));

            Type = type;
        }
    }
}
