namespace PCPartsAPI.Dtos
{
    public class LoginDto
    {
        // Formda "Email \ Kullanıcı_Adı" yazan birleşik alan için
        public string LoginIdentifier { get; set; }

        public string Password { get; set; }
    }
}