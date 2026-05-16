using System.Net;

Console.WriteLine("Enter IP address:");
IPAddress ip = IPAddress.Parse(Console.ReadLine());

Console.WriteLine("Enter network mask (CIDR):");
int netMask = int.Parse(Console.ReadLine());

uint ipAddress = IpToUint(ip);
uint mask = uint.MaxValue << (32 - netMask);
uint network = ipAddress & mask;
uint broadcast = network | ~mask;
uint firstHost = network + 1;
uint lastHost = broadcast - 1;
int hostCount = (int)Math.Pow(2, 32 - netMask) - 2;

Console.WriteLine("---------- Result ----------");
Console.WriteLine($"IP address: {ip}");
Console.WriteLine($"Network mask: {UintToIp(mask)}");
Console.WriteLine($"Network address: {UintToIp(network)}");
Console.WriteLine($"Broadcast address: {UintToIp(broadcast)}");
Console.WriteLine($"First host: {UintToIp(firstHost)}");
Console.WriteLine($"Last host: {UintToIp(lastHost)}");
Console.WriteLine($"Hosts count: {hostCount}");
Console.WriteLine($"Network class: {GetNetworkClass(ip)}");

static uint IpToUint(IPAddress ip)
{
    byte[] bytes = ip.GetAddressBytes();

    return ((uint)bytes[0] << 24) |
           ((uint)bytes[1] << 16) |
           ((uint)bytes[2] << 8) |
           bytes[3];
}

static IPAddress UintToIp(uint value)
{
    byte[] bytes = new byte[4];

    bytes[0] = (byte)((value >> 24) & 255);
    bytes[1] = (byte)((value >> 16) & 255);
    bytes[2] = (byte)((value >> 8) & 255);
    bytes[3] = (byte)(value & 255);

    return new IPAddress(bytes);
}

static string GetNetworkClass(IPAddress ip)
{
    byte firstOctet = ip.GetAddressBytes()[0];

    if (firstOctet >= 1 && firstOctet <= 126)
        return "A";

    if (firstOctet >= 128 && firstOctet <= 191)
        return "B";

    if (firstOctet >= 192 && firstOctet <= 223)
        return "C";

    if (firstOctet >= 224 && firstOctet <= 239)
        return "D";

    return "E";
}