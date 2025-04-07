using Xpense.Core.Features.Categories.Commands;

namespace Xpense.RestApi.Models;

public class UpdateCategoryRequest(int id, string name, int priorityId)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public int PriorityId { get; set; } = priorityId;

    public UpdateCategoryCommand ToCommand() => new UpdateCategoryCommand(Id, Name, PriorityId);
}