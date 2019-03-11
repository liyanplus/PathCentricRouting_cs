using System;
using System.Collections.Generic;
using SpatialNetwork;

namespace PathSelection
{
    public class PathSelector
    {
        #region property
        private CandidateQueue candidates = new CandidateQueue();
        #endregion

        public PathSelector()
        {
        }

        public Path GeneratePath(IRoutingState state)
        {
            Path path = new Path();
            while(state != null)
            {
                for(int i = state.EdgeIDsForState.Count - 1; i >= 0; i--)
                {
                    path.AddFirst(state.EdgeIDsForState[i]);
                }
                state = state.Parent;
            }
            return path;
        }

        public Path FindPath(List<IRoutingState> originStates) {

            // Add initial start states to Priority Queue
            candidates.Insert(originStates);

            while(candidates.Count > 0)
            {
                // Get lowest cost state
                IRoutingState minCostRoutingState = candidates.GetNext();

                // If this is the goal then we are done
                if (minCostRoutingState.IsGoalState())
                {
                    // Return the path here
                    return GeneratePath(minCostRoutingState);
                }

                candidates.Insert(minCostRoutingState.GetNextStates());
            }

            // If queue is empty and we never reached the goal state, no path was possible
            Console.WriteLine("No path found");
            return null;

        }
    }
}
