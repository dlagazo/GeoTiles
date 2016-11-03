
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
		Intent activity;
		int mode, speed;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Menu);
			// Create your application here

			Button btnQuad = FindViewById<Button>(Resource.Id.btnQuad);
			btnQuad.Click += delegate
			{
				activity = new Intent(this, typeof(MainActivity));
				activity.PutExtra("type", "1"); //0-triangle, 1-quadrilaterals
												//StartActivity(activity);
				mode = 1;

				SetContentView(Resource.Layout.Speed);

				Button btnSlow = FindViewById<Button>(Resource.Id.btnSlow);
				btnSlow.Click += delegate
				{

					activity.PutExtra("speed", 6); //0-triangle, 1-quadrilaterals
					speed = 6;
					setSummary();


				};

				Button btnNormal = FindViewById<Button>(Resource.Id.btnNormal);
				btnNormal.Click += delegate
				{

					activity.PutExtra("speed", 4); //0-triangle, 1-quadrilaterals
					speed = 4;
					setSummary();


				};

				Button btnFast = FindViewById<Button>(Resource.Id.btnFast);
				btnFast.Click += delegate
				{

					activity.PutExtra("speed", 2); //0-triangle, 1-quadrilaterals

					speed = 2;
					setSummary();

				};
			};

			Button btnTri = FindViewById<Button>(Resource.Id.btnTri);
			btnTri.Click += delegate
			{
				activity = new Intent(this, typeof(MainActivity));
				activity.PutExtra("type", "0"); //0-triangle, 1-quadrilaterals
				mode = 0;
				SetContentView(Resource.Layout.Speed);

				Button btnSlow = FindViewById<Button>(Resource.Id.btnSlow);
				btnSlow.Click += delegate
				{

					activity.PutExtra("speed", 6); //0-triangle, 1-quadrilaterals
					speed = 6;
					setSummary();

				
				};

				Button btnNormal = FindViewById<Button>(Resource.Id.btnNormal);
				btnNormal.Click += delegate
				{

					activity.PutExtra("speed", 4); //0-triangle, 1-quadrilaterals
					speed = 4;
					setSummary();

				
				};

				Button btnFast = FindViewById<Button>(Resource.Id.btnFast);
				btnFast.Click += delegate
				{

					activity.PutExtra("speed", 2); //0-triangle, 1-quadrilaterals

					speed = 2;
					setSummary();
				
				};
				//StartActivity(activity);
			};


		}





		public void setSummary()
		{
			SetContentView(Resource.Layout.Summary);
			Button btnMode = FindViewById<Button>(Resource.Id.btnMode);

			Button btnSpeed = FindViewById<Button>(Resource.Id.btnSpeed);

			if (mode == 0)
				btnMode.Text = "Triangle";
			else if (mode == 1)
				btnMode.Text = "Quadrilateral";

			if(speed == 6)
				btnSpeed.Text = "Slow";
			else if (speed == 4)
				btnSpeed.Text = "Normal";
			else if (speed == 2)
				btnSpeed.Text = "Fast";

			Button btnStart = FindViewById<Button>(Resource.Id.btnStart);
			btnStart.Click += delegate
			{



				StartActivity(activity);
			};

			Button btnBack = FindViewById<Button>(Resource.Id.btnBack);
			btnBack.Click += delegate
			{


				activity = new Intent(this, typeof(MenuActivity));
				StartActivity(activity);
				Finish();
			};
		}
	}
}
