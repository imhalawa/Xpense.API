using Xpense.Core.Features.Categories.Commands;

namespace Xpense.RestApi.Models;

public class CreateCategoryRequest(string name, int priorityId)
{
    public string Name { get; } = name;
    public int Priority { get; } = priorityId;

    public CreateCategoryCommand ToCommand() => new(Name, priorityId);
}