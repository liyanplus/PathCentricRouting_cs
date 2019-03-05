using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace EnergyCostEstimator
{
    public struct PhysicalParas
    {
        public double MassFactor;
        public double AreaFactor;
    }

    // trace is an alias of trajectory
    public class Trace : List<uint>
    {
        #region property
        public uint Id { get; set; }

        public string Name { get; set; }

        public PhysicalParas Paras { get; set; }

        public Vector<double> Costs { get; set; }
        #endregion

        public Trace()
        {
        }
    }
}
