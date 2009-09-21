using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Forms;

namespace swept.Addin
{
    public class DocTableListener : IVsRunningDocTableEvents
    {


        #region IVsRunningDocTableEvents Members

        public int OnAfterAttributeChange( uint docCookie, uint grfAttribs )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnAfterAttributeChange", docCookie ) );
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide( uint docCookie, IVsWindowFrame pFrame )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnAfterDocumentWindowHide", docCookie ) );
            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock( uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnAfterFirstDocumentLock", docCookie ) );
            return VSConstants.S_OK;
        }

        public int OnAfterSave( uint docCookie )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnAfterSave", docCookie ) );
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow( uint docCookie, int fFirstShow, IVsWindowFrame pFrame )
        {
            MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnBeforeDocumentWindowShow", docCookie ) );
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock( uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnBeforeLastDocumentUnlock", docCookie ) );
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsRunningDocTableEvents2 Members


        public int OnAfterAttributeChangeEx( uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnAfterAttributeChangeEx", docCookie ) );
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsRunningDocTableEvents3 Members


        public int OnBeforeSave( uint docCookie )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnBeforeSave", docCookie ) );
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsRunningDocTableEvents4 Members

        public int OnAfterLastDocumentUnlock( IVsHierarchy pHier, uint itemid, string pszMkDocument, int fClosedWithoutSaving )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "", docCookie ) );
            return VSConstants.S_OK;
        }

        public int OnAfterSaveAll()
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "", docCookie ) );
            return VSConstants.S_OK;
        }

        public int OnBeforeFirstDocumentLock( IVsHierarchy pHier, uint itemid, string pszMkDocument )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "", docCookie ) );
            return VSConstants.S_OK;
        }

        #endregion
    }


}
