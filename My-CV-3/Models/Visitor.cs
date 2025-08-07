using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace My_CV_3.Models
{
    public class Visitor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("PageUrl")]
        public string PageUrl { get; set; } = string.Empty;

        [BsonElement("IpAddress")]
        public string IpAddress { get; set; } = string.Empty;

        [BsonElement("UserAgent")]
        public string UserAgent { get; set; } = string.Empty;

        [BsonElement("VisitedAt")]
        public DateTime VisitedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("VisitedAtFormatted")]
        public string VisitedAtFormatted { get; set; } = TimeZoneInfo
    .ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))
    .ToString("dd-MM-yyyy hh:mm tt");

    }
}