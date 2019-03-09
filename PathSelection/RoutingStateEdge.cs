using System;
using System.Collections.Generic;

namespace PathSelection
{
    public class RoutingStateEdge : IRoutingState
    {
        double IRoutingState.Cost { get; set; }

        public RoutingStateEdge()
        {
        }

        public List<IRoutingState> GetNextStates()
        {
            throw new NotImplementedException();
        }
    }
}
