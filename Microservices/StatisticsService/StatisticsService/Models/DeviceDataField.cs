using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace IoTHubAPI.Models
{
    public class DeviceDataField
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string Id { get; set; }

        [Required]
        public string DeviceId { get; set; }
        
        [Required]
        public string Name { get; set; }
        //public DataType Type { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public bool Statistics { get; set; }
        public string Error { get; set; } = null;
    }
}
