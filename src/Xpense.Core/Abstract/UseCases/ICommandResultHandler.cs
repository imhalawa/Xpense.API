namespace Xpense.Core.Abstract.UseCases;

public interface ICommandResultHandler<in TParam, TResult>
{
    public Task<TResult> Handle(TParam command);
}