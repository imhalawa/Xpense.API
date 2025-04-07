using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Exceptions;

namespace Xpense.Core.Features.Categories.UseCases;

public class DeleteCategoryByIdUseCase(ICategoryRepository repository) : ICommandHandler<int>
{
    public async Task Handle(int id)
    {
        await repository.DeleteById(id);
        var result = await repository.SaveChanges();
        if (result < 1)
            throw new CategoryDeletionFailedException(id);
    }
}