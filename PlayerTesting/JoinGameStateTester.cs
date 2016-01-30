using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading;
using System.Net;
using System.Net.Sockets;

using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using Player;
using SharedObjects;

namespace PlayerTesting
{
    [TestClass]
    public class JoinGameStateTester
    {
        [TestMethod]
        public void TestJoinGame()
        {
            // Create udp client to mock registry
            UdpClient mockClient = new UdpClient(0);
            int mockClientPort = ((IPEndPoint)mockClient.Client.LocalEndPoint).Port;
            PublicEndPoint mockClientEP = new PublicEndPoint()
            {
                Host = "127.0.0.1",
                Port = mockClientPort
            };

            // Create fake player
            TestablePlayer player = new TestablePlayer()
            {
                RegistryEndPoint = mockClientEP,
                ProcessInfo = new ProcessInfo()
                {
                    ProcessId = 5,
                    Type = ProcessInfo.ProcessType.Player,
                    EndPoint = mockClientEP,
                    Label = "Test Player",
                    Status = ProcessInfo.StatusCode.Initializing
                }
            };
            player.GamesList = new GameInfo[]
            {
                new GameInfo()
                {
                    GameId = 7,
                    Label = "Test Game",
                    Status = GameInfo.StatusCode.Available,
                    GameManager = new ProcessInfo()
                    {
                        EndPoint = mockClientEP
                    }
                }
            };
            player.CurrentState = new JoinGameState(player);

            // Run player
            Thread playerThread = new Thread(new ThreadStart(player.Start));
            playerThread.Start();

            // Get message registry client
            IPEndPoint senderEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = mockClient.Receive(ref senderEP);

            // Assert on GameListRequest
            Message msg = Message.Decode(bytes);
            Assert.IsNotNull(msg);
            Assert.IsTrue(msg is JoinGameRequest);
            JoinGameRequest request = msg as JoinGameRequest;
            Assert.AreEqual(request.GameId, player.GamesList[0].GameId);
            Assert.AreEqual(request.Player.ProcessId, player.ProcessInfo.ProcessId);
            Assert.AreEqual(request.Player.Type, player.ProcessInfo.Type);
            Assert.AreEqual(request.Player.Label, player.ProcessInfo.Label);

            // Send GameListReply from registry to player
            JoinGameReply reply = new JoinGameReply()
            {
                GameId = request.GameId,
                InitialLifePoints = 20,
                Success = true
            };
            bytes = reply.Encode();
            mockClient.Send(bytes, bytes.Length, senderEP);

            // Assert on GameListReply, player state
            Thread.Sleep(2000);
            player.Stop();
            playerThread.Join();
            Assert.AreEqual(player.LifePoints, reply.InitialLifePoints);
            Assert.IsTrue(player.CurrentState is NullState);
            Assert.AreEqual(player.ProcessInfo.Status, ProcessInfo.StatusCode.JoinedGame);
        }
    }
}
