namespace FireAuth.Models.Responses
{
    public class AuthResponse
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string? RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public string MD5 { get; set; }
        public DateTime Expiration { get; set; }
        public int Lifetime { get; set; }
    }
};

