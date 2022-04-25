/*
 TUIO C# Library - part of the reacTIVision project
 Copyright (c) 2005-2016 Martin Kaltenbrunner <martin@tuio.org>

 This library is free software; you can redistribute it and/or
 modify it under the terms of the GNU Lesser General Public
 License as published by the Free Software Foundation; either
 version 3.0 of the License, or (at your option) any later version.
 
 This library is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 Lesser General Public License for more details.
 
 You should have received a copy of the GNU Lesser General Public
 License along with this library.
*/

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using OSC.NET;

namespace TUIO
{
    /**
     * <remarks>
     * The TuioClient class is the central TUIO protocol decoder component. It provides a simple callback infrastructure using the {@link TuioListener} interface.
	 * In order to receive and decode TUIO messages an instance of TuioClient needs to be created. The TuioClient instance then generates TUIO events
	 * which are broadcasted to all registered classes that implement the {@link TuioListener} interface.
     * </remarks>
     * <example>
     * <code>
     * TuioClient client = new TuioClient();
	 * client.addTuioListener(myTuioListener);
	 * client.start();
     * </code>
     * </example>
     * 
     * @author Martin Kaltenbrunner
     * @version 1.1.6
     */
    public class TuioClient
    {
        private bool connected = false;
        private int port = 3333;
        private OSCReceiver receiver;
        private Thread thread;

        private object cursorSync = new object();
        private object objectSync = new object();
		private object blobSync = new object();

        private Dictionary<long, TuioObject> objectList = new Dictionary<long, TuioObject>(32);
        private List<long> aliveObjectList = new List<long>(32);
        private List<long> newObjectList = new List<long>(32);
        private Dictionary<long, TuioCursor> cursorList = new Dictionary<long, TuioCursor>(32);
        private List<long> aliveCursorList = new List<long>(32);
        private List<long> newCursorList = new List<long>(32);
		private Dictionary<long, TuioBlob> blobList = new Dictionary<long, TuioBlob>(32);
		private List<long> aliveBlobList = new List<long>(32);
		private List<long> newBlobList = new List<long>(32);
        private List<TuioObject> frameObjects = new List<TuioObject>(32);
        private List<TuioCursor> frameCursors = new List<TuioCursor>(32);
		private List<TuioBlob> frameBlobs = new List<TuioBlob>(32);

        private List<TuioCursor> freeCursorList = new List<TuioCursor>();
        private int maxCursorID = -1;
		private List<TuioBlob> freeBlobList = new List<TuioBlob>();
		private int maxBlobID = -1;

        private int currentFrame = 0;
        private TuioTime currentTime;

        private List<TuioListener> listenerList = new List<TuioListener>();

        #region Constructors
        /**
         * <summary>
		 * The default constructor creates a client that listens to the default TUIO port 3333</summary>
		 */
        public TuioClient() { }

        /**
         * <summary>
         * This constructor creates a client that listens to the provided port</summary>
         * <param name="port">the listening port number</param>
         */
        public TuioClient(int port)
        {
            this.port = port;
        }
        #endregion

        #region Connection Methods
        /**
		 * <summary>
         * Returns the port number listening to.</summary>
         * <returns>the listening port number</returns>
		 */
        public int getPort()
        {
            return port;
        }

        /**
         * <summary>
         * The TuioClient starts listening to TUIO messages on the configured UDP port
         * All reveived TUIO messages are decoded and the resulting TUIO events are broadcasted to all registered TuioListeners</summary>
         */
        public void connect()
        {

            TuioTime.initSession();
            currentTime = new TuioTime();
            currentTime.reset();

            try
            {
                receiver = new OSCReceiver(port);
                connected = true;
                thread = new Thread(new ThreadStart(listen));
                thread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("failed to connect to port " + port);
                Console.WriteLine(e.Message);
            }
        }

        /**
         * <summary>
         * The TuioClient stops listening to TUIO messages on the configured UDP port</summary>
         */
        public void disconnect()
        {
			connected = false;
            if (receiver != null) receiver.Close();
            receiver = null;

            aliveObjectList.Clear();
            aliveCursorList.Clear();
			aliveBlobList.Clear();
            objectList.Clear();
            cursorList.Clear();
			blobList.Clear();
            frameObjects.Clear();
            frameCursors.Clear();
			frameBlobs.Clear();
			freeCursorList.Clear();
			freeBlobList.Clear();
        }

        /**
         * <summary>
         * Returns true if this TuioClient is currently connected.</summary>
         * <returns>true if this TuioClient is currently connected</returns>
         */
        public bool isConnected() { return connected; }

        private void listen()
        {
            while (connected)
            {
                try
                {
                    OSCPacket packet = receiver.Receive();
                    if (packet != null)
                    {
                        if (packet.IsBundle())
                        {
                            ArrayList messages = packet.Values;
                            for (int i = 0; i < messages.Count; i++)
                            {
                                processMessage((OSCMessage)messages[i]);
                            }
                        }
                        else processMessage((OSCMessage)packet);
                    }
                    else Console.WriteLine("null packet");
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }
        #endregion

        /**
		 * <summary>
         * The OSC callback method where all TUIO messages are received and decoded
		 * and where the TUIO event callbacks are dispatched</summary>
         * <param name="message">the received OSC message</param>
		 */
        private void processMessage(OSCMessage message)
        {
            string address = message.Address;
            ArrayList args = message.Values;
            string command = (string)args[0];

            if (address == "/tuio/2Dobj")
            {
                if (command == "set")
                {

                    long s_id = (int)args[1];
                    int f_id = (int)args[2];
                    float xpos = (float)args[3];
                    float ypos = (float)args[4];
                    float angle = (float)args[5];
                    float xspeed = (float)args[6];
                    float yspeed = (float)args[7];
                    float rspeed = (float)args[8];
                    float maccel = (float)args[9];
                    float raccel = (float)args[10];

                    lock (objectSync)
                    {
                        if (!objectList.ContainsKey(s_id))
                        {
                            TuioObject addObject = new TuioObject(s_id, f_id, xpos, ypos, angle);
                            frameObjects.Add(addObject);
                        }
                        else
                        {
                            TuioObject tobj = objectList[s_id];
                            if (tobj == null) return;
                            if ((tobj.X != xpos) || (tobj.Y != ypos) || (tobj.Angle != angle) || (tobj.XSpeed != xspeed) || (tobj.YSpeed != yspeed) || (tobj.RotationSpeed != rspeed) || (tobj.MotionAccel != maccel) || (tobj.RotationAccel != raccel))
                            {

                                TuioObject updateObject = new TuioObject(s_id, f_id, xpos, ypos, angle);
                                updateObject.update(xpos, ypos, angle, xspeed, yspeed, rspeed, maccel, raccel);
                                frameObjects.Add(updateObject);
                            }
                        }
                    }

                }
                else if (command == "alive")
                {

                    newObjectList.Clear();
                    for (int i = 1; i < args.Count; i++)
                    {
                        // get the message content
                        long s_id = (int)args[i];
                        newObjectList.Add(s_id);
                        // reduce the object list to the lost objects
                        if (aliveObjectList.Contains(s_id))
                            aliveObjectList.Remove(s_id);
                    }

                    // remove the remaining objects
                    lock (objectSync)
                    {
                        for (int i = 0; i < aliveObjectList.Count; i++)
                        {
                            long s_id = aliveObjectList[i];
                            TuioObject removeObject = objectList[s_id];
                            removeObject.remove(currentTime);
                            frameObjects.Add(removeObject);
                        }
                    }

                }
                else if (command == "fseq")
                {
                    int fseq = (int)args[1];
                    bool lateFrame = false;

                    if (fseq > 0)
                    {
                        if (fseq > currentFrame) currentTime = TuioTime.SessionTime;
                        if ((fseq >= currentFrame) || ((currentFrame - fseq) > 100)) currentFrame = fseq;
                        else lateFrame = true;
                    }
                    else if ((TuioTime.SessionTime.TotalMilliseconds - currentTime.TotalMilliseconds) > 100)
                    {
                        currentTime = TuioTime.SessionTime;
                    }

                    if (!lateFrame)
                    {

                        IEnumerator<TuioObject> frameEnum = frameObjects.GetEnumerator();
                        while (frameEnum.MoveNext())
                        {
                            TuioObject tobj = frameEnum.Current;

                            switch (tobj.TuioState)
                            {
                                case TuioObject.TUIO_REMOVED:
                                    TuioObject removeObject = tobj;
                                    removeObject.remove(currentTime);

                                    for (int i = 0; i < listenerList.Count; i++)
                                    {
                                        TuioListener listener = (TuioListener)listenerList[i];
                                        if (listener != null) listener.removeTuioObject(removeObject);
                                    }
                                    lock (objectSync)
                                    {
                                        objectList.Remove(removeObject.SessionID);
                                    }
                                    break;
                                case TuioObject.TUIO_ADDED:
                                    TuioObject addObject = new TuioObject(currentTime, tobj.SessionID, tobj.SymbolID, tobj.X, tobj.Y, tobj.Angle);
                                    lock (objectSync)
                                    {
                                        objectList.Add(addObject.SessionID, addObject);
                                    }
                                    for (int i = 0; i < listenerList.Count; i++)
                                    {
                                        TuioListener listener = (TuioListener)listenerList[i];
                                        if (listener != null) listener.addTuioObject(addObject);
                                    }
                                    break;
                                default:
                                    TuioObject updateObject = getTuioObject(tobj.SessionID);
                                    if ((tobj.X != updateObject.X && tobj.XSpeed == 0) || (tobj.Y != updateObject.Y && tobj.YSpeed == 0))
                                        updateObject.update(currentTime, tobj.X, tobj.Y, tobj.Angle);
                                    else
                                        updateObject.update(currentTime, tobj.X, tobj.Y, tobj.Angle, tobj.XSpeed, tobj.YSpeed, tobj.RotationSpeed, tobj.MotionAccel, tobj.RotationAccel);

                                    for (int i = 0; i < listenerList.Count; i++)
                                    {
                                        TuioListener listener = (TuioListener)listenerList[i];
                                        if (listener != null) listener.updateTuioObject(updateObject);
                                    }
                                    break;
                            }
                        }

                        for (int i = 0; i < listenerList.Count; i++)
                        {
                            TuioListener listener = (TuioListener)listenerList[i];
                            if (listener != null) listener.refresh(new TuioTime(currentTime));
                        }

                        List<long> buffer = aliveObjectList;
                        aliveObjectList = newObjectList;
                        // recycling the List
                        newObjectList = buffer;
                    }
                    frameObjects.Clear();
                }

            }

        }

        #region Listener Management
        /**
		 * <summary>
         * Adds the provided TuioListener to the list of registered TUIO event listeners</summary>
         * <param name="listener">the TuioListener to add</param>
		 */
        public void addTuioListener(TuioListener listener)
        {
            listenerList.Add(listener);
        }

        /**
         * <summary>
         * Removes the provided TuioListener from the list of registered TUIO event listeners</summary>
         * <param name="listener">the TuioListener to remove</param>
         */
        public void removeTuioListener(TuioListener listener)
        {
            listenerList.Remove(listener);
        }

        /**
         * <summary>
         * Removes all TuioListener from the list of registered TUIO event listeners</summary>
         */
        public void removeAllTuioListeners()
        {
            listenerList.Clear();
        }
        #endregion

        #region Object Management

        /**
		 * <summary>
         * Returns a List of all currently active TuioObjects</summary>
         * <returns>a List of all currently active TuioObjects</returns>
		 */
        public List<TuioObject> getTuioObjects()
        {
            List<TuioObject> listBuffer;
            lock (objectSync)
            {
                listBuffer = new List<TuioObject>(objectList.Values);
            }
            return listBuffer;
        }

        /**
         * <summary>
         * Returns a List of all currently active TuioCursors</summary>
         * <returns>a List of all currently active TuioCursors</returns>
         */
        public List<TuioCursor> getTuioCursors()
        {
            List<TuioCursor> listBuffer;
            lock (cursorSync)
            {
                listBuffer = new List<TuioCursor>(cursorList.Values);
            }
            return listBuffer;
        }

		/**
         * <summary>
         * Returns a List of all currently active TuioBlobs</summary>
         * <returns>a List of all currently active TuioBlobs</returns>
         */
		public List<TuioBlob> getTuioBlobs()
		{
			List<TuioBlob> listBuffer;
			lock (blobSync)
			{
				listBuffer = new List<TuioBlob>(blobList.Values);
			}
			return listBuffer;
		}

        /**
         * <summary>
         * Returns the TuioObject corresponding to the provided Session ID
         * or NULL if the Session ID does not refer to an active TuioObject</summary>
         * <returns>an active TuioObject corresponding to the provided Session ID or NULL</returns>
         */
        public TuioObject getTuioObject(long s_id)
        {
            TuioObject tobject = null;
            lock (objectSync)
            {
                objectList.TryGetValue(s_id, out tobject);
            }
            return tobject;
        }

        /**
         * <summary>
         * Returns the TuioCursor corresponding to the provided Session ID
         * or NULL if the Session ID does not refer to an active TuioCursor</summary>
         * <returns>an active TuioCursor corresponding to the provided Session ID or NULL</returns>
         */
        public TuioCursor getTuioCursor(long s_id)
        {
            TuioCursor tcursor = null;
            lock (cursorSync)
            {
                cursorList.TryGetValue(s_id, out tcursor);
            }
            return tcursor;
        }

		/**
         * <summary>
         * Returns the TuioBlob corresponding to the provided Session ID
         * or NULL if the Session ID does not refer to an active TuioBlob</summary>
         * <returns>an active TuioBlob corresponding to the provided Session ID or NULL</returns>
         */
		public TuioBlob getTuioBlob(long s_id)
		{
			TuioBlob tblob = null;
			lock (blobSync)
			{
				blobList.TryGetValue(s_id, out tblob);
			}
			return tblob;
		}
        #endregion

    }
}
