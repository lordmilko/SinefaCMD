﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Threading;
using System.ComponentModel;
using System.Net;
using System.Text.RegularExpressions;

namespace SinefaCMD.Sinefa
{
    class SinefaAPI
    {
        RestClient client = new RestClient("https://app.sinefa.com");

        private Settings settings;

        public SinefaAPI(Settings settings)
        {
            this.settings = settings;
        }

        public JObject GetLiveTraffic()
        {
            return GetLiveTraffic(0, 1).First();
        }

        //does the data actually even change each time?

        //test this works, also look at their email and see what sinefa had to say about how long you need to wait between each tick. if its in seconds, we need to change
        //interval to be seconds and multiply by 1000 when doing thread.sleep to make it milliseconds
        public IEnumerable<JObject> GetLiveTraffic(int interval, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                var id = GetRealtimeId();

                Thread.Sleep(1000);

                var request = CreateRequest($"{GetSinefaResourceUrl(SinefaResource.Realtime)}/{id}");
                var response = client.Execute(request);
                var j = JObject.Parse(response.Content);

                yield return j;

                Thread.Sleep(interval);
            }
        }

        public List<Host> GetHostBandwidth()
        {
            var hosts = GetLiveTraffic(1000, 5)
                .SelectMany(GetHosts)
                .GroupBy(g => g.IPAddress)
                .Select(
                    s => new Host
                    {
                        IPAddress = s.Key,
                        Down = Math.Round(s.Average(g => g.Down), 2),
                        Up = Math.Round(s.Average(g => g.Up), 2)
                    }
            ).OrderByDescending(host => host.Down).ToList();

            return hosts;
        }

        public List<Probe> GetProbes()
        {
            var data = GetLiveTraffic();

            var sources = data.Descendants("sources")
                .SelectMany(source => source.Children())
                .SelectMany(s => s.Children())
                .Select(s => new Probe
                {
                    Name = ((JProperty)s).Name,
                    Index = Convert.ToInt32(new Regex("(.+\\(br)(.+)(\\))").Replace(((JProperty)s).Name, "$2"))
                }
            );

            return sources.ToList();
        }

        public List<string> GetHosts()
        {
            var data = GetLiveTraffic();
            var hosts = GetHosts(data);

            return hosts.Select(h => h.IPAddress).ToList();
        }

        public List<Host> GetHosts(JObject data)
        {
            var sources = data.Descendants("sources");
            var probe = sources.SelectMany(
                source => source.Children().SelectMany(
                    probes => probes.Children().Where(
                        p => ((JProperty)p).Name.EndsWith($"(br{settings.Probe})")
                    )
                )
            ).FirstOrDefault();

            if (probe != null)
            {
                var hosts = probe.Children()["int-hosts"].Children().Select(host => new Host
                {
                    IPAddress = host["int-host"].ToString(),
                    Down = Math.Round(Convert.ToDouble(host["bytes-in"]) * 8 / (1024 * 1024), 2),
                    Up = Math.Round(Convert.ToDouble(host["bytes-out"]) * 8 / (1024 * 1024))
                }).OrderByDescending(host => host.Down).ToList();

                return hosts;
            }

            //sinefa recommend asking twice for probes in case they take a while to respond
            //also we need to handle the case where the probe is offline
            //also all the other methods need to handle this too - right now they just plow on right ahead
            throw new NotImplementedException("handling a probe not being found is not implemented"); //need to handle couldnt find our probe
        }

        public IEnumerable<Host> MonitorHost()
        {
            foreach (var result in GetLiveTraffic(1000, settings.Duration.Value))
            {
                var data = GetHosts(result);
                var host = Dns.GetHostEntry(settings.Target);

                var ourhost = data.FirstOrDefault(d => host.AddressList.Select(a => a.ToString()).Contains(d.IPAddress));

                if (ourhost == null)
                {
                    throw new NotImplementedException("host couldnt be found in sinefa");
                }
                else
                    yield return ourhost;
                
                //allow doing an ip address or a hostname - need to resolve either to a hostname 
            }
        }

        private string GetSinefaResourceUrl(SinefaResource resource)
        {
            return resource.GetAttributeValue<DescriptionAttribute>().Description;
        }

        private string GetRealtimeId()
        {
            var request = CreateRequest(GetSinefaResourceUrl(SinefaResource.Realtime));

            var response = client.Execute(request);
            var j = JObject.Parse(response.Content);
            var id = j["realtimeId"].ToString();

            return id;
        }

        RestRequest CreateRequest(string resource)
        {
            var request = new RestRequest(resource, Method.GET);
            request.AddHeader("x-api-key", settings.APIKey);

            if (settings.Account != null)
                request.AddParameter("account", settings.Account);

            return request;
        }
    }
}
