using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using log4net;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace Player
{
    public class Player
    {
        public string ProcessLabel { get; set; }
        public IdentityInfo IdentityInfo { get; set; }
        public ProcessInfo ProcessInfo { get; set; }
        public GameInfo[] GamesList { get; set; }
        public int LifePoints { get; set; }
        public PublicEndPoint RegistryEndPoint { get; set; }
        public Messager Messager { get; private set; }

        public delegate void PlayerUpdateHandler();
        public event PlayerUpdateHandler PlayerUpdate;

        private static readonly ILog logger = LogManager.GetLogger(typeof(Player));
        private object myLock = new object();
        public static readonly TimeSpan timeout = new TimeSpan(0, 0, 10);
        public static readonly int retries = 3;
        private bool keepRunning = true;
        protected State currentState;
        private Thread messagerThread;
        private ConcurrentQueue<Messages.Message> messageQueue;

        private static Dictionary<Type, int> typeDict = new Dictionary<Type, int>
        {
            {typeof(GameListReply),0}, {typeof(JoinGameReply),1},
            {typeof(LoginReply),2}, {typeof(AliveRequest),3}
        };

        public Player()
        {
            currentState = new LoginState(this);
            ProcessInfo = new ProcessInfo();
            ProcessInfo.Status = ProcessInfo.StatusCode.NotInitialized;
            Messager = new Messager();
            messageQueue = Messager.Queue;
            messagerThread = new Thread(
                new ThreadStart(Messager.Receive));
            messagerThread.Start();
        }

        public void Start()
        {
            while (keepRunning)
            {
                ProcessMessages();

                currentState.PerformAction();
                
                if (PlayerUpdate != null)
                {
                    PlayerUpdate();
                }
                Thread.Sleep(100);
            }
            Logout();
            WaitLogoutReply();
        }

        public void Stop()
        {
            keepRunning = false;
            Messager.Stop();
            messagerThread.Join();
        }

        public void ChangeState(State newState)
        {
            currentState = newState;
        }

        private void Logout()
        {
            logger.Debug("Attempting to log out.");
            LogoutRequest logoutRequest = new LogoutRequest();
            Messager.Send(logoutRequest, RegistryEndPoint);
        }

        private bool WaitLogoutReply()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool wait = true;
            while (wait)
            {
                Thread.Sleep(300);
                lock (myLock)
                {
                    if (ProcessInfo == null)
                        wait = false;
                }
                if (stopwatch.Elapsed.CompareTo(timeout) >= 0)
                {
                    logger.Info("Timer timed out waiting for response.");
                    wait = false;
                }
            }
            stopwatch.Stop();
            if (ProcessInfo == null)
                return true;
            else return false;
        }

        private void ProcessMessages()
        {
            Messages.Message message;
            bool success = true;
            while (success)
            {
                success = messageQueue.TryDequeue(out message);
                if (success)
                    Receive(message);
            }
        }

        private void Receive(Messages.Message message)
        {
            try
            {
                switch (typeDict[message.GetType()])
                {
                    case 0:
                        Receive(message as GameListReply);
                        break;
                    case 1:
                        Receive(message as JoinGameReply);
                        break;
                    case 2:
                        Receive(message as LoginReply);
                        break;
                    case 3:
                        Receive(message as AliveRequest);
                        break;
                    default:
                        return;
                }
                currentState.Receive(message);
            } catch (KeyNotFoundException)
            {
                logger.Debug("Unknown message received.");
            }
        }

        private void Receive(LoginReply reply)
        {
            logger.Debug("Received Login Reply");
            if (reply.Success)
                ProcessInfo = reply.ProcessInfo;
            else
                logger.Error(reply.Note);
        }

        private void Receive(GameListReply reply)
        {
            logger.Debug("Received Game List Reply");
            if (reply.Success)
            {
                GamesList = reply.GameInfo;
            }
            else
                logger.Error(reply.Note);
        }
   
        private void Receive(JoinGameReply reply)
        {
            logger.Debug("Received Join Game Reply");
            if (reply.Success)
            {
                LifePoints = reply.InitialLifePoints;
            }
            else
                logger.Error(reply.Note);
        }

        public void Receive(AliveRequest request)
        {
            logger.Debug("Received Alive Request");
            Reply reply = new Reply()
            {
                Success = true,
                Note = "I'm alive!"
            };
            logger.Debug("Sending Alive Reply");
            Messager.Send(reply, RegistryEndPoint);
        }

        private void Receive(Reply reply)
        {
            logger.Debug("Received Logout Reply");
            ProcessInfo = null;
        }
    }
}
