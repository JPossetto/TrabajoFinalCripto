namespace CriptoCarteraApi.DTOs
{
    public class CrearTransaccionDTO
    {
        public string crypto_code { get; set; } = string.Empty;
        public string action { get; set; } = string.Empty;
        public int client_id { get; set; }
        public decimal crypto_amount { get; set; }
        public DateTime datetime { get; set; }
    }
}
