using System;
using System.Collections.Generic;
namespace PathSelection
{
    public interface IRoutingState
    {
        #region property
        double Cost { get; set; }
        IRoutingState Parent { get; set; }
        List<uint> EdgeIDsForState { get; set; }
        #endregion

        #region method
        List<IRoutingState> GetNextStates();
        bool IsGoalState();
        #endregion
    }
}
