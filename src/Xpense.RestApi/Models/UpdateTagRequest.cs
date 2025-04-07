using Xpense.Core.Features.Tags.Commands;

namespace Xpense.RestApi.Models;

public class UpdateTagRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string BgColorHex { get; set; }
    public string FgColorHex { get; set; }

    public UpdateTagCommand ToCommand() => new(Id, Name, BgColorHex, FgColorHex);
}