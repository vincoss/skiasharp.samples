using SkiaSharpSamples.Views;
using SkiaSharpSamples.Views.Gestures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkiaSharpSamples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeView : ContentPage
    {
        public HomeView()
        {
            InitializeComponent();

            List<PageInfo> pages = new List<PageInfo>();
            pages.Add(new PageInfo{ Type = typeof(TapGestureSampleView)});
            pages.Add(new PageInfo{ Type = typeof(TouchGestureMoveBitmapView)});
            pages.Add(new PageInfo{ Type = typeof(TouchGestureSampleView)});

            pages.Add(new PageInfo{ Type = typeof(BitmapRectangleSubsetView)});
            pages.Add(new PageInfo{ Type = typeof(BitmapScaleFitScreenView)});
            pages.Add(new PageInfo{ Type = typeof(BitmapScalingModeView)});
            pages.Add(new PageInfo{ Type = typeof(ContinuousUseView)});
            pages.Add(new PageInfo{ Type = typeof(DisplayingInPixelDimensionsView)});
            pages.Add(new PageInfo{ Type = typeof(ShowBitmapView)});
            pages.Add(new PageInfo{ Type = typeof(SKGLViewDrawLinesView)});
            pages.Add(new PageInfo{ Type = typeof(StretchingAndPreserveAspectRatioView)});
            pages.Add(new PageInfo{ Type = typeof(ZoomSampleView)});

            ListOfPages.ItemsSource = pages;
        }

        async void itemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var info = (PageInfo)e.SelectedItem;
                var page = (Page)Activator.CreateInstance(info.Type);
               
                await this.Navigation.PushAsync(page);
            }
            ListOfPages.SelectedItem = null;
        }

        public class PageInfo
        {
            public Type Type { get; set; }
            public string Name
            {

                get
                {
                    if (Type != null)
                    {
                        return Type.Name;
                    }
                    return base.GetType().ToString();
                }
            }

        }

    }
}