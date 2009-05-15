//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using System.Collections.Generic;
using System.Text;
using System;

namespace swept
{
    public class FileChangesView
    {
        public SourceFile File;
        public ChangeCatalog Catalog;
        public List<Change> Changes
        {
            get
            {
                List<Change> changes = new List<Change>();

                foreach( Change change in Catalog )
                {
                    if( change.Language == File.Language )
                        Changes.Add( new Change( change.ID, change.Description, change.Language ) );
                }

                return changes;
            }
        }

    }
}
