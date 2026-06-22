using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;

namespace DataAccessObjects
{
    public class AccountDAO
    {
        private static AccountDAO? _instance = null;
        private static readonly object _instanceLock = new object();

        private AccountDAO() { }

        public static AccountDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new AccountDAO();
                    }
                    return _instance;
                }
            }
        }

        public List<SystemAccount> GetAccounts()
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.SystemAccounts.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAccounts: " + ex.Message);
            }
        }

        public SystemAccount? GetAccountById(short accountId)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.SystemAccounts.FirstOrDefault(a => a.AccountId == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAccountById: " + ex.Message);
            }
        }

        public SystemAccount? GetAccountByEmail(string email)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.SystemAccounts.FirstOrDefault(a => a.AccountEmail != null && a.AccountEmail.ToLower() == email.ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAccountByEmail: " + ex.Message);
            }
        }

        public void SaveAccount(SystemAccount account)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.SystemAccounts.Add(account);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SaveAccount: " + ex.Message);
            }
        }

        public void UpdateAccount(SystemAccount account)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Entry(account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in UpdateAccount: " + ex.Message);
            }
        }

        public void DeleteAccount(SystemAccount account)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var acc = context.SystemAccounts.FirstOrDefault(a => a.AccountId == account.AccountId);
                if (acc != null)
                {
                    context.SystemAccounts.Remove(acc);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DeleteAccount: " + ex.Message);
            }
        }
    }
}
