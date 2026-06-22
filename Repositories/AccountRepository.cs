using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public List<SystemAccount> GetAccounts() => AccountDAO.Instance.GetAccounts();
        public SystemAccount? GetAccountById(short accountId) => AccountDAO.Instance.GetAccountById(accountId);
        public SystemAccount? GetAccountByEmail(string email) => AccountDAO.Instance.GetAccountByEmail(email);
        public void SaveAccount(SystemAccount account) => AccountDAO.Instance.SaveAccount(account);
        public void UpdateAccount(SystemAccount account) => AccountDAO.Instance.UpdateAccount(account);
        public void DeleteAccount(SystemAccount account) => AccountDAO.Instance.DeleteAccount(account);
    }
}
