using System;
using System.Diagnostics;
using System.Collections.Generic;
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
    public class GetGamesState : MessageState
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GetGamesState));

        public GetGamesState(Player player) : base(player)
        {
        }

        public override void Receive(Message message)
        {
            if (message is GameListReply)
            {
                replyReceived = true;
            }
        }

        protected override Request CreateRequest()
        {
            LogDebug("Sending Game List Request");
            GameListRequest request = new GameListRequest()
            {
                StatusFilter = (int)GameInfo.StatusCode.Available
            };
            return request;
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
            return "Get games attempt number " + tries;
        }

        protected override State NextState()
        {
            return new JoinGameState(player);
        }
    }
}
