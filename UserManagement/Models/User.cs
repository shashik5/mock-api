using System;
using CommonTypes;
using MongoDB.Bson.Serialization.Attributes;
using Utilities;

namespace UserManagement.Models
{
    public class User
    {
        [BsonId]
        public string Id { get; set; } = HelperMethods.GenerateUniqueID();

        [BsonElement, BsonRequired]
        public string FullName { get; set; }

        [BsonRequired, BsonElement]
        public string UserName { get; set; }

        [BsonRequired, BsonElement]
        public string Email { get; set; }

        [BsonElement]
        public string AuthCode { get; set; } = string.Empty;

        [BsonElement]
        public bool IsActive { get; set; }

        [BsonRequired, BsonElement]
        public string DOB { get; set; }

        [BsonRequired, BsonElement]
        public UserAccountType AccountType { get; set; } = UserAccountType.Basic;
    }
}