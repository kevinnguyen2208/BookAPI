namespace API.Models
{
    /// <summary>
    /// Abstract class that accepts a generic type and use for the same logic
    /// </summary>
    /// <typeparam name="T">Guid</typeparam>
    /// <typeparam name="T">BookViewModel</typeparam>
    /// <typeparam name="T">IEnumerable<BookViewModel></typeparam>
    public class ServiceResult<T>
    {
        public ServiceResult(T value)
        {
            Value = value;
            Validation = ValidationTypes.None;
            Message = null;
        }

        public T Value { get; set; }
        public ValidationTypes Validation { get; set; }
        public string Message { get; set; }

        public static ServiceResult<T> CreateErrorMessage(string errorMessage, ValidationTypes validation = ValidationTypes.Invalid)
        {
            var result = new ServiceResult<T>(default(T))
            {
                Validation = validation,
                Message = errorMessage
            };

            return result;
        }
    }
}
