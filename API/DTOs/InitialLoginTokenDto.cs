namespace MeuProxySsl.DTOs
{
    public class CreateInitialLoginTokenDto
    {
        public long UserId { get; set; }
        public string Model { get; set; }
    }

    public class ValidateInitialLoginTokenDto
    {
        public string Token { get; set; }
        public int? AccessTokenDays { get; set; }
    }
}
