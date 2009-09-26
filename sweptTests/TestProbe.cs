//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;

namespace swept.Tests
{
    public class TestProbe
    {
        public static bool IsCompletionSaved(ProjectLibrarian librarian, string fileName)
        {
            return IsCompletionSaved(librarian, fileName, "14");
        }

        public static bool IsCompletionSaved(ProjectLibrarian librarian, string fileName, string changeID)
        {
            SourceFile file = librarian.savedSourceCatalog.Files.Find(f => f.Name == fileName);
            if (file == null) return false;
            return file.Completions.Exists(c => c.ChangeID == changeID);
        }


        public static string SingleFileLibrary_text
        {
            get
            {
                return
@"<SweptProjectData>
<ChangeCatalog>
</ChangeCatalog>
<SourceFileCatalog>
    <SourceFile Name='some_file.cs'>
        <Completion ID='007' />
    </SourceFile>
</SourceFileCatalog>
</SweptProjectData>";
            }
        }


        public static string SingleChangeLibrary_text
        {
            get
            {
                return
@"<SweptProjectData>
<ChangeCatalog>
    <Change ID='30-Persist' Description='Update to use persister' Language='CSharp' />
</ChangeCatalog>
<SourceFileCatalog>
</SourceFileCatalog>
</SweptProjectData>";
            }
        }


        public static string SeveralFileCatalog_text
        {
            get
            {
                return
@"<SweptProjectData>
<SourceFileCatalog>
    <SourceFile Name='bar.cs'>
        <Completion ID='AB1' />
        <Completion ID='AB2' />
    </SourceFile>
    <SourceFile Name='foo.cs'>
        <Completion ID='anotherID' />
    </SourceFile>
</SourceFileCatalog>
</SweptProjectData>";
            }
        }

    }
}
