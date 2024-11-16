using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Animation
{
    internal struct AnimationState
    {
        public int Length;
        public int Location;
        public int Time;

        public AnimationState(int location, int length, int time)
        {
            Location = location;
            Length = length;
            Time = time;
        }
    }
}
