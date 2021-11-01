using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServicioAPI.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("usuario")]
        public string usuario { get; set; }
        [BsonElement("contrasenia")]
        public string contrasenia { get; set; }

        [BsonElement("correo")]
        public string correo { get; set; }

        [BsonElement("nombre")]
        public string nombre { get; set; }
        [BsonElement("apellido")]
        public string apellido { get; set; }


        
    }
}
