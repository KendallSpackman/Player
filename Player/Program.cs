using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

using SharedObjects;

namespace Player
{
    class Program
    {
        static void Main(string[] args)
        {
            Processor processor = Processor.Instance();
            Messager messager = Messager.Instance();
            processor.Queue = messager.Queue;
            Thread messagerThread = new Thread(
                new ThreadStart(Messager.Instance().Receive));
            messagerThread.Start();

            Player player = new Player()
            {
                IdentityInfo = new IdentityInfo()
                {
                    LastName = Properties.Settings.Default.LastName,
                    FirstName = Properties.Settings.Default.FirstName,
                    ANumber = Properties.Settings.Default.ANumber,
                    Alias = Properties.Settings.Default.Alias
                },
                ProcessLabel = Properties.Settings.Default.ProcessLabel,
                RegistryEndPoint = new PublicEndPoint()
                {
                    HostAndPort = Properties.Settings.Default.RegistryEndPoint
                }
            };
            Processor.Instance().Player = player;

            Thread processorThread = new Thread(
                new ThreadStart(Processor.Instance().Run));
            processorThread.Start();

            Console.Write("Run");
            player.Run();

            Console.WriteLine("Press any key to stop player ...");
            Console.ReadKey(true);
            Console.Write("Stop");
            player.Stop();
        }
    }
}
