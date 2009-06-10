using System;
using System.Collections.Generic;
using System.Text;

namespace swept.Tests
{
    class MockDialogPresenter : IDialogPresenter
    {
        public bool KeepHistoricalResponse;
        #region IDialogPresenter Members

        public bool KeepHistoricalCompletionsForChange(Change historicalChange)
        {
            return KeepHistoricalResponse;
        }
        #endregion
    }
}
