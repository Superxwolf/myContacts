using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myContacts.Models
{
    public class ContactModel
    {
        [Key]
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

        [Required]
        public DateTime LastUpdateDate { get; set; }

        [MaxLength(20)]
        [Required]
        public string LastUpdateUserName { get; set; }

        public (bool, string) IsValid()
        {
            if (!String.IsNullOrEmpty(Fax) && (Fax.Length != 10 || !IsDigitsOnly(Fax))) return (false, "Invalid Fax format");
            if (!String.IsNullOrEmpty(eMail) && (eMail.Length > 50 || !IsValidEmail(eMail))) return (false, "eMail format is invalid");

            return (true, null);
        }
        
        static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
