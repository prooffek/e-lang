namespace E_Lang.Application.Interfaces;

public interface IUserValidationService
{
    void ValidateUserId(Guid? userId);
}