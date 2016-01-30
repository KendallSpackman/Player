using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Messages;

namespace Player
{
    public interface State
    {
        void PerformAction();

        void Receive(Message message);
    }
}
