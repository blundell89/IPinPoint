using System.Net;
using System.Net.Sockets;

namespace IPinPoint.Api.Tests.Shared;

public static class RandomIpGenerator
{
    private static readonly Random _random = new(DateTime.UtcNow.Ticks.GetHashCode());

    public static IPAddress Generate(AddressFamily family = AddressFamily.InterNetwork)
    {
        var addressBytes = family switch
        {
            AddressFamily.InterNetwork => new byte[4],
            AddressFamily.InterNetworkV6 => new byte[16],
            _ => throw new ArgumentException("Invalid address family", nameof(family))
        };

        _random.NextBytes(addressBytes);
        return new IPAddress(addressBytes);
    }
}