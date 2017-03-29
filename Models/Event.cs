using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace belt2.Models {

    public class Event : BaseEntity {
        
        [Key]
        public int id {get; set;} 

        [RequiredAttribute()]
        [MinLengthAttribute(2)]
        public string Name {get; set;}          
       
        [RequiredAttribute()]
        [MinLengthAttribute(10)]
        public string Description {get; set;}     
        
        [RequiredAttribute()]           
        public DateTime CreatedAt {get; set;}


        [DateRange()] 
        [RequiredAttribute()]          
        public DateTime EventDateTime {get; set;}

        [GreaterThanMinAttribute(1)]      
        public int Duration {get; set;}

       [Range(1,3)]      
        public int DurationType {get; set;}

        public int UserId { get; set; }
        public User User { get; set; }

        
        public List<Participant> Participants { get; set; }

        public Event(){
            //CreatedAt = DateTime.Now;
            Participants = new List<Participant>(); 
            CreatedAt = DateTime.Now;
      
         }
        


    public class DateRangeAttribute : ValidationAttribute
    {
        private DateTime maxDate;

       public DateRangeAttribute()
        {
            maxDate = DateTime.Now;
        }

       protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime inputDate = (DateTime)value;
                if (inputDate < maxDate)
                {
                    return new ValidationResult("Enter Valid Date");
                }
            
            }

           return ValidationResult.Success;
        }
    }


    public class GreaterThanMinAttribute : ValidationAttribute
    {
        private int min;

       public GreaterThanMinAttribute(int min)
        {
            this.min = min;
        }

       protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                int inputValue = (int)value;
                if (inputValue < min)
                {
                    return new ValidationResult($"Must be greater than {min}");
                }
            
            }

           return ValidationResult.Success;
        }
    }
       



    }
}