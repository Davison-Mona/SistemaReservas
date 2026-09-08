using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReservas.Data;

namespace SistemaReservas.Controllers
{
    public class NegociosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NegociosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var listaNegocios = await _context.Negocios.ToListAsync();
            return View(listaNegocios);
        }
    }
}