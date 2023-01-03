using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace uniformesV51.Model
{
    [Index(nameof(OrgId), IsUnique = false)]
    public class Z110_Usuarios
    {
        [Key]
        public string UserId { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Paterno { get; set; } = null!;
        public string? Materno { get; set; }
        public int Nivel { get; set; } = UserNivel.Participante;
        public string OrgId { get; set; } = null!;
        public string? OldEmail { get; set; }
        public int Estado { get; set; } = 2;
        public bool Status { get; set; } = true;

    }
}
