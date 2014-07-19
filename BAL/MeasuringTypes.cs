using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;
using IMPOS.DQL;

namespace IMPOS.BLL
{
    public static class MeasuringTypes
    {
        private static List<MeasuringType> _measuringTypes;

        public static List<MeasuringType> measuringTypes
        {
            get
            {
                if (_measuringTypes == null)
                {
                    var measuringType = MeasuringTypeQueries.getAllMeasurmentTypes();

                    _measuringTypes = new List<MeasuringType>();//todo:edit this line added
                    foreach (var mt in measuringType)
                    {
                        MeasuringType tempMeasuringType = new MeasuringType(mt.itemea1, mt.meanam);
                        _measuringTypes.Add(tempMeasuringType);
                    }
                }
                return _measuringTypes;
            }
        }
    }

    public class MeasuringType
    {
        /// <summary>
        /// the MeasuringType id
        /// </summary>
        private int _ID;
        
        /// <summary>
        /// the MeasuringType title
        /// </summary>
        private string _title;

        /// <summary>
        /// MeasuringType id
        /// </summary>
        public int ID
        {
            get { return _ID; }
        }

        /// <summary>
        /// MeasuringType title
        /// </summary>
        public string title
        {
            get { return _title; }
            set
            {
                _title = value;
            }
        }

        /// <summary>
        /// creates a new MeasuringType from the one in the database
        /// </summary>
        /// <param name="newID">itemeanum</param>
        /// <param name="newCode">itemeanam</param>
        public MeasuringType(int newID,
            string newTitle)
        {
            this._ID = newID;
            this._title = newTitle;
        }
    }
}
