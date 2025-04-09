using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Exceptions;
using Xpense.Core.Features.Categories.Commands;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Categories.UseCases;

public class UpdateCategoryUseCase(ICategoryRepository repository, IPriorityRepository priorityRepository) : ICommandResultHandler<UpdateCategoryCommand, Category>
{
    public async Task<Category> Handle(UpdateCategoryCommand command)
    {
        var priority = await priorityRepository.GetById(command.PriorityId);

        if (priority == null)
            throw new PriorityNotFoundException(command.PriorityId);

        var category = await repository.GetById(command.Id);

        if (category == null)
            throw new CategoryNotFoundException(command.Id);

        category.Label = command.Name;
        category.Priority = priority;

        repository.Update(category);

        var result = await repository.SaveChanges();
        if (result < 1)
            throw new CategoryUpdateFailedException(command.Id);

        return category;
    }
}