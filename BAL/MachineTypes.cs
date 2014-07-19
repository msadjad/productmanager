using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;
using IMPOS.DQL;

namespace IMPOS.BLL
{
    public static class MachineTypes
    {
        /// <summary>
        /// the list of machine types
        /// </summary>
        private static List<MachineType> _allMachineTypes;

        /// <summary>
        /// the list of machine types
        /// </summary>
        public static List<MachineType> allMachineTypes
        {
            get
            {
                if (_allMachineTypes == null)
                {
                    loadMachineTypeInfoFromDB();
                }
                return _allMachineTypes;
            }
        }

        /// <summary>
        /// loads all the machine types from the database
        /// </summary>
        public static void loadMachineTypeInfoFromDB()
        {
            var allMachineTypesFromDB = MachineTypeQuery.getAllMachineTypes();
            _allMachineTypes = new List<MachineType>();
            foreach (var machineType in allMachineTypesFromDB)
            {
                MachineType newMachineType = new MachineType(
                    machineType.typnum, 
                    machineType.typtit,
                    machineType.restyp1 == 2 ? true : false,
                    machineType.restyp2 == 2 ? true : false,
                    machineType.rescap == 1 ? true : false, 
                    machineType.typetc);
                _allMachineTypes.Add(newMachineType);
            }
        }

        /// <summary>
        /// sorts the machine types by their title
        /// </summary>
        internal static void sortMachineTypesByName()
        {
            _allMachineTypes.Sort(new Comparison<MachineType>((x, y) => string.Compare(x.title, y.title)));
        }

        /// <summary>
        /// add the new machine type to the list
        /// </summary>
        /// <param name="workshop">the machine type just added</param>
        internal static void addMachineTypeToList(MachineType machineType)
        {
            for (int i = 0; i < _allMachineTypes.Count; i++)
                if (machineType.title.CompareTo(_allMachineTypes[i].title) <= 0)
                {
                    _allMachineTypes.Insert(i, machineType);
                    break;
                }
        }

        /// <summary>
        /// deletes the machine type from the machine type list
        /// </summary>
        /// <param name="ID">the id of the machine type</param>
        internal static void deleteFromListById(int ID)
        {
            foreach (var m in _allMachineTypes)
            {
                if (m.ID == ID)
                {
                    _allMachineTypes.Remove(m);
                    break;
                }
            }
        }

        /// <summary>
        /// saves all chanegs made to machine types
        /// </summary>
        public static void saveAllMachineTypes()
        {
            foreach (var machineType in _allMachineTypes)
                if (machineType.hasChanged)
                {
                    machineType.saveChanges();
                }
        }

        /// <summary>
        /// reset all chanegs made to machine type
        /// </summary>
        public static void cancelChanges()
        {
            foreach (var machineType in _allMachineTypes)
                if (machineType.hasChanged)
                {
                    machineType.loadAgain();
                }
        }
    }

    public class MachineType
    {
        /// <summary>
        /// the machine type title
        /// </summary>
        private string _title;
        /// <summary>
        /// the resource type 1
        /// </summary>
        private bool _internalExternal;
        /// <summary>
        /// the resource type 2
        /// </summary>
        private bool _stationMachine;
        /// <summary>
        /// the resource capacity
        /// </summary>
        private bool _limitUnlimit;
        /// <summary>
        /// the resource etc
        /// </summary>
        private string _descriptions;
        /// <summary>
        /// the resource id (type number)
        /// </summary>
        private int _ID;
        /// <summary>
        /// all the machines of this type
        /// </summary>
        private List<Machine> _machinesOfThisType;

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
        /// machine type title
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
        /// resource type 1
        /// </summary>
        public bool internalExternal
        {
            get { return _internalExternal; }
            set 
            {
                _internalExternal = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// resource type 2
        /// </summary>
        public bool stationMachine
        {
            get { return _stationMachine; }
            set 
            {
                _stationMachine = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// resource capacity
        /// </summary>
        public bool limitUnlimit
        {
            get { return _limitUnlimit; }
            set 
            { 
                _limitUnlimit = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// type etc
        /// </summary>
        public string description
        {
            get { return _descriptions; }
            set 
            {
                _descriptions = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// type number
        /// </summary>
        public int ID
        {
            get { return _ID; }
        }

        private int currentVersion = -1;

        /// <summary>
        /// returns all the machines of this type
        /// </summary>
        public List<Machine> machinesOfThisType
        {
            get 
            {
                if (currentVersion < Machines.machineVersion)
                {
                    _machinesOfThisType.Clear();
                    foreach( var machine in Machines.allMechines)
                    {
                        if (machine.type == _ID)
                        {
                            _machinesOfThisType.Add(machine);
                        }
                    }
                    currentVersion = Machines.machineVersion;
                }
                return _machinesOfThisType;
            }
        }

        /// <summary>
        /// Save the changes made to the MachineType to the Database
        /// </summary>
        public void saveChanges()
        {
            MachineTypeQuery.setMachineTypeParametersByID(this._ID,
                this._title,
                this._internalExternal,
                this._stationMachine,
                this._limitUnlimit,
                this._descriptions);
            MachineTypes.sortMachineTypesByName();
            _hasChanged = false;
        }

        /// <summary>
        /// load the MachineType from the database again to revert changes
        /// </summary>
        public void loadAgain()
        {
            var mt = MachineTypeQuery.getMachineTypeParametersByID(_ID);
            this._title = mt.typtit;
            this._internalExternal = mt.restyp1 == 2 ? true : false;
            this._stationMachine = mt.restyp2 == 2 ? true : false;
            this._limitUnlimit = mt.rescap == 1 ? true : false;
            this._descriptions = mt.typetc;
            _hasChanged = false;
        }

        /// <summary>
        /// adds the new machine type to the database
        /// </summary>
        public void addMachineType()
        {
            this._ID = MachineTypeQuery.addNewMachineTypeWithParameters(
            this._title,
            this._internalExternal,
            this._stationMachine,
            this._limitUnlimit,
            this._descriptions);
            MachineTypes.addMachineTypeToList(this);
            _hasChanged = false;
        }

        /// <summary>
        /// deletes this machine type from database
        /// </summary>
        public void deleteThisMachineType()
        {
            if (this.machinesOfThisType.Count == 0)
            {
                MachineTypeQuery.deleteMachineTypeByID(_ID);
                MachineTypes.deleteFromListById(_ID);
            }
        }

        /// <summary>
        /// creates a new machine from the one in the database
        /// </summary>
        /// <param name="newID">typnum</param>
        /// <param name="newTitle">typtit</param>
        /// <param name="newInternalExternal">restyp1</param>
        /// <param name="newStationMachine">restyp2</param>
        /// <param name="newLimitUnlimit">rescap</param>
        /// <param name="newDescription">typetc</param>
        public MachineType(int newID,
            string newTitle,
            bool newInternalExternal,
            bool newStationMachine,
            bool newLimitUnlimit,
            string newDescription)
        {
            this._ID = newID;
            this._title = newTitle;
            this._internalExternal = newInternalExternal;
            this._stationMachine = newStationMachine;
            this._limitUnlimit = newLimitUnlimit;
            this._descriptions = newDescription;
        }

        /// <summary>
        /// creates a new machine by the user and adds it to the database
        /// </summary>
        /// <param name="newTitle">typtit</param>
        /// <param name="newInternalExternal">restyp1</param>
        /// <param name="newStationMachine">restyp2</param>
        /// <param name="newLimitUnlimit">rescap</param>
        /// <param name="newDescription">typetc</param>
        public MachineType(
            string newTitle,
            bool newInternalExternal,
            bool newStationMachine,
            bool newLimitUnlimit,
            string newDescription)
        {
            this._title = newTitle;
            this._internalExternal = newInternalExternal;
            this._stationMachine = newStationMachine;
            this._limitUnlimit = newLimitUnlimit;
            this._descriptions = newDescription;
        }
    }
}
