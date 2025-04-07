using Xpense.Core.Entities;
using Xpense.Core.Enums;

namespace Xpense.RestApi.Models;

public class CreateCategoryResponse(int id, string name, Priority priority)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public Priority Priority { get; set; } = priority;

    public static CreateCategoryResponse Of(Category category) => new(category.Id, category.Label, category.Priority);
}