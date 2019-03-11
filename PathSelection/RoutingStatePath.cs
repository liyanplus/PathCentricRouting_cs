using System;
using System.Collections.Generic;

namespace PathSelection
{
    public class RoutingStatePath : IRoutingState
    {
        public RoutingStatePath()
        {
        }

        public double Cost { get; set; }
        public IRoutingState Parent { get; set; }
        public List<uint> EdgeIDsForState { get; set; }

        public List<IRoutingState> GetNextStates()
        {
            throw new NotImplementedException();
        }

        public bool IsGoalState()
        {
            throw new NotImplementedException();
        }
    }
}
