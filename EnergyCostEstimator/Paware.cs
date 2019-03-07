﻿using System;
using System.Collections.Generic;

namespace EnergyCostEstimator
{
    public class Paware : List<uint>
    {
        #region property
        public HashSet<uint> PassingTraceIds { get; set; }
        public List<int> StartingTracePartIdxes { get; set; }
        #endregion

        public Paware()
        {
        }
    }
}
