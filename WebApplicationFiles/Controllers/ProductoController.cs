using Microsoft.AspNetCore.Mvc;
using WebApplicationFiles.Models;
using WebApplicationFiles.Services;
using Microsoft.EntityFrameworkCore;
using WebApplicationFiles.Models.dto;

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
            
            if(!ModelState.IsValid)
            {
                return View(productoDto);
            }

                //guardamos imagen
                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productoDto.ImageFileUpload!.FileName);

                string path = environment.WebRootPath + "/uploads/" + newFileName;
                using (var stream = System.IO.File.Create(path))
                {
                    productoDto.ImageFileUpload.CopyTo(stream);
                }

  
            Producto producto = new Producto()
            {
                Nombre = productoDto.Nombre,
                Precio = productoDto.Precio,
                ImageFile = newFileName
            };

            await _context.Productos.AddAsync(producto);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Producto producto = await _context.Productos.FindAsync(id);

            if(producto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Producto model)
        {
            //_context.Empleados.Update(model);

            //await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Destroy(int id)
        {
            //Empleado empleado = await _context.Empleados.FirstAsync(e => e.IdEmpleado == id);

            //_context.Empleados.Remove(empleado);

            //await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}

