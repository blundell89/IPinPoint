namespace IPinPoint.Api.IpLocations;

public record GetIpLocationResourceRepresentation(string IpAddress, float Latitude, float Longitude, string Country,
    string CountryCode, string PostalCode, string City, string Region);