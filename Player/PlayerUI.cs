using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Player
{
    public partial class PlayerUI : Form
    {

        private Player player;
        private bool doUpdate = false;

        public PlayerUI(Player player)
        {
            InitializeComponent();
            this.player = player;
            player.PlayerUpdate += UpdateDisplay;
        }

        public void UpdateDisplay()
        {
            doUpdate = true;
        }

        private void UpdateDisplay(object sender, EventArgs e)
        {
            if (doUpdate)
            {
                doUpdate = false;
                if (player.ProcessLabel != null)
                    label3.Text = player.ProcessLabel;
                if (player.ProcessInfo != null)
                {
                    if (player.ProcessInfo.EndPoint != null)
                    {
                        label4.Text = player.ProcessInfo.EndPoint.HostAndPort;
                    }
                    label5.Text = player.ProcessInfo.StatusString;
                }
            }
        }

        private void PlayerUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            player.PlayerUpdate -= UpdateDisplay;
        }

        private void PlayerUI_Load(object sender, EventArgs e)
        {
            Timer Timer = new Timer();
            Timer.Interval = (1000);
            Timer.Tick += new EventHandler(UpdateDisplay);
            Timer.Start();
        }
    }
}
