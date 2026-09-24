using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReservas.Data;
using SistemaReservas.Models;
using System.Text.RegularExpressions;

namespace SistemaReservas.Controllers
{
    [Authorize(Roles = "Negocio,Admin")]
    public class MiNegocioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MiNegocioController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var negocio = await _context.Negocios
                .Include(n => n.Servicios)
                .Include(n => n.DiasBloqueados)
                .FirstOrDefaultAsync(n => n.UsuarioId == user.Id);

            // Si el dueño aún no tiene negocio creado en la BD, se lo crea automáticamente
            if (negocio == null)
            {
                negocio = new Negocio
                {
                    Nombre = "Mi Negocio",
                    Slug = GenerarSlug("Mi Negocio-" + Guid.NewGuid().ToString()[..4]),
                    UsuarioId = user.Id,
                    Activo = true
                };
                _context.Negocios.Add(negocio);
                await _context.SaveChangesAsync();
            }

            return View(negocio);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarNombre(int id, string nombre)
        {
            var negocio = await _context.Negocios.FindAsync(id);
            if (negocio != null)
            {
                negocio.Nombre = nombre;
                // Generar Slug amigable de la URL basado en el nombre
                string baseSlug = GenerarSlug(nombre);
                
                // Asegurar que el slug sea único
                if (await _context.Negocios.AnyAsync(n => n.Slug == baseSlug && n.Id != id))
                {
                    negocio.Slug = $"{baseSlug}-{id}";
                }
                else
                {
                    negocio.Slug = baseSlug;
                }

                await _context.SaveChangesAsync();
                TempData["Exito"] = "Nombre de negocio y enlace de agendamiento actualizados correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AgregarServicio(int negocioId, string nombre, decimal precio, int duracionMinutos)
        {
            var servicio = new Servicio
            {
                NegocioId = negocioId,
                Nombre = nombre,
                Precio = precio,
                DuracionMinutos = duracionMinutos > 0 ? duracionMinutos : 30
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Servicio agregado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> EliminarServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio != null)
            {
                _context.Servicios.Remove(servicio);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Servicio eliminado.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> BloquearDia(int negocioId, DateTime fecha, string motivo)
        {
            var existe = await _context.DiasBloqueados
                .AnyAsync(d => d.NegocioId == negocioId && d.Fecha.Date == fecha.Date);

            if (!existe)
            {
                var dia = new DiaBloqueado
                {
                    NegocioId = negocioId,
                    Fecha = fecha.Date,
                    Motivo = string.IsNullOrWhiteSpace(motivo) ? "No disponible" : motivo
                };
                _context.DiasBloqueados.Add(dia);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Día inhabilitado con éxito.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DesbloquearDia(int id)
        {
            var dia = await _context.DiasBloqueados.FindAsync(id);
            if (dia != null)
            {
                _context.DiasBloqueados.Remove(dia);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Día habilitado nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        private static string GenerarSlug(string texto)
        {
            string str = texto.ToLower().Trim();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }
    }
}