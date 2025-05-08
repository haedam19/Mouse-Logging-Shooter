/**
 * FittsStudy
 *
 *		Jacob O. Wobbrock, Ph.D.
 * 		The Information School
 *		University of Washington
 *		Mary Gates Hall, Box 352840
 *		Seattle, WA 98195-2840
 *		wobbrock@uw.edu
 *		
 * This software is distributed under the "New BSD License" agreement:
 * 
 * Copyright (C) 2007-2022, Jacob O. Wobbrock. All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *    * Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *    * Redistributions in binary form must reproduce the above copyright
 *      notice, this list of conditions and the following disclaimer in the
 *      documentation and/or other materials provided with the distribution.
 *    * Neither the name of the University of Washington nor the names of its 
 *      contributors may be used to endorse or promote products derived from 
 *      this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
 * IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Jacob O. Wobbrock
 * BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
 * GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
**/
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using TimeL = System.Int64;

namespace Data
{
    public class MovementData
    {
        #region Types
        public struct Profiles
        {
            public static readonly Profiles Empty;
            public bool IsEmpty { get { return Position == null && Velocity == null && Acceleration == null && Jerk == null; } }
            public List<TimeL> timeStamp;
            public List<Vector2> Position;
            public List<Vector2> Velocity;
            public List<Vector2> Acceleration;
            public List<Vector2> Jerk;
        }

        /// <summary>
        /// <para> A structure containing the six path analysis measures defined by MacKenzie et al. (CHI 2001). </para>
        /// </summary>
        public struct PathAnalyses
        {
            public static readonly PathAnalyses Empty;
            public bool IsEmpty { get { return MovementVariability == 0.0 && MovementError == 0.0 && MovementOffset == 0.0; } }
            public double MovementVariability;
            public double MovementError;
            public double MovementOffset;
        }
        #endregion
        private TrialData _owner;
        public List<Vector2> mousePos;
        public List<TimeL> time; // ms 단위

        #region Properties: NumMoves, Travel, Duration
        public int NumMoves { get { return mousePos.Count; } }

        /// <summary>
        /// Gets the total distance traveled during movement.
        /// </summary>
        public float Travel
        {
            get
            {
                float travel = 0f;
                for (int i = 1; i < NumMoves; i++)
                    travel += Vector2.Distance(mousePos[i], mousePos[i - 1]);
                return travel;
            }
        }

        /// <summary>
        /// Gets the duration of this movement, in milliseconds.
        /// </summary>
        public long Duration
        {
            get
            {
                if (time != null || time.Count > 0)
                    return time[time.Count - 1] - time[0];
                else return 0L;
            }
        }
        #endregion

        public void AddMove(Vector2 pos, TimeL t)
        {
            // 마우스 움직임 X -> 시간만 업데이트
            if (mousePos.Count > 0 && mousePos[mousePos.Count - 1] == pos)
            {
                time.RemoveAt(time.Count - 1);
                time.Add(t);
            }
            else
            {
                mousePos.Add(pos);
                time.Add(t);
            }
        }

        public void AddMove(float x, float y, TimeL t)
        {
            Vector2 pos = new Vector2(x, y);
            AddMove(pos, t);
        }

        public void ClearMoves()
        {
            mousePos.Clear();
            time.Clear();
            
        }
        /*
        public Profiles CreateResampledProfiles()
        {
            if (NumMoves == 0)
                return Profiles.Empty;

            Profiles resampled;
        }

        #region IXmlLoggable Members

        public bool WriteXmlHeader(XmlTextWriter writer)
        {
            writer.WriteStartElement("Movement");
            writer.WriteAttributeString("count", XmlConvert.ToString(NumMoves));
            writer.WriteAttributeString("travel", XmlConvert.ToString(System.Math.Round(Travel, 4)));
            writer.WriteAttributeString("duration", XmlConvert.ToString(Duration));

            // write out the submovement information
            Profiles resampled = CreateResampledProfiles();
            if (resampled.IsEmpty) // this can happen if the trial is terminated prematurely
            {
                writer.WriteAttributeString("submovements", XmlConvert.ToString(0));
                writer.WriteAttributeString("maxVelocity", PointR.Empty.ToString());
                writer.WriteAttributeString("maxAcceleration", PointR.Empty.ToString());
                writer.WriteAttributeString("maxJerk", PointR.Empty.ToString());
            }
            else
            {
                int nsub = GetNumSubmovements(); // from smoothed velocity profile
                int vidx = SeriesEx.Max(resampled.Velocity, 0, -1);
                int aidx = SeriesEx.Max(resampled.Acceleration, 0, -1);
                int jidx = SeriesEx.Max(resampled.Jerk, 0, -1);

                writer.WriteAttributeString("submovements", XmlConvert.ToString(nsub));
                writer.WriteAttributeString("maxVelocity", resampled.Velocity[vidx].ToString());
                writer.WriteAttributeString("maxAcceleration", resampled.Acceleration[aidx].ToString());
                writer.WriteAttributeString("maxJerk", resampled.Jerk[jidx].ToString());
            }

            // write out all of MacKenzie's path analysis measures
            PathAnalyses analyses = DoPathAnalyses((PointR)_owner.Start, _owner.TargetCenterFromStart);
            writer.WriteAttributeString("taskAxisCrossings", XmlConvert.ToString(analyses.TaskAxisCrossings));
            writer.WriteAttributeString("movementDirectionChanges", XmlConvert.ToString(analyses.MovementDirectionChanges));
            writer.WriteAttributeString("orthogonalDirectionChanges", XmlConvert.ToString(analyses.OrthogonalDirectionChanges));
            writer.WriteAttributeString("movementVariability", XmlConvert.ToString(Math.Round(analyses.MovementVariability, 4)));
            writer.WriteAttributeString("movementError", XmlConvert.ToString(Math.Round(analyses.MovementError, 4)));
            writer.WriteAttributeString("movementOffset", XmlConvert.ToString(Math.Round(analyses.MovementOffset, 4)));

            // write out all the mouse move points that make up this trial
            int i = 0;
            foreach (TimePointR pt in _moves)
            {
                writer.WriteStartElement("move");
                writer.WriteAttributeString("index", XmlConvert.ToString(i++));
                writer.WriteAttributeString("point", pt.ToString());
                writer.WriteEndElement(); // </move>
            }

            writer.WriteEndElement(); // </Movement>

            return true;
        }

        #endregion
        */
    }
}

