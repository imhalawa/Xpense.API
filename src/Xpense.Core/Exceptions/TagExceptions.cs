namespace Xpense.Core.Exceptions;

public class TagNotFoundException(int id, Exception? innerException = null)
    : XpenseException($"Tag with id {id} was not found!", innerException);

public class TagDeletionFailedException(int id, Exception? innerException = null)
    : XpenseException($"Failure attempt to remove tag {id}", innerException);

public class TagCreationFailedException(string name, Exception? innerException = null)
    : XpenseException($"Failure Attempt to create tag {name}", innerException);

public class TagUpdateFailedException(int id, Exception? innerException = null)
    : XpenseException($"Failure to update tag with id {id}", innerException);