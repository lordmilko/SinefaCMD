using System;
using System.Diagnostics;

namespace SinefaCMD
{
    [Serializable]
    public class OutputNotImplementedException : Exception
    {
        public OutputNotImplementedException() : base(GetMessage())
        {
        }

        private static string GetMessage()
        {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(2);
            var method = stackFrame.GetMethod();

            var message = $"'-{method.Name}' is not supported for output mode {method.DeclaringType.Name}";

            return message;
        }
    }
}
