namespace FireAuth.Services.Abstractions.Auth
{
    public interface ICryptographyService
    {
        string CreateHash(string password);
        string CreateHash(string password, string salt);

        bool ValidatePasswordAndHash(string password, string goodHash);
        bool ValidatePasswordAndHash(string password, string salt, string goodHash);
        public string GenerateSalt();
    }
};

