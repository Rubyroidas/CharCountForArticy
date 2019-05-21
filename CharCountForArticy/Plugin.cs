using Articy.Api;
using Articy.Api.Plugins;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Threading;

// To ease working with Loca-Ids use a namespace alias
// using Texts = LIds.CharCountForArticy;

namespace CharCountForArticy
{
    /// <summary>
    /// public implementation part of plugin code, contains all overrides of the plugin class.
    /// </summary>
    public partial class Plugin : MacroPlugin
    {
        CountWindow window = null;
        ObjectProxy observableObject = null;
        Timer updateTimer = null;
        Action updateAction = null;

        public override string DisplayName
        {
            get { return "Text length tool"; }
        }

        public override string ContextName
        {
            get { return "Text length tool"; }
        }

        public override void SelectionChanged(List<ObjectProxy> aObjects, SelectionContext aContext)
        {
            if (aContext != SelectionContext.Flow || aObjects.Count < 1 || aObjects[0].ObjectType != ObjectType.DialogueFragment)
            {
                StopObserve();

                return;
            }

            StartObserve(aObjects[0]);
        }

        private void StartObserve(ObjectProxy objectProxy)
        {
            observableObject = objectProxy;
            // now it's ok
            if (window == null)
            {
                window = new CountWindow();
            }
            window.Show();
            updateAction = () => {
                try
                {
                    String messageText = observableObject.GetText().Length.ToString();
                    window.lblCount.Content = messageText;
                }
                catch (Exception e) { }
            };
            // Create a timer with a 200ms interval.
            updateTimer = new Timer(
                callback: TickTimer,
                state: null,
                dueTime: 200,
                period: 200
            );
        }

        private void TickTimer(Object stateInfo)
        {
            if (updateAction != null)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    updateAction();
                });
            }
        }

        private void StopObserve()
        {
            //if (observableObject == null)
            //{
            //    return;
            //}
            if (updateAction != null)
            {
                updateAction = null;
            }
            if (updateTimer != null)
            {
                // updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
                updateTimer.Dispose();
                updateTimer = null;
            }
            if (window != null)
            {
                window.Hide();
            }
            observableObject = null;
        }

        public override List<MacroCommandDescriptor> GetMenuEntries(List<ObjectProxy> aSelectedObjects, ContextMenuContext aContext)
        {
            var result = new List<MacroCommandDescriptor>();
            switch (aContext)
            {
                case ContextMenuContext.Global:
                    // entries for the "global" commands of the ribbon menu are requested
                    return result;

                default:
                    // normal context menu when working in the content area, navigator, search
                    return result;
            }
        }

        public override Brush GetIcon(string aIconName)
        {
            switch (aIconName)
            {
                // if you have specified the "IconFile" in the PluginManifest.xml you don't need this case
                // unless you want to have an icon that differs when the plugin is loaded from the non-loaded case
                // or you want to put all icons within the resources of your plugin assembly
                
				case "$self":
					// get the main icon for the plugin
					return Session.CreateBrushFromFile(Manifest.ManifestPath+"Resources\\Icon.png");
				
            }
            return null;
        }
    }
}
