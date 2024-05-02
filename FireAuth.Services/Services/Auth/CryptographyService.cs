using System.Security.Cryptography;
using FireAuth.Services.Abstractions.Auth;

namespace FireAuth.Services.Services.Auth
{
    public class CryptographyService : ICryptographyService
    {
    // These constants may be changed without breaking existing hashes.
        private const int SaltBytes = 24;

        private const int HashBytes = 18;
        private const int Pbkdf2Iterations = 64000;

        // These constants define the encoding and may not be changed.
        private const int HashSections = 5;

        private const int HashAlgorithmIndex = 0;
        private const int IterationIndex = 1;
        private const int HashSizeIndex = 2;
        private const int SaltIndex = 3;
        private const int Pbkdf2Index = 4;

        public string CreateHash(string password, string salt) => CreateHash($"{password}/{salt}");
        public string CreateHash(string password)
        {
            // Generate a random salt
            var salt = new byte[SaltBytes];

            try
            {
                using (var csprng = new RNGCryptoServiceProvider())
                {
                    csprng.GetBytes(salt);
                }
            }
            catch (CryptographicException ex)
            {
                throw new Exception("Random number generator not available.", ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception("Invalid argument given to random number generator.", ex);
            }

            var hash = Pbkdf2(password, salt, Pbkdf2Iterations, HashBytes);

            // format: algorithm:iterations:hashSize:salt:hash
            var parts = "sha1:" +
                Pbkdf2Iterations +
                ":" +
                hash.Length +
                ":" +
                Convert.ToBase64String(salt) +
                ":" +
                Convert.ToBase64String(hash);
            return parts;
        }

        public bool ValidatePasswordAndHash(string password, string salt, string goodHash) => ValidatePasswordAndHash($"{password}/{salt}", goodHash);
        public bool ValidatePasswordAndHash(string password, string goodHash)
        {
            char[] delimiter = { ':' };
            var split = goodHash.Split(delimiter);

            if (split.Length != HashSections)
            {
                throw new Exception("Fields are missing from the password hash.");
            }

            // We only support SHA1 with C#.
            if (split[HashAlgorithmIndex] != "sha1")
            {
                throw new Exception("Unsupported hash type.");
            }

            int iterations;
            try
            {
                iterations = int.Parse(split[IterationIndex]);
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception("Invalid argument given to Int32.Parse", ex);
            }
            catch (FormatException ex)
            {
                throw new Exception("Could not parse the iteration count as an integer.", ex);
            }
            catch (OverflowException ex)
            {
                throw new Exception("The iteration count is too large to be represented.", ex);
            }

            if (iterations < 1)
            {
                throw new Exception("Invalid number of iterations. Must be >= 1.");
            }

            byte[] salt;
            try
            {
                salt = Convert.FromBase64String(split[SaltIndex]);
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception("Invalid argument given to Convert.FromBase64String", ex);
            }
            catch (FormatException ex)
            {
                throw new Exception("Base64 decoding of salt failed.", ex);
            }

            byte[] hash;
            try
            {
                hash = Convert.FromBase64String(split[Pbkdf2Index]);
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception("Invalid argument given to Convert.FromBase64String", ex);
            }
            catch (FormatException ex)
            {
                throw new Exception("Base64 decoding of pbkdf2 output failed.", ex);
            }

            int storedHashSize;
            try
            {
                storedHashSize = Int32.Parse(split[HashSizeIndex]);
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception("Invalid argument given to Int32.Parse", ex);
            }
            catch (FormatException ex)
            {
                throw new Exception("Could not parse the hash size as an integer.", ex);
            }
            catch (OverflowException ex)
            {
                throw new Exception("The hash size is too large to be represented.", ex);
            }

            if (storedHashSize != hash.Length)
            {
                throw new Exception("Hash length doesn't match stored hash length.");
            }

            var testHash = Pbkdf2(password, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        private static bool SlowEquals(IReadOnlyList<byte> a, IReadOnlyList<byte> b)
        {
            var diff = (uint)a.Count ^ (uint)b.Count;
            for (var i = 0; i < a.Count && i < b.Count; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private static byte[] Pbkdf2(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt))
            {
                pbkdf2.IterationCount = iterations;
                return pbkdf2.GetBytes(outputBytes);
            }
        }

        public string CombineSaltAndPassword(string salt, string password) => $"{salt}/{password}";
        
        public string GenerateSalt()
        {
            const int saltLength = 16;

            byte[] saltBytes = new byte[saltLength];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }

            string salt = Convert.ToBase64String(saltBytes);

            return salt;
        }
    }
};

