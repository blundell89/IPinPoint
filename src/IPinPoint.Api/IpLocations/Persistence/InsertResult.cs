namespace IPinPoint.Api.IpLocations.Persistence;

public abstract record InsertResult();

public record Inserted() : InsertResult();

public record AlreadyExists() : InsertResult();