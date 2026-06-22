using System.Collections.Generic;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;

        public AccountService()
        {
            _repository = new AccountRepository();
        }

        public AccountService(IAccountRepository repository)
        {
            _repository = repository;
        }

        public List<SystemAccount> GetAccounts() => _repository.GetAccounts();
        public SystemAccount? GetAccountById(short accountId) => _repository.GetAccountById(accountId);
        public SystemAccount? GetAccountByEmail(string email) => _repository.GetAccountByEmail(email);
        public void SaveAccount(SystemAccount account) => _repository.SaveAccount(account);
        public void UpdateAccount(SystemAccount account) => _repository.UpdateAccount(account);
        public void DeleteAccount(SystemAccount account) => _repository.DeleteAccount(account);
    }
}
