using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class StatusCodeHandlerAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Return 200 for any validation type
    /// </summary>
    public bool Return200ForInvalid { get; set; } = false;

    /// <summary>
    /// Custom attribute to handle return types based on HTTP Requests
    /// </summary>
    public override void OnActionExecuted(ActionExecutedContext context)
    {       
        var result = context.Result as ObjectResult;

        if (result?.Value != null)
        {
            var resultType = result.Value.GetType();

            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(ServiceResult<>))
            {
                // Access properties via reflection
                var validationProperty = resultType.GetProperty("Validation");
                var validationValue = (ValidationTypes)validationProperty?.GetValue(result.Value);

                var messageProperty = resultType.GetProperty("Message");
                var messageValue = messageProperty?.GetValue(result.Value) as string;

                var valueProperty = resultType.GetProperty("Value");
                var value = valueProperty?.GetValue(result.Value);

                if (Return200ForInvalid)
                {
                    context.HttpContext.Response.StatusCode = 200;
                    context.Result = HandleResponse(validationValue, messageValue, value);
                }
                else
                {
                    var httpMethod = context.HttpContext.Request.Method;

                    // Determine status code and response based on HTTP method and validation
                    switch (httpMethod)
                    {
                        case "POST":
                            context.HttpContext.Response.StatusCode = validationValue == ValidationTypes.None ? 201 : 405;
                            context.Result = HandleResponse(validationValue, messageValue, value);
                            break;

                        case "GET":
                        case "PUT":
                            context.HttpContext.Response.StatusCode = validationValue == ValidationTypes.None ? 200 : 404;
                            context.Result = HandleResponse(validationValue, messageValue, value);
                            break;

                        default:
                            context.HttpContext.Response.StatusCode = 405;
                            break;
                    }
                }
            }
        }

        base.OnActionExecuted(context);

        static IActionResult HandleResponse(ValidationTypes validation, string message, object value)
        {
            return validation == ValidationTypes.Invalid ?
                            new ContentResult { Content = message }
                          : new ObjectResult(value);
        }
    }
}
