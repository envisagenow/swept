//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace swept
{
    public class DialogPresenter : IDialogPresenter
    {
        public bool KeepHistoricalCompletionsForChange(Change historicalChange)
        {
            //SELF:  Actually bring up dialog box with sane message
            throw new NotImplementedException("We shouldn't call for a real dialog box yet.");
        }
    }
}
