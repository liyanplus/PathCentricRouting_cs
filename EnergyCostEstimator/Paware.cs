using System;
using System.Collections.Generic;

namespace EnergyCostEstimator
{
    public class Paware : List<uint>
    {
        #region property
        public HashSet<uint> PassingTraceIds { get; set; }
        public List<int> StartingTracePartIdxes { get; set; }
        public double Cost { get; set; }
        #endregion

        public Paware(Paware previousPaware, uint newEdgeID)
        {
            // Compute Cost here based on parameters
        }

        public HashSet<uint> GetTracesAlongPath(List<uint> path, TraceDB traceDB)
        {

            // If no paths given, return empty set (No traces)
            if (path.Count == 0)
            {
                return new HashSet<uint>();
            }

            HashSet<uint> result = new HashSet<uint>();

            // Find traces on first path
            traceDB.EdgeTraceMapping.TryGetValue(path[0], out result);

            HashSet<uint> newValues = new HashSet<uint>();

            // Check for traces on remaining paths
            for (int i = 1; i < path.Count; i++)
            {
                // Traces found on path stored in newValues
                bool foundValues = traceDB.EdgeTraceMapping.TryGetValue(path[i], out newValues);

                // If no traces found on path, then no traces returned
                if (!foundValues)
                {
                    return new HashSet<uint>();
                }

                // Otherwise update result to store intersection of these traces
                result.IntersectWith(newValues);
            }

            // Remove those traces that do not follow the same order as path
            result.RemoveWhere(traceID => !RoadSegmentsInOrder(path, traceID, traceDB));
            
            // return result
            return result;

        }

        // TODO: Can we use KMP for this ?
        public Boolean RoadSegmentsInOrder(List<uint> edgeIDs, uint traceID, TraceDB traceDB)
        {
            // Get trace based on traceID
            Trace trace = traceDB[traceID];

            List<int> traceStartIndices = new List<int>();
            // Find indices of trace matching first edgeID
            for(int i = 0; i < trace.Count; i++)
            {
                if(trace[i] == edgeIDs[0])
                {
                    traceStartIndices.Add(i);
                }
            }

            foreach(int startIndex in traceStartIndices)
            {
                int edgeIndex = 1;
                // For each traceID start for that index
                int traceIndex = startIndex+1;

                // Loop to check if edgeId at that index matches trace for all indices
                while (edgeIndex < edgeIDs.Count && traceIndex < trace.Count)
                {
                    // Exit loop if not a match
                    if (edgeIDs[edgeIndex] != trace[traceIndex])
                    {
                        break;
                    }
                    edgeIndex++;
                    traceIndex++;
                }

                // if all edges matched return true
                if(edgeIndex == edgeIDs.Count)
                {
                    return true;
                }
            }

            // If we are here then no match found
            return false;
        }
    }
}
