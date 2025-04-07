namespace Xpense.Core.Abstract.UseCases;

public interface IQueryHandler<TResult>
{
    public Task<TResult> Execute();
}