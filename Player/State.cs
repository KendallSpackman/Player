using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Messages;

namespace Player
{
    interface State
    {
        public void PerformAction();

        public void Receive(Message message);
    }
}
