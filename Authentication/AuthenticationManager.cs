using System;
using MongoDB.Driver;
using Authentication.Models;
using System.Collections.Generic;
using Crypto;
using System.Text;

namespace Authentication
{
    public interface IAuthenticationManager
    {
        User GetUserDetails(string authCode);
        bool Validate(string userName, string authCode);
    }

    public class AuthenticationManagerConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
    }

    public class AuthenticationManager : IAuthenticationManager
    {
        private uint AuthCodeDbEncryptionLevel = 5;
        private uint UserAuthCodeEncryptionLevel = 3;
        private IMongoCollection<User> UserCollection;

        public AuthenticationManager(AuthenticationManagerConfig config)
        {
            MongoClient dbClient = new MongoClient(config.ConnectionString);
            IMongoDatabase userDatabase = dbClient.GetDatabase(config.DatabaseName);
            UserCollection = userDatabase.GetCollection<User>(config.TableName);
        }

        public bool Validate(string userName, string authCode)
        {
            try
            {
                Credential userEnteredCredential = GetDecodedCredential(authCode, UserAuthCodeEncryptionLevel);
                User currentUser = (User)UserCollection.Find(User => User.UserName.Equals(userName));
                Credential credentialFromDb = GetDecodedCredential(currentUser.AuthCode, AuthCodeDbEncryptionLevel);
                return (
                    userEnteredCredential.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                    && userEnteredCredential.UserName.Equals(credentialFromDb.UserName, StringComparison.OrdinalIgnoreCase)
                    && userEnteredCredential.Password.Equals(credentialFromDb.Password)
                    );
            }
            catch
            {
                return false;
            }
        }

        public User GetUserDetails(string authCode)
        {
            Credential userEnteredCredential = GetDecodedCredential(authCode, UserAuthCodeEncryptionLevel);
            return (User)UserCollection.Find(User => User.UserName.Equals(userEnteredCredential.UserName));
        }

        private Credential GetDecodedCredential(string authCode, uint encryptionLevel = 1)
        {
            string decodedString = Encryption.Decode(authCode, encryptionLevel);
            string[] decodedData = decodedString.Split('/');
            if (decodedData.Length < 2)
            {
                throw new Exception("Invalid Credentials, please verify your credentials and try again.");
            }
            return new Credential
            {
                UserName = decodedData[0],
                Password = decodedData[1]
            };
        }

        private string GetAuthCode(Credential credentials, uint encryptionLevel = 1)
        {
            return Encryption.Encode(String.Format("{0}/{1}", credentials.UserName, credentials.Password));
        }
    }
}