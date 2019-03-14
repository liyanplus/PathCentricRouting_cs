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

        Paware Paware { get; set; }

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

        // TODO: Can we use nested constructors here??
        public RoutingStateEdge(uint edgeID, RoutingStateEdge parent)
        {
            this.Parent = parent;
            this.Paware = Parent == null ? null : new Paware(((RoutingStateEdge)Parent).Paware, edgeID);
            this.Cost = Paware == null ? 0 : Paware.Cost;
            this.EdgeIDsForState = new List<uint> {
                edgeID
            };
        }

        public RoutingStateEdge(uint edgeID, RoutingStateEdge parent, Paware paware)
        {
            this.Parent = parent;
            this.Paware = paware;
            this.Cost = Paware == null ? 0 : Paware.Cost;
            this.EdgeIDsForState = new List<uint> {
                edgeID
            };
        }



        public bool IsGoalState()
        {
            return this.EdgeIDsForState[0] == GoalStateID;
        }


        public List<IRoutingState> GetNextStates()
        {
            List<IRoutingState> nextStates = new List<IRoutingState>();

            // Get node that this edge connects to
            uint nextNode = Network.Edges[EdgeIDsForState[0]].ToNodeId;

            // Only get outgoing edges from that node
            List<uint> possibleEdges = Network.Nodes[nextNode].GetOutgoingEdges();

            foreach (uint possibleEdge in possibleEdges)
            {

                // TODO: Make sure this is correct
                Paware extendedPaware = new Paware(this.Paware, possibleEdge);
                if (extendedPaware.Count > 2)
                {
                    // TODO: Determine way to compute cost
                    nextStates.Add(new RoutingStateEdge(possibleEdge, this, extendedPaware));
                }
            }

            // No need to store Paware for this state after genertaing next states
            Paware = null;

            return nextStates;
        }
    }
}
