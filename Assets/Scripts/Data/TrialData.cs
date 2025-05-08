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
using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace Data
{
    /// <summary>
    /// This class encapsulates the data associated with a single trial (single click) within a
    /// condition within a Fitts law study. The class holds all information necessary for defining
    /// a single trial, including its target locations.
    /// </summary>
    public class TrialData
    {
        #region Fields
        protected int _number; // 1-based number of this trial; trial 0 is reserved for the start area for the condition
        protected bool _practice; // true if this is a practice trial; false otherwise
        protected long _tInterval; // the normative movement time interval in milliseconds, or -1L. (메트로놈 안 쓰면 필요 X)

        protected TimePointR _start; // the click point that started this trial
        protected TimePointR _end; // the click point that ended this trial

        protected MovementData _movement; // the movement associated with this trial
        private Target3D _thisTarget;
        private Target3D _lastTarget;
        private PointR _isoCenter;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor for an abstract Fitts' law trial.
        /// </summary>
        public TrialData()
        {
            // do nothing
        }

        /// <summary>
        /// Constructor for an abstract Fitts' law trial. Actual trial instances must be created from
        /// subclasses that define the specific trial mechanics.
        /// </summary>
        /// <param name="index">The 0-based index number of this trial.</param>
        /// <param name="practice">True if this trial is practice; false otherwise. Practice trials aren't included in any calculations.</param>
        /// <param name="tInterval">The metronome time interval in milliseconds, or -1L if unused.</param>
        public TrialData(int index, bool practice, Target3D lastTarget, Target3D thisTarget, PointR center, long tInterval)
        {
            _number = index;
            _practice = practice;
            _tInterval = tInterval;
            _start = TimePointR.Empty;
            _end = TimePointR.Empty;
            _movement = new MovementData(this);
            _lastTarget = lastTarget;
            _thisTarget = thisTarget;
            _isoCenter = center;
        }

        /// <summary>
        /// This static method is to be used by the ConditionData instance when creating the
        /// special start-area trial after reading trial number 1 in a XML log file. The special
        /// start-area trial is not logged explicitly, but it can be created from the information
        /// in trial number 1.
        /// </summary>
        /// <param name="trialOne">The first trial in the condition whose last target is used to
        /// create the special start-area trial.</param>
        /// <returns>The special start-area trial for a 2D task.</returns>
        internal static TrialData CreateStartTarget(TrialData trialOne)
        {
            TrialData st = new TrialData();
            st._number = 0;
            st._practice = true;
            st._thisTarget = trialOne._lastTarget;
            st._isoCenter = trialOne._isoCenter;
            st._tInterval = trialOne._tInterval;
            return st;
        }
        #endregion

        #region Movement
        public MovementData Movement
        {
            get { return _movement; }
            set { _movement = value; }
        }
        #endregion

        #region Condition Values: Number, IsStartAreaTrial, IsPractice, UsedMetronome, ID(unused), MT

        /// <summary>
        /// Gets the trial number of this trial. This is a 1-based index within the
        /// condition although there <i>is</i> a trial at index 0, which is the special 
        /// start-area trial.
        /// </summary>
        public int Number { get { return _number; } }

        /// <summary>
        /// Gets whether or not this trial is the special start-area trial, and therefore not
        /// really a trial at all, but a location for the initial click to begin a condition.
        /// </summary>
        public bool IsStartAreaTrial { get { return _number == 0; } }

        /// <summary>
        /// Gets a value indicating whether or not this trial is a practice trial. Is used
        /// to disregard marked trials for condition-level calculations.
        /// </summary>
        public bool IsPractice
        {
            get { return _practice; }
        }

        /// <summary>
        /// Gets whether or not this trial used a metronome to govern movement time as an
        /// independent variable.
        /// </summary>
        public bool UsedMetronome
        {
            get { return _tInterval != -1L; }
        }

        ///// <summary>
        ///// Gets the nominal index of difficulty for this trial.
        ///// </summary>
        //public double ID
        //{
        //    get
        //    {
        //        return Math.Log((double)this.A / this.W + 1.0, 2.0);
        //    }
        //}

        /// <summary>
        /// Gets the normative movement time in milliseconds for this trial. The normative 
        /// movement time is the time of a desired movement "encouraged" by the metronome 
        /// interval tick time. If a normative time isn't used, this value is -1L.
        /// </summary>
        public long MT
        {
            get { return _tInterval; }
        }

        /// <summary>
        /// Gets the angle of the nominal movement axis for this trial, in radians.
        /// </summary>
        public double Axis
        {
            get
            {
                return PointR.Angle(_lastTarget.Center, _thisTarget.Center, true);
            }
        }
        #endregion

        #region Measured Values

        /// <summary>
        /// Clears the performance data associated with this trial. Does not clear the
        /// independent variables that define the settings for this trial.
        /// </summary>
        public virtual void ClearData()
        {
            _start = TimePointR.Empty;
            _end = TimePointR.Empty;
            _movement.ClearMoves();
        }

        /// <summary>
        /// Gets whether or not this trial has been completed. A completed trial has been
        /// performed and therefore has a non-zero ending time-stamp.
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return (_end.Time != 0L);
            }
        }

        /// <summary>
        /// Gets or sets the start click point and time that began this trial.
        /// </summary>
        public TimePointR Start
        {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary>
        /// Gets or sets the selection endpoint and time that ended this trial.
        /// </summary>
        public TimePointR End
        {
            get { return _end; }
            set { _end = value; }
        }

        /// <summary>
        /// Gets the actual movement angle for this trial, in radians.
        /// </summary>
        public double Angle
        {
            get
            {
                return PointR.Angle((PointR)_start, (PointR)_end, true);
            }
        }

        /// <summary>
        /// Gets the trial start point normalized relative to this trial's target as if the target
        /// were centered at (0,0) and movement towards it was at 0 degrees, that is, straight right
        /// along the +x-axis. Thus, normalized start points will always begin at (-x,0).
        /// </summary>
        public PointR NormalizedStart
        {
            get
            {
                PointR center = this.TargetCenterFromStart;
                double radians = PointR.Angle((PointR)_start, (PointR)center, true);
                PointR newStart = PointR.RotatePoint((PointR)_start, (PointR)center, -radians);
                newStart.X -= center.X;
                newStart.Y -= center.Y;
                return newStart;
            }
        }

        /// <summary>
        /// Gets the selection endpoint normalized relative to this trial's target as if the target
        /// were centered at (0,0) and movement towards it was at 0 degrees, that is, straight right
        /// along the +x-axis. This allows endpoint distributions of trials within a condition to be
        /// compared despite not any of the actual target locations in each condition being the same.
        /// </summary>
        /// <remarks>This property is used in the calculation of the effective target width for the condition, We.</remarks>
        public PointR NormalizedEnd
        {
            get
            {
                // find the angle of the ideal task axis for this trial
                PointR center = this.TargetCenterFromStart;
                double radians = PointR.Angle((PointR)_start, center, true);

                // rotate the endpoint around the target center so that it would have come from 
                // a task whose task-axis was at 0 degrees (+x, straight to the right).
                PointR newEnd = PointR.RotatePoint((PointR)_end, center, -radians);

                // translate the endpoint so that it is as if the target was centered at (0,0).
                newEnd.X -= center.X;
                newEnd.Y -= center.Y;

                return newEnd;
            }
        }

        /// <summary>
        /// Normalizes the trial time so that the start time is zero and each move time
        /// and the end time are relative to that. Only works on completed trials.
        /// </summary>
        public void NormalizeTimes()
        {
            if (!IsComplete)
                return;

            _movement.NormalizeTimes(_start.Time);
            _end.Time -= _start.Time;
            _start.Time = 0L;
        }

        /// <summary>
        /// Gets the actual movement time in milliseconds for this trial.
        /// </summary>
        public long MTe
        {
            get
            {
                return _end.Time - _start.Time;
            }
        }

        /// <summary>
        /// Gets a ratio indicating how the actual movement time (MTe) corresponded to the normative
        /// movement time (MT). The ratio is <c>MTe / MT</c>. Thus, values &gt;1 indicate that the actual movement
        /// time was too slow. Values &lt;1 indicate that the actual movement time was too fast. A value
        /// of 1.00 indicates the actual movement time was the same as that prescribed.
        /// </summary>
        /// <remarks>
        /// The maximum possible number of actual metronome ticks that could have been heard is the <b>Math.Ceiling</b> 
        /// of this property. The minimum number of ticks that could have been heard is the <b>Math.Floor</b> of this 
        /// property.
        /// </remarks>
        public double MTRatio
        {
            get
            {
                if (this.UsedMetronome)
                {
                    double diff = _end.Time - _start.Time;
                    return diff / _tInterval;
                }
                return -1.0; // unused
            }
        }
        /*
        /// <summary>
        /// For a completed trial, gets the number of target entries. If the trial was an error,
        /// it may be that the target was never entered. The most successful trials will have
        /// a target entered once and only once. If target re-entry occurs, the target was entered 
        /// multiple times.
        /// </summary>
        public int TargetEntries
        {
            get
            {
                int n = 0;
                bool inside = false;

                for (int i = 0; i < _movement.NumMoves; i++)
                {
                    TimePointR pt = _movement[i];
                    if (TargetContains((PointR)pt)) // now inside
                    {
                        if (!inside) // were not yet inside
                        {
                            inside = true;
                            n++; // entry
                        }
                    }
                    else inside = false; // not inside
                }
                return n;
            }
        }
        */
        #endregion

        #region Target
        public PointR TargetCenter
        {
            get { return _thisTarget.Center; }
        }

        /// <summary>
        /// Gets the center point of this target relative to the start of the trial.
        /// However, for circular targets, the center is the same regardless of approach
        /// angle.
        /// </summary>
        /// <remarks>If the trial has not been run yet, there will not be a start point
        /// and this property's value is meaningless. In this case, <b>PointF.Empty</b> is
        /// its value.</remarks>
        public PointR TargetCenterFromStart
        {
            get
            {
                if (this.IsComplete)
                {
                    return _thisTarget.Center;
                }
                return PointR.Empty;
            }
        }
        #endregion

    }
}

