using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Tests.Utilities
{
    public static class HttpContextHelper
    {
        public static void MockHttpContext(ControllerBase controller, string authorizationHeader)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = authorizationHeader;
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }
    }
}
