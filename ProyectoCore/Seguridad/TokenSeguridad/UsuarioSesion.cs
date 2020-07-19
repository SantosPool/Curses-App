using System.Linq;
using System.Security.Claims;
using Aplicacion.Contratos;
using Microsoft.AspNetCore.Http;

namespace Seguridad
{
    public class UsuarioSesion : IUsuarioSesion
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public UsuarioSesion(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor=_httpContextAccessor;

        }
        public string ObtenerUsuarioSeion()
        {
            var userName= httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x=>x.Type== ClaimTypes.NameIdentifier)?.Value;
            return userName;
        }
    }
}