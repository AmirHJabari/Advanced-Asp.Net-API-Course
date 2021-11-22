using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebFramework.Swagger
{
    /// <summary>
    /// A filter for swagger to hide action parameters with [BindNever] attribute. (Not done yet)
    /// </summary>
    public class SwaggerExcludeBindNeverFilter1 : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            var parameters = context?.ParameterInfo?.CustomAttributes?.Where(att => att.AttributeType == typeof(BindNeverAttribute)).ToList();

            if (parameters is not null && parameters.Any())
            {

                //context.SchemaRepository.Schemas
                //var removeAtt = context.SchemaRepository.Schemas.Where((schema) => schema.Value == parameter.Schema).ToList();
                //if (removeAtt.Any())
                //foreach (var item in removeAtt)
                //{
                //    context.SchemaRepository.Schemas.Remove(item.Key, out var val);
                //    Console.WriteLine("Yes");
                //}
            }
        }
    }

    public class SwaggerExcludeBindNeverFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var paramsToHide = context.MethodInfo.GetParameters()
                                .Where(para => para.CustomAttributes.Any(att => att.AttributeType == typeof(BindNeverAttribute)))
                                .ToList();
            
            if (paramsToHide.Any())
            {
                foreach (var paramToHide in paramsToHide)
                {
                    var a = operation.RequestBody.Content;
                    operation.RequestBody.Content.Clear();

                    var parameter = operation.Parameters.FirstOrDefault(parameter => parameter.Name.Equals(paramToHide.Name, StringComparison.Ordinal));
                    operation.Parameters.Remove(parameter);
                }
            }
        }
    }


    public class OpenApiParameterIgnoreFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null || context == null || context.ApiDescription?.ParameterDescriptions == null)
                return;

            var paramsToHide = context.ApiDescription.ParameterDescriptions
                .Where(parameterDescription => ParameterHasIgnoreAttribute(parameterDescription))
                .ToList();

            if (paramsToHide.Any())
            {
                foreach (var paramToHide in paramsToHide)
                {
                    var parameter = operation.Parameters.FirstOrDefault(parameter => parameter.Name.Equals(paramToHide.Name, StringComparison.Ordinal));
                    if (parameter != null)
                        operation.Parameters.Remove(parameter);
                }
            }
        }

        private static bool ParameterHasIgnoreAttribute(Microsoft.AspNetCore.Mvc.ApiExplorer.ApiParameterDescription parameterDescription)
        {
            if (parameterDescription.ModelMetadata is Microsoft.AspNetCore.Mvc.ModelBinding.Metadata.DefaultModelMetadata metadata)
            {
                return metadata.Attributes.ParameterAttributes.Any(attribute => attribute.GetType() == typeof(BindNeverAttribute));
            }

            return false;
        }
    }
}

