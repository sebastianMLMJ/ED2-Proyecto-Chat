using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServicioAPI.Models
{
    public class SolicitudContacto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("emisor")]
        public string emisor { get; set; }
        [BsonElement("receptor")]
        public string receptor { get; set; }
        [BsonElement("status")]
        public int status { get; set; }
    }
}
