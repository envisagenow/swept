using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace swept
{
    public interface IDialogPresenter
    {
        bool KeepHistoricalCompletionsForChange(Change historicalChange);
    }
}
