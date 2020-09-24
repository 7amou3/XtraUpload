using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Text;

namespace XtraUpload.StorageManager.Host
{
    public class TokenInterceptor : Interceptor
    {

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation) where TRequest : class where TResponse : class
        {
            AddCallerMetadata(ref context);

            return continuation(request, context);
        }

        private void AddCallerMetadata<TRequest, TResponse>(ref ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var headers = context.Options.Headers;

            // Call doesn't have a headers collection to add to.
            // Need to create a new context with headers for the call.
            if (headers == null)
            {
                headers = new Metadata();
                var options = context.Options.WithHeaders(headers);
                context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, options);
            }

            // Add caller metadata to call headers
            headers.Add("caller-user", Environment.UserName);
            headers.Add("caller-machine", Environment.MachineName);
            headers.Add("caller-os", Environment.OSVersion.ToString());
        }
    }
}
