using System.Net;

namespace IPinPoint.Api.Domain;

public record IpLocation(Guid Id, IPAddress IpAddress, float Latitude, float Longitude,
    string Country, string CountryCode, string PostalCode, string City, string Region);