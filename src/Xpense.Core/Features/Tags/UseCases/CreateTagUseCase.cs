using Xpense.Core.Exceptions;
using Xpense.Core.Features.Tags.Commands;
using Xpense.Core.Interfaces.Persistence;
using Xpense.Core.Interfaces.UseCases;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Tags.UseCases;

public class CreateTagUseCase(ITagRepository repository) : ICommandResultHandler<CreateTagCommand, Tag>
{
    public async Task<Tag> Handle(CreateTagCommand command)
    {
        var tag = command.ToEntity();
        repository.Create(tag);
        var result = await repository.SaveChanges();
        if (result < 1)
            throw new TagCreationFailedException(tag.Label);
        return tag;
    }
}