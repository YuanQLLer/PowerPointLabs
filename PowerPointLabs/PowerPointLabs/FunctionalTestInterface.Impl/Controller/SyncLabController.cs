﻿using System;
using System.Windows;
using System.Windows.Controls.Primitives;

using Microsoft.Office.Interop.PowerPoint;
using PowerPointLabs.ActionFramework.Common.Extension;
using PowerPointLabs.SyncLab.View;
using TestInterface;

namespace PowerPointLabs.FunctionalTestInterface.Impl.Controller
{
    [Serializable]
    class SyncLabController : MarshalByRefObject, ISyncLabController
    {
        private static ISyncLabController _instance = new SyncLabController();

        public static ISyncLabController Instance { get { return _instance; } }

        private SyncPane _pane;

        private SyncLabController() { }

        public void OpenPane()
        {
            UIThreadExecutor.Execute(() =>
            {
                FunctionalTestExtensions.GetRibbonUi().OnAction(
                    new RibbonControl("SyncLabButton"));
                _pane = FunctionalTestExtensions.GetTaskPane(
                    typeof(SyncPane)).Control as SyncPane;
            });
        }

        public void Copy()
        {
            if (_pane != null)
            {
                UIThreadExecutor.Execute(() =>
                {
                    _pane.SyncPaneWPF1.copyButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                });
            }
        }

        public void Sync(int index)
        {
            if (_pane != null)
            {
                UIThreadExecutor.Execute(() =>
                {
                    ((SyncFormatPaneItem)_pane.SyncPaneWPF1.formatListBox.Items[index])
                            .pasteButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                });
            }
        }

    }
}
