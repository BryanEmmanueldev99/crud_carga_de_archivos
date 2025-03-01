using Microsoft.AspNetCore.Mvc;
using WebApplicationFiles.Models;
using WebApplicationFiles.Services;
using Microsoft.EntityFrameworkCore;
using WebApplicationFiles.Models.dto;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplicationFiles.Controllers
{
    public class ProductoController : Controller
    {

        private readonly AppDbPubContext _context;
        private readonly IWebHostEnvironment environment;

        public ProductoController(AppDbPubContext context, IWebHostEnvironment environment)
        {
            this._context = context;
            this.environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Producto> listar = await _context.Productos.ToListAsync();
            return View(listar);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(ProductoDto productoDto)
        {

            if (!ModelState.IsValid)
            {
                return View(productoDto);
            }


            if ((Request.Form["ImageFileUpload"] == "") ) 
            {
                //Console.WriteLine("Vacio...");
                
                Producto producto = new Producto()
                {
                    Nombre = productoDto.Nombre,
                    Precio = productoDto.Precio
                };

                await _context.Productos.AddAsync(producto);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
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

            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //Puedo recuperar los datos de la entidad asi también, pero como ocupo traerme el archivo
            //entonces hare asi
            //Producto producto = await _context.Productos.FindAsync(id);

            //if(producto == null)
            //{
            //    return RedirectToAction(nameof(Index));
            //}

            //return View(producto);

           
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var productoDto = new ProductoDto()
            {
                Nombre = producto.Nombre,
                Precio = producto.Precio,
            };

            ViewData["IdProducto"] = producto.IdProducto;
            ViewData["ImageFile"] = producto.ImageFile;

            return View(productoDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ProductoDto productoDto)
        {
            var producto = await _context.Productos.FindAsync(id);
            //Producto producto = await _context.Productos.FirstAsync(e => e.IdProducto == id);

            if (producto == null)
            {
                Console.WriteLine("No encontrado");
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                ViewData["IdProducto"] = producto.IdProducto;
                ViewData["ImageFile"] = producto.ImageFile;
                return View(productoDto);
            }

            string newFileName = producto.ImageFile;

            if (productoDto.ImageFileUpload != null)
            {
                //guardamos imagen
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productoDto.ImageFileUpload.FileName);

                string path = environment.WebRootPath + "/uploads/" + newFileName;

                using (var stream = System.IO.File.Create(path))
                {
                    productoDto.ImageFileUpload.CopyTo(stream);
                }

                //borro el archivo anterior
                string oldPath = environment.WebRootPath + "/uploads/" + producto.ImageFile;
                System.IO.File.Delete(oldPath);
            }

            //actualizamos la informacion
            producto.Nombre = productoDto.Nombre;
            producto.Precio = productoDto.Precio;
            producto.ImageFile = newFileName;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Destroy(int id)
        {

            var producto = await _context.Productos.FindAsync(id);


             if(producto.ImageFile != "")
            {
                //borro el archivo anterior
                string oldPath = environment.WebRootPath + "/uploads/" + producto.ImageFile;
                System.IO.File.Delete(oldPath);
            }

            Console.WriteLine("No hay archivos existentes para borrar.");

            _context.Productos.Remove(producto);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}

