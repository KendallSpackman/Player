using System;
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
    class JoinGameState : MessageState
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GetGamesState));

        public JoinGameState(Player player) : base(player)
        {
            maxTries = player.GamesList.Length;
        }
    
        public void Receive(Message message)
        {
            if (message is JoinGameReply)
            {
                replyReceived = true;
            }
        }

        public Request createRequest()
        {
            JoinGameRequest request = new JoinGameRequest()
            {
                GameId = player.GamesList[tries].GameId,
                Player = player.ProcessInfo
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
