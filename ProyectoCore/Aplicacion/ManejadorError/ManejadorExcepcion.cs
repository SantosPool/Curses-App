using System;
using System.Net;

namespace Aplicacion.ManejadorError
{
    public class ManejadorExcepcion: Exception
    {
        public HttpStatusCode Codigo{get;}
        public object Errores{get;}
        public ManejadorExcepcion(HttpStatusCode codigo,Object errores=null)
        {
            Codigo= codigo;
            Errores=errores;
        }
    }
}