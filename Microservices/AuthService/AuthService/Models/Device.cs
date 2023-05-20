﻿using IoTHubAPI.Models.Dtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTHubAPI.Models
{
    public class Device
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string Id { get; set; }

        [Required]
        public string ConnectionString { get; set; }

        [Required]
        public string Name { get; set; }

    }
}