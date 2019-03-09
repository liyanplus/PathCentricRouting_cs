using System;
using SpatialNetwork;

namespace PathSelection
{
    public class PathSelector
    {
        #region property
        private CandidateQueue candidates = new CandidateQueue();
        private Network network;
        #endregion
        public PathSelector(Network network)
        {
            this.network = network;
        }

        public Path FindPath(uint[] porigin, uint pdestination) {
 
            // Initialize List of Candidate Paths
            // Add Candidate Paths to queue

            // While Stopping condition is false

                // Extract Min Cost CP
                // Extend CP
                // Add CP to Queue
            Edge destination = network.Edges[pdestination];

            //
            candidates.Insert( new RoutingStateEdge());

            while(candidates.Count > 0)
            {

            }

            Console.WriteLine("No path found");
            return null;

        }
    }
}
