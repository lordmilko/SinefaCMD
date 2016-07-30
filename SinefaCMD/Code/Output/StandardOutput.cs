using System.DirectoryServices;
using System.Linq;
using System.Net;
using Microsoft.Management.Infrastructure;

namespace SinefaCMD.Output
{
    abstract class StandardOutput
    {
        protected string GetMessage(Host host)
        {
            var hostname = GetHostname(host.IPAddress);
            string username = GetUsername(hostname);

            string text = username == null ?
                $"{hostname}! ({host.Down}mbps)" :
                $"{username}! ({hostname}: {host.Down}mbps)";

            return text;
        }

        protected string GetHostname(string ipAddress)
        {
            try
            {
                return Dns.GetHostEntry(ipAddress).HostName;
            }
            catch
            {
                return null;
            }
        }

        protected string GetUsername(string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
                return null;

            string rawUsername;

            try
            {
                var @namespace = @"root\cimv2";
                //var query = "select * from win32_computersystem";
                var query = "select * from win32_process where name = 'explorer.exe'"; //how do we filter?
                using (var session = CimSession.Create(hostname))
                {
                    var instance = session.QueryInstances(@namespace, "WQL", query).FirstOrDefault();
                    var result = session.InvokeMethod(instance, "GetOwner", null);
                    rawUsername = result.OutParameters["user"].Value.ToString();
                    //how do we call getowner() if we dont have a managementobject? we either need to get one or find a way to call methods
                    //rawUsername = instance.CimInstanceProperties["Username"].Value.ToString();
                }
            }
            catch
            {
                return null;
            }

            try
            {
                var entry = new DirectoryEntry();
                var adSearcher = new DirectorySearcher(entry)
                {
                    SearchScope = SearchScope.Subtree,
                    Filter = $"(&(objectClass=user)(samaccountname={rawUsername}))"
                };

                var userObject = adSearcher.FindOne();

                return $"{userObject.Properties["givenname"][0]} {userObject.Properties["sn"][0]}";
            }
            catch
            {
                return rawUsername;
            }
        }
    }
}
