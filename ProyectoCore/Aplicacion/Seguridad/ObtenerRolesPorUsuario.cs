using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class ObtenerRolesPorUsuario
    {
        public class Ejecuta: IRequest<List<string>>
        {
            public string UserName{get;set;}
        }
        public class EjecutaValida:AbstractValidator<Ejecuta>
        {
            public EjecutaValida()
            {
                RuleFor(x=> x.UserName).NotEmpty();
            }
        }
        public class Manejador : IRequestHandler<Ejecuta, List<string>>
        {
            private readonly UserManager<Usuario> userManager;
            private readonly RoleManager<IdentityRole> roleManager;

            public Manejador(UserManager<Usuario> _userManager, RoleManager<IdentityRole> _roleManager)
            {
                userManager = _userManager;
                roleManager = _roleManager;

            }
            public async Task<List<string>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var usuario= await userManager.FindByNameAsync(request.UserName);
                if(usuario is null)
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No existe el usuario" });
                
                var resultado= await userManager.GetRolesAsync(usuario);

                return new List<string>(resultado);
                
            }
        }
    }
}