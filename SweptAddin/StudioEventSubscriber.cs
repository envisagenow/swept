//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using EnvDTE80;
using EnvDTE;

namespace swept.Addin
{
    internal class StudioEventSubscriber : IDisposable
    {
        private DTE2 _studio;
        private AddIn _sweptAddin;
        private EventSwitchboard _switchboard;

        //  Class scoped to hold these references for the lifetime of the addin.
        private SolutionEvents _solutionEvents;
        private DocumentEvents _documentEvents;
        private StudioEventChannel _channel;
        private UserGUIAdapter _guiAdapter;

        public void Subscribe( EventSwitchboard switchboard, DTE2 studio, AddIn sweptAddin, UserGUIAdapter guiAdapter )
        {
            _studio = studio;
            _sweptAddin = sweptAddin;
            _switchboard = switchboard;
            _guiAdapter = guiAdapter;

            _solutionEvents = _studio.Events.SolutionEvents;
            _documentEvents = _studio.Events.get_DocumentEvents( null );

            _channel = new StudioEventChannel( _switchboard, _studio );

            _solutionEvents.Opened += _channel.Hear_SolutionOpened;
            _solutionEvents.AfterClosing += _channel.Hear_SolutionClosed;

            _documentEvents.DocumentSaved += _channel.Hear_DocumentSaved;
            _documentEvents.DocumentOpened += _channel.Hear_DocumentOpened;
            _documentEvents.DocumentClosing += _channel.Hear_DocumentClosing;

            switchboard.Event_TaskListChanged += _guiAdapter.Hear_TasksChangedEvent;
            _studio.Events.get_TaskListEvents( "" ).TaskNavigated += _guiAdapter.Hear_TaskNavigated;


            if (_studio.Solution != null)
            {
                _channel.Hear_SolutionOpened();
            }
        }

        public void Unsubscribe()
        {
            _documentEvents.DocumentSaved -= _channel.Hear_DocumentSaved;
            _documentEvents.DocumentOpened -= _channel.Hear_DocumentOpened;
            _documentEvents.DocumentClosing -= _channel.Hear_DocumentClosing;

            _solutionEvents.Opened -= _channel.Hear_SolutionOpened;
            _solutionEvents.AfterClosing -= _channel.Hear_SolutionClosed;

            _documentEvents = null;
            _solutionEvents = null;

            _switchboard = null;
            _studio = null;
            _channel = null;
        }

        void IDisposable.Dispose()
        {
            //_taskWindowControl.Dispose();
        }
    }

}
