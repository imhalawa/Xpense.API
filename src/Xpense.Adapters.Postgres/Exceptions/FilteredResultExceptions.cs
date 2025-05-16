using Xpense.Adapters.Postgres.Models;

namespace Xpense.Adapters.Postgres.Exceptions
{
    public class InvalidFilteredResultParams(FilterQuery query, Exception? innerException = null)
        : XpenseException($"Invalid filtration params {nameof(query.Page)}:{query.Page}, {nameof(query.Size)}:{query.Size} must be greater than 0", innerException);
}
