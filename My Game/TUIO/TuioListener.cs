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

namespace TUIO
{

    /**
     * <para>
     * The TuioListener interface provides a simple callback infrastructure which is used by the {@link TuioClient} class
     * to dispatch TUIO events to all registered instances of classes that implement the TuioListener interface defined here.
     * </para>
     * <para>
     * Any class that implements the TuioListener interface is required to implement all of the callback methods defined here.
     * The {@link TuioClient} makes use of these interface methods in order to dispatch TUIO events to all registered TuioListener implementations.
     * </para>
     * <example>
     * <code>
     * public class MyTuioListener implements TuioListener
     * ...
     * MyTuioListener listener = new MyTuioListener();
     * TuioClient client = new TuioClient();
     * client.addTuioListener(listener);
     * client.start();
     * </code>
     * </example>
     *
     * @author Martin Kaltenbrunner
     * @version 1.1.6
     */
    public interface TuioListener
    {
        /**
         * <summary>
         * This callback method is invoked by the TuioClient when a new TuioObject is added to the session.</summary>
         *
         * <param name="tobj">the TuioObject reference associated to the addTuioObject event</param>
         */
        void addTuioObject(TuioObject tobj);

        /**
         * <summary>
         * This callback method is invoked by the TuioClient when an existing TuioObject is updated during the session.</summary>
         *
         * <param name="tobj">the TuioObject reference associated to the updateTuioObject event</param>
         */
        void updateTuioObject(TuioObject tobj);

        /**
         * <summary>
         * This callback method is invoked by the TuioClient when an existing TuioObject is removed from the session.</summary>
         *
         * <param name="tobj">the TuioObject reference associated to the removeTuioObject event</param>
         */
        void removeTuioObject(TuioObject tobj);

        
        void refresh(TuioTime ftime);
    }
}
