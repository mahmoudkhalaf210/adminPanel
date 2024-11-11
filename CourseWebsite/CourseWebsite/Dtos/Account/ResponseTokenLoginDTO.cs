namespace CourseWebsite.DTOs.Account
{
    public class ResponseTokenLoginDTO
    {
        public string Token { get; set; }
        public DateTime expire { get; set; }

        public int SuccessorNot { get; set; }
        public string Role { get; set; } = "";
        public string userName { get; set; } = "";
        public string key { get; set; } = "";




    }
}
