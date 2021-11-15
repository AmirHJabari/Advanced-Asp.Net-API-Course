using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace WebFramework.API
{
    public class ApiResult<TData> : ApiResult
    {
        public ApiResult()
            : base()
        {
            this.Data = default;
        }

        public ApiResult(bool status) 
            : base(status)
        {
            this.Data = default;
        }

        public ApiResult(bool success, string message, TData data, ApiResultStatusCode code = ApiResultStatusCode.None)
           : base(success, message, code)
        {
            this.Data = data;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TData Data { get; set; }

        public override ApiResult<TData> WithStatus(bool success = true)
        {
            this.IsSuccess = success;
            return this;
        }

        public override ApiResult<TData> WithMessage(string message)
        {
            this.Message = message;
            return this;
        }

        public override ApiResult<TData> WithCode(ApiResultStatusCode code)
        {
            this.Code = code;
            return this;
        }

        public ApiResult<TData> WithData(TData data)
        {
            this.Data = data;
            return this;
        }

        #region Implicit Operators
        //public static implicit operator ApiResult<TData>(TData data)
        //{
        //    return new ApiResult<TData>(true, ApiResultStatusCode.Success, data);
        //}

        //public static implicit operator ApiResult<TData>(OkResult result)
        //{
        //    return new ApiResult<TData>(true, ApiResultStatusCode.Success, null);
        //}

        //public static implicit operator ApiResult<TData>(OkObjectResult result)
        //{
        //    return new ApiResult<TData>(true, ApiResultStatusCode.Success, (TData)result.Value);
        //}

        //public static implicit operator ApiResult<TData>(BadRequestResult result)
        //{
        //    return new ApiResult<TData>(false, ApiResultStatusCode.BadRequest, null);
        //}

        //public static implicit operator ApiResult<TData>(BadRequestObjectResult result)
        //{
        //    var message = result.Value.ToString();
        //    if (result.Value is SerializableError errors)
        //    {
        //        var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
        //        message = string.Join(" | ", errorMessages);
        //    }
        //    return new ApiResult<TData>(false, message, default);
        //}

        //public static implicit operator ApiResult<TData>(ContentResult result)
        //{
        //    return new ApiResult<TData>(true, ApiResultStatusCode.Success, null, result.Content);
        //}

        //public static implicit operator ApiResult<TData>(NotFoundResult result)
        //{
        //    return new ApiResult<TData>(false, ApiResultStatusCode.NotFound, null);
        //}

        //public static implicit operator ApiResult<TData>(NotFoundObjectResult result)
        //{
        //    return new ApiResult<TData>(false, ApiResultStatusCode.NotFound, (TData)result.Value);
        //}
        #endregion
    }

    public class ApiResult
    {
        public ApiResult()
            : this(true, "Operation was successful")
        { }

        public ApiResult(bool status)
        {
            this.IsSuccess = status;
            this.Message = status ? "Operation was successful" : "Operation was not successful";
            this.Code = ApiResultStatusCode.None;
        }

        public ApiResult(bool success, string message, ApiResultStatusCode code = ApiResultStatusCode.None)
        {
            this.IsSuccess = success;
            this.Message = message;
            this.Code = code;
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public ApiResultStatusCode Code { get; set; }

        public virtual ApiResult WithStatus(bool success = true)
        {
            this.IsSuccess = success;
            return this;
        }

        public virtual ApiResult WithCode(ApiResultStatusCode code)
        {
            this.Code = code;
            return this;
        }

        public virtual ApiResult WithMessage(string message)
        {
            this.Message = message;
            return this;
        }

        #region Implicit Operators
        //public static implicit operator ApiResult(OkResult result)
        //{
        //    return new ApiResult(true, ApiResultStatusCode.Success);
        //}

        //public static implicit operator ApiResult(BadRequestResult result)
        //{
        //    return new ApiResult(false, ApiResultStatusCode.BadRequest);
        //}

        //public static implicit operator ApiResult(BadRequestObjectResult result)
        //{
        //    var message = result.Value.ToString();
        //    if (result.Value is SerializableError errors)
        //    {
        //        var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
        //        message = string.Join(" | ", errorMessages);
        //    }
        //    return new ApiResult(false, ApiResultStatusCode.BadRequest, message);
        //}

        //public static implicit operator ApiResult(ContentResult result)
        //{
        //    return new ApiResult(true, ApiResultStatusCode.Success, result.Content);
        //}

        //public static implicit operator ApiResult(NotFoundResult result)
        //{
        //    return new ApiResult(false, ApiResultStatusCode.NotFound);
        //}
        #endregion
    }
}
