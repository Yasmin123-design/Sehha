using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Hubs;
using E_PharmaHub.Models;
using E_PharmaHub.Services.UserServ;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Services.ChatServ
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserService _userService;
        public ChatService(IUnitOfWork unitOfWork, IHubContext<ChatHub> hubContext, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _userService = userService;
        }
        public async Task<MessageThreadDto> StartConversationWithPharmacistAsync(string userId, int pharmacistId)
        {
            var pharmacist = await _unitOfWork.PharmasistsProfile.GetByIdAsync(pharmacistId);
            if (pharmacist == null)
                throw new Exception("Pharmacist not found.");

            var pharmacistUserId = pharmacist.AppUserId;

            var existingThread = await _unitOfWork.Chat.GetThreadBetweenUsersAsync(userId, pharmacistUserId);
            if (existingThread != null)
            {
                return new MessageThreadDto
                {
                    Id = existingThread.Id,
                    Title = existingThread.Title,
                    ParticipantIds = existingThread.Participants.Select(p => p.UserId).ToList()
                };
            }

            var thread = new MessageThread
            {
                Title = $"Chat between {userId} and {pharmacistUserId}",
                Participants = new List<MessageThreadParticipant>
        {
            new MessageThreadParticipant { UserId = userId },
            new MessageThreadParticipant { UserId = pharmacistUserId }
        }
            };

            await _unitOfWork.MessageThread.AddAsync(thread);
            await _unitOfWork.CompleteAsync();

            return new MessageThreadDto
            {
                Id = thread.Id,
                Title = thread.Title,
                ParticipantIds = thread.Participants.Select(p => p.UserId).ToList()
            };
        }
        public async Task<MessageThreadDto> StartConversationWithDoctorAsync(string userId, int doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var doctorUserId = doctor.AppUserId;

            var existingThread = await _unitOfWork.Chat.GetThreadBetweenUsersAsync(userId, doctorUserId);
            if (existingThread != null)
            {
                return new MessageThreadDto
                {
                    Id = existingThread.Id,
                    Title = existingThread.Title,
                    ParticipantIds = existingThread.Participants.Select(p => p.UserId).ToList()
                };
            }

            var thread = new MessageThread
            {
                Title = $"Chat between {userId} and {doctorUserId}",
                Participants = new List<MessageThreadParticipant>
        {
            new MessageThreadParticipant { UserId = userId },
            new MessageThreadParticipant { UserId = doctorUserId }
        }
            };

            await _unitOfWork.MessageThread.AddAsync(thread);
            await _unitOfWork.CompleteAsync();

            return new MessageThreadDto
            {
                Id = thread.Id,
                Title = thread.Title,
                ParticipantIds = thread.Participants.Select(p => p.UserId).ToList()
            };
        }
        public async Task<ChatMessage> SendMessageAsync(int threadId, string senderId, string text)
        {
            var message = new ChatMessage
            {
                ThreadId = threadId,
                SenderId = senderId,
                Text = text
            };

            await _unitOfWork.Chat.AddAsync(message);
            await _unitOfWork.CompleteAsync();

            await _hubContext.Clients.Group(threadId.ToString())
     .SendAsync("ReceiveMessage", senderId, text, message.SentAt);

            return message;
        }
        public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(int threadId)
        {
            return await _unitOfWork.Chat.GetMessagesByThreadIdAsync(threadId);
        }

        public async Task<IEnumerable<ThreadDto>> GetUserThreadsAsync(string userId)
        {
            return await _unitOfWork.Chat.GetUserThreadsQueryable(userId)
                .ProjectToThreadDto()
                .ToListAsync();
        }

        public async Task<MessageThreadDto> StartConversationWithAdminAsync(string userId)
        {
            var adminUserId = await _userService.GetAdminUserIdAsync();
            if (string.IsNullOrEmpty(adminUserId))
                throw new Exception("Admin user not found.");

            var existingThread = await _unitOfWork.Chat.GetThreadBetweenUsersAsync(userId, adminUserId);
            if (existingThread != null)
            {
                return new MessageThreadDto
                {
                    Id = existingThread.Id,
                    Title = existingThread.Title,
                    ParticipantIds = existingThread.Participants.Select(p => p.UserId).ToList()
                };
            }

            var thread = new MessageThread
            {
                Title = $"Support Chat",
                Participants = new List<MessageThreadParticipant>
                {
                    new MessageThreadParticipant { UserId = userId },
                    new MessageThreadParticipant { UserId = adminUserId }
                }
            };

            await _unitOfWork.MessageThread.AddAsync(thread);
            await _unitOfWork.CompleteAsync();

            return new MessageThreadDto
            {
                Id = thread.Id,
                Title = thread.Title,
                ParticipantIds = thread.Participants.Select(p => p.UserId).ToList()
            };
        }
    }
}
