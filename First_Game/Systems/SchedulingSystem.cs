using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using First_Game.Interfaces;

namespace First_Game.Systems
{
    public class SchedulingSystem
    {
        #region Prop
        private int _time;
        private readonly SortedDictionary<int, List<IScheduleable>> _scheduleables;

        #endregion Prop

        #region Constructor
        public SchedulingSystem()
        {
            //the time starts at 0 and we create a new list of scheduables
            _time = 0;
            _scheduleables = new SortedDictionary<int, List<IScheduleable>>();
        }

        #endregion Constructor

        #region Methods

        //this adds a new object too the schedule
        //we place it at the current time plus the objects time property
        public void Add(IScheduleable scheduleable)
        {
            //gets the key time
            int key = _time + scheduleable.Time;

            //if the list does not contain a schedule at this time create a new schedule
            if (!_scheduleables.ContainsKey(key))
            {
                _scheduleables.Add(key, new List<IScheduleable>());
            }
            //add this schedule too the list of schedules at this time
            _scheduleables[key].Add(scheduleable);
        }

        //this removes an object from the schedule
        //used too remove a monsters schedule after it is killed etc
        public void Remove(IScheduleable scheduleable)
        {
            //this represents a key in the dictionary which represents a time
            //we have a list of them because multiple actors can act at the same time
            KeyValuePair<int, List<IScheduleable>> scheduleableListFound = new KeyValuePair<int, List<IScheduleable>>(-1, null);

            //for each schedule in the lisr of schedules find this particular schedule
            foreach (var scheduleableList in _scheduleables)
            {
                if (scheduleableList.Value.Contains(scheduleable))
                {
                    scheduleableListFound = scheduleableList;
                    break;
                }
            }

            //if the particular schedule is found remove it fom the list of schedules
            if (scheduleableListFound.Value != null)
            {
                scheduleableListFound.Value.Remove(scheduleable);
                if (scheduleableListFound.Value.Count <= 0)
                {
                    _scheduleables.Remove(scheduleableListFound.Key);
                }
            }
        }

        //get the next object wgis turn it is from the schedule. Advance time if necessary
        public IScheduleable Get()
        {
            var firstScheduleGroup = _scheduleables.First();
            var firstScheduleable = firstScheduleGroup.Value.First();

            Remove(firstScheduleable);
            _time = firstScheduleGroup.Key;
            return firstScheduleable;
        }

        //Get the current time for the schedule
        public int GetTime()
        {
            return _time;
        }

        //resets the time and clears the schedule
        public void Clear()
        {
            _time = 0;
            _scheduleables.Clear();
        }

        #endregion Methods
    }
}
