using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using WebApplicationFiles.Models;
using WebApplicationFiles.Models.dto;
using WebApplicationFiles.Services;
using System.IO;

namespace WebApplicationFiles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoesAPIController : ControllerBase
    {
        private readonly AppDbPubContext _context;
        private readonly IWebHostEnvironment environment;

        public ProductoesAPIController(AppDbPubContext context, IWebHostEnvironment environment)
        {
            _context = context;
            this.environment = environment;
        }

        // GET: api/ProductoesAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        // GET: api/ProductoesAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // PUT: api/ProductoesAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return BadRequest();
            }

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProductoesAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(ProductoDto productoDto)
        {
            if ( (Request.Form["ImageFileUpload"] == "") )
            {

                Producto producto = new Producto()
                {
                    Nombre = productoDto.Nombre,
                    Precio = productoDto.Precio
                };

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetProducto", new { id = producto.IdProducto }, producto);
            }

                 //guardamos imagen
                 string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                 newFileName += Path.GetExtension(productoDto.ImageFileUpload!.FileName);

                 string path = environment.WebRootPath + "/uploads/" + newFileName;
                 using (var stream = System.IO.File.Create(path))
                 {
                productoDto.ImageFileUpload.CopyTo(stream);
                 }

                 Producto productoConArchivo = new Producto()
                 {
                    Nombre = productoDto.Nombre,
                    Precio = productoDto.Precio,
                    ImageFile = newFileName
                 };

            await _context.Productos.AddAsync(productoConArchivo);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducto", new { id = productoConArchivo.IdProducto }, productoConArchivo);
        }

        // DELETE: api/ProductoesAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }
    }
}
