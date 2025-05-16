using Xpense.Core.Exceptions;
using Xpense.Core.Features.Tags.Commands;
using Xpense.Core.Interfaces.Persistence;
using Xpense.Core.Interfaces.UseCases;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Tags.UseCases;

public class UpdateTagUseCase(ITagRepository repository) : ICommandResultHandler<UpdateTagCommand, Tag>
{
    public async Task<Tag> Handle(UpdateTagCommand command)
    {
        var entity = await repository.GetById(command.Id);
        entity.Label = command.Name;
        entity.BgColorHex = command.BgColorHex;
        entity.FgColorHex = command.FgColorHex;
        repository.Update(entity);
        var result = await repository.SaveChanges();
        if (result < 1)
            throw new TagUpdateFailedException(command.Id);

        return entity;
    }
}