using System.Collections.Immutable;
using Xpense.Adapters.Postgres.Models;

namespace Xpense.Adapters.Postgres.Persistence;

public interface ITagRepository : IOptionRepository<Tag>
{
    Task<StorageResult<IImmutableDictionary<string, bool>>> Exists(string[] multipleTagsIds);
    Task<StorageResult<IImmutableList<Tag>>> CreateRange(IImmutableList<Tag> tags);
}