using System.Threading.Tasks;
using Nethereum.HdWallet;

namespace WebApiMyGalleryPolygon.Interfaces
{
    public interface IWalletManager
    {
        Wallet Wallet { get; }

        Task CreateWalletAsync();

        Task SaveWalletAsync(string password);

        Task<bool> UnlockWalletAsync(string password, bool bypass = false);

        Task<bool> RestoreWallet(string seedWords, string password);
    }
}
