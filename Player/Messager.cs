using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using log4net;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace Player
{
    public class Messager
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Messager));
        private object myLock = new object();
        private UdpClient udpClient;
        private bool keepGoing;
        public ConcurrentQueue<Message> Queue { get; private set; }
        public IPEndPoint EndPoint { get; private set; }

        public Messager()
        {
            keepGoing = true;
            udpClient = new UdpClient(0);
            EndPoint = (IPEndPoint)udpClient.Client.LocalEndPoint;
            Queue = new ConcurrentQueue<Message>();
        }

        public void Send(Message message, PublicEndPoint ep)
        {
            try
            {
                byte[] bytes = message.Encode();
                udpClient.Send(bytes, bytes.Length, ep.IPEndPoint);
            } catch (Exception e)
            {
                logger.Debug("Exception: " + e.Message);
            }
        }

        public void Receive()
        {
            IPEndPoint receiveEp = new IPEndPoint(IPAddress.Any, 0); //Receive from anyone - 0.0.0.0

            while (keepGoing)
            {
                try
                {
                    byte[] bytes = udpClient.Receive(ref receiveEp);
                    if (bytes != null)
                    {
                        Message msg = Message.Decode(bytes);
                        Queue.Enqueue(msg);
                    }
                } catch (Exception e)
                {
                    logger.Debug("Exception: " + e.Message);
                }
            }
        }

        public void Stop()
        {
            keepGoing = false;
            udpClient.Close();
        }
    }
}
