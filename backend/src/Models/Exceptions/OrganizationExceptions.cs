using ShootingAcademy.Models.Exceptions;

namespace ShootingAcademy.Models.Exceptions
{
    public class OrganizationNotFoundException : BaseException
    {
        public OrganizationNotFoundException(Guid id) 
            : base($"Организация с ID {id} не найдена", 404)
        {
        }
    }

    public class OrganizationValidationException : BaseException
    {
        public OrganizationValidationException(string message) 
            : base(message, 400)
        {
        }
    }
} 