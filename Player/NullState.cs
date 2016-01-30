using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Messages;

namespace Player
{
    public class NullState : State
    {
        Player player;

        public NullState(Player player)
        {
            this.player = player;
        }

        public void PerformAction()
        {
        }

        public void Receive(Message message)
        {
        }
    }
}
