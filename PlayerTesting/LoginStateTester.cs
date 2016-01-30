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
    public class LoginStateTester
    {
        [TestMethod]
        public void TestLogin()
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
                IdentityInfo = new IdentityInfo()
                {
                    FirstName = "Test",
                    LastName = "Player",
                    ANumber = "A01234983",
                    Alias = "TP"
                },
                ProcessLabel = "Test TP"
            };

            // Run player
            Thread loginThread = new Thread(new ThreadStart(player.Start));
            loginThread.Start();

            // Get message from registry client
            IPEndPoint senderEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = mockClient.Receive(ref senderEP);
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);

            // Assert on LoginRequest
            Message msg = Message.Decode(bytes);
            Assert.IsNotNull(msg);
            Assert.IsTrue(msg is LoginRequest);
            LoginRequest request = msg as LoginRequest;
            Assert.AreEqual(player.IdentityInfo.Alias, request.Identity.Alias);
            Assert.AreEqual(player.IdentityInfo.ANumber, request.Identity.ANumber);
            Assert.AreEqual(player.IdentityInfo.FirstName, request.Identity.FirstName);
            Assert.AreEqual(player.IdentityInfo.LastName, request.Identity.LastName);
            Assert.AreEqual(player.ProcessLabel, request.ProcessLabel);
            Assert.AreEqual(ProcessInfo.ProcessType.Player,request.ProcessType);

            // Send LoginReply from registry to player
            LoginReply reply = new LoginReply()
            {
                ProcessInfo = new ProcessInfo()
                {
                    ProcessId = 5,
                    Type = ProcessInfo.ProcessType.Player,
                    EndPoint = new PublicEndPoint()
                    {
                        HostAndPort = senderEP.ToString()
                    },
                    Label = "Test Player",
                    Status = ProcessInfo.StatusCode.Initializing
                },
                Success = true
            };
            bytes = reply.Encode();
            mockClient.Send(bytes, bytes.Length, senderEP);

            // Assert on LoginReply, player state
            Thread.Sleep(2000);
            player.Stop();
            loginThread.Join();
            Assert.AreEqual(player.ProcessInfo.ProcessId, reply.ProcessInfo.ProcessId);
            Assert.AreEqual(player.ProcessInfo.Type, reply.ProcessInfo.Type);
            Assert.AreEqual(player.ProcessInfo.EndPoint.HostAndPort, reply.ProcessInfo.EndPoint.HostAndPort);
            Assert.AreEqual(player.ProcessInfo.Label, reply.ProcessInfo.Label);
            Assert.AreEqual(player.ProcessInfo.Status, reply.ProcessInfo.Status);
            Assert.IsTrue(player.CurrentState is GetGamesState);
        }
    }
}
