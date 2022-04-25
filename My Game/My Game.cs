


//There is no copies and paste in this page excpet the things of the libary


using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using TUIO;

	public class TuioDemo : Form , TuioListener
	{
	 private bool maketrue = false;
		private bool makefalsetrue = false;
	float posxangryperson, posxangryface;
	float posx, posxsadperson;
	int posximg, posxsadface;
	private Bitmap True;
	private Bitmap False;
	private Bitmap img;
	private Bitmap img3;
	private Bitmap img4;
	private Bitmap img5;
	private Bitmap img6;
	private Bitmap img2;
	private Bitmap img1;
	private TuioClient client;
		private Dictionary<long,TuioObject> objectList;
		private Dictionary<long,TuioCursor> cursorList;
		private Dictionary<long,TuioBlob> blobList;

		public static int width, height;
		private int window_width =  640;
		private int window_height = 480;
		private bool fullscreen;
		private bool verbose;

		Font font = new Font("Arial", 10.0f);
		SolidBrush fntBrush = new SolidBrush(Color.White);
		SolidBrush word = new SolidBrush(Color.Yellow);
		

		public TuioDemo(int port) {
		
			verbose = false;
			fullscreen = false;
			width = window_width;
			height = window_height;

			this.ClientSize = new System.Drawing.Size(width, height);
			this.Name = "My Game";
			this.Text = "My Game";
			
			

			this.SetStyle( ControlStyles.AllPaintingInWmPaint |
							ControlStyles.UserPaint |
							ControlStyles.DoubleBuffer, true);

			objectList = new Dictionary<long,TuioObject>(128);
			cursorList = new Dictionary<long,TuioCursor>(128);
			blobList   = new Dictionary<long,TuioBlob>(128);
			
			client = new TuioClient(port);
			client.addTuioListener(this);

			client.connect();
		}

		

		public void addTuioObject(TuioObject o) {
			lock(objectList) {
				objectList.Add(o.SessionID,o);
			} if (verbose) Console.WriteLine("add obj "+o.SymbolID+" ("+o.SessionID+") "+o.X+" "+o.Y+" "+o.Angle);
		Console.Read();
		}

		public void updateTuioObject(TuioObject o) {

		
		if (verbose) Console.WriteLine("set obj "+o.SymbolID+" "+o.SessionID+" "+o.X+" "+o.Y+" "+o.Angle+" "+o.MotionSpeed+" "+o.RotationSpeed+" "+o.MotionAccel+" "+o.RotationAccel);

				///Happy Face
				if (o.SymbolID == 0 && o.isMoving)
				{
					posxsadface = -9999;
					posxsadperson = -9999;
					posxangryface = -9999;
					posxangryperson = -9999;
					posx = o.getScreenX(width);
				}
				if(o.SymbolID == 10 && o.isMoving)
			    {
					posxsadface = -9999;
					posxsadperson = -9999;
					posxangryface = -9999;
					posxangryperson = -9999;
					posximg = o.getScreenX(width);
					
				}

				//Angry Face
			if (o.SymbolID == 1 && o.isMoving)
			{
				posxsadface = -9999;
				posxsadperson = -9999;
				posx = -9999;
				posximg = -9999;
				posxangryface = o.getScreenX(width);
			}
			if (o.SymbolID == 11 && o.isMoving)
			{
				posxsadface = -9999;
				posxsadperson = -9999;
				posx = -9999;
				posximg = -9999;
				posxangryperson = o.getScreenX(width);

			}


		//Sad Face
		if (o.SymbolID == 2 && o.isMoving)
		{
			posxangryface = -9999;
			posxangryperson = -9999;
			posx = -9999;
			posximg = -9999;
			posxsadface = o.getScreenX(width);
		}
		if (o.SymbolID == 9 && o.isMoving)
		{
			posxangryface = -9999;
			posxangryperson = -9999;
			posx = -9999;
			posximg = -9999;
			posxsadperson = o.getScreenX(width);

		}
		if (o.SymbolID==10 && posxangryperson == -9999 && posxangryface == -9999 && posxsadperson == -9999 && posxsadface == -9999)
        {
			if (posximg > posx - 200 && posximg < posx + 200)

			{
				makefalsetrue = false;
				maketrue = true;
			}
			else
			{

				makefalsetrue = true;
				maketrue = false;

			}
		}
		
		else if(o.SymbolID == 11 && posxsadperson == -9999 && posxsadface == -9999 && posx == - 9999 && posximg == -9999 )
		{
			if (posxangryface > posxangryperson - 200 && posxangryface < posxangryperson + 200)

			{
				makefalsetrue = false;
				maketrue = true;
			}
			else
			{

				makefalsetrue = true;
				maketrue = false;

			}
		}

		 else if (o.SymbolID == 9 && posx == -9999 && posximg == -9999 && posxangryperson == -9999 && posxangryface == -9999)
		{
			if (posxsadface > posxsadperson - 200 && posxsadface < posxsadperson + 200)

			{
				makefalsetrue = false;
				maketrue = true;
			}
			else
			{

				makefalsetrue = true;
				maketrue = false;

			}
		}
	}

		public void removeTuioObject(TuioObject o) {
			lock(objectList) {
				objectList.Remove(o.SessionID);
			}
			if (verbose) Console.WriteLine("del obj "+o.SymbolID+" ("+o.SessionID+")");
		}

		
		public void refresh(TuioTime frameTime) {
			Invalidate();
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			// Getting the graphics object
			Graphics g = pevent.Graphics;
			img1 = new Bitmap("background.png");

			img1.MakeTransparent(img1.GetPixel(0, 0));
			False = new Bitmap("false.png");
			False.MakeTransparent(False.GetPixel(0, 0));
			True = new Bitmap("true.png");
			True.MakeTransparent(True.GetPixel(0, 0));

		//g.FillRectangle(bgrBrush, new Rectangle(0,0,width,height));
		g.DrawImage(img1,0,0,width,Height);
		if (makefalsetrue == true)
		{
			g.DrawImage(False, 640 / 2, 20, 50, 50);
		}
		if(maketrue == true)
        {
			g.DrawImage(True,640/2,20, 50, 50);
        }
		// draw the objects
		if (objectList.Count > 0) {
 				lock(objectList) {
					foreach (TuioObject tobj in objectList.Values) {
						int ox = tobj.getScreenX(width);
						int oy = tobj.getScreenY(height);
						int size = height / 10;

						g.TranslateTransform(ox, oy);
						g.RotateTransform((float)(tobj.Angle / Math.PI * 180.0f));
						g.TranslateTransform(-ox, -oy);
						False = new Bitmap("false.png");
						img3 = new Bitmap("Angry-person.png");
						img = new Bitmap("Sad-Person.bmp");
						img2 = new Bitmap("Happy-person.png");
						img4 = new Bitmap("Happy.png");
						img5 = new Bitmap("Angry.png");
						img6 = new Bitmap("Affrad.png");
						img3.MakeTransparent(img3.GetPixel(0, 0));
						False.MakeTransparent(False.GetPixel(0, 0));
					img4.MakeTransparent(img4.GetPixel(0, 0));
					img5.MakeTransparent(img5.GetPixel(0, 0));
					img6.MakeTransparent(img6.GetPixel(0, 0));
					img2.MakeTransparent(img2.GetPixel(0, 0));
						img.MakeTransparent(img.GetPixel(0, 0));

					if (tobj.SymbolID == 10)
					{
						g.DrawImage(img2, ox, oy, 150, 150);
					}
					if (tobj.SymbolID == 11)
					{
						g.DrawImage(img3, ox, oy, 150, 150);
					}

					//g.FillRectangle(objBrush, new Rectangle(ox - size / 2, oy - size / 2, size, size));
					if (tobj.SymbolID == 0)
	                    {
							g.DrawImage(img4, ox, oy, 100, 100);
							g.DrawString("Happy Face", font, word, new PointF(ox +20, oy +105));
						}
						if (tobj.SymbolID == 9)
						{
							g.DrawImage(img, ox, oy, 150, 150);
						}
					if (tobj.SymbolID == 1)
						{
							g.DrawImage(img5, ox, oy, 100, 100);
							g.DrawString("Angry Face", font, word, new PointF(ox + 20, oy + 105));
						}
						if (tobj.SymbolID == 2)
						{
							g.DrawImage(img6, ox, oy, 100, 100);
							g.DrawString("Sad Face", font, word, new PointF(ox + 20, oy + 105));
						}

					g.TranslateTransform(ox, oy);
						g.RotateTransform(-1 * (float)(tobj.Angle / Math.PI * 180.0f));
						g.TranslateTransform(-ox, -oy);

					}
				}
			}

			
		}

    private void InitializeComponent()
    {
            this.SuspendLayout();
            // 
            // TuioDemo
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "TuioDemo";
            this.Load += new System.EventHandler(this.TuioDemo_Load);
            this.ResumeLayout(false);

    }

    private void TuioDemo_Load(object sender, EventArgs e)
    {

    }

    public static void Main(String[] argv) {
	 		int port = 0;
			switch (argv.Length) {
				case 1:
					port = int.Parse(argv[0],null);
					if(port==0) goto default;
					break;
				case 0:
					port = 3333;
					break;
				default:
					Console.WriteLine("usage: mono TuioDemo [port]");
					System.Environment.Exit(0);
					break;
			}
			
			TuioDemo app = new TuioDemo(port);
			Application.Run(app);
		}
	}
