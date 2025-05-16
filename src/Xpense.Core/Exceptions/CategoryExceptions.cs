namespace Xpense.Core.Exceptions;

public class CategoryCreationFailedException(string name, Exception? innerException = null)
    : XpenseException($"Failure attempt to create category {name}", innerException);

public class CategoryDeletionFailedException(int id, Exception? innerException = null)
    : XpenseException($"Failure attempt to remove category with id:[{id}]", innerException);

public class CategoryNotFoundException(int id, Exception? innerException = null)
    : XpenseException($"Category with id:[{id}] was not found", innerException);
    
public class CategoryUpdateFailedException(int id, Exception? innerException = null)
    : XpenseException($"Failure attempt to update category with id:[{id}]", innerException);