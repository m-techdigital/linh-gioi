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

        Task<CharacterResponse> SaveMap01AStateAsync(
            string accessToken, string characterId, float laneX, int facing, CancellationToken cancellationToken);
    }

    public static class ProductCharacterRoutes
    {
        public const string List = "/auth/characters";
        public const string LoadPrefix = "/auth/characters/";
        public const string Map01AStateSuffix = "/map01a-state";
    }
}
