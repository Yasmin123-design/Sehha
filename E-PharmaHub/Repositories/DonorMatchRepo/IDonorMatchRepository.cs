using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories.DonorMatchRepo
{
    public interface IDonorMatchRepository
    {
        Task<IEnumerable<DonorMatch>> MatchDonorsForRequestAsync(int requestId);

    }
}
