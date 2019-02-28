using System;
using System.Collections.Generic;

namespace EnergyCostEstimator
{
    struct TracePart
    {
        public uint EdgeId;
        public double Cost;
    }

    public struct PhysicalParas
    {
        public double MassFactor;
        public double AreaFactor;
    }

    // trace is an alias of trajectory
    class Trace : List<TracePart>
    {
        #region property
        public uint Id { get; set; }

        public string Name { get; set; }

        public PhysicalParas Paras { get; set; }
        #endregion

        public Trace()
        {
        }
    }
}
