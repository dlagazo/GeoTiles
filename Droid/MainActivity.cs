using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System;
using Android.Graphics.Drawables;

namespace GeoTiles.Droid
{
	[Activity(Label = "GeoMatch", MainLauncher = false, Icon = "@mipmap/i",  ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
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
		Random rand = new Random(System.DateTime.Now.Millisecond);
		Android.Media.MediaPlayer check;
		Android.Media.MediaPlayer wrong;
		Android.Media.MediaPlayer over;
		Android.Media.MediaPlayer clap;



		protected override void OnDestroy()
		{
			base.OnDestroy();
			timer.Stop();
			timer.Enabled = false;
			timer = null;
			check.Release();
			wrong.Release();

		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			
			base.OnCreate(savedInstanceState);
			RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			generateItems();
			GridLayout cells = FindViewById<GridLayout>(Resource.Id.cells);

			int speedValue = Intent.GetIntExtra("speed", -1);


			var metrics = Resources.DisplayMetrics;
			widthInDp = metrics.WidthPixels/4;
			//heightInDp = ConvertPixelsToDp(metrics.HeightPixels);


			check = Android.Media.MediaPlayer.Create(this, Resource.Raw.c);
			wrong = Android.Media.MediaPlayer.Create(this, Resource.Raw.x);
			over = Android.Media.MediaPlayer.Create(this, Resource.Raw.over);
			clap = Android.Media.MediaPlayer.Create(this, Resource.Raw.clap);

			for (int i = 0; i < cells.ChildCount; i++)
			{
				ImageView cell = (ImageView) cells.GetChildAt(i);
				cell.LayoutParameters.Width = widthInDp;
				cell.LayoutParameters.Height = widthInDp;
				cell.SetImageResource(Resource.Mipmap.empty);
				cell.Enabled = false;
			}
			generateQuestion();
			timer = new System.Timers.Timer();
			timer.Interval = speedValue*1000;
			timer.Elapsed += OnTimedEvent;
			timer.Start();
			timer.Enabled = true;





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
					rand = random.Next() % quadrilaterals.Count;


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
						rand = random.Next() % quadrilaterals.Count;


					}
					while (rand == prevQuestion);
					prevQuestion = rand;
					question = quadrilaterals[rand].Split('.')[0];
					answers = quadrilaterals[rand].Split('.')[1].Split(',');

				}


			}


			TextView clue = FindViewById<TextView>(Resource.Id.txtClue);
			clue.Text = question;
			clue.TextSize = 30;
			//Toast.MakeText(ApplicationContext, question, ToastLength.Short).Show();
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

		private void gameOver()
		{
			string type = Intent.GetStringExtra("type") ?? "-1";
			Android.Content.ISharedPreferences prefs = GetSharedPreferences("scores", Android.Content.FileCreationMode.Private);
			int speedValue = Intent.GetIntExtra("speed", -1);
			SetContentView(Resource.Layout.Scores);

			if (type.Equals("1"))
			{
				Button btnShapeType = FindViewById<Button>(Resource.Id.btnShapeType);
				btnShapeType.Text = "Quadrilaterals";
				String[] scores = prefs.GetString("QScores", "0,0,0").Split(',');
				String[] qHistoryScores = prefs.GetString("qHis", "0, 0, 0").Split(',');

				TextView tvScore = FindViewById<TextView>(Resource.Id.txtScore);
				tvScore.Text = " Your score is " + correct;
				Button btnBack = FindViewById<Button>(Resource.Id.btnScoreBack);
				btnBack.Click += delegate
				{
					Finish();
				};
				if (speedValue == 6)
				{
					int highScore = Int32.Parse(scores[0]);
					qHistoryScores[0] = (int.Parse(qHistoryScores[0]) + (correct + incorrect)).ToString();

					if (correct > highScore || true)
					{
						scores[0] = correct.ToString();

						Button btnQSlow = FindViewById<Button>(Resource.Id.btnQSlow);
						btnQSlow.Text = "    Slow:" + Int32.Parse(scores[0]) + "%" + " | " + qHistoryScores[0];
						btnQSlow.SetBackgroundColor(Android.Graphics.Color.DarkGreen);

						Button btnQNormal = FindViewById<Button>(Resource.Id.btnQNormal);
						btnQNormal.Text = "    Normal:" + Int32.Parse(scores[1]) + "%" + " | " + qHistoryScores[1];

						Button btnQFast = FindViewById<Button>(Resource.Id.btnQFast);
						btnQFast.Text = "    Fast:" + Int32.Parse(scores[2]) + "%" + " | " + qHistoryScores[2];
						clap.Start();
					}
					else
					{
						Button btnQSlow = FindViewById<Button>(Resource.Id.btnQSlow);
						btnQSlow.Text = "    Slow:" + Int32.Parse(scores[0]) + "%" + " | " + qHistoryScores[0];


						Button btnQNormal = FindViewById<Button>(Resource.Id.btnQNormal);
						btnQNormal.Text = "    Normal:" + Int32.Parse(scores[1]) + "%" + " | " + qHistoryScores[1];

						Button btnQFast = FindViewById<Button>(Resource.Id.btnQFast);
						btnQFast.Text = "    Fast:" + Int32.Parse(scores[2]) + "%" + " | " + qHistoryScores[2];
					}


				}
				else if (speedValue == 4)
				{
					int highScore = Int32.Parse(scores[1]);
					qHistoryScores[1] = (int.Parse(qHistoryScores[1]) + (correct + incorrect)).ToString();

					if (correct > highScore || true)
					{
						scores[1] = correct.ToString();


						Button btnQSlow = FindViewById<Button>(Resource.Id.btnQSlow);
						btnQSlow.Text = "    Slow:" + Int32.Parse(scores[0]);


						Button btnQNormal = FindViewById<Button>(Resource.Id.btnQNormal);
						btnQNormal.Text = "    Normal:" + Int32.Parse(scores[1]);
						btnQNormal.SetBackgroundColor(Android.Graphics.Color.DarkGreen);

						Button btnQFast = FindViewById<Button>(Resource.Id.btnQFast);
						btnQFast.Text = "    Fast:" + Int32.Parse(scores[2]);
						clap.Start();
					}
					else
					{
						Button btnQSlow = FindViewById<Button>(Resource.Id.btnQSlow);
						btnQSlow.Text = "    Slow:" + Int32.Parse(scores[0]);


						Button btnQNormal = FindViewById<Button>(Resource.Id.btnQNormal);
						btnQNormal.Text = "    Normal:" + Int32.Parse(scores[1]);

						Button btnQFast = FindViewById<Button>(Resource.Id.btnQFast);
						btnQFast.Text = "    Fast:" + Int32.Parse(scores[2]);
					}

				}
				else if (speedValue == 2)
				{
					int highScore = Int32.Parse(scores[2]);
					qHistoryScores[2] = (int.Parse(qHistoryScores[2]) + (correct + incorrect)).ToString();

					if (correct > highScore || true)
					{
						scores[2] = correct.ToString();

						Button btnQSlow = FindViewById<Button>(Resource.Id.btnQSlow);
						btnQSlow.Text = "    Slow:" + Int32.Parse(scores[0]);


						Button btnQNormal = FindViewById<Button>(Resource.Id.btnQNormal);
						btnQNormal.Text = "    Normal:" + Int32.Parse(scores[1]);

						Button btnQFast = FindViewById<Button>(Resource.Id.btnQFast);
						btnQFast.Text = "    Fast:" + Int32.Parse(scores[2]);
						btnQFast.SetBackgroundColor(Android.Graphics.Color.DarkGreen);

						clap.Start();
					}
					else
					{
						Button btnQSlow = FindViewById<Button>(Resource.Id.btnQSlow);
						btnQSlow.Text = "    Slow:" + Int32.Parse(scores[0]);


						Button btnQNormal = FindViewById<Button>(Resource.Id.btnQNormal);
						btnQNormal.Text = "    Normal:" + Int32.Parse(scores[1]);

						Button btnQFast = FindViewById<Button>(Resource.Id.btnQFast);
						btnQFast.Text = "    Fast:" + Int32.Parse(scores[2]);
					}

				}


				prefs.Edit().PutString("QScores", scores[0] + "," + scores[1] + "," + scores[2]).Commit();
				prefs.Edit().PutString("qHis", qHistoryScores[0] + "," + qHistoryScores[1] + "," + qHistoryScores[2]).Commit();
			}
			else if (type.Equals("0"))
			{
				Button btnShapeType = FindViewById<Button>(Resource.Id.btnShapeType);
				btnShapeType.Text = "Triangles";
				String[] scores = prefs.GetString("TScores", "0,0,0").Split(',');
				String[] tHistoryScores = prefs.GetString("tHis", "0, 0, 0").Split(',');

				TextView tvScore = FindViewById<TextView>(Resource.Id.txtScore);
				tvScore.Text = " Your score is " + correct;
				Button btnBack = FindViewById<Button>(Resource.Id.btnScoreBack);
				btnBack.Click += delegate
				{
					Finish();
				};

				if (speedValue == 6)
				{
					double highScore = Double.Parse(scores[2]);
					tHistoryScores[0] = (int.Parse(tHistoryScores[0]) + (correct + incorrect)).ToString();

					if (correct > highScore)
					{
						scores[0] = correct.ToString();

						Button btnTSlow = FindViewById<Button>(Resource.Id.btnQSlow);
						btnTSlow.Text = "    Slow:" + Int32.Parse(scores[0]);
						btnTSlow.SetBackgroundColor(Android.Graphics.Color.DarkGreen);


						Button btnTNormal = FindViewById<Button>(Resource.Id.btnQNormal);
						btnTNormal.Text = "    Normal:" + Int32.Parse(scores[1]);

						Button btnTFast = FindViewById<Button>(Resource.Id.btnQFast);
						btnTFast.Text = "    Fast:" + Int32.Parse(scores[2]);

						clap.Start();
					}
					else
					{
						Button btnTSlow = FindViewById<Button>(Resource.Id.btnQSlow);
						btnTSlow.Text = "    Slow:" + Int32.Parse(scores[0]);


						Button btnTNormal = FindViewById<Button>(Resource.Id.btnQNormal);
						btnTNormal.Text = "    Normal:" + Int32.Parse(scores[1]);

						Button btnTFast = FindViewById<Button>(Resource.Id.btnQFast);
						btnTFast.Text = "    Fast:" + Int32.Parse(scores[2]);
					}
				}
				else if (speedValue == 4)
				{
					double highScore = Double.Parse(scores[2]);
					tHistoryScores[1] = (int.Parse(tHistoryScores[1]) + (correct + incorrect)).ToString();

					if (correct > highScore)
					{
						scores[1] = correct.ToString();

						Button btnTSlow = FindViewById<Button>(Resource.Id.btnQSlow);
						btnTSlow.Text = "    Slow:" + Int32.Parse(scores[0]);


						Button btnTNormal = FindViewById<Button>(Resource.Id.btnQNormal);
						btnTNormal.Text = "    Normal:" + Int32.Parse(scores[1]);
						btnTNormal.SetBackgroundColor(Android.Graphics.Color.DarkGreen);


						Button btnTFast = FindViewById<Button>(Resource.Id.btnQFast);
						btnTFast.Text = "    Fast:" + Int32.Parse(scores[2]);

						clap.Start();
					}
					else
					{
						Button btnTSlow = FindViewById<Button>(Resource.Id.btnQSlow);
						btnTSlow.Text = "    Slow:" + Int32.Parse(scores[0]);


						Button btnTNormal = FindViewById<Button>(Resource.Id.btnQNormal);
						btnTNormal.Text = "    Normal:" + Int32.Parse(scores[1]);

						Button btnTFast = FindViewById<Button>(Resource.Id.btnQFast);
						btnTFast.Text = "    Fast:" + Int32.Parse(scores[2]);
					}
				}
				else if (speedValue == 2)
				{
					tHistoryScores[2] = (int.Parse(tHistoryScores[2]) + (correct + incorrect)).ToString();

					double highScore = Double.Parse(scores[2]);
					if (correct > highScore)
					{
						scores[2] = correct.ToString();

						Button btnTSlow = FindViewById<Button>(Resource.Id.btnQSlow);
						btnTSlow.Text = "    Slow:" + Int32.Parse(scores[0]);


						Button btnTNormal = FindViewById<Button>(Resource.Id.btnQNormal);
						btnTNormal.Text = "    Normal:" + Int32.Parse(scores[1]);


						Button btnTFast = FindViewById<Button>(Resource.Id.btnQFast);
						btnTFast.Text = "    Fast:" + Int32.Parse(scores[2]);
						btnTFast.SetBackgroundColor(Android.Graphics.Color.DarkGreen);

						clap.Start();
					}
					else
					{
						Button btnTSlow = FindViewById<Button>(Resource.Id.btnQSlow);
						btnTSlow.Text = "    Slow:" + Int32.Parse(scores[0]);


						Button btnTNormal = FindViewById<Button>(Resource.Id.btnQNormal);
						btnTNormal.Text = "    Normal:" + Int32.Parse(scores[1]);

						Button btnTFast = FindViewById<Button>(Resource.Id.btnQFast);
						btnTFast.Text = "    Fast:" + Int32.Parse(scores[2]);
					}
				}


				prefs.Edit().PutString("tHis", tHistoryScores[0] + "," + tHistoryScores[1] + "," + tHistoryScores[2]).Commit();


				prefs.Edit().PutString("TScores", scores[0] + "," + scores[1] + "," + scores[2]).Commit();
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

					int random = 0;

					int flip = rand.Next() % 2;
					if (flip == 0)
					{
						do
						{
							random = rand.Next() % imagesQuadrilaterals.Count;

						}
						while (random == prevShape);

					}
					else {

						for (int i = 0; i < shapes.Count; i++)
						{
							if (shapes[i].Contains(answers[0]))
							{
								random = i;
								break;
							}
						}

					}


					prevShape = random;
					//string[] keys = images.Keys.ToString().Split(',');
					cell.SetImageDrawable(imagesQuadrilaterals[random]);
					cheat.Add(cellValue, shapes[random]);
					cell.LayoutParameters.Width = widthInDp;
					cell.LayoutParameters.Height = widthInDp;
					cell.Enabled = true;
					//cell.SetTag(0, true);
					EventHandler handler = delegate
					{
						bool isFalse = true;
						bool isActive = true;
						foreach (string answer in answers)
						{
							if (answer.Contains(shapes[random]))
							{
								//Toast.MakeText(this.ApplicationContext, "correct", ToastLength.Short).Show();


								check.Start();
								isFalse = false;
								result = true;
								correct++;
								generateQuestion();
								cell.SetImageResource(Resource.Mipmap.empty);
								cell.Enabled = false;
								ptr--;
								order.Remove(cellValue);
								cheat.Remove(cellValue);
								//order.Add(order[ptr]);

								break;
							}
						}
						if (isFalse)
						{

							wrong.Start();
							result = false;
							incorrect++;
							if (incorrect == 3)
							{
								//set alert for executing the task
								AlertDialog.Builder alert = new AlertDialog.Builder(this);
								alert.SetTitle("Game Over");
								alert.SetMessage("You have no more lives left! Your score is " + correct + ".");
								timer.Stop();
								over.Start();
								alert.SetPositiveButton("High Scores", (senderAlert, args) =>
								{
									if (correct >= 0)
									{
										gameOver();
									}

								});

								alert.SetNegativeButton("Back", (senderAlert, args) =>
								{
									Finish();
								});

								Dialog dialog = alert.Create();
								dialog.Show();
							}

						}
					};

					cell.Click += handler;
					handlers.Add(cellValue, handler);
					TextView txtScore = FindViewById<TextView>(Resource.Id.txtScore);
					txtScore.Text = "Score:" + correct.ToString();
					TextView txtLives = FindViewById<TextView>(Resource.Id.txtLives);
					txtLives.Text = "Lives:" + (3-incorrect).ToString();

				}

				else if (order.Count >= 16)
				{
					Toast.MakeText(this, "Game over", ToastLength.Short).Show();
					TextView clue = FindViewById<TextView>(Resource.Id.txtClue);
					//clue.Text = "Game over. Press back to go to main menu.";
					timer.Stop();
					over.Start();
					if (correct > 0)
					{
						gameOver();
					}
					//order.Remove(0);
				}
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
			string type = Intent.GetStringExtra("type") ?? "-1";

			if (type.Equals("1"))
			{
				//Toast.MakeText(ApplicationContext, "Quadrilateral", ToastLength.Short).Show();
				quadrilaterals.Add("Diagonals bisect each other.rectangle, square, rhombus, parallelogram");
				quadrilaterals.Add("Diagonals are perpendicular.square, rhombus, kite");
				quadrilaterals.Add("Diagonals are congruent.rectangle, square, trapezoid");
				quadrilaterals.Add("Opposite angles are congruent.square, rectangle, rhombus, parallelogram");
				quadrilaterals.Add("Opposite angles are supplementary.rectangle, square");
				quadrilaterals.Add("Consecutive angles are congruent.square, rectangle");
				quadrilaterals.Add("Consecutive angles are supplementary.parallelogram, rhombus, square, rectangle");
				quadrilaterals.Add("Not more than two sides are congruent.trapezoid, parallelogram, rectangle");
				quadrilaterals.Add("Only one diagonal bisects the other diagonal.kite");


				/*items.Add("All sides are different. trapezoid");
				items.Add("Two sides are equal. square, rectangle");
				items.Add("All three sides are equal", "Square 1, Square 2");
				items.Add("One angle is obtuse", "Square 1, Square 2");
				items.Add("All angles are acute", "Square 1, Square 2");
				items.Add("One angle is right", "Square 1, Square 2");
				items.Add("All angles are equal", "Square 1, Square 2");*/

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.square1));
				shapes.Add("square");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rectangle1));
				shapes.Add("rectangle");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rhombus1));
				shapes.Add("rhombus");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.trapezoid1));
				shapes.Add("trapezoid");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.parallelogram1));
				shapes.Add("parallelogram");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.kite1));
				shapes.Add("kite");

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.square2));
				shapes.Add("square");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rectangle2));
				shapes.Add("rectangle");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rhombus2));
				shapes.Add("rhombus");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.trapezoid2));
				shapes.Add("trapezoid");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.parallelogram2));
				shapes.Add("parallelogram");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.kite2));
				shapes.Add("kite");

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.square3));
				shapes.Add("square");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rectangle3));
				shapes.Add("rectangle");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rhombus3));
				shapes.Add("rhombus");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.trapezoid3));
				shapes.Add("trapezoid");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.parallelogram3));
				shapes.Add("parallelogram");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.kite3));
				shapes.Add("kite");

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.square4));
				shapes.Add("square");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rectangle4));
				shapes.Add("rectangle");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rhombus4));
				shapes.Add("rhombus");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.trapezoid4));
				shapes.Add("trapezoid");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.parallelogram4));
				shapes.Add("parallelogram");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.kite4));
				shapes.Add("kite");

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.square5));
				shapes.Add("square");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rectangle5));
				shapes.Add("rectangle");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rhombus5));
				shapes.Add("rhombus");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.trapezoid5));
				shapes.Add("trapezoid");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.parallelogram5));
				shapes.Add("parallelogram");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.kite5));
				shapes.Add("kite");

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.square6));
				shapes.Add("square");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rectangle6));
				shapes.Add("rectangle");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rhombus6));
				shapes.Add("rhombus");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.trapezoid6));
				shapes.Add("trapezoid");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.parallelogram6));
				shapes.Add("parallelogram");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.kite6));
				shapes.Add("kite");

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.square7));
				shapes.Add("square");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rectangle7));
				shapes.Add("rectangle");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rhombus7));
				shapes.Add("rhombus");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.trapezoid7));
				shapes.Add("trapezoid");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.parallelogram7));
				shapes.Add("parallelogram");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.kite7));
				shapes.Add("kite");

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.square8));
				shapes.Add("square");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rectangle8));
				shapes.Add("rectangle");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rhombus8));
				shapes.Add("rhombus");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.trapezoid8));
				shapes.Add("trapezoid");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.parallelogram8));
				shapes.Add("parallelogram");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.kite8));
				shapes.Add("kite");
			}

			else if (type.Equals("0"))
			{
				//Toast.MakeText(ApplicationContext, "Triangle", ToastLength.Short).Show();

				quadrilaterals.Add("Scalene.s-acute, s-obtuse, r-scalene, acute");
				quadrilaterals.Add("Isosceles.i-acute, iso, o-iso, r-iso, equi");
				quadrilaterals.Add("Equilateral.equi, s-obtuse");
				quadrilaterals.Add("Right.r-iso, r-scalene");
				quadrilaterals.Add("Acute.s-acute, i-acute, iso, equi, acute");
				quadrilaterals.Add("Obtuse.s-obtuse, o-iso");
				quadrilaterals.Add("Equiangular.equi");
				//H
				quadrilaterals.Add("All sides are different.s-acute, s-obtuse, r-scalene, acute");
				//I
				quadrilaterals.Add("Two sides are equal.i-acute, iso, o-iso, r-iso, equi");
				//J
				quadrilaterals.Add("All three sides are equal.equi");
				//K
				quadrilaterals.Add("One angle is obtuse.s-obtuse, o-iso");
				//L
				quadrilaterals.Add("All angles are acute.s-acute, i-acute, iso, equi, acute");
				//M
				quadrilaterals.Add("One angle is right.r-iso, r-scalene");
				//N
				quadrilaterals.Add("All angles are equal.equi");

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.sacute1));
				shapes.Add("s-acute");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.sacute2));
				shapes.Add("s-acute");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.sacute3));
				shapes.Add("s-acute");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.sacute4));
				shapes.Add("s-acute");

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.sobtuse1));
				shapes.Add("s-obtuse");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.sobtuse2));
				shapes.Add("s-obtuse");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.sobtuse3));
				shapes.Add("s-obtuse");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.sobtuse4));
				shapes.Add("s-obtuse");

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.iacute1));
				shapes.Add("i-acute");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.iacute2));
				shapes.Add("i-acute");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.iacute3));
				shapes.Add("i-acute");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.iacute4));
				shapes.Add("i-acute");



				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.iso1));
				shapes.Add("iso");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.iso2));
				shapes.Add("iso");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.iso3));
				shapes.Add("iso");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.iso4));
				shapes.Add("iso");

				

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.equi1));
				shapes.Add("equi");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.equi2));
				shapes.Add("equi");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.equi3));
				shapes.Add("equi");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.equi4));
				shapes.Add("equi");

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.equi1a));
				shapes.Add("equi");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.equi2a));
				shapes.Add("equi");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.equi3a));
				shapes.Add("equi");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.equi4a));
				shapes.Add("equi");

				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.oiso1));
				shapes.Add("o-iso");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.oiso2));
				shapes.Add("o-iso");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.oiso3));
				shapes.Add("o-iso");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.oiso4));
				shapes.Add("o-iso");



				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.riso1));
				shapes.Add("r-iso");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.riso2));
				shapes.Add("r-iso");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.riso3));
				shapes.Add("r-iso");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.riso4));
				shapes.Add("r-iso");


				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rscalene1));
				shapes.Add("r-scalene");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rscalene2));
				shapes.Add("r-scalene");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rscalene3));
				shapes.Add("r-scalene");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.rscalene4));
				shapes.Add("r-scalene");


				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.acute1));
				shapes.Add("acute");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.acute2));
				shapes.Add("acute");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.acute3));
				shapes.Add("acute");
				imagesQuadrilaterals.Add(Resources.GetDrawable(Resource.Mipmap.acute4));
				shapes.Add("acute");








			}

		}
	}
}


