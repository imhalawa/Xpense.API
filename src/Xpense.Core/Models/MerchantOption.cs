using Xpense.Core.Interfaces.Models;

namespace Xpense.Core.Models
{
    public class MerchantOption : IOption<Merchant>
    {
        public int? Id { get; set; }
        public required string Label { get; set; }
        public bool Create { get; set; }

        public Merchant ToEntity()
        {
            return new Merchant
            {
                Label = this.Label,
                CreatedOn = DateTime.Now
            };
        }
    }
}
