/*
	TUIO C# Demo - part of the reacTIVision project
	Copyright (c) 2005-2016 Martin Kaltenbrunner <martin@tuio.org>

	This program is free software; you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

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
		float posx=0, posy;
	int posximg=0, posyimg;
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
		private int window_left = 0;
		private int window_top = 0;
		private int screen_width = Screen.PrimaryScreen.Bounds.Width;
		private int screen_height = Screen.PrimaryScreen.Bounds.Height;

		private bool fullscreen;
		private bool verbose;

		Font font = new Font("Arial", 10.0f);
		SolidBrush fntBrush = new SolidBrush(Color.White);
		SolidBrush word = new SolidBrush(Color.Yellow);
		SolidBrush bgrBrush = new SolidBrush(Color.FromArgb(0,0,64));
		SolidBrush objBrush = new SolidBrush(Color.FromArgb(64, 0, 0));
		Pen curPen = new Pen(new SolidBrush(Color.Blue), 1);

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

			 if (verbose)
			 {
				if (o.SymbolID == 9 && o.isMoving == true)
				{
					posx = o.X;
					posy = o.Y;
				}
			 }
				Console.WriteLine(posx);
				Console.Read ();
				if (o.SymbolID == 10)
				{
					if(o.getScreenX(width) >= posx-50 && o.getScreenX(width) < posx+50)
					{
						maketrue = true;
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
			g.FillRectangle(bgrBrush, new Rectangle(0,0,width,height));
			g.DrawImage(img1,0,0,width,Height);
			
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
						img3 = new Bitmap("4.png");
						img = new Bitmap("Sad-Person.bmp");
						img2 = new Bitmap("Happy-person.png");
						img4 = new Bitmap("Happy.png");
						img5 = new Bitmap("Angry.png");
						img6 = new Bitmap("Affrad.png");
						img3.MakeTransparent(img3.GetPixel(0, 0));
					img4.MakeTransparent(img4.GetPixel(0, 0));
					img5.MakeTransparent(img5.GetPixel(0, 0));
					img6.MakeTransparent(img6.GetPixel(0, 0));
					img2.MakeTransparent(img2.GetPixel(0, 0));
						img.MakeTransparent(img.GetPixel(0, 0));
						if(tobj.SymbolID == 10)
						 {
						if(maketrue ==	true)
                        {
							posximg += 5;
							
							
							g.DrawImage(img, ox, oy, 200, 200);
							g.DrawImage(img3, img.Width + ox + posximg, img.Height + oy, 20, 20);
						}
                        else
                        {
							posximg = 0;
							
							g.DrawImage(img, ox, oy, 200, 200);

						}
							
						}
									
						//g.FillRectangle(objBrush, new Rectangle(ox - size / 2, oy - size / 2, size, size));
						if(tobj.SymbolID == 0)
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

						//g.DrawString(tobj.SymbolID + "", font, fntBrush, new PointF(ox - 10, oy - 10));
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
