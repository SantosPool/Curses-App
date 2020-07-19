using System;
using System.Collections.Generic;
namespace Dominio
{
    public class Curso
    {
        public Guid CursoId{get; set;}
        public string Titulo{get;set;}
        public string Descripcion{get;set;}
        public DateTime ? FechaPublicacion{get;set;}
        public byte[] FotoPortada{get;set;}
        public DateTime? FechaCreacion{get;set;}
        public Precio PrecioPromocion{get;set;}
        public List<Comentario> ComentarioLista{get;set;}
        public List<CursoInstructor> InstructorLink{get;set;}
    }
}