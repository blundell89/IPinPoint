using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace IPinPoint.Api.IntegrationTests.CustomAssertions;

public static class JsonDocumentExtensions 
{
    public static JsonDocumentAssertions Should(this JsonDocument instance)
    {
        return new JsonDocumentAssertions(instance); 
    } 
}

public class JsonDocumentAssertions : 
    ReferenceTypeAssertions<JsonDocument, JsonDocumentAssertions>
{
    public JsonDocumentAssertions(JsonDocument instance)
        : base(instance)
    {
    }

    protected override string Identifier => "jsondocument";

    public AndConstraint<JsonDocumentAssertions> BeProblemDetails(
        string title, int statusCode, string detail)
    {
        var rootElement = Subject.RootElement;
        rootElement.GetProperty("title").GetString().Should().Be(title);
        rootElement.GetProperty("status").GetInt32().Should().Be(statusCode);
        rootElement.GetProperty("detail").GetString().Should().Be(detail);

        return new(this);
    }
}