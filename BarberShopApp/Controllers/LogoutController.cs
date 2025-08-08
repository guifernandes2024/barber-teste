using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BarberShopApp.Data;

namespace BarberShopApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok(new { success = true, message = "Logout realizado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Erro ao fazer logout" });
            }
        }
    }
}
