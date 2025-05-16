using System.Data;
using Dapper;
using Npgsql;
using Xpense.Adapters.Postgres.Models;
using Xpense.Adapters.Postgres.Persistence;

namespace Xpense.Adapters.Postgres.Repositories;

public class CategoryRepository(IDbConnection? connection) : ICategoryRepository
{
    public async Task<StorageResult<Category>> Create(Category category)
    {
        try
        {
            const string sql = """
                                Insert into Xpense.Category (category, created_on, last_modified, is_deleted, priority_id)
                                VALUES (@Category, @CreatedOn, null, false, @PriorityId)
                                Returning id;
                               """;
            var categoryId = await connection.ExecuteScalarAsync<int>(sql,
                new { @Category = category.Label, category.CreatedOn, category.PriorityId });
            return StorageResult<Category>.Success(category.With(categoryId: categoryId));
        }
        catch (PostgresException exception)
        {
            // TODO: Log the exception details
            return StorageResult<Category>.Failure(exception);
        }
    }

    public async Task<StorageResult<IEnumerable<Category>?>> Get(bool includeDeleted = false)
    {
        try
        {
            const string sql = """
                                Select 
                                    c.id as CategoryId,
                                    c.created_on as CreatedOn,
                                    c.last_modified as LastUpdated,
                                    c.is_deleted as IsDeleted,
                                    c.category as Label,
                                    p.id as PriorityId,
                                    p.created_on as CreatedOn,
                                    p.last_modified as LastUpdated,
                                    p.is_deleted  as IsDeleted,
                                    p.priority as Label,
                                    p.weight as Weight
                                From Xpense.Category c
                                join xpense.priority p on p.id = c.priority_id
                                where c.is_deleted = @IsDeleted;
                               """;

            var result = (await connection.QueryAsync<Category, Priority, Category>(sql,
                (category, priority) => category.With(priorityId: priority.PriorityId, priority: priority),
                new { IsDeleted = includeDeleted },
                splitOn: nameof(Priority.PriorityId)
            )).ToList();

            return result.Count == 0
                ? StorageResult<IEnumerable<Category>?>.NotFound
                : StorageResult<IEnumerable<Category>?>.Success(result);
        }
        catch (PostgresException exception)
        {
            // TODO Log the exception
            return StorageResult<IEnumerable<Category>?>.Failure(exception);
        }
    }

    public async Task<StorageResult<Category?>> GetById(int categoryId, bool includeDeleted = false)
    {
        try
        {
            const string sql = """
                                 Select 
                                     c.id as CategoryId,
                                     c.created_on as CreatedOn,
                                     c.last_modified as LastUpdated,
                                     c.is_deleted as IsDeleted,
                                     c.category as Label,
                                     p.id as PriorityId,
                                     p.created_on as CreatedOn,
                                     p.last_modified as LastUpdated,
                                     p.is_deleted  as IsDeleted,
                                     p.priority as Label,
                                     p.weight as Weight
                                 From Xpense.Category c
                                 join xpense.priority p on p.id = c.priority_id
                                 where c.id = @CategoryId and c.is_deleted = @IsDeleted;
                               """;

            var result = (await connection.QueryAsync<Category, Priority, Category>(sql,
                (category, priority) => category.With(priorityId: priority.PriorityId, priority: priority),
                new { CategoryId = categoryId, IsDeleted = includeDeleted },
                splitOn: nameof(Priority.PriorityId)
            )).ToList();

            return result.Count == 0
                ? StorageResult<Category?>.NotFound
                : StorageResult<Category?>.Success(result.Single());
        }
        catch (PostgresException exception)
        {
            // TODO Log the exception
            // You might have to simplify this StorageResult (e.g. something like StorageResult.Failure)
            return StorageResult<Category?>.Failure(exception);
        }
    }

    async Task<SimpleStorageResult> ICategoryRepository.DeleteById(int categoryId)
    {
        try
        {
            const string sql = "Update Xpense.Category Set is_deleted = true where id = @Id ";
            await connection.ExecuteAsync(sql, new { Id = categoryId });
            return SimpleStorageResult.Success;
        }
        catch (PostgresException exception)
        {
            // TODO Log the exception
            return SimpleStorageResult.Failure;
        }
    }

    public async Task<SimpleStorageResult> Restore(int categoryId)
    {
        try
        {
            var result = await Exists(categoryId);
            if (result.Status != StorageResultStatus.Success)
            {
                return SimpleStorageResult.NotFound;
            }

            const string sql = "Update Xpense.Category Set is_deleted = false where id = @Id ";
            await connection.ExecuteAsync(sql, new { Id = categoryId });
            return SimpleStorageResult.Success;
        }
        catch (PostgresException exception)
        {
            // TODO Log the exception
            return SimpleStorageResult.Failure;
        }
    }

    public async Task<SimpleStorageResult> IsDeleted(int categoryId)
    {
        try
        {
            const string sql = "select is_deleted from xpense.category where id = @Id ";
            var result = await connection.QuerySingleOrDefaultAsync<bool?>(sql, new { Id = categoryId });
            return result is null
                ? SimpleStorageResult.NotFound
                : SimpleStorageResult.Success;
        }
        catch (PostgresException exception)
        {
            // TODO Log the exception
            return StorageResult<bool>.Failure(exception);
        }
    }

    public async Task<SimpleStorageResult> Exists(int categoryId)
    {
        const string sql = "select * from xpense.category where id = @Id ";
        var result = await connection.QuerySingleOrDefaultAsync<bool?>(sql, new { Id = categoryId });
        return result is null
            ? SimpleStorageResult.NotFound
            : SimpleStorageResult.Success;
    }
}