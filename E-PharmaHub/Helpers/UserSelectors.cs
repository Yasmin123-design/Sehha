using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Helpers
{
    public static class UserSelectors
    {
        public static RegularUserDto ToRegularUserDto(this AppUser user)
        {
            return new RegularUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                Latitude = user.Latitude,
                Longitude = user.Longitude,
                ProfileImage = user.ProfileImage
            };
        }
    }

}
