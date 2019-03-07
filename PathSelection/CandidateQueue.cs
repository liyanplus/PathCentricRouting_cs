using System;
using MinMaxHeap;
using System.Collections.Generic;

namespace PathSelection
{
    public class CandidateQueue : MinHeap<double, IRoutingState>
    {
        public CandidateQueue()
        {
        }

        public IRoutingState GetNext()
        {
            return ExtractMin().Value;
        }

        public void Insert(IRoutingState pstate) { }

        public void Insert(List<IRoutingState> pstates) { }
    }
}
