using System.ComponentModel.DataAnnotations;
using Xpense.Core.Features.Tags.Commands;

namespace Xpense.RestApi.Models;

public class CreateTagRequest
{
    [Required]
    public string Label { get; set; }
    public string BgColorHex { get; set; }
    public string FgColorHex { get; set; }

    public CreateTagCommand ToCommand() => new(Label, BgColorHex, FgColorHex);
}