using System.Collections.Immutable;
using System.Data;
using System.Text;
using Dapper;
using Npgsql;
using Xpense.Adapters.Postgres.Models;
using Xpense.Adapters.Postgres.Persistence;

namespace Xpense.Adapters.Postgres.Repositories;

public class TagRepository(IDbConnection connection) : ITagRepository
{
    private readonly NpgsqlConnection _connection =
        connection as NpgsqlConnection ?? throw new ArgumentNullException(nameof(connection));

    async Task<StorageResult<IImmutableDictionary<string, bool>>> ITagRepository.Exists(string[] labels)
    {
        try
        {
            var result = labels.ToDictionary(x => x, _ => false);
            const string sql = """
                                   select   id            as TagId,
                                           created_on    as CreatedOn,
                                           last_modified as LastModified,
                                           is_deleted    as IsDeleted,
                                           tag           as Label,
                                           bg_color_hex  as BgColorHex,
                                           fg_color_hex  as FgColorHex
                                   from xpense.tag t       
                                   where t.tag = any(@Labels)
                               """;

            var tags = await _connection.QueryAsync<Tag>(sql, new { @Labels = labels });
            var existingLabels = tags.Select(t => t.Label).Distinct().ToList();
            
            foreach (var label in existingLabels)
            {
                result[label] = true;
            }

            return StorageResult<IImmutableDictionary<string, bool>>.Success(result.ToImmutableDictionary());
        }
        catch (PostgresException exception)
        {
            // TODO: log the exception details
            return StorageResult<IImmutableDictionary<string, bool>>.Failure(exception);
        }
    }

    async Task<StorageResult<IImmutableList<Tag>>> ITagRepository.CreateRange(IImmutableList<Tag> tags)
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }

        await using var transaction = await _connection.BeginTransactionAsync();

        try
        {
            var (sql, parameters) = BuildTagCreationCommand(tags);
            var createdTags = await _connection.QueryAsync<Tag>(sql, parameters,
                transaction: transaction);
            await transaction.CommitAsync();
            return StorageResult<IImmutableList<Tag>>.Success(createdTags.ToImmutableList());
        }
        catch (PostgresException exception)
        {
            // TODO: Log exception details
            await transaction.RollbackAsync();
            return StorageResult<IImmutableList<Tag>>.Failure(exception);
        }
    }


    // public int[] Exists(int[]? tagIds)
    // {
    //     if (tagIds is null or { Length: 0 }) return Array.Empty<int>();
    //     return tagIds.Where(id => DbSet.Any(t => t.Id == id)).ToArray();
    // }

    public async Task<StorageResult<Tag>> GetByLabel(string label, bool includeDeleted = false)
    {
        try
        {
            const string sql = """
                                    select   id            as TagId,
                                           created_on    as CreatedOn,
                                           last_modified as LastModified,
                                           is_deleted    as IsDeleted,
                                           tag           as Label,
                                           bg_color_hex  as BgColorHex,
                                           fg_color_hex  as FgColorHex
                                   from xpense.tag t       
                                   where t.tag = @Label
                               """;
            var result = await _connection.QuerySingleOrDefaultAsync<Tag?>(sql, new { Label = label });

            return result == null
                ? StorageResult<Tag>.NotFound
                : StorageResult<Tag>.Success(result);
        }
        catch (PostgresException exception)
        {
            // TODO: Log the exception
            return StorageResult<Tag>.Failure(exception);
        }
    }

    public Task<SimpleStorageResult> Restore(string label)
    {
        throw new NotImplementedException();
    }

    public Task<StorageResult<Tag?>> GetOrCreateIfMissing(Tag tag)
    {
        throw new NotImplementedException();
    }

    // TODO: refactor and move to another place
    private static (string sql, DynamicParameters parameters) BuildTagCreationCommand(IImmutableList<Tag> tags)
    {
        var values = new StringBuilder();
        var dynamicParameters = new DynamicParameters();
        for (var i = 0; i < tags.Count; i++)
        {
            var createdOn = DateTimeOffset.UtcNow;
            values.Append('(');
            values.Append($"@CreatedOn{i}, "); // created_on
            values.Append("null, "); // last_modified
            values.Append("false, "); // is_deleted
            values.Append($"@Tag{i}, "); // tag
            values.Append($"@FgColorHex{i}, "); // fg_color_hex
            values.Append($"@BgColorHex{i}"); // bg_color_hex
            values.Append(')');
            if (i < tags.Count - 1)
                values.AppendLine(", ");

            dynamicParameters.Add($"CreatedOn{i}", createdOn);
            dynamicParameters.Add($"Tag{i}", tags[i].Label);
            dynamicParameters.Add($"FgColorHex{i}", tags[i].FgColorHex);
            dynamicParameters.Add($"BgColorHex{i}", tags[i].BgColorHex);
        }

        dynamicParameters.Add("TagLabels", tags.Select(t => t.Label).ToArray());

        var sql = $"""
                          with created_tags as 
                          (
                              insert into xpense.tag (created_on, last_modified, is_deleted, tag, fg_color_hex, bg_color_hex)
                              values 
                                  {values}
                              ON conflict (tag) do update
                                  set
                                      bg_color_hex = excluded.bg_color_hex,
                                      fg_color_hex = excluded.fg_color_hex,
                                      last_modified = now(),
                                      is_deleted = excluded.is_deleted
                                  where
                                      tag.bg_color_hex is distinct from excluded.bg_color_hex
                                          or tag.fg_color_hex is distinct from excluded.fg_color_hex
                                          or tag.is_deleted is distinct from excluded.is_deleted
                              returning *
                          )
                          select   id            as TagId,
                                   created_on    as CreatedOn,
                                   last_modified as LastModified,
                                   is_deleted    as IsDeleted,
                                   tag           as Label,
                                   bg_color_hex  as BgColorHex,
                                   fg_color_hex  as FgColorHex
                          from created_tags
                          union 
                          select   id            as TagId,
                                   created_on    as CreatedOn,
                                   last_modified as LastModified,
                                   is_deleted    as IsDeleted,
                                   tag           as Label,
                                   bg_color_hex  as BgColorHex,
                                   fg_color_hex  as FgColorHex
                           from xpense.tag t       
                           where t.tag = any (@TagLabels)
                           and t.tag not in (select tag from created_tags);
                   """;

        return (sql, dynamicParameters);
    }
}