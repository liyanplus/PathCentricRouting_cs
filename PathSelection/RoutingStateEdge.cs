using System;
using System.Collections.Generic;
using SpatialNetwork;
using EnergyCostEstimator;
using System.Linq;

namespace PathSelection
{
    public class RoutingStateEdge : IRoutingState
    {
        // inherited variables
        public double Cost { get; set; }
        public IRoutingState Parent { get; set; }
        public List<uint> EdgeIDsForState { get; set; }

        private uint EdgeID { get; set; }

        static private uint GoalStateID { get; set; }
        static private Network Network { get; set; }
        static private TraceDB TraceDB { get; set; }
        static private uint Beta { get; set; }

        public static void Init(uint goalStateID, Network network, TraceDB traceDB, uint beta)
        {
            GoalStateID = goalStateID;
            Network = network;
            TraceDB = traceDB;
            Beta = beta;
        }


        public RoutingStateEdge(uint edgeID, double cost, RoutingStateEdge parent)
        {
            this.EdgeID = edgeID;
            this.Cost = cost;
            this.Parent = parent;
            this.EdgeIDsForState = new List<uint> {
                edgeID
            };
        }


        public bool IsGoalState()
        {
            return this.EdgeID == GoalStateID;
        }


        public List<IRoutingState> GetNextStates()
        {
            List<IRoutingState> nextStates = new List<IRoutingState>();

            // Get node that this edge connects to
            uint nextNode = Network.Edges[EdgeID].ToNodeId;

            // Get all Edges from that node
            List<uint> allEdgesFromNode = Network.Nodes[nextNode].ConnectedEdgesId;

            // Only get outgoing edges from that node
            List<uint> possibleEdges = allEdgesFromNode.Where(edge => Network.Edges[edge].FromNodeId == nextNode ).ToList();

            List<uint> previousEdges = GetPreviousTwoEdges();

            foreach(uint possibleEdge in possibleEdges)
            {
                previousEdges.Add(possibleEdge);
                HashSet<uint> tracesAlongPath = TraceDB.GetTracesAlongPath(previousEdges);
                if (tracesAlongPath.Count >= Beta)
                {
                    // TODO: Determine way to compute cost
                    nextStates.Add(new RoutingStateEdge(possibleEdge, 123, this));
                }
            }

            return nextStates;
        }

        public List<uint> GetPreviousTwoEdges()
        {
            List<uint> previousEdges = new List<uint>();

            // Get previous State
            RoutingStateEdge previousState = (RoutingStateEdge)Parent;

            if (previousState == null)
                return previousEdges;

            // Add EdgeID of Previous state
            previousEdges.Add(previousState.EdgeID);

            // Get state 2 before current
            RoutingStateEdge prevPreviousState = (RoutingStateEdge)previousState.Parent;
            if (prevPreviousState == null)
                return previousEdges;

            // Add that edgeId to list
            previousEdges.Add(prevPreviousState.EdgeID);

            return previousEdges;
        }


    }
}
