﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace Bowerbird.Test.Utils
{
    public static class SetupHelpers
    {

        public static HttpContextBase SetupHttpContext(string url)
        {
            var context = MockHelpers.MockAnonymousHttpContext().Object;

            context.Request.SetupRequestUrl(url);

            return context;
        }

        public static Mock<HttpContextBase> SetupAuthenticatedHttpContext()
        {
            var context = MockHelpers.MockAnonymousHttpContext();
            var user = MockHelpers.MockPrincipal();

            context.Setup(ctx => ctx.User).Returns(user.Object);

            return context;
        }

        public static void SetupControllerContext(this Controller controller)
        {
            var httpContext = MockHelpers.MockAnonymousHttpContext();

            var context = new ControllerContext(new RequestContext(httpContext.Object, new RouteData()), controller);

            controller.ControllerContext = context;
        }

        public static void SetupAjaxRequest(this Controller controller)
        {
            var context = new ControllerContext(new RequestContext(MockHttpContextAjaxRequest().Object, new RouteData()), controller);

            controller.ControllerContext = context;
        }

        public static void SetupFormRequest(this Controller controller)
        {
            var context = new ControllerContext(new RequestContext(MockHttpContextRequest().Object, new RouteData()), controller);

            controller.ControllerContext = context;
        }

        public static void SetupAuthenticatedControllerContext(this Controller controller)
        {
            var httpContext = SetupAuthenticatedHttpContext().Object;

            var context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);

            controller.ControllerContext = context;
        }

        public static void SetupRequestUrl(this HttpRequestBase request, string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            if (!url.StartsWith("~/"))
                throw new ArgumentException("Sorry, we expect a virtual url starting with \"~/\".");

            var mock = Mock.Get(request);

            mock.Setup(req => req.QueryString)
                .Returns(url.GetQueryStringParameters());

            mock.Setup(req => req.AppRelativeCurrentExecutionFilePath)
                .Returns(url.GetUrlFileName());

            mock.Setup(req => req.PathInfo)
                .Returns(string.Empty);
        }

        public static void SetupHttpMethodResult(this HttpRequestBase request, string httpMethod)
        {
            Mock.Get(request)
                .Setup(req => req.HttpMethod)
                .Returns(httpMethod);
        }

        private static Mock<HttpContextBase> MockHttpContextAjaxRequest()
        {
            var context = new Mock<HttpContextBase>();

            var mockRequest = new Mock<HttpRequestBase>();
            var mockResponse = new Mock<HttpResponseBase>();
            var mockSession = new Mock<HttpSessionStateBase>();
            var mockServer = new Mock<HttpServerUtilityBase>();

            context.Setup(ctx => ctx.Request).Returns(mockRequest.Object);
            context.Setup(ctx => ctx.Response).Returns(mockResponse.Object);
            context.Setup(ctx => ctx.Session).Returns(mockSession.Object);
            context.Setup(ctx => ctx.Server).Returns(mockServer.Object);

            context.Setup(x => x.Request["X-Requested-With"]).Returns("XMLHttpRequest");

            return context;
        }

        private static Mock<HttpContextBase> MockHttpContextRequest()
        {
            var context = new Mock<HttpContextBase>();

            var mockRequest = new Mock<HttpRequestBase>();
            var mockResponse = new Mock<HttpResponseBase>();
            var mockSession = new Mock<HttpSessionStateBase>();
            var mockServer = new Mock<HttpServerUtilityBase>();

            context.Setup(ctx => ctx.Request).Returns(mockRequest.Object);
            context.Setup(ctx => ctx.Response).Returns(mockResponse.Object);
            context.Setup(ctx => ctx.Session).Returns(mockSession.Object);
            context.Setup(ctx => ctx.Server).Returns(mockServer.Object);

            //context.Setup(x => x.Request["X-Requested-With"]).Returns("XMLHttpRequest");

            return context;
        }
    }
}