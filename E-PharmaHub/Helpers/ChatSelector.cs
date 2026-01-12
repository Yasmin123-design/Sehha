using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using System.Linq;

namespace E_PharmaHub.Helpers
{
    public static class ChatSelector
    {
        public static IQueryable<ThreadDto> ProjectToThreadDto(this IQueryable<MessageThread> query)
        {
            return query.Select(t => new ThreadDto
            {
                Id = t.Id,
                Title = t.Title,
                Participants = t.Participants.Select(p => new ParticipantDto
                {
                    UserId = p.UserId,
                    UserName = p.User.UserName ?? "Unknown",
                    ProfileImage = p.User.ProfileImage
                }).ToList(),
                LastMessage = t.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => new LastMessageDto
                    {
                        Text = m.Text,
                        SentAt = m.SentAt
                    })
                    .FirstOrDefault()
            });
        }
    }
}
