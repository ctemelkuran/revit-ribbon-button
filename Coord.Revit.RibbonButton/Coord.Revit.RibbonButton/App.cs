#region Namespaces
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace Coord.Revit.RibbonButton
{
    class App : IExternalApplication
    {
        const string RIBBON_TAB = "Cgr Tools";
        const string RIBBON_PANEL = "Coordinates";

        // get the absolute path of this assembly
        static string ExecutingAssemblyPath = Assembly
          .GetExecutingAssembly().Location;

        public Result OnStartup(UIControlledApplication a)
        {

            AddMenu(a);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }


        private void AddMenu(UIControlledApplication app)
        {
            // get the ribbon tab
            try
            {
                app.CreateRibbonTab(RIBBON_TAB);
            }
            catch (Exception) { } // tab already exists

            // get or create panel
            RibbonPanel panel = null;
            List<RibbonPanel> panels = app.GetRibbonPanels(RIBBON_TAB);
            foreach (RibbonPanel pnl in panels)
            {
                if (pnl.Name == RIBBON_PANEL)
                {
                    panel = pnl;
                    break;
                }
            }

            // couldn find the panel
            if (panel == null)
            {
                panel = app.CreateRibbonPanel(RIBBON_TAB, RIBBON_PANEL);
            }

            PulldownButtonData data = new PulldownButtonData("Options", RIBBON_PANEL);
            RibbonItem item = panel.AddItem(data);
            PulldownButton optionsBtn = item as PulldownButton;

            // get the image for the button
            Image img = Properties.Resources.location32x32;
            ImageSource imgSrc = GetImageSources(img);

            // create button data

            PushButtonData btnData = new PushButtonData(
                "CursorCoord", "Cursor" + "\n" + "Coordinates", ExecutingAssemblyPath,
                "Coord.Revit.RibbonButton.Command")
            {
                ToolTip = "Click to save the cursor coordinates",
                LongDescription = "This button makes you save the coordinates of the cursor while you hover over the screen.",
                Image = imgSrc,
                LargeImage = imgSrc
            };
            // add the button to the ribbon
            PushButton button = panel.AddItem(btnData) as PushButton;
            button.Enabled = true;
        }
        private BitmapSource GetImageSources(Image img)
        {
            BitmapImage bmp = new BitmapImage();

            using(MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = null;
                bmp.StreamSource = ms;

                bmp.EndInit();
            }
            return bmp;
        }
    }
}
