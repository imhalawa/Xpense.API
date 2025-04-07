using Xpense.Core.Entities;

namespace Xpense.Core.Abstract.Persistence;

public interface ITagRepository : IOptionRepository<Tag>
{
    int[] Exists(int[]? tagIds);
    Task<IEnumerable<Tag>> CreateRange(IEnumerable<Tag> tags);
}