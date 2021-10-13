using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Evote_Service.Util
{
    public class ITSCInputFormatter : InputFormatter
    {
        public override Boolean CanRead(InputFormatterContext context)
        {

            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
            return true;
        }
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
            var request = context.HttpContext.Request;
            var contentType = request.ContentType;
            if (contentType.StartsWith("application/x-www-form-urlencoded")) // in case it ends with ";charset=UTF-8"
            {
                var content = string.Empty;
                content = "{ ";
                foreach (var key in request.Form.Keys)
                {
                    if (request.Form.TryGetValue(key, out var value))
                    {
                        // content += $"{key}={value}&";
                        content += "\"" + $"{key}" + "\" : \"" + $"{value}" + "\"";
                    }
                    content = content + ",";
                }
                content = content + " }";
                return await InputFormatterResult.SuccessAsync(content);

            }
            using (var reader = new StreamReader(request.Body))
            {
                try
                {
                    var content = await reader.ReadToEndAsync();
                    return await InputFormatterResult.SuccessAsync(content);
                }
                catch
                {
                    return await InputFormatterResult.FailureAsync();
                }
            }
        }
    }
}
