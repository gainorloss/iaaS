using Galosoft.IaaS.Core;
using System.Linq;

namespace Microsoft.AspNetCore.Mvc.Filters
{
    public class GlobalModelStateValidationActionFilter
        : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.SelectMany(state => state.Value.Errors);
                var errorMessages = string.Join(";", errors.Select(e => e.ErrorMessage));
                context.Result = new JsonResult(RestResult.Fail(RestResultCode.ParameterInvalid,$"实体验证失败:{errorMessages}", errors));
            }
        }
    }
}
