using System.Net;
using IPinPoint.Api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace IPinPoint.Api.IpLocations;

public static class IpLocationsEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/ip-locations/{ipAddress}",
            async ([FromRoute] string ipAddress,
                [FromServices] GetIpLocationHandler handler,
                CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrEmpty(ipAddress) || !IPAddress.TryParse(ipAddress, out var validIpAddress))
                    return Results.Problem(
                        statusCode: 400,
                        detail: "Invalid IP address format",
                        title: "Validation error");

                var result = await handler.Handle(new GetIpLocationHandler.GetIpLocation(validIpAddress),
                    cancellationToken);
                
                return result switch
                {
                    GetIpLocationHandler.Match match => Results.Ok(MapIpLocation(match.IpLocation)),
                    GetIpLocationHandler.NotFound => Results.NotFound(),
                    _ => Results.Problem(statusCode: 500, detail: "Internal server error", title: "Server error")
                };
            });
    }

    private static GetIpLocationResourceRepresentation MapIpLocation(IpLocation ipLocation) =>
        new(
            ipLocation.IpAddress.ToString(),
            ipLocation.Latitude,
            ipLocation.Longitude,
            ipLocation.Country,
            ipLocation.CountryCode,
            ipLocation.PostalCode,
            ipLocation.City,
            ipLocation.Region);
}