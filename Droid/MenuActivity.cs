
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

namespace GeoTiles.Droid
{
	[Activity(Label = "GeoMatch", MainLauncher = true, Icon = "@mipmap/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MenuActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Menu);
			// Create your application here

			Button btnQuad = FindViewById<Button>(Resource.Id.btnQuad);
			btnQuad.Click += delegate
			{
				StartActivity(typeof(MainActivity));
			};
		}
	}
}
