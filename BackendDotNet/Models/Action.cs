using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace IoTHubAPI.Models
{
    public class Action
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string Id { get; set; }

        [Required]
        public string DeviceId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Code { get; set; }

    }
}
