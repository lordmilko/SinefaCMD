using System;
using System.Linq;
using SinefaCMD.Output;
using SinefaCMD.Sinefa;

namespace SinefaCMD
{
    class Settings
    {
        public string APIKey { get; private set; }

        public int? Probe { get; private set; }

        public OutputMode OutputMode { get; private set; } = OutputMode.Console;

        public string Account { get; private set; }

        public SinefaMode? Mode { get; private set; }

        public bool Help { get; private set; }

        public int? Duration { get; private set; }

        public string Target { get; private set; }

        public Settings(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-top":
                    case "-topup":
                    case "-topdown":
                    case "-listhosts":
                    case "-listprobes":
                        Mode = GetMode(args[i]);
                        break;

                    case "-apikey":
                        APIKey = GetArgument(i, args);
                        break;

                    case "-probe":
                        Probe = GetProbe(i, args);
                        break;

                    case "-output":
                        OutputMode = GetOutputMode(i, args);
                        break;

                    case "-account":
                        Account = GetArgument(i, args);
                        break;

                    case "-monitor":
                        Mode = GetMode(args[i]);
                        Target = GetArgument(i, args);
                        break;

                    case "-duration":
                        Duration = Convert.ToInt32(GetArgument(i, args));
                        break;

                    case "/?":
                    case "-?":
                    case "-help":
                    case "/help":
                        Help = true;
                        break;

                    default:
                        if (args[i].StartsWith("-"))
                            throw new UnknownArgumentException(args[i].ToLower()); //need to test this gets caught
                        break;
                }
            }

            if(!Help)
                ValidateSettings();
        }

        void ValidateSettings()
        {
            if (APIKey == null)
                throw new ArgumentMissingException("apikey");

            switch (Mode)
            {
                case SinefaMode.Top:
                case SinefaMode.TopDown:
                case SinefaMode.TopUp:
                    if (Probe == null)
                        throw new ArgumentMissingException("probe");
                    break;

                case SinefaMode.ListHosts:
                    if (Probe == null)
                        throw new ArgumentMissingException("probe");
                    break;

                case SinefaMode.Monitor:
                    if (Target == null)
                        throw new ArgumentMissingException("monitor");
                    if (Probe == null)
                        throw new ArgumentMissingException("probe");
                    if (Duration == null)
                        Duration = 5;
                    break;
            }
        }

        SinefaMode GetMode(string str)
        {
            var str1 = str.Substring(1);

            if (Mode != null & Mode.ToString().ToLower() != str1.ToLower())
                throw new ArgumentException($"Processing mode '-{Mode.ToString().ToLower()}' conflicts with mode '-{str1}'");

            return ((SinefaMode[])typeof(SinefaMode).GetEnumValues()).Where(e => e.ToString().ToLower() == str1.ToLower()).First();
        }

        int GetProbe(int index, string[] args)
        {
            var rawArg = GetArgument(index, args);
            int output;

            if (rawArg == null)
                throw new ArgumentMissingException("probe");

            if (Int32.TryParse(rawArg, out output))
                return output;
            else
                throw new ArgumentException($"'{rawArg}' is not a valid value for parameter '-probe'");
        }

        OutputMode GetOutputMode(int index, string[] args)
        {
            string rawMode = GetArgument(index, args);

            if (rawMode == null)
                throw new ArgumentMissingException("outputmode");

            var formattedMode = rawMode.First().ToString().ToUpper() + rawMode.Substring(1);

            if (Enum.IsDefined(typeof(OutputMode), formattedMode))
                return (OutputMode)Enum.Parse(typeof(OutputMode), formattedMode);
            else
                throw new ArgumentException($"'{rawMode}' is not a valid value for parameter '-mode'");
        }

        string GetArgument(int index, string[] args)
        {
            if (index < args.Length - 1 && !args[index + 1].StartsWith("-"))
            {
                if (args[index + 1] == string.Empty)
                    return null;

                return args[index + 1];
            }

            return null;
        }
    }
}