using System;

namespace Aplicacion.Cursos
{
    public class ComentarioDto
    {
        public Guid ComentarioId{get;set;}
        public string Alumno{get;set;}
        public int puntaje {get;set;}
        public string ComentarioTexto{get;set;}
        public Guid CursoId{get;set;}
        public DateTime? FechaCreacion{get;set;}
        
    }
}