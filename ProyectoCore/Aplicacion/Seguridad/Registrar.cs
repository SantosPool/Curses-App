using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class Registrar
    {
        public class Ejecuta: IRequest<UsuarioData>
        {
            public string NombreCompleto{get;set;}
            public string Email{get;set;}
            public string Password{get;set;}
            public string UserName{get;set;}
        }

        public class EjecutaValidador:AbstractValidator<Ejecuta>
        {
            public EjecutaValidador()
            {
                RuleFor(i=>i.NombreCompleto).NotEmpty();
                RuleFor(i=>i.Email).NotEmpty();
                RuleFor(i=>i.Password).NotEmpty();
                RuleFor(i=>i.UserName).NotEmpty();
            }
            
        }

        public class Manejador: IRequestHandler<Ejecuta,UsuarioData>
        {
            private readonly CursosOnlineContext context;
            private readonly UserManager<Usuario> userManager;
            private readonly IJwtGenerador jwtGenerador;
            public Manejador(CursosOnlineContext _context, UserManager<Usuario> _userManager, IJwtGenerador _jwtGenerador){
                context=_context;
                userManager=_userManager;
                jwtGenerador=_jwtGenerador;
            }
            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var existe= await context.Users.Where(u=>u.Email== request.Email).AnyAsync();
                if(existe)
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest,new{mensaje="El Usuario ya Existe con este email"});

                var existeUserName= await context.Users.Where(p=> p.UserName== request.UserName).AnyAsync();
                if(existeUserName)
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest,new{mensaje="El UserName ya fue registrado"});

                var usuario= new Usuario{
                    NombreCompleto= request.NombreCompleto,
                    Email=request.Email,
                    UserName=request.UserName
                };

                var resultado= await userManager.CreateAsync(usuario,request.Password);
                
                if(resultado.Succeeded){
                    return new UsuarioData{
                        NombreCompleto= usuario.NombreCompleto,
                        Token=jwtGenerador.CrearToken(usuario,null),
                        UserName=usuario.UserName,
                        Email= usuario.Email
                    };
                }

                throw new Exception("No se pudo agregar al nuevo usuario");
            }
        }
    }
}