//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;

namespace swept
{
    public class Task : Change
    {
        public bool Completed;

        public static Task FromChange( Change change )
        {
            return new Task { 
                ID = change.ID, 
                Description = change.Description, 
                Language = change.Language, 
                Completed = false 
            };
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
