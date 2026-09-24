using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReservas.Data;
using SistemaReservas.Models;

namespace SistemaReservas.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Index -> Carga los datos en el panel
        public async Task<IActionResult> Index()
        {
            var negocios = await _context.Negocios.ToListAsync();

            ViewBag.TotalNegocios = negocios.Count;
            ViewBag.Activos = negocios.Count(n => n.Activo);
            ViewBag.Inactivos = negocios.Count(n => !n.Activo);

            return View(negocios);
        }

        // POST: Admin/Crear -> Inserta Dueño y Negocio vinculados
        [HttpPost]
        public async Task<IActionResult> Crear(string nombre, string correo, string contrasena, string telefono, string direccion)
        {
            // Validar que el correo no exista
            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == correo))
            {
                TempData["Error"] = "El correo ya está registrado para otro usuario.";
                return RedirectToAction("Index");
            }

            // 1. Crear Credenciales del Dueño con Rol 2
            var usuario = new Usuario
            {
                NombreUsuario = correo,
                Contrasena = contrasena,
                Rol = "2" // Asigna el Rol 2 explícitamente
            };
            
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(); // Se guarda y se genera el Id del usuario

            // Generar el Slug automáticamente desde el nombre (ejemplo: "Peluquería Central" -> "peluqueria-central")
            string slug = nombre.Trim().ToLower()
                .Replace(" ", "-")
                .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                .Replace("ñ", "n");

            // 2. Crear el Negocio con todos sus campos obligatorios amarrados al Usuario
            var negocio = new Negocio
            {
                Nombre = nombre,
                Correo = correo,
                Telefono = telefono,
                Direccion = direccion,
                Slug = slug,
                Activo = true,
                UsuarioId = usuario.Id.ToString() // Amarra el negocio al ID del dueño creado
            };

            _context.Negocios.Add(negocio);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Negocio y usuario vinculados correctamente.";
            return RedirectToAction("Index");
        }

        // POST: Admin/Editar -> Modifica los datos de un negocio
        [HttpPost]
        public async Task<IActionResult> Editar(int id, string nombre, string telefono, string direccion)
        {
            var negocio = await _context.Negocios.FindAsync(id);
            if (negocio != null)
            {
                negocio.Nombre = nombre;
                negocio.Telefono = telefono;
                negocio.Direccion = direccion;
                
                // Actualizar el slug si cambió el nombre
                negocio.Slug = nombre.Trim().ToLower()
                    .Replace(" ", "-")
                    .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                    .Replace("ñ", "n");

                await _context.SaveChangesAsync();
                TempData["Exito"] = "Datos del negocio actualizados correctamente.";
            }
            else
            {
                TempData["Error"] = "No se encontró el negocio a editar.";
            }
            return RedirectToAction("Index");
        }

        // POST: Admin/CambiarEstado -> Habilita o Deshabilita
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var negocio = await _context.Negocios.FindAsync(id);
            if (negocio != null)
            {
                negocio.Activo = !negocio.Activo;
                await _context.SaveChangesAsync();
                TempData["Exito"] = $"Estado de {negocio.Nombre} cambiado a {(negocio.Activo ? "Activo" : "Inactivo")}.";
            }
            return RedirectToAction("Index");
        }
    }
}