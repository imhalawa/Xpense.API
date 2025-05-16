using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Interfaces.Persistence;
using Xpense.Core.Interfaces.UseCases;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Categories.UseCases;

public class GetAllCategoriesUseCase(ICategoryRepository repository) : IQueryHandler<IEnumerable<Category>>
{
    public async Task<IEnumerable<Category>> Execute()
    {
        //var categories = await repository.GetAll(c => c.Priority);
        //return categories;
        throw new NotImplementedException();

    }
}