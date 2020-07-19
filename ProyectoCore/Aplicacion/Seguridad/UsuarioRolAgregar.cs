using System;
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
    public class UsuarioRolAgregar
    {
        public class Ejecuta : IRequest
        {
            public string UserName { get; set; }
            public string Rolnombre { get; set; }

        }

        public class EjecutaValida : AbstractValidator<Ejecuta>
        {
            public EjecutaValida()
            {
                RuleFor(x => x.UserName).NotEmpty();
                RuleFor(x => x.Rolnombre).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly UserManager<Usuario> userManager;
            private readonly RoleManager<IdentityRole> roleManager;
            public Manejador(UserManager<Usuario> _userManager, RoleManager<IdentityRole> _roleManager)
            {
                userManager = _userManager;
                roleManager = _roleManager;

            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var role = await roleManager.FindByNameAsync(request.Rolnombre);
                if (role is null)
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "el rol no existe" });
                var usuario = await userManager.FindByNameAsync(request.UserName);
                if (usuario is null)
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "el usuario no existe" });
                var resultado = await userManager.AddToRoleAsync(usuario, request.Rolnombre);
                if (resultado.Succeeded)
                    return Unit.Value;
                throw new Exception("No se pudo agregar el rol al usuario");
            }
        }

    }
}