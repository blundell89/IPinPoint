using System.ComponentModel.DataAnnotations;

namespace IPinPoint.Api.MongoDb;

public record MongoDbOptions
{
    [Required]
    [MinLength(1)]
    public string ConnectionString { get; init; } = null!;
}