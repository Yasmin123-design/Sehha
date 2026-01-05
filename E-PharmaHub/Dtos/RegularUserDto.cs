namespace E_PharmaHub.Dtos
{
    public class RegularUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ProfileImage { get; set; }
        public string? PhoneNumber { get; set; }
    }

}
