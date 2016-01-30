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
    public class GetGamesStateTester
    {
        [TestMethod]
        public void TestGetGames()
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
                RegistryEndPoint = mockClientEP
            };
            player.CurrentState = new GetGamesState(player);

            // Run player
            Thread playerThread = new Thread(new ThreadStart(player.Start));
            playerThread.Start();

            // Get message registry client
            IPEndPoint senderEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = mockClient.Receive(ref senderEP);

            // Assert on GameListRequest
            Message msg = Message.Decode(bytes);
            Assert.IsNotNull(msg);
            Assert.IsTrue(msg is GameListRequest);
            GameListRequest request = msg as GameListRequest;
            Assert.AreEqual(request.StatusFilter, (int)GameInfo.StatusCode.Available);

            // Send GameListReply from registry to player
            GameListReply reply = new GameListReply()
            {
                GameInfo = new GameInfo[] {
                    new GameInfo() {
                        GameId = 7,
                        Label = "Test Game",
                        Status = GameInfo.StatusCode.Available,
                        GameManager = new ProcessInfo()
                        {
                            EndPoint = new PublicEndPoint()
                            {
                                HostAndPort = senderEP.ToString()
                            }
                        }
                    }
                },
                Success = true
            };
            bytes = reply.Encode();
            mockClient.Send(bytes, bytes.Length, senderEP);

            // Assert on GameListReply, player state
            Thread.Sleep(2000);
            player.Stop();
            playerThread.Join();
            Assert.AreEqual(player.GamesList.Length, 1);
            GameInfo playerInfo = player.GamesList[0];
            GameInfo replyInfo = reply.GameInfo[0];
            Assert.AreEqual(playerInfo.GameId, replyInfo.GameId);
            Assert.AreEqual(playerInfo.Label, replyInfo.Label);
            Assert.AreEqual(playerInfo.Status, replyInfo.Status);
            Assert.AreEqual(playerInfo.GameManager.EndPoint.HostAndPort, replyInfo.GameManager.EndPoint.HostAndPort);
            Assert.IsTrue(player.CurrentState is JoinGameState);
        }
    }
}
