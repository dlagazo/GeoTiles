
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Threading;

namespace GeoTiles.Droid
{
    [Activity(Label = "GeoMatch", Icon = "@mipmap/i", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
			 Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
	public class Splash5Activity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.Inro);
            ImageView ivIntro = FindViewById<ImageView>(Resource.Id.ivIntro);
            ivIntro.SetImageDrawable(GetDrawable(Resource.Mipmap.mathplus_logo));
          
            ivIntro.Animate().AlphaBy(1.0f).SetDuration(2000);
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();

            

        }

        async void SimulateStartup()
        {
            
            await Task.Delay(2000); // Simulate a bit of startup work.
            StartActivity(new Intent(Application.Context, typeof(Splash6Activity)));
            Finish();

        }

    }

}
