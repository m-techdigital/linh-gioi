using System.Threading;
using System.Threading.Tasks;

namespace LinhGioi.Account
{
    public interface IProductAccountClient
    {
        Task<ProductRegisterResponse> RegisterAsync(string email, string password, bool acceptedTerms,
            CancellationToken cancellationToken);
        Task<PasswordRecoveryRequestResponse> RequestPasswordRecoveryAsync(string email,
            CancellationToken cancellationToken);
        Task<PasswordRecoveryVerifyResponse> VerifyPasswordRecoveryAsync(string challengeId, string code,
            CancellationToken cancellationToken);
        Task ResetPasswordAsync(string resetToken, string newPassword, CancellationToken cancellationToken);
    }
}
