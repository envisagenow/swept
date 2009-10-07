//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;

namespace swept
{
    public interface IUserAdapter
    {
        bool KeepChangeHistory( Change historicalChange );
        bool KeepSourceFileHistory( SourceFile historicalFile );
        void BadXmlInExpectedLibrary( string libraryPath );
        void DebugMessage( string message );
    }
}
