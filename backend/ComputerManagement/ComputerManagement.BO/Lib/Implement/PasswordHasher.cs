using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ComputerManagement.BO.Lib.Interface;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ComputerManagement.BO.Lib.Implement
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 128 / 8;
        private const int KeySize = 256 / 8;
        private const int Iterations = 10000;
        private readonly KeyDerivationPrf _hashAlgorithmName = KeyDerivationPrf.HMACSHA256;
        private const char Delimiter = ';';

        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = KeyDerivation.Pbkdf2(password,salt, _hashAlgorithmName,Iterations, KeySize);

            return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public bool Verify(string passwordHash, string inputPassword)
        {
            var elements = passwordHash.Split(Delimiter);
            var salt = Convert.FromBase64String(elements[0]);
            var hash = Convert.FromBase64String(elements[1]);

            var hashInput = KeyDerivation.Pbkdf2(inputPassword, salt, _hashAlgorithmName, Iterations, KeySize);

            return CryptographicOperations.FixedTimeEquals(hash, hashInput);
        }
    }
}
