namespace CriptoCarteraApi.DTOs
{
    public class TransaccionDTO
    {
        public int id { get; set; }
        public string crypto_code { get; set; } = string.Empty;
        public string action { get; set; } = string.Empty;
        public int client_id { get; set; }
        public string client_name { get; set; } = string.Empty;
        public decimal crypto_amount { get; set; }
        public decimal money { get; set; }
        public DateTime datetime { get; set; }
    }
}
