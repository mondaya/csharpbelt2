using System;
using System.ComponentModel.DataAnnotations;


namespace belt2.Models
{

    public class Participant : BaseEntity
    {

        [Key]
        public int id { get; set; }

        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public Participant()
        {
            CreatedAt = DateTime.Now;
        }





    }
}