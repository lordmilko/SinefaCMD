# SinefaCMD

SinefaCMD is a command line interface for interacting with the Sinefa REST API.

SinefaCMD currently supports a number of different functions based on the `api/v1/reports/realtime` resource. Additional resources can be implemented by extending the `SinefaAPI` class and `SinefaResource` enumeration (see below).

By default, SinefaCMD will output text to the console. Additional output methods can be specified however, allowing you to pipe output to a monitoring system (see below). SinefaCMD currently supports the following output modes:
* Console
* PRTG Network Monitor

SinefaCMD utilizes the RestSharp and Newtonsoft.Json NuGet packages. These packages will automatically be installed when you first compile the project.

## Usage

There are three pieces of information you need to know to use SinefaCMD
* Your API Key
* Your Account ID
* Your Probe ID

Your API Key can be found by logging into https://app.sinefa.com and navigating to Settings -> Profile. It is recommended by set up a separate account in your organization dedicated to making API requests.

Your Account ID can be identified by logging into https://app.sinefa.com, selecting your organization name in the top right hand corner, navigating to **Accounts** and observing the five digit number listed next to your account name. If you do not specify an account with the `-account` switch, SinefaCMD will use whatever account was last 'active' in the Sinefa Web Portal.

The Probe ID corresponds to **br** number of the probe in your account. A full list of probes can be enumerated with SinefaCMD by utilizing the `-listprobes` switch. See `SinefaCMD.exe /?` for usage details.

## Usage Examples

### Top Downloader

```
sinefacmd -apikey 1234-5678 -account 1234 -topdown -probe 1

    IP Address: 192.168.0.1
    Hostname  : dc-1
    Username  : administrator
    Bandwidth : 4.12mbps
```

### Top Downloader (PRTG)

```xml
sinefacmd -apikey 1234-5678 -account 1234 -topdown -probe 1 -output prtg

<prtg>
  <text>test-pc! (7.49mbps)</text>
  <result>
    <channel>Bandwidth</channel>
    <value>7.49</value>
    <float>1</float>
    <Unit>SpeedNet</Unit>
    <VolumeSize>MegaBit</VolumeSize>
  </result>
</prtg>
```

### List Hosts

```
sinefacmd -apikey 1234-5678 -account 1234 -listhosts -probe 0

    IP Address       Hostname                  Username
    ---------------- ------------------------- ---------------
    192.168.0.1      dc-1                      Administrator
    192.168.0.2      exch-1                    exchange.admin
```

### Monitor Host

```
sinefacmd -apikey 1234-5678 -account 1234 -monitor 192.168.0.1 -probe 1 -duration 3

    IP Address       Download Upload
    ---------------- -------- ------
    192.168.0.1      8.11     0.1
    192.168.0.1      5.03     0.3
    192.168.0.1      6.01     1.0
```

A full list of supported commands and usage details can be found by running `SinefaCMD /?`

## Extensibility

### Output

Custom output modes can be specified by performing the following steps:

1. Create a new type under Code/Output/Type that implements `IOutput`
2. Add the new type to the `OutputMode` enumeration
3. Implement all `IOutput` methods usable (or that you are interested in using) with your monitoring system

#### Step 1: Implement IOutput

`IOutput` defines all output actions known to SinefaCMD. Implementors are not required to fully implement the methods defined in this interface, as depending on your output method many of these methods may simply be unworkable (such as `Monitor`).

Custom output types are organized in the Code/Output/Type folder, under the `SinefaCMD.Output.Type` namespace. If you are interested in performing 'common' actions such as resolving a hosts IP Address to a hostname, or identifying the username of a given system, your type can also inherit from the `StandardOutput` class. Both `ConsoleOutput` and `PrtgOutput` provide examples of this behaviour

#### Step 2: Extended OutputMode

When an output mode is specified as an argument to SinefaCMD, the argument is checked against the members of the `OutputMode` enumeration. `OutputMode` fields include an `OutputTypeDescription`, specifying the type of object that should be created for displaying the output.

```c#
enum OutputMode
{
    [OutputType(typeof(ConsoleOutput))]
    Console,

    [OutputType(typeof(PrtgOutput))]
    Prtg
}
```

#### Step 3: Implement IOutput

As noted in Step 1, all methods defined by `IOutput` do not need to be implemented. Methods that are left unimplemented however should be configured to throw `OutputNotImplementedException`. This exception will dynamically determine the name of the unimplemented that was called, and display an error message via the `IOutput` `Error` method.

At a bare minimum the `Error` method of `IOutput` should be implemented.

### Sinefa API

Resources known to SinefaCMD are defined in the `SinefaResource` enumeration found under Code/Sinefa. New resources can be added by specifying a resource name, and a description containing the URL of the resource

```c#
[Description("api/v1/reports/realtime")]
Realtime
```

Actions based on these resources can then be implemented inside class `SinefaAPI`. As a general rule, four methods need to be called to execute a Sinefa REST API Request

1. `GetSinefaResourceUrl` - retrieves the `Description` property of the `SinefaResource` enum member
2. `CreateRequest` - construct a RestSharp `RestRequest` from your `SinefaResource` URL
3. `client.Execute` - execute the request with the `SinefaAPI` `RestClient`
4. `JObject.Parse` - parse the response JSON into a Json.NET `JObject`

`JObject` objects can then be parsed with LINQ. See methods `GetLiveTraffic` and `GetHostBandwidth` for great examples.
