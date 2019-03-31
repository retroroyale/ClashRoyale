using System;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using SharpRaven.Data;

namespace ClashRoyale.Core.Network.Handlers
{
    public class TimeoutHandler : ChannelHandlerAdapter
    {
        public override async void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent evv)
                switch (evv.State)
                {
                    case IdleState.AllIdle:
                        Logger.Log($"All time out: {DateTime.Now:mm:ss}, Id: {context.Channel.Id}", GetType(), ErrorLevel.Debug);
                        break;
                    case IdleState.WriterIdle:
                        Logger.Log($"Write time out: {DateTime.Now:mm:ss}, Id: {context.Channel.Id}", GetType(), ErrorLevel.Debug);
                        break;
                    case IdleState.ReaderIdle:
                        Logger.Log($"Read time out: {DateTime.Now:mm:ss}, Id: {context.Channel.Id}", GetType(), ErrorLevel.Debug);
                        await context.CloseAsync();
                        break;
                }
            else
                base.UserEventTriggered(context, evt);
        }
    }
}