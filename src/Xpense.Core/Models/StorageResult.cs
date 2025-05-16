using Xpense.Core.Enums;

namespace Xpense.Core.Models
{
    public record StorageResult<T>(StorageResultStatus Status, T? Data = default) : SimpleStorageResult(Status)
    {
        public new static StorageResult<T> Success(T data) => new(StorageResultStatus.Success, data);
        public new static StorageResult<T> Failure => new(StorageResultStatus.Failure);
        public new static StorageResult<T> NotFound => new(StorageResultStatus.NotFound);
    }
}
