using Xpense.Services.Enums;

namespace Xpense.Services.Features.Categories.Commands;

public class UpdateCategoryCommand(int id, string name, Priority priority)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public Priority Priority { get; set; } = priority;
}