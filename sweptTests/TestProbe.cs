using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            SourceFile file = librarian.savedSourceImage.Files.Find(f => f.Name == fileName);
            if (file == null) return false;
            return file.Completions.Exists(c => c.ChangeID == changeID);
        }

        public static string Testing_SourceFileCatalog_text
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
