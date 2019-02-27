using System;
using System.Collections.Generic;

namespace SpatialNetwork
{
    public class Edge
    {
        #region property
        public uint Id { get; set; }
        public uint FromNodeId { get; set; }
        public uint ToNodeId { get; set; }
        public bool BiDirectional { get; set; }
        public Dictionary<string,string> Attrs { get; set; }

        #endregion

        public Edge()
        {
            Attrs = new Dictionary<string, string>();
        }
    }
}
