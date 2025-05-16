namespace Xpense.Core.Interfaces.UseCases;

public interface IQueryHandler<TResult>
{
    public Task<TResult> Execute();
}