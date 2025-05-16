using System.Data;
using Dapper;
using Xpense.Adapters.Postgres.Models;
using Xpense.Adapters.Postgres.Persistence;

namespace Xpense.Adapters.Postgres.Repositories;

public class AccountRepository(IDbConnection? connection) : IAccountRepository
{
    public async Task<StorageResult<Account>> Create(Account account)
    {
        const string sql = """
                               insert into Xpense.Account (is_deleted, created_on, last_modified, account, account_number, balance, is_default_account)
                               values (@IsDeleted, now(), null, @Account, @AccountNumber, @Balance, @IsDefaultAccount)
                               Returning id;
                           """;

        var accountId = await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                account.IsDeleted,
                account.AccountNumber,
                account.Balance,
                account.IsDefaultAccount,
                Account = account.Name,
            }
        );

        account.Id = accountId;

        return accountId > 0
            ? StorageResult<Account>.Success(account)
            : StorageResult<Account>.Failure();
    }

    public async Task<SimpleStorageResult> DeleteById(int accountId)
    {
        const string sql = "Update Xpense.Account Set is_deleted=true where id=@Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = accountId });
        return affectedRows == 1 ? SimpleStorageResult.Success : SimpleStorageResult.NotFound;
    }

    public async Task<SimpleStorageResult> DeleteByAccountNumber(string accountNumber)
    {
        const string sql =
            "Update Xpense.Account Set is_deleted=true where account_number=@AccountNumber";
        var affectedRows = await connection.ExecuteAsync(
            sql,
            new { AccountNumber = accountNumber }
        );
        return affectedRows == 1 ? SimpleStorageResult.Success : SimpleStorageResult.NotFound;
    }

    public async Task<SimpleStorageResult> Exists(int accountId)
    {
        const string sql = "select * from Xpense.Account where id=@Id";
        var result = await connection.QuerySingleOrDefaultAsync<Account>(
            sql,
            new { Id = accountId }
        );
        return result != null ? SimpleStorageResult.Success : SimpleStorageResult.NotFound;
    }

    public async Task<StorageResult<Account>> GetById(int accountId, bool excludeDeleted = true)
    {
        var sql = $"""
                       select 
                       id as Id,
                       is_deleted as IsDeleted,
                       created_on as CreatedOn,
                       last_modified as LastModified,
                       account as Name,
                       account_number as AccountNumber,
                       balance as Balance,
                       is_default_account as IsDefaultAccount
                       from Xpense.Account
                       where id=@Id {(excludeDeleted ? "and is_deleted != true" : "")}
                   """;
        var account = await connection.QuerySingleOrDefaultAsync<Account>(
            sql,
            new { Id = accountId }
        );
        return account == null
            ? StorageResult<Account>.NotFound
            : StorageResult<Account>.Success(account);
    }

    public async Task<StorageResult<Account>> GetByAccountNumber(string accountNumber, bool excludeDeleted = true)
    {
        var sql = $"""
                       select 
                       id as Id,
                       is_deleted as IsDeleted,
                       created_on as CreatedOn,
                       last_modified as LastModified,
                       account as Name,
                       account_number as AccountNumber,
                       balance as Balance,
                       is_default_account as IsDefaultAccount
                       from Xpense.Account
                       where account_number=@AccountNumber {(excludeDeleted ? "and is_deleted != true" : "")}
                   """;
        var account = await connection.QuerySingleOrDefaultAsync<Account>(
            sql,
            new { AccountNumber = accountNumber }
        );
        return account == null
            ? StorageResult<Account>.NotFound
            : StorageResult<Account>.Success(account);
    }

    public async Task<StorageResult<Account>> GetDefaultAccount(bool excludeDeleted = true)
    {
        var sql = $"""
                       select 
                       id as Id,
                       is_deleted as IsDeleted,
                       created_on as CreatedOn,
                       last_modified as LastModified,
                       account as Name,
                       account_number as AccountNumber,
                       balance as Balance,
                       is_default_account as IsDefaultAccount
                       from Xpense.Account
                       where is_default_account=true {(excludeDeleted ? "and is_deleted != true" : "")}
                   """;
        var account = await connection.QuerySingleOrDefaultAsync<Account>(sql);
        return account == null
            ? StorageResult<Account>.NotFound
            : StorageResult<Account>.Success(account);
    }

    public Task<StorageResult<string>> GetNextAccountNumber()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> HasDefaultAccount() =>
        (await GetDefaultAccount()).Status == StorageResultStatus.Success;

    public async Task<bool> IsDeleted(int accountId)
    {
        const string sql = "select is_deleted from Xpense.Account where id = @Id";
        var result = await connection.QuerySingleOrDefaultAsync<bool?>(sql, new { Id = accountId });
        return result is null or true;
    }

    public async Task<SimpleStorageResult> Restore(int accountId)
    {
        var result = await GetById(accountId, false);
        if (result.Status != StorageResultStatus.Success)
        {
            return SimpleStorageResult.NotFound;
        }

        const string sql = "update Xpense.Account set is_deleted = false where id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = accountId });
        return affectedRows == 1 ? SimpleStorageResult.Success : SimpleStorageResult.Failure;
    }
}