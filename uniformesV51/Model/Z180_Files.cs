using System.ComponentModel.DataAnnotations;

namespace uniformesV51.Model
{
    public class Z180_Files 
    {
        [Key]
        public string FileId { get; set; } = Guid.NewGuid().ToString();
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Fuente { get; set; } = null!;
        public string FuenteId { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string Archivo { get; set; } = null!;
        public string? Gpo { get; set; }
        public string Org { get; set; } = null!;
        public int Estado { get; set; } = 2;
        public bool Status { get; set; } = true;
    }
}
