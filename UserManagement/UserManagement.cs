using System;
using MongoDB.Driver;
using UserManagement.Models;
using System.Collections.Generic;
using Crypto;
using System.Text;
using UserManagement.Validations;
using Utilities;
using CommonTypes;

namespace UserManagement
{
    public interface IUserManager
    {
        User AuthenticateUser(string userName, string loginAuthCode);
        bool ValidateUser(string authCode);
        User GetUserDetailsByAuthCode(string authCode);
        bool IsUserNameAvailable(string userName);
        bool IsEmailAlreadyRegistered(string email);
        bool IsUserNameValid(string userName);
        bool IsEmailValid(string email);
        bool IsProUser(string authCode);
        bool IsAdmin(string authCode);
        bool IsUserAuthorised(string authCode, UserAccountType accountType);
        string RegisterUser(User userDetails);
        bool RequestActivationCode(string userName);
        bool ActivateUser(string userName, string authCode, string activationCode);
        bool DeactivateUser(string authCode);
        bool RemoveUser(string authCode);
        bool UpdatePassword(string authCode, string passwordChangeRequestString);
    }

    public class UserManagerConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public class UserManager : IUserManager
    {
        private uint AuthCodeDbEncryptionLevel = 5;
        private uint UserAuthCodeEncryptionLevel = 3;
        private IMongoCollection<User> UserCollection;
        private IMongoCollection<UserActivation> UserActivationCollection;
        private IClientSessionHandle DBSession;

        public UserManager(UserManagerConfig config)
        {
            MongoClient dbClient = new MongoClient(config.ConnectionString);
            DBSession = dbClient.StartSession();
            IMongoDatabase userDatabase = dbClient.GetDatabase(config.DatabaseName);
            UserCollection = userDatabase.GetCollection<User>("Users");
            UserActivationCollection = userDatabase.GetCollection<UserActivation>("UserActivationData");
        }

        public User AuthenticateUser(string userName, string loginAuthCode)
        {
            Credential userEnteredCredential = GetDecodedCredential(loginAuthCode, UserAuthCodeEncryptionLevel);
            User currentUser = GetUserDetailsByUserName(userName);
            Credential credentialFromDb = GetDecodedCredential(currentUser.AuthCode, AuthCodeDbEncryptionLevel);
            if (
                userEnteredCredential.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                && userEnteredCredential.UserName.Equals(credentialFromDb.UserName, StringComparison.OrdinalIgnoreCase)
                && userEnteredCredential.Password.Equals(credentialFromDb.Password)
                )
            {
                return currentUser;
            }

            throw new Exception("Invalid credentials, username or password did not match");
        }

        private User GetUserDetailsByUserName(string userName)
        {
            return (User)UserCollection.Find(User => User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        public bool ValidateUser(string authCode)
        {
            try
            {
                User currentUser = GetUserDetailsByAuthCode(authCode);
                return currentUser.IsActive;
            }
            catch
            {
                return false;
            }
        }

        public User GetUserDetailsByAuthCode(string authCode)
        {
            return (User)UserCollection.Find(User => User.AuthCode.Equals(authCode)).FirstOrDefault();
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
            return Encryption.Encode(String.Format("{0}/{1}", credentials.UserName, credentials.Password), encryptionLevel);
        }

        public bool IsUserNameAvailable(string userName)
        {
            var filterDefinition = Builders<User>.Filter.Eq(user => user.UserName, userName);
            return UserCollection.CountDocuments(filterDefinition) == 0;
        }

        public bool IsEmailAlreadyRegistered(string email)
        {
            var filterDefinition = Builders<User>.Filter.Eq(user => user.Email, email);
            return UserCollection.CountDocuments(filterDefinition) == 0;
        }

        public bool IsUserNameValid(string userName)
        {
            if (!IsUserNameAvailable(userName))
            {
                throw new Exception("Username already taken");
            }
            if (!UserNameValidation.IsValid(userName))
            {
                throw new Exception("Username is not valid");
            }
            return true;
        }

        public bool IsEmailValid(string email)
        {
            if (!IsUserNameAvailable(email))
            {
                throw new Exception("Email already registered");
            }
            if (!EmailValidations.IsValid(email))
            {
                throw new Exception("Email is not valid");
            }
            return true;
        }

        public bool IsProUser(string authCode)
        {
            var user = GetUserDetailsByAuthCode(authCode);
            return user.AccountType == UserAccountType.Pro;
        }

        public bool IsAdmin(string authCode)
        {
            var user = GetUserDetailsByAuthCode(authCode);
            return user.AccountType == UserAccountType.Admin;
        }

        public bool IsUserAuthorised(string authCode, UserAccountType accountType)
        {
            var user = GetUserDetailsByAuthCode(authCode);
            return user != null && user.IsActive && user.AccountType == accountType;
        }

        private bool AddUser(User userDetails)
        {
            if (IsUserNameValid(userDetails.UserName) && IsEmailValid(userDetails.Email))
            {
                userDetails.AuthCode = GetAuthCode(GetDecodedCredential(userDetails.AuthCode, UserAuthCodeEncryptionLevel), AuthCodeDbEncryptionLevel);
                UserCollection.InsertOne(userDetails);
                return true;
            }
            return false;
        }

        private string generateUserActivationCode(string userName)
        {
            var activationCode = HelperMethods.GenerateUniqueID();
            UserActivationCollection.InsertOne(new UserActivation
            {
                Id = activationCode,
                UserName = userName
            });
            return activationCode;
        }

        private void RemoveUserActivationCode(string userName)
        {
            try
            {
                DBSession.StartTransaction();
                UserCollection.DeleteOne(user => user.UserName.Equals(user.UserName));
                DBSession.CommitTransaction();
            }
            catch (System.Exception)
            {
                DBSession.AbortTransaction();
                throw;
            }
        }

        public string RegisterUser(User userDetails)
        {
            try
            {
                DBSession.StartTransaction();
                AddUser(userDetails);
                var activationCode = generateUserActivationCode(userDetails.UserName);
                DBSession.CommitTransaction();
                return activationCode;
            }
            catch (System.Exception)
            {
                DBSession.AbortTransaction();
                throw;
            }
        }

        private User UpdateUserRecord(string authCode, UpdateDefinition<User> updateDefinition)
        {
            try
            {
                DBSession.StartTransaction();
                var userDetails = GetUserDetailsByAuthCode(authCode);
                UserCollection.UpdateOne(user => user.UserName.Equals(user.UserName), updateDefinition);
                DBSession.CommitTransaction();
                return GetUserDetailsByUserName(userDetails.UserName);
            }
            catch (System.Exception)
            {
                DBSession.AbortTransaction();
                throw;
            }
        }

        private bool IsActivationCodeValid(string userName, string activationCode)
        {
            try
            {
                var activationDetails = UserActivationCollection.Find(user => user.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                return activationDetails.Id.Equals(activationCode);
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public bool RequestActivationCode(string userName)
        {
            try
            {
                DBSession.StartTransaction();
                var activationCode = generateUserActivationCode(userName);
                DBSession.CommitTransaction();
            }
            catch (System.Exception)
            {
                DBSession.AbortTransaction();
                throw;
            }
            return SendActivationCode(userName);
        }

        private bool SendActivationCode(string userName)
        {
            var userDetails = GetUserDetailsByUserName(userName);
            return HelperMethods.SendEmail(new EmailDetails
            {
                Subject = "Account Activation Code",
                To = userDetails.Email
            });
        }

        public bool ActivateUser(string userName, string loginAuthCode, string activationCode)
        {
            var userDetails = AuthenticateUser(userName, loginAuthCode);
            if (!IsActivationCodeValid(userDetails.UserName, activationCode))
            {
                throw new Exception("Invalid activation code");
            }
            var updateDef = Builders<User>.Update.Set(user => user.IsActive, true);
            UpdateUserRecord(userDetails.AuthCode, updateDef);
            RemoveUserActivationCode(userDetails.UserName);
            return true;
        }

        public bool DeactivateUser(string authCode)
        {
            var updateDef = Builders<User>.Update.Set(user => user.IsActive, false);
            UpdateUserRecord(authCode, updateDef);
            return true;
        }

        public bool RemoveUser(string authCode)
        {
            try
            {
                DBSession.StartTransaction();
                var userDetails = GetUserDetailsByAuthCode(authCode);
                UserCollection.DeleteOne(user => user.UserName.Equals(userDetails.UserName));
                DBSession.CommitTransaction();
                return true;
            }
            catch (System.Exception)
            {
                DBSession.AbortTransaction();
                throw;
            }
        }

        private PasswordChangeRequestData GetPasswordChangeRequestData(string passwordChangeRequestString)
        {
            string decodedString = Encryption.Decode(passwordChangeRequestString, UserAuthCodeEncryptionLevel);
            string[] decodedData = decodedString.Split('/');
            if (decodedData.Length != 3)
            {
                throw new Exception("Invalid Credentials, please verify your credentials and try again.");
            }
            return new PasswordChangeRequestData
            {
                UserName = decodedData[0],
                CurrentPassword = decodedData[1],
                NewPassword = decodedData[2]
            };
        }

        public bool UpdatePassword(string authCode, string passwordChangeRequestString)
        {
            var passwordChangeRequestData = GetPasswordChangeRequestData(passwordChangeRequestString);
            var userCredentials = GetDecodedCredential(authCode, UserAuthCodeEncryptionLevel);
            if (!passwordChangeRequestData.UserName.Equals(userCredentials.UserName))
            {
                throw new Exception("Invalid request, username did not match");
            }
            if (!userCredentials.Password.Equals(Encryption.Decode(passwordChangeRequestData.CurrentPassword, UserAuthCodeEncryptionLevel)))
            {
                throw new Exception("Current password did not match");
            }
            var updateDef = Builders<User>.Update.Set(user => user.AuthCode, GetAuthCode(
                    new Credential
                    {
                        Password = passwordChangeRequestData.NewPassword,
                        UserName = userCredentials.UserName
                    }, AuthCodeDbEncryptionLevel));
            UpdateUserRecord(authCode, updateDef);
            return true;
        }
    }
}