//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;

namespace swept
{
    public class Completion
    {
        public string ChangeID;

        public Completion() {}
        public Completion( string changeID )
        {
            ChangeID = changeID;
        }

        public Completion Clone()
        {
            return new Completion( ChangeID );
        }
    }
}
