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
    class GetGamesState : MessageState
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GetGamesState));

        public GetGamesState(Player player) : base(player)
        {
        }
    
        public void Receive(Message message)
        {
            if (message is GameListReply)
            {
                replyReceived = true;
            }
        }

        public Request createRequest()
        {
            GameListRequest request = new GameListRequest()
            {
                StatusFilter = (int)GameInfo.StatusCode.Available
            };
            return request;
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
            return new JoinGameState(player);
        }
    }
}
