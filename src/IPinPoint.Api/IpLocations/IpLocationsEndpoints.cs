using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace IPinPoint.Api.IpLocations;

public record GetIpLocation([FromRoute]string IpAddress);

public class ValidIpAddressAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not string ipAddressValue || !IPAddress.TryParse(ipAddressValue, out var _))
        {
            return new ValidationResult("Invalid IP address format.");
        }

        return ValidationResult.Success;
    }
}


public static class IpLocationsEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/ip-locations/{ipAddress}",
                async ([FromRoute] string ipAddress, CancellationToken cancellationToken) =>
                {
                    return Task.FromResult(TypedResults.Problem("error", statusCode: 400));
                })
                
            .AddEndpointFilterFactory((context, next) =>
            {
                return async invocationContext =>
                {
                    var routeIpAddress = invocationContext.GetArgument<string>(0);
                    if (string.IsNullOrEmpty(routeIpAddress) || !IPAddress.TryParse(routeIpAddress, out _))
                        return Results.Problem(statusCode: 400, detail: "Invalid IP address format", title: "Validation error");
                    
                    return await next(invocationContext);
                };
            });
    }
}

public record GetIpLocationResourceRepresentation(string IpAddress, float Latitude, float Longitude, string Country,
    string CountryCode, string PostalCode, string City, string Region);