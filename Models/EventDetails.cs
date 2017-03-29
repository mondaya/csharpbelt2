using System;
using System.Collections.Generic;



namespace belt2.Models
{

    public class EventDetails : BaseEntity
    {

        public int id { get; set; }


        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }


        public DateTime EventDateTime { get; set; }

        public string DurationStr { get; set; }
        
        public int userLoginId { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; }

        public List<Participant> Participants { get; set; }
        public EventDetails()
        {
            Participants = new List<Participant>();
        }

        
        public bool isUserIdParticipating(int id) {
            foreach(Participant user in Participants)
            {
                if(user.UserId == id)
                    return true;
            }

            return false;
        }



    }
}