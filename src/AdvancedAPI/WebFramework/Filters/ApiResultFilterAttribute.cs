using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework.API;

namespace WebFramework.Filters
{
    /*
        OnActionExecutionAsync
        OnActionExecuting
- action is executing
        OnActionExecuted
        OnResultExecutionAsync
        OnResultExecuting
        OnResultExecuted
     */

    /// <summary>
    /// Just for test and sample.
    /// </summary>
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            switch (context.Result)
            {
                case StatusCodeResult result when result.StatusCode is not 204:
                    {
                        bool success = result.StatusCode < 400;

                        var apiResult = new ApiResult(success);
                        context.Result = new JsonResult(apiResult) { StatusCode = result.StatusCode };
                        break;
                    }

                case ObjectResult result
                        when result.StatusCode is not null && result.Value is not ApiResult:
                    {
                        if (result.Value is ValidationProblemDetails validationProblem)
                        {
                            var errorMessages = validationProblem.Errors.SelectMany(p => p.Value).Distinct();
                            string message = string.Join(" | ", errorMessages);
                            var apiResult = new ApiResult(false, message, ApiResultStatusCode.InvalidInputs);
                            context.Result = new JsonResult(apiResult) { StatusCode = result.StatusCode };
                        }
                        else
                        {
                            bool success = result.StatusCode < 400;
                            var apiResult = new ApiResult<object>(success);

                            if (result.Value is not ProblemDetails)
                                apiResult.WithData(result.Value);

                            context.Result = new JsonResult(apiResult) { StatusCode = result.StatusCode };
                        }
                        break;
                    }
            }

            base.OnResultExecuting(context);
        }
    }
}
