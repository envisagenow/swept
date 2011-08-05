//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept.Tests
{
    public class TestProbe
    {
        public static string SingleChangeLibrary_text
        {
            get
            {
                return
@"<SweptProjectData>
<ChangeCatalog>
    <Change ID='30-Persist' Description='Update to use persister'> ^CSharp </Change>
</ChangeCatalog>
</SweptProjectData>";
            }
        }
    }
}
