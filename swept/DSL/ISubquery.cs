//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept.DSL
{
    public interface ISubquery
    {
        ClauseMatch Answer( SourceFile file );
    }
}