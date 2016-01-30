using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace Player
{
    public abstract class MessageState : State
    {
        protected int tries = 0;
        protected int maxTries = Player.retries;
        protected bool requestSent = false;
        protected bool replyReceived = false;
        private Stopwatch stopwatch = new Stopwatch();
        protected Player player;

        protected MessageState(Player player)
        {
            this.player = player;
        }

        public void PerformAction()
        {
            if (!requestSent && tries < maxTries)
            {
                LogDebug(GetAttemptMessage());

                player.Messager.Send(CreateRequest(), GetEndPoint());
                requestSent = true;
                ++tries;
                stopwatch.Start();
            }
            else if (replyReceived)
            {
                stopwatch.Stop();
                stopwatch.Reset();
                player.ChangeState(NextState());
            }
            else if (stopwatch.Elapsed.CompareTo(Player.timeout) >= 0)
            {
                LogInfo("Timer timed out waiting for response");
                stopwatch.Stop();
                stopwatch.Reset();
                requestSent = false;
            }
            // TODO: Do something if all tries expended.
        }

        public abstract void Receive(Message message);

        protected abstract Request CreateRequest();

        protected abstract PublicEndPoint GetEndPoint();

        protected abstract void LogDebug(string msg);

        protected abstract void LogInfo(string msg);

        protected abstract string GetAttemptMessage();

        protected abstract State NextState();
    }
}
