namespace E_PharmaHub.Dtos
{
    public class DoctorRegisterResponseDto
    {

        public string UserName { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public int DoctorProfileId { get; set; }

        public decimal ConsultationPrice { get; set; }
    }
}
