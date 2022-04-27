using System.ComponentModel.DataAnnotations;

namespace myContacts.Models
{
    public class UpdateContactRequestModel
    {
        [Required]
        public int ContactID { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [MinLength(10)]
        [MaxLength(10)]
        [Required]
        public string Phone { get; set; }

        [MaxLength(10)]
        public string? Fax { get; set; }

        [MaxLength(50)]
        public string? eMail { get; set; }

        public string? Notes { get; set; }
    }
}
