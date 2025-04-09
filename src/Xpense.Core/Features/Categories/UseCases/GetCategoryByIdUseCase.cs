using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Exceptions;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Categories.UseCases;

public class GetCategoryByIdUseCase(ICategoryRepository repository) : IQueryParamHandler<int, Category>
{
    public async Task<Category> Execute(int accountNumber, CancellationToken cancellationToken = default)
    {
        var category = await repository.GetById(accountNumber);
        if (category == null)
            throw new CategoryNotFoundException(accountNumber);
        return category;
    }
}