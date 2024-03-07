using MongoDB.Entities;
using System.ComponentModel.DataAnnotations;
using Vnoun.Core.Entities.MetaEntities;

namespace Vnoun.Core.Entities
{
    [Collection("location")]
    public class Location : Entity
    {
        [Field("information")]
        public InformationLocation Information { get; set; }

        [Field("phone")]
        [RegularExpression("\\d{3}-\\d{3}-\\d{4}")]
        public string Phone { get; set; }

        [Field("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Field("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Field("__v")]
        public int Version { get; set; } = 0;
    }
}
