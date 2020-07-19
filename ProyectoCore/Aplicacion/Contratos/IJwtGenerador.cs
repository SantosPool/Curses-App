using System.Collections.Generic;
using Dominio;

namespace Aplicacion.Contratos
{
    public interface IJwtGenerador
    {
         string CrearToken(Usuario usuario, List<string> roles);
    }
}