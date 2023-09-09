using System.ComponentModel.DataAnnotations;

namespace IPinPoint.Api.IpLocations.FreeIpApi;

public record FreeIpApiOptions
{
    [Required]
    [MinLength(1)]
    public string BaseAddress { get; init; } = null!;
}