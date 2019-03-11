using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialNetwork;
using EnergyCostEstimator;

namespace PathSelection
{
    // Caller class for Old Method
    class FindPathExtendEdge
    {
        Network Network { get; set; }
        TraceDB TraceDB { get; set; }
        uint Beta { get; set; }

        public FindPathExtendEdge(Network network, TraceDB traceDB, uint beta = 5)
        {
            this.Network = network;
            this.TraceDB = traceDB;
            this.Beta = beta;
        }

        public Path FindPath(uint[] porigin, uint pdestination)
        {
            // Initialize parameters for RoutingStateEdge
            RoutingStateEdge.Init(pdestination, Network, TraceDB, Beta);

            // Find number of trajectories along first 2 edges
            if (porigin.Length != 2)
                return null;

            // Get Number of Traces along first 2 edges
            HashSet<uint> pathsAlongFirstTwoEdges = TraceDB.GetTracesAlongPath(porigin.ToList());

            // If less than Beta traces along first 2 edges, then no path possible
            if (pathsAlongFirstTwoEdges.Count < Beta)
                return null;

            PathSelector pathSelector = new PathSelector();
            List<IRoutingState> origins = new List<IRoutingState>();

            // TODO: Need some way to compute the cost

            // First edge is a dummy edge to help in the GetPreviousTwoEdges function
            RoutingStateEdge firstEdge = new RoutingStateEdge(porigin[0], 0, null);
  
            RoutingStateEdge secondEdge = new RoutingStateEdge(porigin[1], 0, firstEdge);

            // Add only the second state to the list, this corresponds to the path consisting of the first 2 edges
            origins.Add(secondEdge);

            return pathSelector.FindPath(origins);
        }

    }
}
