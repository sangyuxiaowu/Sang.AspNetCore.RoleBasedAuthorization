namespace TestDemo
{
    public class JWTSettings
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string SecretKey { get; set; } = "You_JWT_Secret_Key";

        public int ExpireSeconds { get; set; }
    }
}
