using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace My_CV_3.Models
{
    public class Contact
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Company")]
        public string Company { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Phone")]
        public string Phone { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }

        [BsonElement("SubmittedAt")]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
