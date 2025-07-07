namespace Pokok.BuildingBlocks.Cqrs.Validation
{
    public interface IValidator<T>
    {
        void Validate(T request);
    }
}
