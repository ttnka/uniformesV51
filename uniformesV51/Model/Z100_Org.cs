using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace uniformesV51.Model
{
    [Index(nameof(Rfc), IsUnique = true)]
    public class Z100_Org
    {
        [Key]
        public string OrgId { get; set; } = Guid.NewGuid().ToString();
        [Required, MinLength(10)]
        public string Rfc { get; set; } = null!;
        [Required]
        public string Comercial { get; set; } = null!;
        [Required]
        public bool Moral { get; set; } = true;
        public string? Nombre { get; set; }
        public string? Paterno { get; set; }
        public string? Materno { get; set; }
        public string? RazonSocial { get; set; }
        public int Estado { get; set; } = 2;
        public bool Status { get; set; } = true;

    }
}
