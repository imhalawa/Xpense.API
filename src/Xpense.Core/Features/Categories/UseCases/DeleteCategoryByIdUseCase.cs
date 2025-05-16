using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Exceptions;
using Xpense.Core.Interfaces.Persistence;
using Xpense.Core.Interfaces.UseCases;

namespace Xpense.Core.Features.Categories.UseCases;

public class DeleteCategoryByIdUseCase(ICategoryRepository repository) : ICommandHandler<int>
{
    public async Task Handle(int id)
    {
        //await repository.DeleteById(id);
        //var result = await repository.SaveChanges();
        //if (result < 1)
        //    throw new CategoryDeletionFailedException(id);
        throw new NotImplementedException();

    }
}