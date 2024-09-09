using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace WpfApp2
{
    public class User
    {
        private const string FilePath = "passwords.txt";

        public void SavePassword(string label, string password)
        {
            if (string.IsNullOrWhiteSpace(label) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Email/User and password cannot be empty.");

            var passwords = LoadPasswords();
            var encryptedLabel = EncryptionHelper.Encrypt(label);
            if (passwords.ContainsKey(encryptedLabel))
            {
                throw new InvalidOperationException("A password with this Email/User already exists.");
            }
            passwords[encryptedLabel] = EncryptionHelper.Encrypt(password);
            SavePasswords(passwords);
        }

        public Dictionary<string, string> GetPasswords()
        {
            var passwords = LoadPasswords();
            var decryptedPasswords = new Dictionary<string, string>();

            foreach (var kvp in passwords)
            {
                try
                {
                    var decryptedLabel = EncryptionHelper.Decrypt(kvp.Key);
                    decryptedPasswords[decryptedLabel] = EncryptionHelper.Decrypt(kvp.Value);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to decrypt password for Email/User: {kvp.Key}", ex);
                }
            }

            return decryptedPasswords;
        }

        public void RemovePassword(string label)
        {
            var passwords = LoadPasswords();
            var encryptedLabel = EncryptionHelper.Encrypt(label);
            if (!passwords.ContainsKey(encryptedLabel))
            {
                throw new ArgumentException("No password found with this Email/User.");
            }
            passwords.Remove(encryptedLabel);
            SavePasswords(passwords);
        }

        private Dictionary<string, string> LoadPasswords()
        {
            if (!File.Exists(FilePath))
                return new Dictionary<string, string>();

            try
            {
                var encryptedData = File.ReadAllText(FilePath);

                
                System.Diagnostics.Debug.WriteLine($"File content: {encryptedData}");

                if (string.IsNullOrWhiteSpace(encryptedData))
                    return new Dictionary<string, string>();

                return JsonConvert.DeserializeObject<Dictionary<string, string>>(encryptedData) ?? new Dictionary<string, string>();
            }
            catch (JsonException ex)
            {
                throw new IOException("Failed to parse the passwords file. The file might be corrupted.", ex);
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to load passwords from file.", ex);
            }
        }

        private void SavePasswords(Dictionary<string, string> passwords)
        {
            try
            {
                var encryptedData = JsonConvert.SerializeObject(passwords);
                File.WriteAllText(FilePath, encryptedData);
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to save passwords to file.", ex);
            }
        }
    }
}
