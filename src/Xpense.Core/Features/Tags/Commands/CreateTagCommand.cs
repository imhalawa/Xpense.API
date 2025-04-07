using Xpense.Core.Entities;

namespace Xpense.Core.Features.Tags.Commands;

public class CreateTagCommand(string label, string bgColorHex, string fgColorHex)
{
    public string Label { get; set; } = label;
    public string BgColorHex { get; set; } = bgColorHex;
    public string FgColorHex { get; set; } = fgColorHex;

    public Tag ToEntity() => new()
    {
        Label = Label,
        BgColorHex = BgColorHex,
        FgColorHex = FgColorHex
    };
}