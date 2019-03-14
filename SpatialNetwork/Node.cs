using System;
using System.Collections.Generic;

namespace SpatialNetwork
{
    public class Node
    {
        #region property
        public uint Id { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public Dictionary<string, string> Attrs { get; set; }
        public List<uint> ConnectedEdgesId { get; set; }

        #endregion
        public Node()
        {
            ConnectedEdgesId = new List<uint>();
            Attrs = new Dictionary<string, string>();
        }

        public List<uint> GetOutgoingEdges()
        {
            // TODO: Fill this out
            return null;
        }
    }
}
