using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace Player
{
    class LoginState : MessageState
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LoginState));

        public LoginState(Player player) : base(player)
        {
        }
    
        public void Receive(Message message)
        {
            if (message is LoginReply)
            {
                replyReceived = true;
            }
        }

        public Request createRequest()
        {
            LoginRequest loginRequest = new LoginRequest()
            {
                Identity = player.IdentityInfo,
                ProcessLabel = player.ProcessLabel,
                ProcessType = ProcessInfo.ProcessType.Player
            };
            return loginRequest;
        }

        public void logDebug(string msg)
        {
            logger.Debug(msg);
        }

        public void logInfo(string msg)
        {
            logger.Info(msg);
        }

        public State nextState()
        {
            return new GetGamesState(player);
        }
    }
}
