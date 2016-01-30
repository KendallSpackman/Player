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
    public class LoginState : MessageState
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LoginState));

        public LoginState(Player player) : base(player)
        {
        }

        public override void Receive(Message message)
        {
            if (message is LoginReply)
            {
                replyReceived = true;
            }
        }

        protected override Request CreateRequest()
        {
            LogDebug("Sending Login Request");
            LoginRequest loginRequest = new LoginRequest()
            {
                Identity = player.IdentityInfo,
                ProcessLabel = player.ProcessLabel,
                ProcessType = ProcessInfo.ProcessType.Player
            };
            player.ProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            return loginRequest;
        }

        protected override PublicEndPoint GetEndPoint()
        {
            return player.RegistryEndPoint;
        }

        protected override void LogDebug(string msg)
        {
            logger.Debug(msg);
        }

        protected override void LogInfo(string msg)
        {
            logger.Info(msg);
        }

        protected override string GetAttemptMessage()
        {
            return "Login attempt number " + tries;
        }

        protected override State NextState()
        {
            return new GetGamesState(player);
        }
    }
}
