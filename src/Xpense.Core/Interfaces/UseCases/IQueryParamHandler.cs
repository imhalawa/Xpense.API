namespace Xpense.Core.Interfaces.UseCases;

public interface IQueryParamHandler<in TParam, TResult>
{
    public Task<TResult> Execute(TParam param, CancellationToken cancellationToken = default);
}
