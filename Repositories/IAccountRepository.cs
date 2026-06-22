using System.Collections.Generic;
using BusinessObjects;

namespace Repositories
{
    public interface IAccountRepository
    {
        List<SystemAccount> GetAccounts();
        SystemAccount? GetAccountById(short accountId);
        SystemAccount? GetAccountByEmail(string email);
        void SaveAccount(SystemAccount account);
        void UpdateAccount(SystemAccount account);
        void DeleteAccount(SystemAccount account);
    }
}
