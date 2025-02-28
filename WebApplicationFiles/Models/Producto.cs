using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationFiles.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [MaxLength(100)]
        public string Nombre { get; set; } = "";

        [Precision(16, 2)]
        public decimal Precio { get; set; }

        [MaxLength(100)]
        public string ImageFile { get; set; } = "";

    }
}
