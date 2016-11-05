using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Net;

namespace ClassLibRemotingIPSink
{
    /*****************************************************************
     * 说明：ClassLibRemotingIPSink 为 remoting 提供数据操作支撑
     * 
     * 编写人：罗俊杰 2012-5-24
     * 
     * 修改人：
     * 
     * ***************************************************************/
    public class ClientIPServerSinkProvider : IServerChannelSinkProvider
    {
        private IServerChannelSinkProvider next = null;
        public ClientIPServerSinkProvider()
        {

        }
        
        public void GetChannelData(IChannelDataStore channelData)
        {

        }

        public IServerChannelSink CreateSink(IChannelReceiver channel)
        {
            IServerChannelSink nextSink = null;
            if (next != null)
                nextSink = next.CreateSink(channel);
            return new ClientIPServerSink(nextSink);
        }

        public IServerChannelSinkProvider Next
        {
            get { return next; }
            set { next = value; }
        }

    }

    public class ClientIPServerSink : BaseChannelObjectWithProperties, IServerChannelSink, IChannelSinkBase
    {
        private IServerChannelSink _next;
        public ClientIPServerSink(IServerChannelSink next)
        {
            _next = next;
        }

        public void AsyncProcessResponse(System.Runtime.Remoting.Channels.IServerResponseChannelSinkStack sinkStack, System.Object state, System.Runtime.Remoting.Messaging.IMessage msg, System.Runtime.Remoting.Channels.ITransportHeaders headers, System.IO.Stream stream)
        {

        }
        public Stream GetResponseStream(System.Runtime.Remoting.Channels.IServerResponseChannelSinkStack sinkStack, System.Object state, System.Runtime.Remoting.Messaging.IMessage msg, System.Runtime.Remoting.Channels.ITransportHeaders headers)
        {
            return null;
        }
        public System.Runtime.Remoting.Channels.ServerProcessing ProcessMessage(System.Runtime.Remoting.Channels.IServerChannelSinkStack sinkStack, System.Runtime.Remoting.Messaging.IMessage requestMsg, System.Runtime.Remoting.Channels.ITransportHeaders requestHeaders, System.IO.Stream requestStream, out System.Runtime.Remoting.Messaging.IMessage responseMsg, out System.Runtime.Remoting.Channels.ITransportHeaders responseHeaders, out System.IO.Stream responseStream)
        {
            if (_next != null)
            {
                try
                {
                    Thread.Sleep(1);
                    IPAddress ip = requestHeaders[CommonTransportKeys.IPAddress] as IPAddress;
                    CallContext.SetData("ClientIPAddress", ip);
                    ServerProcessing spres = _next.ProcessMessage(sinkStack, requestMsg, requestHeaders, requestStream, out responseMsg, out responseHeaders, out responseStream);
                    return spres;
                }
                catch
                {
                    responseMsg = null;
                    responseHeaders = null;
                    responseStream = null;
                    return new ServerProcessing();
                }
            }
            else
            {
                responseMsg = null;
                responseHeaders = null;
                responseStream = null;
                return new ServerProcessing();
            }
        }

        public IServerChannelSink NextChannelSink
        {
            get { return _next; }
            set { _next = value; }
        }

    }

}
