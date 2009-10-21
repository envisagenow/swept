//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
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
