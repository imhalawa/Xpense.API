namespace Xpense.Adapters.Postgres.Models
{
    public interface IOptionEntity
    {
        int Id { get; set; }
        string Label { get; set; }
    }
}
