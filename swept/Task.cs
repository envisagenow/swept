using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System;

namespace swept
{
    public class Task : Change
    {
        public bool Completed;

        private Task() { }

        public static Task FromChange( Change change )
        {
            Task entry = new Task();
            entry.ID = change.ID;
            entry.Description = change.Description;
            entry.Language = change.Language;
            entry.Completed = false;
            return entry;
        }
    }
}
