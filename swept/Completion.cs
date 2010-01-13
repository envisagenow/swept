//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept
{
    public class Completion
    {
        public string ChangeID;

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
