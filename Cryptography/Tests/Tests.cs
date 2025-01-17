using System.Security.Cryptography;
using System.Threading.Tasks.Sources;
using Cryptography.Libs;

using static System.Console;

namespace Cryptography.Tests
{
    internal static class Tests
    {
        public static void TestProtector()
        {
            Write("Enter a message that you want to encrypt: ");
            string? message = ReadLine();

            Write("Enter a password: ");
            string? password = ReadLine();

            if ((password is null) || (message is null))
            {
                WriteLine("Message or password cannot be null.");
                return;
            }

            string cipherText = Protector.Encrypt(message, password);

            WriteLine($"Encrypted text: {cipherText}");

            Write("Enter the password: ");
            string? password2Decrypt = ReadLine();

            if(password2Decrypt is null)
            {
                WriteLine("Password to decrypt cannot be null.");
                return;
            }

            try
            {
                string clearText = Protector.Decrypt(cipherText, password2Decrypt);
                WriteLine($"Decrypted text: {clearText}");
            }
            catch (CryptographicException ex)
            {
                WriteLine("{0}\nMore details: {1}",
                    arg0: "You entered the wrong password!",
                    arg1: ex.Message);
            }
            catch (Exception ex)
            {
                WriteLine("Non-cryptographic exception: {0}, {1}",
                   arg0: ex.GetType().Name,
                   arg1: ex.Message);
            }
        }

        public static void TestUsers()
        {
            WriteLine("Register Alice with Pa$$w0rd:");
            User alice = Protector.Register("Alice", "Pa$$w0rd");

            WriteLine($"    Name: {alice.Name}");
            WriteLine($"    Salt: {alice.Salt}");
            WriteLine($"    Password (salted and hashed): {alice.SaltedHashedPassword}");
            WriteLine();

            WriteLine("Enter a new user to register: ");
            string? username = ReadLine();

            WriteLine($"Enter a password for {username}: ");
            string? password = ReadLine();

            if ((username is null) || (password is null))
            {
                WriteLine("Username or password cannot be null.");
                return;
            }

            WriteLine("Register a new user:");
            User newUser = Protector.Register(username, password);

            WriteLine($"    Name: {newUser.Name}");
            WriteLine($"    Salt: {newUser.Salt}");
            WriteLine($"    Password (salted and hashed): {newUser.SaltedHashedPassword}");
            WriteLine();

            bool correctPassword = false;

            while(!correctPassword)
            {
                Write("Enter a username to log in: ");
                string? loginUsername = ReadLine();

                Write("Enter a password to log in: ");
                string? loginPassword = ReadLine();

                if ((loginUsername is null) || (loginPassword is null))
                {
                    WriteLine("Login username or password cannot be null.");
                    continue;
                }

                correctPassword = Protector.CheckPassword(loginUsername, loginPassword);

                if(correctPassword)
                {
                    WriteLine($"Correct! {loginUsername} has been logged in.");
                }
                else
                {
                    WriteLine("Invalid username or password. Try again.");
                }
            }
        }

        public static void TestSigning()
        {
            Write("Enter some text to sign: ");
            string? data = ReadLine();

            string signature = Protector.GenerateSignature(data);

            WriteLine($"Signature: {signature}");
            WriteLine($"Public Key used to check signature: ");
            WriteLine(Protector.PublicKey);

            if(Protector.ValidateSignature(data, signature))
            {
                WriteLine("Correct! Signature is valid.");
            }
            else
            {
                WriteLine("Invalid signature.");
            }

            // imitate fake sign (very casual)
            string fakeSignature = signature.Replace(signature[0], 'X');
            if(fakeSignature == signature)
            {
                fakeSignature = signature.Replace(signature[0], 'Y');
            }

            if(Protector.ValidateSignature(data, fakeSignature))
            {
                WriteLine("Correct! Signature is valid.");
            }
            else
            {
                WriteLine($"Invalid signature: {fakeSignature}");
            }
        }
    }
}
