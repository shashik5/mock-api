using System;
using MongoDB.Bson.Serialization.Attributes;

namespace UserManagement.Models
{
    public class UserActivation
    {
        [BsonId]
        public string Id { get; set; } = string.Empty;

        [BsonRequired, BsonElement]
        public string UserName { get; set; }
    }
}