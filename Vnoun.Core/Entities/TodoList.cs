using MongoDB.Bson;
using MongoDB.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vnoun.Core.Entities
{
    public class TodoList : Entity
    {
        [Field("body")]
        public string Body { get; set; }

        [Field("done")]
        public bool Done { get; set; }

        [Field("createdAt")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("User")]
        public ObjectId UserId { get; set; }

        [Ignore]
        public User User { get; set; }
    }
}