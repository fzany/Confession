using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Backend.Helpers
{
    public class Socketer
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentBag<WebSocket> WebSockets = new ConcurrentBag<WebSocket>();

        public Socketer(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Response.StatusCode == 101)
            {
                // this forces ClientWebSocket to believe their is content
                // to process thus it keeps the socket alive for processing??
                httpContext.Response.Headers.ContentLength = 1;
            }
            //if (httpContext.WebSockets.IsWebSocketRequest)
            //{
            //    WebSocket socket = await httpContext.WebSockets.AcceptWebSocketAsync();
            //    System.Net.ServicePointManager.MaxServicePointIdleTime = int.MaxValue;
            //    WebSockets.Add(socket);
            //    while (socket.State == WebSocketState.Open)
            //    {
            //        CancellationToken token = CancellationToken.None;
            //        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
            //        WebSocketReceiveResult received = await socket.ReceiveAsync(buffer, token);
            //        switch (received.MessageType)
            //        {
            //            case WebSocketMessageType.Close:
            //                // nothing to do for now...
            //                break;

            //            case WebSocketMessageType.Text:
            //                string incoming = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            //                // get rid of trailing crap from buffer
            //                incoming = incoming.Replace("\0", "");
            //                Console.WriteLine($"Incoming: {incoming}");
            //                string chatreturn = Store.ChatClass.ProcessMessage(incoming);
            //                byte[] data = Encoding.UTF8.GetBytes(chatreturn);
            //                buffer = new ArraySegment<byte>(data);

            //                // send to all open sockets
            //                await Task.WhenAll(WebSockets.Where(s => s.State == WebSocketState.Open)
            //                    .Select(s => s.SendAsync(buffer, WebSocketMessageType.Text, true, token)));
            //                break;
            //        }
            //    }
            //}
            else
            {
                await _next.Invoke(httpContext);
            }
        }
    }
    public static class RequestExtensions
    {
        public static IApplicationBuilder UseWebSocketHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Socketer>();
        }
    }
}
