using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System;
using Android.Graphics.Drawables;

namespace GeoTiles.Droid
{
	[Activity(Label = "GeoMatch", MainLauncher = false, Icon = "@mipmap/icon",  ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		//int count = 1;

		List<int> order = new List<int>();
		List<string> quadrilaterals = new List<string>();
		List<string> triangles = new List<string>();
		List<Drawable> imagesQuadrilaterals = new List<Drawable>();
		List<string> shapes = new List<string>();

		Dictionary<int, string> cheat = new Dictionary<int, string>();
		Dictionary<int, EventHandler> handlers = new Dictionary<int, EventHandler>();

		System.Timers.Timer timer;
		int ptr = 0;
		string[] answers;
		bool result = false;
		int correct = 0, incorrect = 0;
		int prevQuestion = -1, prevShape = -1;
		int widthInDp, heightInDp;


		protected override void OnCreate(Bundle savedInstanceState)
		{
			
			base.OnCreate(savedInstanceState);
			RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			generateItems();
			GridLayout cells = FindViewById<GridLayout>(Resource.Id.cells);


			var metrics = Resources.DisplayMetrics;
			widthInDp = metrics.WidthPixels/4;
			//heightInDp = ConvertPixelsToDp(metrics.HeightPixels);

			EditText speed = FindViewById<EditText>(Resource.Id.numSpeed);
			//speed.Value = 6;
			//speed.MinValue = 6;


			for (int i = 0; i < cells.ChildCount; i++)
			{
				ImageView cell = (ImageView) cells.GetChildAt(i);
				cell.LayoutParameters.Width = widthInDp;
				cell.LayoutParameters.Height = widthInDp;

			}

			Button btnStart = FindViewById<Button>(Resource.Id.btnStart);
			btnStart.Click += delegate {
				generateQuestion();

				//generateRandom();
				ptr = 0;
				correct = 0;
				incorrect = 0;
				int interval = Int32.Parse(speed.Text) * 1000;
				Toast.MakeText(this, "Ready, get set, go...speed=" + interval.ToString(), ToastLength.Short).Show();
				timer = new System.Timers.Timer();
				timer.Interval = interval;
				timer.Elapsed += OnTimedEvent;
				timer.Start();
				timer.Enabled = true;


			};



			Button btnStop = FindViewById<Button>(Resource.Id.btnStop);
			btnStop.Click += delegate {
				try
				{
					timer.Stop();
					timer.Enabled = false;
					timer = null;

					Toast.MakeText(this, "Game stopped", ToastLength.Short).Show();
					for (int i = 0; i < cells.ChildCount; i++)
					{
						ImageView cell = (ImageView)cells.GetChildAt(i);
						cell.SetImageDrawable(null);
						try
						{
							cell.Click -= handlers[i];
						}
						catch (Exception e)
						{
						}

					}
					handlers.Clear();
					cheat.Clear();
					order.Clear();
					//clearCells();
					//this.Recreate();
				}
				catch (Exception e)
				{
				}

			};

			// Get our button from the layout resource,
			// and attach an event to it
			//Button button = FindViewById<Button>(Resource.Id.myButton);

			//button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

		}

		void generateQuestion()
		{
			Random random = new Random(System.DateTime.Now.Millisecond);
			int rand = 0;
			string question = "";
			if (order.Count < 8)
			{
				do
				{
					rand = random.Next() % 6;


				}
				while (rand == prevQuestion);
				prevQuestion = rand;
				question = quadrilaterals[rand].Split('.')[0];
				answers = quadrilaterals[rand].Split('.')[1].Split(',');

			}
			else {
				if (cheat.Count > 0)
				{
					foreach (string q in quadrilaterals)
					{
						if (q.Split('.')[1].Contains(cheat[order[1]]))
						{
							question = q.Split('.')[0];
							answers = q.Split('.')[1].Split(',');
							break;
						}
					}

				}
				else
				{
					do
					{
						rand = random.Next() % 6;


					}
					while (rand == prevQuestion);
					prevQuestion = rand;
					question = quadrilaterals[rand].Split('.')[0];
					answers = quadrilaterals[rand].Split('.')[1].Split(',');

				}


			}


			TextView clue = FindViewById<TextView>(Resource.Id.txtClue);
			clue.Text = question;
			Toast.MakeText(ApplicationContext, question, ToastLength.Short).Show();
			Console.WriteLine(question);
			Console.WriteLine(answers.ToString());


		}

		private void clearCells()
		{
			GridLayout cells = FindViewById<GridLayout>(Resource.Id.cells);
			for (int i = 0; i < cells.ChildCount; i++)
			{
				ImageView cell = (ImageView)cells.GetChildAt(i);

				cell.SetImageDrawable(null);
				cell = new ImageView(this.ApplicationContext);
				//cell = null;
			}


		}

		private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
		{
			
			RunOnUiThread(() => {
				if (order.Count < 16)
				{

					//Toast.MakeText(this, "Timer elapsed", ToastLength.Short).Show();
					GridLayout cells = FindViewById<GridLayout>(Resource.Id.cells);
					int cellValue = generateRandom();
					ImageView cell = (ImageView)cells.GetChildAt(cellValue);
					try
					{
						cell.Click -= handlers[cellValue];
						handlers.Remove(cellValue);
					}
					catch (Exception ex)
					{
					}
					//cell = new ImageView();
					Random rand = new Random(System.DateTime.Now.Millisecond);
					int random;
					do
					{
						random = rand.Next() % imagesQuadrilaterals.Count;

					}
					while (random == prevShape);
					prevShape = random;
					//string[] keys = images.Keys.ToString().Split(',');
					cell.SetImageDrawable(imagesQuadrilaterals[random]);
					cheat.Add(cellValue, shapes[random]);
					cell.LayoutParameters.Width = widthInDp;
					cell.LayoutParameters.Height = widthInDp;
					EventHandler handler = delegate {
						bool isFalse = true;
						bool isActive = true;
						foreach (string answer in answers)
						{
							if (answer.Contains(shapes[random]))
							{
								//Toast.MakeText(this.ApplicationContext, "correct", ToastLength.Short).Show();
								isFalse = false;
								result = true;
								correct++;
								generateQuestion();
								cell.SetImageDrawable(null);
								ptr--;
								order.Remove(cellValue);
								cheat.Remove(cellValue);
								//order.Add(order[ptr]);

								break;
							}
						}
						if (isFalse)
						{
							result = false;
							incorrect++;

						}
					};

					cell.Click += handler;
					handlers.Add(cellValue, handler);

				}

				else if(order.Count >= 16){
					Toast.MakeText(this, "Game over", ToastLength.Short).Show();
					timer.Stop();

				}

				TextView txtScore = FindViewById<TextView>(Resource.Id.txtScore);
				txtScore.Text = correct.ToString() + "/" + (correct+incorrect).ToString();
				//order.Remove(0);
			});

			foreach (string q in cheat.Values)
			{
				//Console.WriteLine(q);
			}
		}

		int generateRandom()
		{
			Random random = new Random(System.DateTime.Now.Millisecond);
			//int count = 0;
			//order.Clear();

			do
			{
				int rand = random.Next()%16;
				if (!order.Contains(rand))
				{
					order.Add(rand);
					return rand;
				}

			}
			while (true);

		}

		private int ConvertPixelsToDp(float pixelValue)
		{
			var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
			return dp;
		}

		void generateItems()
		{
			

			quadrilaterals.Add("Two pairs of parallel sides. rectangle, square, rhombus, parallelogram");
			quadrilaterals.Add("Four congruent sides. square, rhombus");
			quadrilaterals.Add("Four right angles. rectangle, square");
			quadrilaterals.Add("Four congruent sides and four right angles. square");
			quadrilaterals.Add("Exactly one pair of parallel sides. trapezoid");
			quadrilaterals.Add("Two pairs of equal-length sides that are adjacent to each other. square, rhombus");

			/*items.Add("All sides are different. trapezoid");
			items.Add("Two sides are equal. square, rectangle");
			items.Add("All three sides are equal", "Square 1, Square 2");
			items.Add("One angle is obtuse", "Square 1, Square 2");
			items.Add("All angles are acute", "Square 1, Square 2");
			items.Add("One angle is right", "Square 1, Square 2");
			items.Add("All angles are equal", "Square 1, Square 2");*/

			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.square1));
			shapes.Add("square");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rectangle1));
			shapes.Add("rectangle");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rhombus1));
			shapes.Add("rhombus");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.trapezoid1));
			shapes.Add("trapezoid");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.parallelogram1));
			shapes.Add("parallelogram");
			//imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.kite1));
			//shapes.Add("kite");

			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.square2));
			shapes.Add("square");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rectangle2));
			shapes.Add("rectangle");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rhombus2));
			shapes.Add("rhombus");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.trapezoid2));
			shapes.Add("trapezoid");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.parallelogram2));
			shapes.Add("parallelogram");
			//imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.kite2));
			//shapes.Add("kite");

			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.square3));
			shapes.Add("square");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rectangle3));
			shapes.Add("rectangle");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rhombus3));
			shapes.Add("rhombus");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.trapezoid3));
			shapes.Add("trapezoid");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.parallelogram3));
			shapes.Add("parallelogram");
			//imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.kite3));
			//shapes.Add("kite");

			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.square4));
			shapes.Add("square");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rectangle4));
			shapes.Add("rectangle");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rhombus4));
			shapes.Add("rhombus");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.trapezoid4));
			shapes.Add("trapezoid");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.parallelogram4));
			shapes.Add("parallelogram");
			//imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.kite4));
			//shapes.Add("kite");

			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.square5));
			shapes.Add("square");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rectangle5));
			shapes.Add("rectangle");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rhombus5));
			shapes.Add("rhombus");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.trapezoid5));
			shapes.Add("trapezoid");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.parallelogram5));
			shapes.Add("parallelogram");
			//imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.kite5));
			//shapes.Add("kite");

			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.square6));
			shapes.Add("square");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rectangle6));
			shapes.Add("rectangle");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rhombus6));
			shapes.Add("rhombus");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.trapezoid6));
			shapes.Add("trapezoid");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.parallelogram6));
			shapes.Add("parallelogram");
			//imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.kite6));
			//shapes.Add("kite");

			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.square7));
			shapes.Add("square");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rectangle7));
			shapes.Add("rectangle");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rhombus7));
			shapes.Add("rhombus");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.trapezoid7));
			shapes.Add("trapezoid");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.parallelogram7));
			shapes.Add("parallelogram");
			//imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.kite7));
			//shapes.Add("kite");

			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.square8));
			shapes.Add("square");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rectangle8));
			shapes.Add("rectangle");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.rhombus8));
			shapes.Add("rhombus");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.trapezoid8));
			shapes.Add("trapezoid");
			imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.parallelogram8));
			shapes.Add("parallelogram");
			//imagesQuadrilaterals.Add(GetDrawable(Resource.Mipmap.kite8));
			//shapes.Add("kite");
		}
	}
}


