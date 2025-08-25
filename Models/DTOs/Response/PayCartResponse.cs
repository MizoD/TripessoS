namespace Models.DTOs.Response
{
    public class PayCartResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? StripeSessionUrl { get; set; }
    }
}
