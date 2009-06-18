//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System;

namespace swept
{
    public class Task : Change
    {
        public bool Completed;

        private Task() { }

        public static Task FromChange( Change change )
        {
            Task entry = new Task();
            entry.ID = change.ID;
            entry.Description = change.Description;
            entry.Language = change.Language;
            entry.Completed = false;
            return entry;
        }
    }
}
