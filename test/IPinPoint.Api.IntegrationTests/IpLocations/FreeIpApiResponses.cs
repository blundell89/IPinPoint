using System.Text.Json;

namespace IPinPoint.Api.IntegrationTests.IpLocations;

public static class FreeIpApiResponses
{
    public static JsonDocument SuccessResponse(string ipAddress) =>
        JsonDocument.Parse(
$$"""
{"ipVersion":4,"ipAddress":"{{ipAddress}}","latitude":51.221535,"longitude":6.77617,"countryName":"Germany","countryCode":"DE","timeZone":"+02:00","zipCode":"40213","cityName":"Dusseldorf","regionName":"Nordrhein-Westfalen","continent":"Europe","continentCode":"EU"}
""");
}