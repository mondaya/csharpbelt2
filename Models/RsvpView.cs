using System;
using System.ComponentModel.DataAnnotations;


namespace belt2.Models
{

    public class RsvpView : BaseEntity
    {


        [RequiredAttribute()]
        public string RsvpName { get; set; }

        public char RsvpSide { get; set; }

    }
}