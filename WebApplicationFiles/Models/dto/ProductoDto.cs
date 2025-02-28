using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationFiles.Models.dto
{
    public class ProductoDto
    {

        [Required,MaxLength(100)]
        public string Nombre { get; set; } = "";

        [Required, Precision(16, 2)]
        public decimal Precio { get; set; }

       
        public IFormFile? ImageFileUpload { get; set; }

    }
}
