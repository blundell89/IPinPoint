namespace IPinPoint.Api.IpLocations.FreeIpApi;

public record GetIpResponse(int IpVersion, string IpAddress, float Latitude, float Longitude,
    string CountryName, string CountryCode, string TimeZone, string ZipCode, string CityName,
    string RegionName, string Continent, string ContinentCode);