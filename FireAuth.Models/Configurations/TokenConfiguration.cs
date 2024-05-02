namespace FireAuth.Models.Configurations
{
    public class TokenConfiguration
    {
        public string Key { get; set; }
        public int Expiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
};

