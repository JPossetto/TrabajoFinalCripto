using System.ComponentModel.DataAnnotations;

namespace CriptoCarteraApi.Models
{
    public class Cliente
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string email { get; set; } = string.Empty;

        public List<Transaccion> transacciones { get; set; } = new();
    }
}
