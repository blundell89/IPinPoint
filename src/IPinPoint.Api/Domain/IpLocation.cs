using System.Net;
using MongoDB.Bson;

namespace IPinPoint.Api.Domain;

public record IpLocation(ObjectId Id, IPAddress IpAddress, float Latitude, float Longitude,
    string Country, string CountryCode, string PostalCode, string City, string Region);