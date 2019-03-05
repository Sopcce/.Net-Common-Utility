﻿using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sop.Common.Charts.JsonConverter.MVC
{
    /// <summary>
    ///     忽略NULL以及属性名命名遵循首字母小写命名
    /// </summary>
    public class JavaScriptJsonResult : JsonResult
    {
        public JavaScriptJsonResult()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error,
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                //DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                //使用JS CamelCase命名，即首字母小写
                ContractResolver = new CamelCasePropertyNamesContractResolver()
                //DefaultValueHandling = DefaultValueHandling.Ignore
            };
        }

        public JsonSerializerSettings Settings { get; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if ((JsonRequestBehavior == JsonRequestBehavior.DenyGet) &&
                string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            var response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            if (Data == null)
                return;

            var scriptSerializer = JsonSerializer.Create(Settings);
            scriptSerializer.Serialize(response.Output, Data);
        }
    }
}