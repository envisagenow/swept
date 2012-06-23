//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept.Tests
{
    public class TestProbe
    {
        public static string SingleRuleLibrary_text
        {
            get
            {
                return
@"<SweptProjectData>
<RuleCatalog>
    <Rule ID='30-Persist' Description='Update to use persister'> ^CSharp </Rule>
</RuleCatalog>
</SweptProjectData>";
            }
        }
    }
}
