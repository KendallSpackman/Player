using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;

namespace Player
{
    class Processor
    {
        private static Dictionary<Type, int> typeDict = new Dictionary<Type, int>
        {
            {typeof(GameListReply),0},
            {typeof(JoinGameReply),1},
            {typeof(LoginReply),2},
            {typeof(AliveRequest),3}
        };

        private static Processor processor;
        private static object myLock = new object();
        public bool Running { get; set; }
        public ConcurrentQueue<Message> Queue { get; set; }
        public Player Player { get; set; }

        public static Processor Instance()
        {
            if (processor == null)
            {
                lock (myLock)
                {
                    if (processor == null)
                    {
                        processor = new Processor();
                    }
                }
            }
            return processor;
        }

        private Processor()
        {
            Running = true;
        }

        public void Run()
        {
            Message message;
            bool success;
            while (Running)
            {
                success = Queue.TryDequeue(out message);
                if (success)
                    Send(message);
                else
                    Thread.Sleep(500);
            }
        }

        private void Send(Message message)
        {
            switch (typeDict[message.GetType()])
            {
                case 0:
                    Player.Receive(message as GameListReply);
                    break;
                case 1:
                    Player.Receive(message as JoinGameReply);
                    break;
                case 2:
                    Player.Receive(message as LoginReply);
                    break;
                case 3:
                    Player.Receive(message as AliveRequest);
                    break;
            }
        }
    }
}
