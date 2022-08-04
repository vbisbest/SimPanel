using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;
using System.IO;
using SkiaSharp.Views;
using SkiaSharp.Views.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Xamarin.Forms.Shapes;
using Xamarin.Essentials;

namespace Panel.Views
{
    public partial class AboutPage : ContentPage
    {
        int x = 0;

        SimPanelStream simStream = null;
        private double width;
        private double height;

        public AboutPage()
        {
            InitializeComponent();

            var assembly = this.GetType().Assembly;

            imageHeadingInner.Source = ImageSource.FromResource("Panel.heading-inner.png");
            imageHeadingOuter.Source = ImageSource.FromResource("Panel.heading-outer.png");
            imageHeadingPlane.Source = ImageSource.FromResource("Panel.heading-plane.png");

            imageAttitudeInner.Source = ImageSource.FromResource("Panel.atitude-inner.png"); ;
            imageAttitudeOuter.Source = ImageSource.FromResource("Panel.atitude-outer.png"); ;

            imageAirspeedInner.Source = ImageSource.FromResource("Panel.airspeed-panel.png"); ;
            imageAirspeedNeedle.Source = ImageSource.FromResource("Panel.airspeed-needle.png"); ;

            //System.Timers.Timer t = new System.Timers.Timer();
            //t.Interval = 100;
            //t.Elapsed += T_Elapsed;
            //t.Start();
            //Test();

            simStream = new SimPanelStream(10000);
            simStream.OnSimData += SimStream_OnSimData;

            Task.Run(() =>
            {
                simStream.StartSocket();
            });


        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != this.width || height != this.height)
            {
                this.width = width;
                this.height = height;
                if (width > height)
                {                    
                    stackLayoutPanel.Orientation = StackOrientation.Horizontal;
                }
                else
                {
                    stackLayoutPanel.Orientation = StackOrientation.Vertical;
                }
            }
        }

        private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            x++;
            //currentAngle++;
            //imageHeadingInner.RotateTo(currentAngle, 50, null);
            //imageAirspeedNeedle.RotateTo(x, 10, null);

        }

        private void SimStream_OnSimData(SimData data)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                UpdatePanel(data);
            });
        }

        private void UpdatePanel(SimData data)
        {
            imageHeadingInner.RotateTo(data.Heading, 100, null);            
            imageAttitudeInner.RotateTo(data.AttitudeBank, 100, null);
            imageAttitudeInner.TranslationY = data.AttitudePitch;

            var airspeedRadians = (data.Airspeed / 280) * 360;            
            imageAirspeedNeedle.RotateTo(airspeedRadians, 100, null);
        }
    }

    public static class ExtensionMethods
    {
        public static double Map(this double value, double fromSource, double toSource, double fromTarget, double toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }

}