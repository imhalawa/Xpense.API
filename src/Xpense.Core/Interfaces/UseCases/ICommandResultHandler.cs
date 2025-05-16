namespace Xpense.Core.Interfaces.UseCases;

public interface ICommandResultHandler<in TParam, TResult>
{
    public Task<TResult> Handle(TParam command);
}