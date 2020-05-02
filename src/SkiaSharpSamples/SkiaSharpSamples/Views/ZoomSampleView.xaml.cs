using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkiaSharpSamples.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ZoomSampleView : ContentPage
    {
        public ZoomSampleView()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            try
            {
                btn.IsEnabled = false;
                string resourceID = $"SkiaSharpSamples.resources.{btn.Text}";
                Assembly assembly = GetType().GetTypeInfo().Assembly;

                using (var stream = assembly.GetManifestResourceStream(resourceID))
                {
                    zoomView.ImageStream = stream;
                }
            }
            finally
            {
                btn.IsEnabled = true;
            }
        }
    }
}