using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace Player
{
    public class Messager
    {
        private static object myLock = new object();
        private static Messager messager;
        private static UdpClient udpClient;
        public bool Running { get; set; }
        public ConcurrentQueue<Message> Queue { get; private set; }

        private Messager()
        {
            Running = true;
            udpClient = new UdpClient(0);
            Queue = new ConcurrentQueue<Message>();
        }
        public static Messager Instance()
        {
            if (messager == null)
            {
                lock (myLock)
                {
                    if (messager == null)
                    {
                        messager = new Messager();
                    }
                }
            }
            return messager;
        }

        public void Send(Message message, PublicEndPoint ep)
        {
            byte[] bytes = message.Encode();
            udpClient.Send(bytes, bytes.Length, ep.IPEndPoint);
        }

        // Need to thread. Sender/Receiver?
        // Observer pattern?
        public void Receive()
        {
            //Wait for response
            IPEndPoint receiveEp = new IPEndPoint(IPAddress.Any, 0); //Receive from anyone - 0.0.0.0

            while (Running)
            {
                byte[] bytes = udpClient.Receive(ref receiveEp);
                if (bytes != null)
                {
                    Message msg = Message.Decode(bytes);
                    Queue.Enqueue(msg);
                }
            }
        }
    }
}
