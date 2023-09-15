using FluentValidation.Results;

namespace FluentValidation
{
    /// <summary>
    /// 
    /// </summary>
    public static class FluentValidationResultExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string ErrorMessage(this ValidationResult result)
        {
            if (result.IsValid)
                return string.Empty;

            return string.Join("\n", result.Errors);
        }
    }
}
