using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServicioAPI.Models
{
    public class Contacto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("miusuario")]
        public string miusuario { get; set; }
        [BsonElement("micontacto")]
        public string micontacto { get; set; }
    }
}
