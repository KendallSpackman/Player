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
        public PlayerUI()
        {
            InitializeComponent();
        }

        public void setProcessLabel(string processID)
        {
            label3.Text = processID;
        }

        public void setPublicEndPoint(string endPoint)
        {
            label4.Text = endPoint;
        }

        public void setStatus(string status)
        {
            label5.Text = status;
        }
    }
}
