using System;
using System.Windows.Media;

using Microsoft.Windows.Controls.Ribbon;
using Microsoft.Practices.Prism.Regions;
using Spring.Context.Support;

using SpectralSpring.Utils;

namespace SpectralSpring.ModuleSupport.Ui
{
    // FIXME: explain this
    public class SwitchViewMenuItem : RibbonApplicationMenuItem
    {
        private ModuleView view;

        public SwitchViewMenuItem(string header, string keyTip, ImageSource iconImageSource, ModuleView viewToActivate)
        {
            ImageSource = iconImageSource;
            Header = header;
            KeyTip = keyTip;

            this.view = viewToActivate;
        }

        public SwitchViewMenuItem(string id, ModuleView viewToActivate)
        {
            ImageSource = ResourceUtils.GetImageSourceById(id + "Icon");
            Header = ResourceUtils.GetMessage(id + "MenuHeader");

            this.view = viewToActivate;
        }

        protected override void OnClick()
        {
            IRegionManager regionManager = (IRegionManager) ContextRegistry.GetContext().GetObject("IRegionManager");
            ModuleView loadedView = (ModuleView)regionManager.Regions["MainRegion"].GetView(view.ViewName);

            if (loadedView != null)
            {
                foreach (object viewObject in regionManager.Regions["MainRegion"].Views)
                {
                    regionManager.Regions["MainRegion"].Deactivate(viewObject);
                }

                regionManager.Regions["MainRegion"].Activate(loadedView);
                loadedView.Refresh();
            }
            else
            {
                regionManager.Regions["MainRegion"].Add(view, view.ViewName);
                regionManager.Regions["MainRegion"].Activate(view);
                view.Refresh();
            }
        }

    }
}
