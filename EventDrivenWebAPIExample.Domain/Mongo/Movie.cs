using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EventDrivenWebAPIExample.Domain.Mongo
{
    [BsonCollection("articles")]
    [BsonIgnoreExtraElements]
    public class Movie : IDocument
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }

        [BsonElement("title")]
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [BsonElement("description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [BsonElement("category")]
        [JsonPropertyName("category")]
        public string Category { get; set; }
    }
}
