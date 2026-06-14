namespace CriptoCarteraApi.DTOs
{
    public class EditarTransaccionDTO
    {
        public string? crypto_code { get; set; }
        public string? action { get; set; }
        public int? client_id { get; set; }
        public decimal? crypto_amount { get; set; }
        public decimal? money { get; set; }
        public DateTime? datetime { get; set; }
    }
}
