using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Aplicacion.Contratos;
using Dominio;
using Microsoft.IdentityModel.Tokens;

namespace Seguridad
{
    public class JwtGenerador : IJwtGenerador
    {
        public string CrearToken(Usuario usuario, List<string> roles)
        {
            var claims= new List<Claim>{ new Claim(JwtRegisteredClaimNames.NameId,usuario.UserName)};

            if( roles != null)
                foreach(var rol in roles)
                    claims.Add(new Claim(ClaimTypes.Role,rol));
                    

            var key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi Palabra Secreta"));
            var credenciales= new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            var tokenDescripcion= new SecurityTokenDescriptor{
                Subject=new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(30),
                SigningCredentials=credenciales
                };
            var tokenManejador= new JwtSecurityTokenHandler();
            var token= tokenManejador.CreateToken(tokenDescripcion);

            return tokenManejador.WriteToken(token);

        }
    }
}