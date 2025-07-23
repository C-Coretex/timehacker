namespace TimeHacker.Domain.BusinessLogicExceptions
{
    public class NotFoundException(string resourceName, string resourceId) : Exception
    {
        public string ResourceName { get; set; } = resourceName;
        public string ResourceId { get; set; } = resourceId;
    }
}
