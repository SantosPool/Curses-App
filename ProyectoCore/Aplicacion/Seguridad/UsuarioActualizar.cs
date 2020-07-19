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
    public class UsuarioActualizar
    {
        public class Ejecuta : IRequest<UsuarioData>
        {
            public string NombreCompleto { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string UserName { get; set; }
            public ImagenGeneral ImagenPerfil { get; set; }
        }
        public class EjecutaValida : AbstractValidator<Ejecuta>
        {
            public EjecutaValida()
            {
                RuleFor(x => x.NombreCompleto).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty();
            }
        }
        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly IPasswordHasher<Usuario> passwordHasher;
            public readonly CursosOnlineContext context;
            public readonly UserManager<Usuario> userManager;
            public readonly IJwtGenerador jwtGenerador;
            public Manejador(CursosOnlineContext _context, UserManager<Usuario> _userManager, IJwtGenerador _jwtGenerador, IPasswordHasher<Usuario> _passwordHasher)
            {
                context = _context;
                userManager = _userManager;
                jwtGenerador = _jwtGenerador;
                passwordHasher = _passwordHasher;
            }
            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var usuario = await userManager.FindByNameAsync(request.UserName);

                if (usuario is null)
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No se encontro el usuario" });

                var existeEmail = await context.Users.Where(x => x.Email == request.Email && x.UserName != request.UserName).AnyAsync();
                if (existeEmail)
                    throw new ManejadorExcepcion(HttpStatusCode.InternalServerError, new { mensaje = "este email pertenece a otro usuario" });

                if (request.ImagenPerfil != null)
                {
                    var resultadoImagen = await context.Documento.Where(x => x.ObjetoReferencia == new Guid(usuario.Id)).FirstOrDefaultAsync();

                    if (resultadoImagen is null)
                    {
                        var imagen = new Documento
                        {
                            Contenido = Convert.FromBase64String(request.ImagenPerfil.Data),
                            Nombre = request.ImagenPerfil.Nombre,
                            Extension = request.ImagenPerfil.Extension,
                            ObjetoReferencia = new Guid(usuario.Id),
                            DocumentoId = Guid.NewGuid(),
                            FechaCreacion = DateTime.UtcNow
                        };
                        context.Documento.Add(imagen);
                    }
                    else
                    {
                        resultadoImagen.Contenido = Convert.FromBase64String(request.ImagenPerfil.Data);
                        resultadoImagen.Nombre = request.ImagenPerfil.Nombre;
                        resultadoImagen.Extension = request.ImagenPerfil.Extension;
                    }

                }

                usuario.NombreCompleto = request.NombreCompleto;
                usuario.PasswordHash = passwordHasher.HashPassword(usuario, request.Password);
                usuario.Email = request.Email;

                var resultado = await userManager.UpdateAsync(usuario);

                var usuarioRoles = await userManager.GetRolesAsync(usuario);

                var imagenPerfil = await context.Documento.Where(x => x.ObjetoReferencia == new Guid(usuario.Id)).FirstOrDefaultAsync();
                ImagenGeneral imagenCliente = null;
                if (imagenPerfil != null)
                {
                    imagenCliente = new ImagenGeneral
                    {
                        Data = Convert.ToBase64String(imagenPerfil.Contenido),
                        Extension = imagenPerfil.Extension,
                        Nombre = imagenPerfil.Nombre
                    };

                }

                if (resultado.Succeeded)
                    return new UsuarioData
                    {
                        NombreCompleto = usuario.NombreCompleto,
                        UserName = usuario.UserName,
                        Email = usuario.Email,
                        Token = jwtGenerador.CrearToken(usuario, usuarioRoles.ToList()),
                        ImagenPerfil= imagenCliente
                    };

                throw new Exception("No se pudo actualizar el usuario");

            }
        }
    }
}