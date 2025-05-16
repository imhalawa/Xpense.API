using System.Diagnostics.CodeAnalysis;
using Dapper;
using FluentAssertions;
using Xpense.Adapters.Postgres.Models;
using Xpense.Adapters.Postgres.Persistence;
using Xpense.Adapters.Postgres.Repositories;
using Xunit.Abstractions;

namespace Xpense.Adapters.Postgres.Tests;

public class AccountRepositoryTests(ITestOutputHelper outputHelper) : IntegrationTestSuite(outputHelper)
{
    private IAccountRepository _repository = null!;

    [MemberNotNull(nameof(_repository))]
    protected override void Construct()
    {
        _repository = new AccountRepository(Connection);
    }

    protected override async Task TruncateTable()
    {
        const string truncateTable = "truncate Table Xpense.Account CASCADE;";
        _ = await Connection.ExecuteAsync(truncateTable);
    }


    [Fact]
    public async Task Create_Called_CreateAccountAndReturnId()
    {
        // Arrange
        var account = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = "0123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        // Act
        var result = await _repository.Create(account);

        // Assert
        result.Status.Should().Be(StorageResultStatus.Success);

        var createdAccount = result.Data;
        createdAccount.Should().NotBeNull();
        createdAccount.Id.Should().BeGreaterThan(0);
        createdAccount.AccountNumber.Should().Be("0123456489");
    }

    [Fact]
    public async Task GetById_Called_ReturnSuccessfulStorageResult()
    {
        // Arrange
        var account = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = "0123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        var createAccountResult = await _repository.Create(account);
        var createdAccount = createAccountResult.Data;

        // Act
        var result = await _repository.GetById(createdAccount!.Id);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(StorageResultStatus.Success);

        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Account1");
        result.Data.AccountNumber.Should().Be("0123456489");
        result.Data.Balance.Should().Be(123.34M);
        result.Data.IsDefaultAccount.Should().BeFalse();
    }

    [Fact]
    public async Task GetById_CalledOnArbitraryAccount_ReturnNotFoundStorageResult()
    {
        // Arrange
        const int accountId = -1;

        // Act
        var result = await _repository.GetById(accountId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(StorageResultStatus.NotFound);

        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetByAccountNumber_Called_ReturnSuccessfulStorageResult()
    {
        // Arrange
        const string accountNumber = "0123456489";
        var account = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = accountNumber,
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        _ = await _repository.Create(account);

        // Act
        var result = await _repository.GetByAccountNumber(accountNumber);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(StorageResultStatus.Success);

        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Account1");
        result.Data.AccountNumber.Should().Be("0123456489");
        result.Data.Balance.Should().Be(123.34M);
        result.Data.IsDefaultAccount.Should().BeFalse();
    }

    [Fact]
    public async Task GetByAccountNumber_CalledOnArbitraryAccount_ReturnNotFoundStorageResult()
    {
        // Arrange
        var accountNumber = string.Empty;

        // Act
        var result = await _repository.GetByAccountNumber(accountNumber);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(StorageResultStatus.NotFound);

        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetDefaultAccount_CalledWithDefaultAccountSet_ReturnsSuccessfulStorageResult()
    {
        // Arrange
        var defaultAccount = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = "0123456489",
            Balance = 123.34M,
            IsDefaultAccount = true,
        };

        var account2 = new Account
        {
            IsDeleted = false,
            Name = "Account" + 2,
            AccountNumber = "1123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        _ = await _repository.Create(defaultAccount);
        _ = await _repository.Create(account2);

        // Act
        var result = await _repository.GetDefaultAccount();

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(StorageResultStatus.Success);

        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Account1");
        result.Data.AccountNumber.Should().Be("0123456489");
        result.Data.Balance.Should().Be(123.34M);
    }

    [Fact]
    public async Task GetDefaultAccount_CalledWithDefaultAccountUnSet_ReturnsNotFoundStorageResult()
    {
        // Arrange
        var account1 = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = "0123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        var account2 = new Account
        {
            IsDeleted = false,
            Name = "Account" + 2,
            AccountNumber = "1123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        _ = await _repository.Create(account1);
        _ = await _repository.Create(account2);

        // Act
        var result = await _repository.GetDefaultAccount();

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(StorageResultStatus.NotFound);

        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task HasDefaultAccount_CalledWithDefaultAccountSet_ReturnsTrue()
    {
        // Arrange
        var defaultAccount = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = "0123456489",
            Balance = 123.34M,
            IsDefaultAccount = true,
        };

        var account2 = new Account
        {
            IsDeleted = false,
            Name = "Account" + 2,
            AccountNumber = "1123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        _ = await _repository.Create(defaultAccount);
        _ = await _repository.Create(account2);

        // Act
        var result = await _repository.HasDefaultAccount();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasDefaultAccount_CalledWithDefaultAccountUnSet_ReturnsNotFoundStorageResult()
    {
        // Arrange
        var account1 = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = "0123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        var account2 = new Account
        {
            IsDeleted = false,
            Name = "Account" + 2,
            AccountNumber = "1123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        _ = await _repository.Create(account1);
        _ = await _repository.Create(account2);

        // Act
        var result = await _repository.HasDefaultAccount();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteById_CalledOnExistingAccount_ReturnsSuccessfulStorageResult()
    {
        // Arrange
        var account = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = "0123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        var createAccountResult = await _repository.Create(account);
        var createdAccount = createAccountResult.Data;

        // Act
        var result = await _repository.DeleteById(createdAccount!.Id);
        var removedAccount = await _repository.GetById(createdAccount.Id);

        // Assert
        result.Status.Should().Be(StorageResultStatus.Success);

        removedAccount.Status.Should().Be(StorageResultStatus.NotFound);
        removedAccount.Data.Should().BeNull();
    }

    [Fact]
    public async Task DeleteById_CalledOnMissingAccount_ReturnsNotFoundStorageResult()
    {
        // Arrange
        var accountId = -1;

        // Act
        var result = await _repository.DeleteById(accountId);

        // Assert
        result.Status.Should().Be(StorageResultStatus.NotFound);
    }

    [Fact]
    public async Task DeleteByAccountNumber_CalledOnExistingAccount_ReturnsSuccessfulStorageResult()
    {
        // Arrange
        var account = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = "0123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        var createAccountResult = await _repository.Create(account);
        var createdAccount = createAccountResult.Data;

        // Act
        var result = await _repository.DeleteByAccountNumber(createdAccount!.AccountNumber);
        var removedAccount = await _repository.GetByAccountNumber(createdAccount.AccountNumber);

        // Assert
        result.Status.Should().Be(StorageResultStatus.Success);

        removedAccount.Status.Should().Be(StorageResultStatus.NotFound);
        removedAccount.Data.Should().BeNull();
    }

    [Fact]
    public async Task DeleteByAccountNumber_CalledOnMissingAccount_ReturnsNotFoundStorageResult()
    {
        // Arrange
        var accountNumber = string.Empty;

        // Act
        var result = await _repository.DeleteByAccountNumber(accountNumber);

        // Assert
        result.Status.Should().Be(StorageResultStatus.NotFound);
    }

    [Fact]
    public async Task Exists_CalledOnExistingAccount_ReturnsSuccessfulResult()
    {
        // Arrange
        var account = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = "0123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        var createAccountResult = await _repository.Create(account);
        var createdAccount = createAccountResult.Data;

        // Act
        var result = await _repository.Exists(createdAccount!.Id);

        // Assert
        result.Status.Should().Be(StorageResultStatus.Success);
    }

    [Fact]
    public async Task Exists_CalledOnMissingAccount_ReturnsSuccessfulResult()
    {
        // Arrange
        const int accountId = -1;

        // Act
        var result = await _repository.Exists(accountId);

        // Assert
        result.Status.Should().Be(StorageResultStatus.NotFound);
    }

    [Fact]
    public async Task IsDeleted_WhenCalledOnDeletedAccount_ReturnsTrue()
    {
        // Arrange
        var account = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = "0123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        var createAccountResult = await _repository.Create(account);
        var createdAccount = createAccountResult.Data;

        _ = await _repository.DeleteById(createdAccount!.Id);

        // Act
        var result = await _repository.IsDeleted(createdAccount!.Id);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public async Task IsDeleted_WhenCalledOnActiveAccount_ReturnsFalse()
    {
        // Arrange
        var account = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = "0123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        var createAccountResult = await _repository.Create(account);
        var createdAccount = createAccountResult.Data;

        // Act
        var result = await _repository.IsDeleted(createdAccount!.Id);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public async Task Restore_WhenCalledOnMissingAccount_ReturnsNotFoundStorageResult()
    {
        // Arrange
        const int accountId = -1;

        // Act
        var result = await _repository.Restore(accountId);

        // Assert
        result.Status.Should().Be(StorageResultStatus.NotFound);
    }

    [Fact]
    public async Task Restore_WhenCalledOnSoftDeletedAccount_ReturnsNotFoundStorageResult()
    {
        // Arrange
        // Arrange
        var account = new Account
        {
            IsDeleted = false,
            Name = "Account" + 1,
            AccountNumber = "0123456489",
            Balance = 123.34M,
            IsDefaultAccount = false,
        };

        var createAccountResult = await _repository.Create(account);
        var createdAccount = createAccountResult.Data;

        _ = await _repository.DeleteById(createdAccount!.Id);

        // Act
        var result = await _repository.Restore(createdAccount.Id);

        // Assert
        result.Status.Should().Be(StorageResultStatus.Success);
    }
}