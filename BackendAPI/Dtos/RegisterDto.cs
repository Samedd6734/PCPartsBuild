namespace PCPartsAPI.Dtos
{
    public class RegisterDto
    {
        // Frontend'deki "username" input'undan gelecek
        public string Username { get; set; }

        // Frontend'deki "email" input'undan gelecek
        public string Email { get; set; }

        // Frontend'deki "password" input'undan gelecek
        public string Password { get; set; }
    }
}