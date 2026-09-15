using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReservas.Data;
using SistemaReservas.Models;
using SistemaReservas.Services;

namespace SistemaReservas.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public LoginController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Login/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: Login/Entrar
        [HttpPost]
        public IActionResult Entrar(string nombreUsuario, string contrasena)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.NombreUsuario == nombreUsuario && u.Contrasena == contrasena);

            if (usuario == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos.";
                return View("Index");
            }

            // Redirigir al inicio del sistema tras autenticar
            return RedirectToAction("Index", "Home");
        }

        // GET: Login/OlvideContrasena
        public IActionResult OlvideContrasena()
        {
            return View();
        }

        // POST: Login/OlvideContrasena
        [HttpPost]
        public async Task<IActionResult> OlvideContrasena(string correo)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == correo);
            if (usuario == null)
            {
                ViewBag.Error = "El correo ingresado no se encuentra registrado.";
                return View();
            }

            // Generar token y expiración de 1 hora
            string token = Guid.NewGuid().ToString();
            usuario.TokenRestablecimiento = token;
            usuario.TokenExpiracion = DateTime.Now.AddHours(1);
            await _context.SaveChangesAsync();

            // Generar la URL de recuperación
            var link = Url.Action("RestablecerContrasena", "Login", new { token, email = correo }, Request.Scheme);

            string mensajeHtml = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                    <h2>Restablecimiento de Contraseña</h2>
                    <p>Has solicitado restablecer tu contraseña para el Sistema de Reservas.</p>
                    <p>Haz clic en el siguiente botón para continuar:</p>
                    <a href='{link}' style='display: inline-block; padding: 10px 20px; color: #fff; background-color: #192a3e; text-decoration: none; border-radius: 5px;'>Restablecer Contraseña</a>
                    <p style='margin-top: 15px; font-size: 12px; color: #777;'>Este enlace expirará en 1 hora.</p>
                </div>";

            try
            {
                await _emailService.EnviarCorreoAsync(correo, "Restablecer Contraseña - Sistema de Reservas", mensajeHtml);
                ViewBag.Exito = "Se ha enviado un enlace de recuperación a tu correo electrónico.";
            }
            catch
            {
                ViewBag.Error = "Ocurrió un error al enviar el correo. Verifica las credenciales SMTP en appsettings.json.";
            }

            return View();
        }

        // GET: Login/RestablecerContrasena
        public async Task<IActionResult> RestablecerContrasena(string token, string email)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == email && u.TokenRestablecimiento == token);

            if (usuario == null || usuario.TokenExpiracion < DateTime.Now)
            {
                TempData["Error"] = "El enlace es inválido o ha expirado.";
                return RedirectToAction("Index");
            }

            ViewBag.Token = token;
            ViewBag.Email = email;
            return View();
        }

        // POST: Login/RestablecerContrasena
        [HttpPost]
        public async Task<IActionResult> RestablecerContrasena(string token, string email, string nuevaContrasena)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == email && u.TokenRestablecimiento == token);

            if (usuario == null || usuario.TokenExpiracion < DateTime.Now)
            {
                ViewBag.Error = "El enlace es inválido o ha expirado.";
                return View();
            }

            usuario.Contrasena = nuevaContrasena;
            usuario.TokenRestablecimiento = null;
            usuario.TokenExpiracion = null;
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Contraseña actualizada correctamente. Ya puedes ingresar.";
            return RedirectToAction("Index");
        }
    }
}