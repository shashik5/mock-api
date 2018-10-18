using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Authentication.Models
{
    public enum AccountType
    {
        Basic = 0,
        Pro = 1
    }

    public class User
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement, BsonRequired]
        public string FullName { get; set; }

        [BsonRequired, BsonElement]
        public string UserName { get; set; }

        [BsonElement]
        public string AuthCode { get; set; } = string.Empty;

        [BsonRequired, BsonElement]
        public string DOB { get; set; }

        [BsonRequired, BsonElement]
        public AccountType AccountType { get; set; }
    }
}