using Xpense.Core.Interfaces.Persistence;
using Xpense.Core.Interfaces.UseCases;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Tags.UseCases;

public class GetAllTagsUseCase(ITagRepository repository): IQueryHandler<IEnumerable<Tag>>
{
    public async Task<IEnumerable<Tag>> Execute()
    {
        return await repository.GetAll();
    }
}