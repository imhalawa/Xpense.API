namespace Xpense.Core.Abstract.UseCases;

public interface ICommandHandler<TParam>
{
    public Task Handle(TParam command);
}