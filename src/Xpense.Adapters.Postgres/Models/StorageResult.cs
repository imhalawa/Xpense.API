namespace Xpense.Adapters.Postgres.Models
{
    public record StorageResult<T>(StorageResultStatus Status, T? Data = default, Exception? Exception = null)
        : SimpleStorageResult(Status)
    {
        public new static StorageResult<T> Success(T data) => new(StorageResultStatus.Success, data);

        public new static StorageResult<T> Failure(Exception? exception = null) =>
            new(StorageResultStatus.Failure, Exception: exception);

        public new static StorageResult<T> NotFound => new(StorageResultStatus.NotFound);
    }
}