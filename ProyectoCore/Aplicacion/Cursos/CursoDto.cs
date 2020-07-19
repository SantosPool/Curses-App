using System;
using System.Collections.Generic;

namespace Aplicacion.Cursos
{
    public class CursoDto
    {
        public Guid CursoId{get; set;}
        public string Titulo{get;set;}
        public string Descripcion{get;set;}
        public DateTime ? FechaPublicacion{get;set;}
        public byte[] FotoPortada{get;set;}
        public DateTime? FechaCreacion{get;set;}

        public List<InstructorDto> Instructores{get;set;}
        public PrecioDto Precio{get;set;}
         public List<ComentarioDto> Comentarios{get;set;}
    }
}