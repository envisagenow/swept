//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace swept
{
    public class LibraryPersister : ILibraryPersister
    {
        private string _emptyCatalogString = 
@"<SweptProjectData>
<ChangeCatalog>
</ChangeCatalog>
<SourceFileCatalog>
</SourceFileCatalog>
</SweptProjectData>";

        public void Save(string fileName, string xmlText)
        {
            throw new NotImplementedException("I can't save to disk yet");
        }

        public string LoadLibrary( string libraryPath )
        {
            throw new NotImplementedException( "I don't load from disk yet" );

            try
            {
                // TODO: load file
                //libraryXmlText = persister.LoadLibrary( LibraryPath );
            }
            catch
            {
                libraryXmlText =
emptyCatalogString;
            }




            /*
             * Check if file exists
             * if so, 
             *      read and return text
             * If not, 
             *      return empty catalog text
             * 
             * (for the future, perhaps:)
             * 
             * if not,
             *      Dialog "I didn't find one.  Is this new, or do you want to choose a different file location?"
             *      then either new, or hunt in the other location
             *
             */
        }

    }
}
