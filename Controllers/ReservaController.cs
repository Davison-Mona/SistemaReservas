using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReservas.Data;

namespace SistemaReservas.Controllers
{
    public class ReservaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acceso directo tipo: /agendar/barberia-urban-cuts
        [HttpGet("agendar/{slug}")]
        public async Task<IActionResult> ClienteView(string slug)
        {
            if (string.IsNullOrEmpty(slug)) return NotFound();

            var negocio = await _context.Negocios
                .Include(n => n.Servicios)
                .Include(n => n.DiasBloqueados)
                .FirstOrDefaultAsync(n => n.Slug.ToLower() == slug.ToLower() && n.Activo);

            if (negocio == null)
            {
                ViewBag.Mensaje = "El negocio no existe o se encuentra deshabilitado temporalmente.";
                return View("NoEncontrado");
            }

            return View(negocio);
        }
    }
}