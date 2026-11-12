using Banned.Qbittorrent.Models.Search;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Banned.Qbittorrent.Serialization;

/// <summary>
///     Supports both the string status returned by current qBittorrent servers and the legacy object shape.
/// </summary>
public sealed class SearchResultStatusConverter : JsonConverter<SearchStatus?>
{
    /// <inheritdoc />
    public override bool HandleNull => true;

    /// <inheritdoc />
    public override SearchStatus? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
            return new SearchStatus { Status = reader.GetString()! };

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var typeInfo = QBittorrentJsonContext.Default.GetTypeInfo(typeof(SearchStatus))
                               as JsonTypeInfo<SearchStatus>
                        ?? throw new InvalidOperationException("SearchStatus JSON metadata is unavailable.");
            return JsonSerializer.Deserialize(ref reader, typeInfo);
        }

        throw new JsonException($"Unexpected token {reader.TokenType} for search result status.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, SearchStatus? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var typeInfo = QBittorrentJsonContext.Default.GetTypeInfo(typeof(SearchStatus))
                           as JsonTypeInfo<SearchStatus>
                    ?? throw new InvalidOperationException("SearchStatus JSON metadata is unavailable.");
        JsonSerializer.Serialize(writer, value, typeInfo);
    }
}
