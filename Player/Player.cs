using System;
using System.Collections.Generic;
//using System.Diagnostics;
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
        private static readonly ILog logger = LogManager.GetLogger(typeof(Player));
        private object myLock = new object();
        public static readonly TimeSpan timeout = new TimeSpan(0, 0, 10);
        public static readonly int retries = 3;
        private bool keepRunning = true;
        private PlayerUI ui;
        private State currentState;
        public string ProcessLabel { get; set; }
        public IdentityInfo IdentityInfo { get; set; }
        public ProcessInfo ProcessInfo { get; set; }
        public GameInfo[] GamesList { get; set; }
        public int LifePoints { get; set; }
        public PublicEndPoint RegistryEndPoint { get; set; }

        public Player()
        {
            ui = new PlayerUI();
            ui.setProcessLabel(ProcessLabel);
            currentState = new LoginState(this);
        }

        public void Run()
        {
            bool success = true;
            int tries;
            while (keepRunning)
            {
                currentState.PerformAction();
                //if (ProcessInfo == null)
                //{
                //    success = false;
                //    tries = 0;
                //    while (!success && tries < retries)
                //    {
                //        Login();
                //        success = WaitLoginReply();
                //        ++tries;
                //    }
                //}
                //if (!success) return;

                //if (ProcessInfo.Status == ProcessInfo.StatusCode.Registered)
                //{
                //    success = false;
                //    tries = 0;
                //    while (!success && tries < retries)
                //    {
                //        GetGamesList();
                //        success = WaitGamesListReply();
                //        ++tries;
                //    }
                //    if (!success) return;

                //    success = false;
                //    tries = 0;
                //    while (!success && tries < GamesList.Length)
                //    {
                //        JoinGame(GamesList[tries]);
                //        success = WaitJoinGameReply();
                //        ++tries;
                //    }
                //}

                //UpdateUi();
            }
            Logout();
            WaitLogoutReply();
        }

        public void Stop()
        {
            keepRunning = false;
        }

        private void UpdateUi()
        {
        }

        //public bool WaitLoginReply()
        //{
        //    Stopwatch stopwatch = new Stopwatch();
        //    stopwatch.Start();
        //    bool wait = true;
        //    while (wait)
        //    {
        //        Thread.Sleep(300);
        //        lock (myLock)
        //        {
        //            if (ProcessInfo != null)
        //                wait = false;
        //        }
        //        if (stopwatch.Elapsed.CompareTo(timeout) >= 0)
        //            wait = false;
        //    }
        //    stopwatch.Stop();
        //    if (ProcessInfo != null)
        //        return true;
        //    else return false;
        //}

        //public void GetGamesList()
        //{
        //    logger.Debug("Attempting to request games list.");
        //    GameListRequest gameListRequest = new GameListRequest()
        //    {
        //        StatusFilter = (int)GameInfo.StatusCode.Available
        //    };
        //    Messager.Instance().Send(gameListRequest, RegistryEndPoint);
        //}

        public bool WaitGamesListReply()
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //bool wait = true;
            //while (wait)
            //{
            //    Thread.Sleep(300);
            //    lock (myLock)
            //    {
            //        if (GamesList != null)
            //            wait = false;
            //    }
            //    if (stopwatch.Elapsed.CompareTo(timeout) >= 0)
            //    {
            //        logger.Info("Timer timed out waiting for response");
            //        wait = false;
            //    }
            //}
            //stopwatch.Stop();
            //if (GamesList != null)
            //    return true;
            //else return false;
        }

        public void JoinGame(GameInfo game)
        {
            logger.Debug("Attempting to join game.");
            if (joinedGame)
                logger.Warn("Already joined a game.");
            JoinGameRequest joinGameRequest = new JoinGameRequest()
            {
                GameId = game.GameId,
                Player = ProcessInfo
            };
            Messager.Instance().Send(joinGameRequest, game.GameManager.EndPoint);
        }

        public bool WaitJoinGameReply()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool wait = true;
            while (wait)
            {
                Thread.Sleep(300);
                lock (myLock)
                {
                    if (joinedGame)
                        wait = false;
                }
                if (stopwatch.Elapsed.CompareTo(timeout) >= 0)
                {
                    logger.Info("Timer timed out waiting for response");
                    wait = false;
                }
            }
            stopwatch.Stop();
            if (joinedGame)
                return true;
            else return false;
        }

        public void Logout()
        {
            logger.Debug("Attempting to log out.");
            LogoutRequest logoutRequest = new LogoutRequest();
            Messager.Instance().Send(logoutRequest, RegistryEndPoint);
        }

        public bool WaitLogoutReply()
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

        public void Receive(LoginReply reply)
        {
            logger.Debug("Received Login Reply.");
            if (reply.Success)
                ProcessInfo = reply.ProcessInfo;
            else
                logger.Error(reply.Note);
        }

        public void Receive(GameListReply reply)
        {
            logger.Debug("Received Game List Reply.");
            if (reply.Success)
            {
                GamesList = reply.GameInfo;
            }
            else
                logger.Error(reply.Note);
        }
   
        public void Receive(JoinGameReply reply)
        {
            logger.Debug("Received Join Game Reply.");
            if (reply.Success)
            {
                joinedGame = true;
                LifePoints = reply.InitialLifePoints;
            }
            else
                logger.Error(reply.Note);
        }

        public void Receive(AliveRequest request)
        {
            logger.Debug("Received Alive Request.");
            Reply reply = new Reply()
            {
                Success = true,
                Note = "I'm alive!"
            };
            Messager.Instance().Send(reply, RegistryEndPoint);
        }

        public void Receive(Reply reply)
        {
            logger.Debug("Received Logout Reply.");
            ProcessInfo = null;
        }

        public void ChangeState(State newState)
        {
            currentState = newState;
        }
    }
}
