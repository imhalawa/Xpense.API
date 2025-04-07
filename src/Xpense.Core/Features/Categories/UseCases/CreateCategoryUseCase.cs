using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Entities;
using Xpense.Core.Exceptions;
using Xpense.Core.Features.Categories.Commands;

namespace Xpense.Core.Features.Categories.UseCases;

public class CreateCategoryUseCase(
        ICategoryRepository repository,
        IPriorityRepository priorityRepository)
    : ICommandResultHandler<CreateCategoryCommand, Category>
{
    public async Task<Category> Handle(CreateCategoryCommand command)
    {
        var priority = await priorityRepository.GetById(command.PriorityId);

        if (priority == null)
            throw new PriorityNotFoundException(command.PriorityId);

        var category = new Category()
        {
            Label = command.Name,
            Priority = priority
        };

        repository.Create(category);
        var result = await repository.SaveChanges();

        if (result < 1)
            throw new CategoryCreationFailedException(command.Name);

        return category;
    }
}