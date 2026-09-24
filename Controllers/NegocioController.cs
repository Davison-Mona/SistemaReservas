using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReservas.Data;
using SistemaReservas.Models;

namespace SistemaReservas.Controllers
{
    public class NegocioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NegocioController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUsuarioId() => HttpContext.Session.GetString("UsuarioId");

        // 1. PANEL PRINCIPAL
        public async Task<IActionResult> Index()
        {
            var usuarioId = GetUsuarioId();
            if (string.IsNullOrEmpty(usuarioId)) return RedirectToAction("Index", "Login");

            var negocio = await _context.Negocios.FirstOrDefaultAsync(n => n.UsuarioId == usuarioId);
            if (negocio == null) return RedirectToAction("Index", "Login");

            return View(negocio);
        }

        // 2. GESTIÓN DE DISPONIBILIDAD (Días Bloqueados)
        public async Task<IActionResult> Disponibilidad()
        {
            var usuarioId = GetUsuarioId();
            var negocio = await _context.Negocios.FirstOrDefaultAsync(n => n.UsuarioId == usuarioId);
            
            if (negocio == null) return RedirectToAction("Index", "Login");

            // Traemos solo los días bloqueados futuros de este negocio
            var diasBloqueados = await _context.DiasBloqueados
                .Where(d => d.NegocioId == negocio.Id && d.Fecha >= DateTime.Today)
                .OrderBy(d => d.Fecha)
                .ToListAsync();

            return View(diasBloqueados);
        }

        [HttpPost]
        public async Task<IActionResult> BloquearDia(DateTime fecha, string motivo)
        {
            var usuarioId = GetUsuarioId();
            var negocio = await _context.Negocios.FirstOrDefaultAsync(n => n.UsuarioId == usuarioId);

            // Evitamos duplicar bloqueos el mismo día
            bool yaExiste = await _context.DiasBloqueados
                .AnyAsync(d => d.NegocioId == negocio.Id && d.Fecha.Date == fecha.Date);

            if (!yaExiste)
            {
                var diaBloqueado = new DiaBloqueado
                {
                    NegocioId = negocio.Id,
                    Fecha = fecha,
                    Motivo = motivo ?? "No disponible"
                };
                
                _context.DiasBloqueados.Add(diaBloqueado);
                await _context.SaveChangesAsync();
                TempData["Exito"] = $"El día {fecha:dd/MM/yyyy} ha sido deshabilitado.";
            }
            else
            {
                TempData["Error"] = "Ese día ya está bloqueado.";
            }

            return RedirectToAction(nameof(Disponibilidad));
        }

        [HttpPost]
        public async Task<IActionResult> HabilitarDia(int id)
        {
            var dia = await _context.DiasBloqueados.FindAsync(id);
            if (dia != null)
            {
                _context.DiasBloqueados.Remove(dia);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "El día ha sido habilitado nuevamente para reservas.";
            }
            return RedirectToAction(nameof(Disponibilidad));
        }

        public async Task<IActionResult> Servicios()
        {
            var usuarioId = GetUsuarioId();
            var negocio = await _context.Negocios
                .Include(n => n.Servicios)
                .FirstOrDefaultAsync(n => n.UsuarioId == usuarioId);

            if (negocio == null) return RedirectToAction("Index", "Login");

            return View(negocio);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarServicio(int? id, string nombre, decimal precio, int duracionMinutos)
        {
            var usuarioId = GetUsuarioId();
            var negocio = await _context.Negocios.FirstOrDefaultAsync(n => n.UsuarioId == usuarioId);

            if (id.HasValue && id > 0)
            {
                // Editar servicio existente
                var servicio = await _context.Servicios.FirstOrDefaultAsync(s => s.Id == id && s.NegocioId == negocio.Id);
                if (servicio != null)
                {
                    servicio.Nombre = nombre;
                    servicio.Precio = precio;
                    servicio.DuracionMinutos = duracionMinutos;
                    TempData["Exito"] = "Servicio actualizado correctamente.";
                }
            }
            else
            {
                // Crear nuevo servicio
                _context.Servicios.Add(new Servicio
                {
                    NegocioId = negocio.Id,
                    Nombre = nombre,
                    Precio = precio,
                    DuracionMinutos = duracionMinutos
                });
                TempData["Exito"] = "Nuevo servicio agregado a tu catálogo.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Servicios));
        }

        [HttpPost]
        public async Task<IActionResult> EliminarServicio(int id)
        {
            var usuarioId = GetUsuarioId();
            var negocio = await _context.Negocios.FirstOrDefaultAsync(n => n.UsuarioId == usuarioId);
            
            var servicio = await _context.Servicios.FirstOrDefaultAsync(s => s.Id == id && s.NegocioId == negocio.Id);
            if (servicio != null)
            {
                _context.Servicios.Remove(servicio);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Servicio eliminado correctamente.";
            }
            return RedirectToAction(nameof(Servicios));
        }
    }
}