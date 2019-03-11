using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace PathSelection
{
    public class Path : LinkedList<uint>
    {
        #region property
        public Vector<double> Cost { get; set; }
        #endregion

        public Path()
        {
        }
    }
}
