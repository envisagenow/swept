using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace swept
{
    public interface ILibraryWriter
    {
        void Save( string fileName, string xmlText );
    }

}
