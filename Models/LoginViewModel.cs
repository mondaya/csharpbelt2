using System.ComponentModel.DataAnnotations;

namespace belt2.Models
{

    public class LoginViewModel : BaseEntity {
        
        [Key]    

       
        [RequiredAttribute]
        [EmailAddressAttribute]
        public string Email {get; set;}

        [RequiredAttribute]
        [MinLengthAttribute(8)]
        [DataType(DataType.Password)]
        public string Password {get; set;}




    }
}