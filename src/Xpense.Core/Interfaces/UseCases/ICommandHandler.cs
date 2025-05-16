namespace Xpense.Core.Interfaces.UseCases;

public interface ICommandHandler<TParam>
{
    public Task Handle(TParam command);
}