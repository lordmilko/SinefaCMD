using System;
using System.Linq;
using System.Text;
using SinefaCMD.Output;
using SinefaCMD.Sinefa;

namespace SinefaCMD
{
    class Program
    {
        static void Main(string[] args)
        {
            //todo - bugs
            //test monitoring a host not in sinefa
            //pass a whole bunch of arguments or lots of spaces as args
            //should we impose a limit on the number of monitor recursions
            //handle probenotfound and hostnotfound exceptions
            //all sinefaapi methods need to be able to handle a probe not being found or a host not being found
            //consoleoutput.monitor output needs to be formatted correctly
            //consoleoutput.gethosts (but also any other method that displays the hostname) needs to be able to truncate hostnames that are super long

            //todo - features
            //migrate username resolution to use management objects
            //implement and test il merge
            //make prtgoutput return the right number type

            try
            {
                if (args.Length == 0)
                {
                    ShowHelp();
                    return;
                }

                var settings = new Settings(args);

                if (settings.Help)
                    ShowHelp();
                else if (settings.Mode == null)
                    ShowError("ShowCMD was unable to find a valid processing mode");
                else
                    Run(settings);
            }
            catch (ArgumentMissingException ex)
            {
                ShowError($"A required parameter could not be found: '-{ex.Message}'");
            }
            catch (UnknownArgumentException ex)
            {
                ShowError($"Unknown parameter '{ex.Message}'");
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nA critical error occurred:");

                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        static void ShowError(string message)
        {
            Console.WriteLine($"\n{message}");
            Console.WriteLine("Please see 'SinefaCMD /?' for usage details.");
        }

        static void Run(Settings settings)
        {
            IOutput output = null;

            try
            {
                var api = new SinefaAPI(settings);
                output = GetOutput(settings.OutputMode);

                switch (settings.Mode)
                {
                    case SinefaMode.TopDown:
                        var hosts = api.GetHostBandwidth();
                        output.TopDown(hosts);
                        break;
                    case SinefaMode.ListProbes:
                        var probes = api.GetProbes();
                        output.ListProbes(probes);
                        break;
                    case SinefaMode.ListHosts:
                        var hostNames = api.GetHosts();
                        output.ListHosts(hostNames);
                        break;
                    case SinefaMode.Monitor:
                        output.Monitor(api.MonitorHost());
                        break;

                    default:
                        throw new NotImplementedException("the desired mode is not yet implemented");
                }
            }
            catch (Exception ex) when (output != null)
            {
                output.Error(ex);
            }
        }

        static IOutput GetOutput(OutputMode mode)
        {
            return (IOutput)Activator.CreateInstance(mode.GetAttributeValue<OutputTypeAttribute>().Type);
        }

        static void ShowHelp()
        {
            var outputModes = typeof(OutputMode).GetEnumNames().Select(m => m.ToLower()).ToList();

            var pipe = new StringBuilder();
            var comma = new StringBuilder();

            for (int i = 0; i < outputModes.Count; i++)
            {
                pipe.Append(outputModes[i]);
                comma.Append("'" + outputModes[i] + "'");

                if (i < outputModes.Count - 1)
                {
                    pipe.Append(" | ");
                    comma.Append(", ");
                }
            }

            Console.WriteLine("\nSinefaCMD.exe -APIKey <key> [-Account <accountID>] <modeArgs>\n    [-Output {" + pipe + "}]");

            Console.WriteLine("\nSinefaCMD.exe -Help | -? | /?");
            Console.WriteLine();

            Console.WriteLine("MODE ARGS");
            Console.WriteLine("    -Top[{Down | Up}] -Probe <probeNum>");
            Console.WriteLine("    -ListProbes");
            Console.WriteLine("    -ListHosts -Probe <probeNum>");
            Console.WriteLine("    -Monitor -Target <ipOrHostname> -Probe <probeNum> [-Duration <seconds>]");
            Console.WriteLine();

            Console.WriteLine("HELP");
            Console.WriteLine("    -APIKey     API Key of the Sinefa account to use for requests.");
            Console.WriteLine("                To locate your API Key navigate to https://app.sinefa.com,");
            Console.WriteLine("                click Settings then click Profile");
            Console.WriteLine();

            Console.WriteLine("    -Account    Account ID to make requests against. If you have multiple");
            Console.WriteLine("                accounts under your profile and this parameter is ommitted,");
            Console.WriteLine("                Sinefa will query whatever account was last selected in the ");
            Console.WriteLine("                web interface");
            Console.WriteLine();

            Console.WriteLine("    -Top        Display hostname and username of the top uploader and");
            Console.WriteLine("                downloader for a given site");
            Console.WriteLine();

            Console.WriteLine("    -TopDown    Display the hostname and username of the top downloader for a");
            Console.WriteLine("                given site");
            Console.WriteLine();

            Console.WriteLine("    -TopUp      Display the hostname and username of the top uploader for a");
            Console.WriteLine("                given site");
            Console.WriteLine();

            Console.WriteLine("    -Probe      The probe index to execute requests against. The probe index");
            Console.WriteLine("                corresponds to the 'bridge number' displayed on the probe name.");
            Console.WriteLine("                For help on listing all probes under a given account, please");
            Console.WriteLine("                see the help topic for the -ListProbes parameter");
            Console.WriteLine();

            Console.WriteLine("    -Output     Output mode to use for displaying results. If no output mode");
            Console.WriteLine("                is specified, program will default to using console.");
            Console.WriteLine($"                Valid output modes are {comma}");
            Console.WriteLine();

            Console.WriteLine("    -ListProbes List all probes under your account");
            Console.WriteLine();

            Console.WriteLine("    -ListHosts  List all hosts under a specified probe");
            Console.WriteLine();

            Console.WriteLine("    -Monitor    Monitor the uploads and downloads of a target for a specified");
            Console.WriteLine("                duration. If no duration is specified, defaults to 5 seconds.");
            Console.WriteLine();

            Console.WriteLine("    -Duration   Duration to perform an action for, in seconds");
            Console.WriteLine();

            Console.WriteLine("    -Help, /?   Show this help screen.");
            Console.WriteLine();

            Console.WriteLine("EXAMPLES");
            Console.WriteLine("    SinefaCMD -APIKey 1111-2222 -Account 1234 -TopDown -Probe 0 -Output prtg");
            Console.WriteLine("    SinefaCMD -APIKey 1111-2222 -Account 5678 -ListProbes");
            Console.WriteLine("    SinefaCMD -APIKey 1111-2222 -ListHosts -Probe 1");
        }   
    }
}
