//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using swept;

namespace swept.Addin
{
	/// <summary>The class pointed to by the .addin file, that Studio pokes when Studio/Addin lifecycle events occur.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
        #region Boilerplate
		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
            //listener = new StudioEventListener();
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if(commandName == "SweptAddin.Connect.SweptAddin")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
				if(commandName == "SweptAddin.Connect.SweptAddin")
				{
                    //some turn on/off switchery
                    // studio events come through here
					handled = true;
					return;
				}
			}
		}

#endregion Boilerplate

        public void OnConnection( object application, ext_ConnectMode connectMode, object addInInst, ref Array custom )
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;

            if( connectMode != ext_ConnectMode.ext_cm_UISetup )
            {
                loadSwept();
            }
		}


        public void OnStartupComplete( ref Array custom )
        {
            try
            {
                loadSwept();
            }
            catch( Exception e )
            {
                StudioEventListener.describeException( e );
            }
        }

        public void OnDisconnection( ext_DisconnectMode disconnectMode, ref Array custom )
        {
            try
            {
                unloadSwept();
            }
            catch( Exception e )
            {
                StudioEventListener.describeException( e );
            }
        }

		private DTE2 _applicationObject;
		private AddIn _addInInstance;
        private bool _sweptLoaded;

        private Starter _sweptStarter;
        private StudioEventListener _listener;

        private void loadSwept()
        {
            if( _sweptLoaded )
                return;

            _sweptStarter = new Starter();
            _listener = new StudioEventListener();

            _sweptStarter.Start();
            _listener.Connect( _applicationObject, _sweptStarter, _addInInstance );

            _sweptLoaded = true;
        }

        private void unloadSwept()
        {
            if( !_sweptLoaded )
                return;

            _listener.Disconnect( _sweptStarter );
            _listener = null;

            _sweptStarter.Stop();

            _applicationObject = null;

            _sweptLoaded = false;
        }
	}
}