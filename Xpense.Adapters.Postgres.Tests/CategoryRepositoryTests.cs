using Dapper;
using FluentAssertions;
using Xpense.Adapters.Postgres.Models;
using Xpense.Adapters.Postgres.Persistence;
using Xpense.Adapters.Postgres.Repositories;
using Xunit.Abstractions;

namespace Xpense.Adapters.Postgres.Tests;

public class CategoryRepositoryTests(ITestOutputHelper outputHelper) : IntegrationTestSuite(outputHelper)
{
    private ICategoryRepository _repository = null!;

    protected override void Construct()
    {
        _repository = new CategoryRepository(Connection);
    }

    protected override async Task TruncateTable()
    {
        await Connection.ExecuteAsync("truncate table xpense.category cascade;");
    }

    [Fact]
    public async Task Create_WhenCalled_ReturnCreatedCategoryWithId()
    {
        // Arrange
        const int priorityId = 5;
        var createdOn = DateTimeOffset.UtcNow;
        var category = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = priorityId,
            Priority = null
        };

        // Act
        var result = await _repository.Create(category);

        // Assert
        result.Status.Should().Be(StorageResultStatus.Success);
        result.Data.Should().NotBeNull();

        result.Data.CategoryId.Should().BeGreaterThan(0);
        result.Data.CreatedOn.Should().Be(createdOn);
        result.Data.LastUpdated.Should().BeNull();
        result.Data.PriorityId.Should().Be(priorityId);
        result.Data.IsDeleted.Should().BeFalse();
        result.Data.Label.Should().Be(category.Label);
    }

    [Fact]
    public async Task Create_WhenCalledWithInvalidPriorityId_ReturnFailure()
    {
        // Arrange
        const int priorityId = 6;
        var createdOn = DateTimeOffset.UtcNow;
        var category = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = priorityId,
            Priority = null
        };

        // Act
        var result = await _repository.Create(category);

        // Assert
        result.Status.Should().Be(StorageResultStatus.Failure);
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task Create_CalledOnDuplicateCategoryLabels_ReturnFailure()
    {
        // Arrange
        const int priorityId = 5;
        var createdOn = DateTimeOffset.UtcNow;
        var category1 = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = priorityId,
            Priority = null
        };
        var category2 = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = priorityId,
            Priority = null
        };

        // Act
        var result = await _repository.Create(category1);
        var duplicate = await _repository.Create(category2);

        // Assert
        result.Status.Should().Be(StorageResultStatus.Success);
        result.Data.Should().NotBeNull();

        duplicate.Status.Should().Be(StorageResultStatus.Failure);
    }

    [Fact]
    public async Task Get_Called_ReturnAllCategoriesIncludingPriorities()
    {
        // Arrange
        var createdOn = DateTimeOffset.UtcNow;
        var category1 = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = 1,
            Priority = null
        };

        var category2 = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Traveling",
            PriorityId = 2,
            Priority = null
        };

        var category3 = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Shopping",
            PriorityId = 3,
            Priority = null
        };

        var category4 = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Grocery",
            PriorityId = 4,
            Priority = null
        };

        _ = await _repository.Create(category1);
        _ = await _repository.Create(category2);
        _ = await _repository.Create(category3);
        _ = await _repository.Create(category4);

        // Act

        var result = await _repository.Get();

        // Assert
        result.Status.Should().Be(StorageResultStatus.Success);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(4);

        var categories = result.Data.ToList();

        categories.Should().Contain(c => c.Label == "Food" && c.PriorityId == 1);
        categories.Should().Contain(c => c.Label == "Traveling" && c.PriorityId == 2);
        categories.Should().Contain(c => c.Label == "Shopping" && c.PriorityId == 3);
        categories.Should().Contain(c => c.Label == "Grocery" && c.PriorityId == 4);

        foreach (var category in categories)
        {
            category.CategoryId.Should().BeGreaterThan(0); // assuming inserted and ID generated
            category.LastUpdated.Should().BeNull();
            category.IsDeleted.Should().BeFalse();
            category.Label.Should().NotBeNullOrWhiteSpace();
            category.Priority.Should().NotBeNull();
            category.Priority.PriorityId.Should().Be(category.PriorityId);
            category.Priority.Label.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public async Task Get_CalledWithAllCategoriesDeleted_ReturnsNotFound()
    {
        // Arrange
        var createdOn = DateTimeOffset.UtcNow;
        var category1 = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = 1,
            Priority = null
        };

        var category2 = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Traveling",
            PriorityId = 2,
            Priority = null
        };

        var category3 = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Shopping",
            PriorityId = 3,
            Priority = null
        };

        var category4 = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Grocery",
            PriorityId = 4,
            Priority = null
        };

        var createdCategory1 = await _repository.Create(category1);
        createdCategory1.Status.Should().Be(StorageResultStatus.Success);
        createdCategory1.Data.Should().NotBeNull();

        var createdCategory2 = await _repository.Create(category2);
        createdCategory2.Status.Should().Be(StorageResultStatus.Success);
        createdCategory2.Data.Should().NotBeNull();

        var createdCategory3 = await _repository.Create(category3);
        createdCategory3.Status.Should().Be(StorageResultStatus.Success);
        createdCategory3.Data.Should().NotBeNull();

        var createdCategory4 = await _repository.Create(category4);
        createdCategory4.Status.Should().Be(StorageResultStatus.Success);
        createdCategory4.Data.Should().NotBeNull();

        await _repository.DeleteById(createdCategory1.Data.CategoryId);
        await _repository.DeleteById(createdCategory2.Data.CategoryId);
        await _repository.DeleteById(createdCategory3.Data.CategoryId);
        await _repository.DeleteById(createdCategory4.Data.CategoryId);

        // Act

        var result = await _repository.Get();

        // Assert
        result.Status.Should().Be(StorageResultStatus.NotFound);
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetById_Called_ReturnCategoryIncludingPriority()
    {
        // Arrange
        var createdOn = DateTimeOffset.UtcNow;
        var category = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = 1,
            Priority = null
        };

        // Act & Assert
        var createdCategory = await _repository.Create(category);
        createdCategory.Status.Should().Be(StorageResultStatus.Success);
        createdCategory.Data.Should().NotBeNull();

        var result = await _repository.GetById(createdCategory.Data.CategoryId);
        result.Status.Should().Be(StorageResultStatus.Success);
        result.Data.Should().NotBeNull();

        result.Data.CategoryId.Should().Be(createdCategory.Data.CategoryId);
        result.Data.LastUpdated.Should().BeNull();
        result.Data.IsDeleted.Should().BeFalse();
        result.Data.Label.Should().NotBeNullOrWhiteSpace();
        result.Data.Label.Should().Be("Food");
        result.Data.Priority.Should().NotBeNull();
        result.Data.Priority.PriorityId.Should().Be(createdCategory.Data.PriorityId);
        result.Data.Priority.Label.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetById_WithDeletedCategory_ReturnNotFound()
    {
        // Arrange
        var createdOn = DateTimeOffset.UtcNow;
        var category = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = 1,
            Priority = null
        };

        // Act & Assert
        var createdCategory = await _repository.Create(category);
        createdCategory.Status.Should().Be(StorageResultStatus.Success);
        createdCategory.Data.Should().NotBeNull();

        await _repository.DeleteById(createdCategory.Data.CategoryId);

        var result = await _repository.GetById(createdCategory.Data.CategoryId);
        result.Status.Should().Be(StorageResultStatus.NotFound);
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task DeleteById_CalledOnDeletedItem_ShouldDeleteCategory()
    {
        // Arrange
        var createdOn = DateTimeOffset.UtcNow;
        var category = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = 1,
            Priority = null
        };

        // Act & Assert
        var createdCategory = await _repository.Create(category);
        createdCategory.Status.Should().Be(StorageResultStatus.Success);
        createdCategory.Data.Should().NotBeNull();

        var result = await _repository.DeleteById(createdCategory.Data.CategoryId);

        result.Status.Should().Be(StorageResultStatus.Success);
    }

    [Fact]
    public async Task Restore_CalledOnDeletedCategory_ShouldRestoreCategory()
    {
        // Arrange
        var createdOn = DateTimeOffset.UtcNow;
        var category = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = 1,
            Priority = null
        };

        // Act & Assert
        var createdCategory = await _repository.Create(category);
        createdCategory.Status.Should().Be(StorageResultStatus.Success);
        createdCategory.Data.Should().NotBeNull();

        var deleteCategoryResult = await _repository.DeleteById(createdCategory.Data.CategoryId);
        deleteCategoryResult.Status.Should().Be(StorageResultStatus.Success);

        var restoreCategoryResult = await _repository.Restore(createdCategory.Data.CategoryId);
        restoreCategoryResult.Status.Should().Be(StorageResultStatus.Success);

        var restoredCategory = await _repository.GetById(createdCategory.Data.CategoryId);
        restoredCategory.Status.Should().Be(StorageResultStatus.Success);
        restoredCategory.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Restore_CalledOnNonExistingCategory_ShouldReturnNotFound()
    {
        // Arrange
        var restoreCategoryResult = await _repository.Restore(0);
        restoreCategoryResult.Status.Should().Be(StorageResultStatus.NotFound);
    }

    [Fact]
    public async Task IsDeleted_CalledOnDeletedCategory_ShouldReturnSuccess()
    {
        // Arrange
        var createdOn = DateTimeOffset.UtcNow;
        var category = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = 1,
            Priority = null
        };

        // Act & Assert
        var createdCategory = await _repository.Create(category);
        createdCategory.Status.Should().Be(StorageResultStatus.Success);
        createdCategory.Data.Should().NotBeNull();

        var deleteCategoryResult = await _repository.DeleteById(createdCategory.Data.CategoryId);
        deleteCategoryResult.Status.Should().Be(StorageResultStatus.Success);

        var result = await _repository.IsDeleted(createdCategory.Data.CategoryId);
        result.Status.Should().Be(StorageResultStatus.Success);
    }

    [Fact]
    public async Task IsDeleted_CalledOnNonExistingCategory_ShouldReturnNotFound()
    {
        // Arrange
        var result = await _repository.IsDeleted(0);
        result.Status.Should().Be(StorageResultStatus.NotFound);
    }

    [Fact]
    public async Task Exists_CalledOnExistingCategoryWithNoSoftDeletion_ShouldReturnSuccess()
    {
        // Arrange
        var createdOn = DateTimeOffset.UtcNow;
        var category = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = 1,
            Priority = null
        };

        // Act & Assert
        var createdCategory = await _repository.Create(category);
        createdCategory.Status.Should().Be(StorageResultStatus.Success);
        createdCategory.Data.Should().NotBeNull();

        var result = await _repository.Exists(createdCategory.Data.CategoryId);
        result.Status.Should().Be(StorageResultStatus.Success);
    }

    [Fact]
    public async Task Exists_CalledOnExistingCategoryWithSoftDeletion_ShouldReturnSuccess()
    {
        // Arrange
        var createdOn = DateTimeOffset.UtcNow;
        var category = new Category
        {
            CategoryId = 0,
            CreatedOn = createdOn,
            LastUpdated = null,
            IsDeleted = false,
            Label = "Food",
            PriorityId = 1,
            Priority = null
        };

        // Act & Assert
        var createdCategory = await _repository.Create(category);
        createdCategory.Status.Should().Be(StorageResultStatus.Success);
        createdCategory.Data.Should().NotBeNull();

        var result = await _repository.Exists(createdCategory.Data.CategoryId);
        result.Status.Should().Be(StorageResultStatus.Success);
    }

    [Fact]
    public async Task Exists_CalledOnNonExistingCategory_ShouldReturnSuccess()
    {
        var result = await _repository.Exists(0);
        result.Status.Should().Be(StorageResultStatus.NotFound);
    }
}