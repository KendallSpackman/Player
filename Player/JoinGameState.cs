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
    public class JoinGameState : MessageState
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(JoinGameState));

        public JoinGameState(Player player) : base(player)
        {
            maxTries = player.GamesList.Length;
        }

        public override void Receive(Message message)
        {
            if (message is JoinGameReply)
            {
                replyReceived = true;
                player.ProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            }
        }

        protected override Request CreateRequest()
        {
            LogDebug("Sending Join Game Request");
            JoinGameRequest request = new JoinGameRequest()
            {
                GameId = player.GamesList[tries].GameId,
                Player = player.ProcessInfo
            };
            player.ProcessInfo.Status = ProcessInfo.StatusCode.JoiningGame;
            return request;
        }

        protected override PublicEndPoint GetEndPoint()
        {
            return player.GamesList[tries].GameManager.EndPoint;
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
            return "Join game attempt number " + tries;
        }

        protected override State NextState()
        {
            // For HW1, there is nothing to do after joining a game, so enter a null state
            // Should be replaced in further assignments
            return new NullState(player);
        }
    }
}
