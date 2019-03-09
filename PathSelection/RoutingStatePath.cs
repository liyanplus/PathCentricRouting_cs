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

        public List<IRoutingState> GetNextStates()
        {
            throw new NotImplementedException();
        }
    }
}
