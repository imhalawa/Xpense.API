using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Models;

namespace Xpense.Core.Interfaces.Persistence;

public interface ITagRepository : IOptionRepository<Tag>
{
    int[] Exists(int[]? tagIds);
    Task<IEnumerable<Tag>> CreateRange(IEnumerable<Tag> tags);
}