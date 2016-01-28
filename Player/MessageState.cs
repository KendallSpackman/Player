using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;

namespace Player
{
    abstract class MessageState : State
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
                logDebug("Login attempt number " + tries);

                Request request = createRequest();
                Messager.Instance().Send(request, player.RegistryEndPoint);
                requestSent = true;
                ++tries;
                stopwatch.Restart();
            }
            else if (replyReceived)
            {
                stopwatch.Stop();
                player.ChangeState(nextState());
            }
            else if (stopwatch.Elapsed.CompareTo(Player.timeout) >= 0)
            {
                logInfo("Timer timed out waiting for response");
                stopwatch.Stop();
                requestSent = false;
            }
        }

        public Request createRequest();

        public void logDebug(string msg);

        public void logInfo(string msg);

        public State nextState();
    }
}
