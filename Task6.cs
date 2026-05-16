using System;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;


string subnet = "172.20.10";

Console.WriteLine($"Scanning subnet {subnet}.0/24...\n");

List<(string ip, long time)> activeHosts = new List<(string, long)>();

for (int i = 1; i <= 254; i++)
{
    string ip = $"{subnet}.{i}";
    PingReply reply = PingHost(ip);

    if (reply != null && reply.Status == IPStatus.Success)
    {
        Console.WriteLine($"{ip} - ACTIVE | {reply.RoundtripTime} ms");
        activeHosts.Add((ip, reply.RoundtripTime));
    }
}

Console.WriteLine("\n--- MAC ADDRESSES (ARP TABLE) ---\n");

string arpTable = GetArpTable();

foreach (var host in activeHosts)
{
    string mac = FindMacFromArp(arpTable, host.ip);

    Console.WriteLine($"{host.ip} | {host.time} ms | MAC: {mac}");
}

Console.WriteLine("\nScan complete.");


static PingReply PingHost(string ip)
{
    try
    {
        using (Ping ping = new Ping())
        {
            return ping.Send(ip, 500);
        }
    }
    catch
    {
        return null;
    }
}

static string GetArpTable()
{
    ProcessStartInfo psi = new ProcessStartInfo
    {
        FileName = "arp",
        Arguments = "-a",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    Process process = Process.Start(psi);
    string output = process.StandardOutput.ReadToEnd();
    process.WaitForExit();

    return output;
}

static string FindMacFromArp(string arpTable, string ip)
{
    string pattern = $@"{Regex.Escape(ip)}\s+([\w-]+)";

    Match match = Regex.Match(arpTable, pattern);

    if (match.Success)
        return match.Groups[1].Value;

    return "NOT FOUND";
}
