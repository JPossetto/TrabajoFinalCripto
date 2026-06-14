using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CriptoCarteraApi.Models
{
    public class Transaccion
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string crypto_code { get; set; } = string.Empty;

        [Required]
        public string action { get; set; } = string.Empty;

        public int client_id { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal crypto_amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal money { get; set; }

        public DateTime datetime { get; set; }

        public Cliente? cliente { get; set; }
    }
}
