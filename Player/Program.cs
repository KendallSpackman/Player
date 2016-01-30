using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;

using log4net;
using log4net.Config;
using SharedObjects;

namespace Player
{
    class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            // Setup up logging with log4net
            XmlConfigurator.Configure();

            string ep = Properties.Settings.Default.RegistryEndPoint;
            if (args.Length > 0)
            {
                if (args[0].ToLower().Contains("local"))
                {
                    ep = Properties.Settings.Default.LocalRegistryEndPoint;
                }
            }
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
                    HostAndPort = ep
                }
            };
            
            logger.Debug("Starting player");
            Thread playerThread = new Thread(new ThreadStart(player.Start));
            playerThread.Start();

            PlayerUI ui = new PlayerUI(player);
            Application.Run(ui);

            player.Stop();
            playerThread.Join();
        }
    }
}
