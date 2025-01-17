using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;

using static System.Console;
using static System.Convert;

namespace Cryptography.Libs
{
    public static class Protector
    {
        // Salt size is 16 byte (default min is 8)
        private static readonly byte[] salt =
            Encoding.Unicode.GetBytes("7BANANAS");

        // Keep it value high, so generative proccess will
        // require at least 100ms
        // 150 000 iters require ~139 ms (Intel Core i7, 2,8 GH)
        private static readonly int iterations = 150_000;

        public static string Encrypt(string plainText, string password)
        {
            byte[] encryptedBytes;
            byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);

            // Advanced Encryption Standart
            using(Aes aes = Aes.Create())
            {
                Stopwatch timer = Stopwatch.StartNew(); 

                using(Rfc2898DeriveBytes pbkdf2 = new(password, salt, iterations))
                {
                    aes.Key = pbkdf2.GetBytes(32); // set 256-byte key
                    aes.IV = pbkdf2.GetBytes(16); // set 128-byte IV
                }

                timer.Stop();

                WriteLine("{0:N0} ms to generate Key and IV using {1:N0} iters.",
                    arg0: timer.ElapsedMilliseconds,
                    arg1: iterations
                    );

                using (MemoryStream ms = new())
                {
                    using(ICryptoTransform transformer = aes.CreateEncryptor())
                    {
                        using (CryptoStream cs = new(ms, transformer, CryptoStreamMode.Write))
                        {
                            cs.Write(plainBytes, 0, plainBytes.Length);
                        }
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string cipherText, string password) 
        {
            byte[] plainBytes;
            byte[] cryptoBytes = FromBase64String(cipherText);

            using(Aes aes = Aes.Create())
            {
                using(Rfc2898DeriveBytes pbkdf2 = new(password, salt, iterations))
                {
                    aes.Key = pbkdf2.GetBytes(32);
                    aes.IV = pbkdf2.GetBytes(16);
                }

                using(MemoryStream ms = new())
                {
                    using(ICryptoTransform transformer = aes.CreateDecryptor())
                    {
                        using(CryptoStream cs = new(ms, transformer, CryptoStreamMode.Write))
                        {
                            cs.Write(cryptoBytes, 0, cryptoBytes.Length);
                        }
                    }
                    plainBytes = ms.ToArray();
                }
            }

            return Encoding.Unicode.GetString(plainBytes);
        }

        ///
        /// USERS
        ///
        private static Dictionary<string, User> Users = new();

        public static User Register(string username, string password, string[]? roles = null)
        {
            // Generate rng salt
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            string saltText = ToBase64String(saltBytes);

            // generate salt & hash password
            string saltedhashedPassword = SaltAndHashPassword(password, saltText);
            
            User user = new(username, saltText, saltedhashedPassword, roles);
            Users.Add(user.Name, user);

            return user;
        }

        public static bool CheckPassword(string username, string password)
        {
            if(!Users.ContainsKey(username)) return false;

            User u = Users[username];

            return CheckPassword(password, u.Salt, u.SaltedHashedPassword);
        }

        public static bool CheckPassword(string password, string salt, string hashedPassword)
        {
            string saltedhashedPassword = SaltAndHashPassword(password, salt);

            return (saltedhashedPassword == hashedPassword);
        }

        private static string SaltAndHashPassword(string password, string salt)
        {
            using(SHA256 sha = SHA256.Create())
            {
                string saltedPassword = password + salt;
                return ToBase64String(sha.ComputeHash(Encoding.Unicode.GetBytes(saltedPassword)));
            }
        }

        ///
        /// DIGITAL SIGNATURE
        ///

        public static string? PublicKey;

        public static string GenerateSignature(string data)
        {
            byte[] dataBytes = Encoding.Unicode.GetBytes(data);
            SHA256 sha = SHA256.Create();
            byte[] hashedData = sha.ComputeHash(dataBytes);
            RSA rsa = RSA.Create();

            PublicKey = rsa.ToXmlString(false); // exclude private key

            return ToBase64String(rsa.SignHash(hashedData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
        }

        public static bool ValidateSignature(string data, string signature)
        {
            if(PublicKey == null) { return false; }
            byte[] dataBytes = Encoding.Unicode.GetBytes(data);
            SHA256 sha = SHA256.Create();
            byte[] hashedData = sha.ComputeHash(dataBytes);
            byte[] signatureBytes = FromBase64String(signature);
            RSA rsa= RSA.Create();
            rsa.FromXmlString(PublicKey);
            return rsa.VerifyHash(hashedData, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        ///
        /// RANDOM
        ///

        public static byte[] GetRandomKeyOrIV(int size)
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] data = new byte[size];

            return data;
        }

        ///
        /// Auth
        ///
        public static void LogIn(string username, string password)
        {
            if(CheckPassword(username, password))
            {
                GenericIdentity gi = new(name: username, type: "PacktAuth");

                GenericPrincipal gp = new(identity: gi, roles: Users[username].Roles);

                Thread.CurrentPrincipal = gp;
            }
        }
    }
}
