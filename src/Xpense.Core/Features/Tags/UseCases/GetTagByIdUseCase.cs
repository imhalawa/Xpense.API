using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Exceptions;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Tags.UseCases;

public class GetTagByIdUseCase(ITagRepository repository) : IQueryParamHandler<int, Tag>
{
    public async Task<Tag> Execute(int accountNumber, CancellationToken cancellationToken = default)
    {
        var tag = await repository.GetById(accountNumber);
        if (tag == null)
            throw new TagNotFoundException(accountNumber);
        return tag;
    }
}