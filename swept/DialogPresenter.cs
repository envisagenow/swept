using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace swept
{
    public class DialogPresenter : IDialogPresenter
    {
        public DialogPresenter()
        {
        }

        public bool KeepHistoricalCompletionsForChange(Change historicalChange)
        {
            //TODO:  Actually bring up dialog box with sane message
            throw new NotImplementedException("We shouldn't call for a real dialog box yet.");
        }

    }
}
