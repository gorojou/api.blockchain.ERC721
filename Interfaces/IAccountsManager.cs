
using System.Threading.Tasks;

namespace WebApiMyGalleryPolygon.Interfaces
{
public interface IAccountsManager
    {
        string DefaultAccountAddress { get; }

        Task<string[]> GetAccountsAsync();

        Task<decimal> GetTokensAsync(string accountAddress);
        Task<decimal> GetBalanceInETHAsync(string accountAddress);

        // Task<TransactionModel[]> GetTransactionsAsync(bool sent = false);

        Task<string> TransferAsync(string from, string to, decimal amount);
    }
}