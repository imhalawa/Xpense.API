namespace Xpense.Adapters.Postgres.Models
{
    public record SimpleStorageResult(StorageResultStatus Status)
    {
        public static SimpleStorageResult Success => new(StorageResultStatus.Success);
        public static SimpleStorageResult Failure => new(StorageResultStatus.Failure);
        public static SimpleStorageResult NotFound => new(StorageResultStatus.NotFound);

    }
}
