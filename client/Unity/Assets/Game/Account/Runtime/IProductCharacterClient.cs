using System.Threading;
using System.Threading.Tasks;

namespace LinhGioi.Account
{
    public interface IProductCharacterClient
    {
        Task<CharacterResponse[]> ListProductCharactersAsync(
            string accessToken, CancellationToken cancellationToken);

        Task<CharacterResponse> LoadProductCharacterAsync(
            string accessToken, string characterId, CancellationToken cancellationToken);
    }

    public static class ProductCharacterRoutes
    {
        public const string List = "/auth/characters";
        public const string LoadPrefix = "/auth/characters/";
    }
}
