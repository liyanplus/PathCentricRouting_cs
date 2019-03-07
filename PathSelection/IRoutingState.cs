using System;
using System.Collections.Generic;
namespace PathSelection
{
    public interface IRoutingState
    {
        #region property
        double Cost { get; set; }
        #endregion

        #region method
        List<IRoutingState> GetNextStates();
        #endregion
    }
}
