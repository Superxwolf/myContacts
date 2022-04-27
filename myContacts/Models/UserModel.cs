using System.ComponentModel.DataAnnotations;

namespace myContacts.Models
{
    public class UserModel
    {
        [Required]
        [MaxLength(20)]
        public string username { get; set; }
    }
}
