using System.Threading;
using System.Threading.Tasks;

namespace LinhGioi.Account
{
    public interface IProductAuthClient
    {
        Task<ProductLoginResponse> LoginAsync(string identifier, string password, CancellationToken cancellationToken);
        Task<ProductSessionResponse> ValidateSessionAsync(string accessToken, CancellationToken cancellationToken);
        Task LogoutAsync(string accessToken, CancellationToken cancellationToken);
    }
}
