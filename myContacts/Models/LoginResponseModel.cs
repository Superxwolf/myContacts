namespace myContacts.Models
{
    public class LoginResponseModel
    {
        public string token { get; set; }

        public LoginResponseModel(string token)
        {
            this.token = token;
        }
    }
}
