//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;

namespace swept.Tests
{
    class MockDialogPresenter : IDialogPresenter
    {
        public bool KeepHistoricalResponse;
        #region IDialogPresenter Members

        public bool KeepChangeHistory( Change historicalChange )
        {
            return KeepHistoricalResponse;
        }

        public bool KeepSourceFileHistory( SourceFile historicalFile )
        {
            return KeepHistoricalResponse;
        }
        #endregion
    }
}
