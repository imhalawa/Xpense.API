using Xpense.Core.Interfaces.Models;

namespace Xpense.Core.Models
{
    public class TagOption : IOption<Tag>
    {
        public int? Id { get; set; }
        public required string Label { get; set; }
        public bool Create { get; set; } = false;

        public Tag ToEntity()
        {
            return new Tag
            {
                Label = this.Label,
                CreatedOn = DateTime.Now
            };
        }
        public static IEnumerable<Tag> ToEntityRange(IEnumerable<TagOption> tags)
        {
            return tags.Select(t => t.ToEntity());
        }
    }
}
