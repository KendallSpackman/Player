using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading;
using System.Net.Sockets;

using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using Player;
using SharedObjects;

namespace PlayerTesting
{
    [TestClass]
    public class LoginTester
    {
        [TestMethod]
        public void Login_BasicLogic()
        {
            //// Create udp client that represents the registry
            //PublicEndPoint mockEndpoint = new PublicEndPoint
            //{
            //    Host = 
            //    Port = 
            //};

            //// Create player with all the necessary registry end point and identity information
            //TestablePlayer player = new TestablePlayer()
            //{
            //    RegistryEndPoint = mockClientEP,
            //    FirstName = "Joe",
            //    ...
            //};

            //// Initialize the player
            //player.Initialize();

            //// Call TryLogin()
            //Thread loginThread = new Thread(new ThreadStart(player.TestLogin));
            //loginThread.Start();

            //// Get message from udp client
            //IPEndPoint senderEP = new IPEndPoint(IPAddress.Any, 0);
            //byte[] bytes = mockClient.Receive(ref senderEP);

            //// Assert not null and is a LoginRequest
            //Assert.IsNotNull(bytes);
            //Assert.AreNotEqual(0, bytes.Length);

            //Message msg = Message.Decode(bytes);
            //Assert.IsNotNull(msg);
            //Assert.IsTrue(msg is LoginRequest);
            //LoginRequest request = msg as LoginRequest;
            //Assert.AreEqual("Joe", request.Identity.FirstName);

            //// Send back a LoginReply from udp client with mock information
            //LoginReply reply = new LoginReply();

            //// Assert ProcessInfo of player is same as mock LoginReply's ProcessInfo
            //Thread.Sleep(2000);
            //Assert.AreEqual(player.ProcessInfo, reply.ProcessInfo);
        }
    }

    public class TestablePlayer : Player.Player
    {
        public void TestLogin()
        {
            //base.TryToLogin();
        }
    }
}
