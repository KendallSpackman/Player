using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Player;

namespace PlayerTesting
{
    public class TestablePlayer : Player.Player
    {
        public State CurrentState { get { return currentState; } set { currentState = value; } }
    }
}
