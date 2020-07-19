using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class UsuarioActual
    {
        public class Ejecuta:IRequest<UsuarioData>
        {

        }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly UserManager<Usuario> userManager;
            private readonly IJwtGenerador jwtGenerador;
            private readonly IUsuarioSesion usuarioSesion;

            private readonly CursosOnlineContext context;
            public Manejador(UserManager<Usuario> _userManager, IJwtGenerador _jwtGenerador,IUsuarioSesion _usuarioSesion, CursosOnlineContext _context)
            {
                userManager=_userManager;
                jwtGenerador=_jwtGenerador;
                usuarioSesion=_usuarioSesion;
                context=_context;
            }
            public  async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var usuario = await userManager.FindByNameAsync(usuarioSesion.ObtenerUsuarioSeion());

                var listaRoles = await userManager.GetRolesAsync(usuario);
                
                var imagenPerfil= await context.Documento.Where(x=> x.ObjetoReferencia== new Guid(usuario.Id)).FirstOrDefaultAsync();
                ImagenGeneral imagenCliente=null;
                if(imagenPerfil!=null){
                    imagenCliente= new ImagenGeneral{
                        Data=Convert.ToBase64String(imagenPerfil.Contenido),
                        Extension=imagenPerfil.Extension,
                        Nombre=imagenPerfil.Nombre
                    };
                    
                }
                return new UsuarioData{
                    NombreCompleto=usuario.NombreCompleto,
                    UserName= usuario.UserName,
                    Token= jwtGenerador.CrearToken(usuario, listaRoles.ToList()),
                    Email=usuario.Email,
                    ImagenPerfil=imagenCliente
                };
            }
        }
    }
}