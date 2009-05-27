﻿using System;
using System.Collections.Generic;
using System.Text;

namespace swept
{
    public class ChangeWindow
    {
        public int ChangeCount {
            get
            {
                return Changes.changes.Count;
            }
        }

        public ChangeCatalog Changes { get; set; }

        public void AddChange(Change change)
        {
            Changes.Add(change);
        }
    }
}
