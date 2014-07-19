using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;
using IMPOS.DQL;

namespace IMPOS.BLL
{
    public static class Machines
    {
        /// <summary>
        /// the version of the machine
        /// </summary>
        private static int _machineVersion = 0;

        /// <summary>
        /// the version of the machine
        /// </summary>
        public static int machineVersion
        {
            get
            {
                return _machineVersion;
            }
            set
            {
                _machineVersion = value;
            }
        }

        /// <summary>
        /// the list of machines
        /// </summary>
        static List<Machine> _allMechines;

        /// <summary>
        /// the list of machines
        /// </summary>
        public static List<Machine> allMechines
        {
            get
            {
                if (_allMechines == null)
                {
                    loadMachineInfoFromDB();
                }
                return _allMechines;
            }
        }

        /// <summary>
        /// load all the machine type from the database
        /// </summary>
        public static void loadMachineInfoFromDB()
        {
            var allMachinesFromDB = MachineQuery.getAllMachines();
            _allMechines = new List<Machine>();
            int i = 0;
            foreach (var machine in allMachinesFromDB)
            {
                Machine tempMechine = new Machine(
                    machine.macnum,
                    machine.maccod,
                    machine.mactit,
                    machine.macsta == 1 ? false : true,
                    machine.maccal,
                    machine.macetc,
                    machine.typnum,
                    (int)machine.macwor);
                _allMechines.Add(tempMechine);
            }
        }

        /// <summary>
        /// sorts the machines by their title
        /// </summary>
        public static void sortMachinesByName()
        {
            _allMechines.Sort(new Comparison<Machine>((x, y) => string.Compare(x.title, y.title)));
            machineVersion++;
        }

        /// <summary>
        /// deletes the machine from the machine list
        /// </summary>
        /// <param name="ID">the id of the machine</param>
        public static void deleteFromListByID(int ID)
        {
            foreach (var m in _allMechines)
            {
                if (m.ID == ID)
                {
                    _allMechines.Remove(m);
                    break;
                }
            }
            _machineVersion++;
        }

        /// <summary>
        /// add the new machine to the list
        /// </summary>
        /// <param name="machine">the machine just added</param>
        public static void addMachineToList(Machine machine)
        {
            for (int i = 0; i < _allMechines.Count; i++)
                if (machine.title.CompareTo(_allMechines[i].title) <= 0)
                {
                    _allMechines.Insert(i, machine);
                    break;
                }
            _machineVersion++;
        }

        /// <summary>
        /// saves all the changes made to machines
        /// </summary>
        public static void saveAllMachines()
        {
            foreach(var machine in _allMechines)
                if (machine.hasChanged)
                {
                    machine.saveChanges();
                }
        }

        /// <summary>
        /// reset all the changes made to machines
        /// </summary>
        public static void cancelChanges()
        {
            foreach (var machine in _allMechines)
                if (machine.hasChanged)
                {
                    machine.loadAgain();
                }
        }
    }

    public class Machine
    {
        /// <summary>
        /// the machine id(machine number)
        /// </summary>
        private int _ID;
        /// <summary>
        /// the machine code
        /// </summary>
        private string _code;
        /// <summary>
        /// the machine title
        /// </summary>
        private string _title;
        /// <summary>
        /// the machine status
        /// </summary>
        private bool _status;
        /// <summary>
        /// the machine calendar id
        /// </summary>
        private int? _calID;
        /// <summary>
        /// the machine description
        /// </summary>
        private string _description;
        /// <summary>
        /// the machine type id
        /// </summary>
        private int _type;
        /// <summary>
        /// the workshop of the machine
        /// </summary>
        private int _workshop;

        /// <summary>
        /// has any parameter changed
        /// </summary>
        private bool _hasChanged;

        /// <summary>
        /// has any parameter changed
        /// </summary>
        public bool hasChanged
        {
            get
            {
                return _hasChanged;
            }
        }

        /// <summary>
        /// machine id(machine number)
        /// </summary>
        public int ID
        {
            get { return _ID; }
            //set { _ID = value; }
        }

        /// <summary>
        /// machine code
        /// </summary>
        public string code
        {
            get { return _code; }
            set 
            { 
                _code = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// machine title
        /// </summary>
        public string title
        {
            get { return _title; }
            set 
            { 
                _title = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// machine status
        /// </summary>
        public bool status
        {
            get{ return _status; }
            set 
            {
                if (value != _status)
                {
                    _status = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// machine calendar id
        /// </summary>
        public int? calID
        {
            get { return _calID; }
            set 
            { 
                _calID = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// machine description
        /// </summary>
        public string description
        {
            get { return _description; }
            set 
            { 
                _description = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// machine type
        /// </summary>
        public int type
        {
            get { return _type; }
            set 
            { 
                _type = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// machine workshop
        /// </summary>
        public int workshop
        {
            get { return _workshop; }
            set 
            { 
                _workshop = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// Save the changes made to the Machine to the database
        /// </summary>
        public void saveChanges()
        {
            MachineQuery.setMachineParametersByID(this._ID,
                this._code, 
                this._title, 
                this._status, 
                this._calID, 
                this._description, 
                this._type,
                this._workshop);
            Machines.sortMachinesByName();
            _hasChanged = false;
        }

        public void addMachine()
        {
            this._ID = MachineQuery.addNewMachineWithParameters(
                this._code,
                this._title,
                this._status,
                this._calID,
                this._description,
                this._type,
                this._workshop);
            Machines.addMachineToList(this);
            _hasChanged = false;
        }

        /// <summary>
        /// load the Machine from the database again to revert changes
        /// </summary>
        public void loadAgain()
        {
            var m = MachineQuery.getMachineParametersByID(_ID);
            this._code = m.maccod;
            this._title = m.mactit;
            this._status = m.macsta == 1 ? false : true;
            this._calID = m.maccal;
            this._description = m.macetc;
            this._type = m.typnum;
            _hasChanged = false;
        }

        /// <summary>
        /// deletes this machine from database
        /// </summary>
        public void deleteThisMachine()
        {
            Machines.deleteFromListByID(_ID);
            MachineQuery.deleteMachineByID(_ID);
        }

        /// <summary>
        /// creates a new machine form the one in the database
        /// </summary>
        /// <param name="newID">macnum</param>
        /// <param name="newCode">maccod</param>
        /// <param name="newTitle">mactit</param>
        /// <param name="newStatus">macsta</param>
        /// <param name="newCalID">maccal</param>
        /// <param name="newDescription">macetc</param>
        /// <param name="newType">typnum</param>
        public Machine(int newID,
            string newCode,
            string newTitle,
            bool newStatus,
            int? newCalID,
            string newDescription,
            int newType,
            int newWorkshop)
        {
            this._ID = newID;
            this._code = newCode;
            this._title = newTitle;
            this._status = newStatus;
            this._calID = newCalID;
            this._description = newDescription;
            this._type = newType;
            this._workshop = newWorkshop;
            _hasChanged = false;
        }

        /// <summary>
        /// creates a new machine by the user and adds it to the database
        /// </summary>
        /// <param name="newCode">maccod</param>
        /// <param name="newTitle">mactit</param>
        /// <param name="newStatus">macsta</param>
        /// <param name="newCalID">maccal</param>
        /// <param name="newDescription">macetc</param>
        /// <param name="newType">typnum</param>
        public Machine(
            string newCode,
            string newTitle,
            bool newStatus,
            int? newCalID,
            string newDescription,
            int newType,
            int newWorkshop)
        {
            this._code = newCode;
            this._title = newTitle;
            this._status = newStatus;
            this._calID = newCalID;
            this.description = newDescription;
            this._type = newType;
            this._workshop = newWorkshop;
            _hasChanged = true;
        }
    }
}
