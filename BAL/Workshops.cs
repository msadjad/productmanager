using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;
using IMPOS.DQL;

namespace IMPOS.BLL
{
    public static class Workshops
    {
        /// <summary>
        /// the list of workshops
        /// </summary>
        private static List<Workshop> _allWorkshops;

        /// <summary>
        /// the list of machine types
        /// </summary>
        public static List<Workshop> allWorkshops
        {
            get
            {
                if (_allWorkshops == null)
                {
                    loadWorkshopInfoFromDB();
                }
                return _allWorkshops;
            }
        }

        /// <summary>
        /// loads all Workshops from the database
        /// </summary>
        public static void loadWorkshopInfoFromDB()
        {
            var allWorkshopsFromDB = WorkshopsQuery.getAllWorkshops();
            _allWorkshops = new List<Workshop>();
            foreach (var workshop in allWorkshopsFromDB)
            {
                Workshop newWorkshop = new Workshop(workshop.wornum,workshop.wornam);
                _allWorkshops.Add(newWorkshop);
            }
        }

        /// <summary>
        /// sorts the workshops by their title
        /// </summary>
        public static void sortWorkshopsByName()
        {
            _allWorkshops.Sort(new Comparison<Workshop>((x, y) => string.Compare(x.title, y.title)));
        }

        /// <summary>
        /// deletes the workshop from the workshop list
        /// </summary>
        /// <param name="ID">the id of the workshop</param>
        public static void deleteFromListByID(int ID)
        {
            foreach (var m in _allWorkshops)
            {
                if (m.id == ID)
                {
                    _allWorkshops.Remove(m);
                    break;
                }
            }
        }

        /// <summary>
        /// add the new workshop to the list
        /// </summary>
        /// <param name="workshop">the workshop just added</param>
        public static void addWorkshopToList(Workshop workshop)
        { 
            for(int i=0 ; i<_allWorkshops.Count ; i++)
                if (workshop.title.CompareTo(_allWorkshops[i].title) <= 0)
                {
                    _allWorkshops.Insert(i, workshop);
                    break;
                }
        }
        
        /// <summary>
        /// saves all chanegs made to workshops
        /// </summary>
        public static void saveAllWorkshops()
        {
            foreach (var workshop in _allWorkshops)
                if (workshop.hasChanged)
                {
                    workshop.saveChanges();
                }
        }

        /// <summary>
        /// reset all chanegs made to workshop
        /// </summary>
        public static void cancelChanges()
        {
            foreach (var workshop in _allWorkshops)
                if (workshop.hasChanged)
                {
                    workshop.loadAgain();
                }
        }
    }

    public class Workshop
    {
        /// <summary>
        /// the workshop title
        /// </summary>
        private string _title;
        
        /// <summary>
        /// the workshop id
        /// </summary>
        private int _id;

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
        /// the machines in the workshop
        /// </summary>
        private List<Machine> _allMachinesInWorkshop = null;

        /// <summary>
        /// workshop title
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
        /// workshop id
        /// </summary>
        public int id
        {
            get { return _id; }
        }

        private int currentVersion = -1;
        /// <summary>
        /// returns all the machines in the workshop
        /// </summary>
        public List<Machine> allMachinesInWorkshop
        {
            get 
            {
                if (currentVersion < Machines.machineVersion )
                {
                    _allMachinesInWorkshop.Clear();
                    foreach (var machine in Machines.allMechines)
                    {
                        if (machine.workshop == id)
                            _allMachinesInWorkshop.Add(machine);
                    }
                    currentVersion = Machines.machineVersion;
                }
                return _allMachinesInWorkshop;
            }
        }

        /// <summary>
        /// Save the changes made to the Workshop to the Database
        /// </summary>
        public void saveChanges()
        {
            WorkshopsQuery.setWorkstationTitleByID(this._id,
                this._title);
            Workshops.sortWorkshopsByName();
            _hasChanged = false;
        }

        /// <summary>
        /// load the Workshop from the database again to revert changes
        /// </summary>
        public void loadAgain()
        {
            var workshop = WorkshopsQuery.getWorkshopParametersByID(_id);
            this._title = workshop.wornam;
            _hasChanged = false;
        }

        /// <summary>
        /// adds the new workshop to the database
        /// </summary>
        public void addWorkshop()
        {
            this._id = WorkshopsQuery.addNewWorkshopWithParameters(_title);
            Workshops.addWorkshopToList(this);
            _hasChanged = false;
        }

        /// <summary>
        /// deletes this workshop from database
        /// </summary>
        public void deleteThisWorkshop()
        {
            if (this.allMachinesInWorkshop.Count == 0)
            {
                WorkshopsQuery.deleteWorkshopByID(_id);
                Workshops.deleteFromListByID(_id);
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
        public Workshop(int newID,
            string newTitle)
        {
            this._id = newID;
            this._title = newTitle;
            this._allMachinesInWorkshop = new List<Machine>();
            _hasChanged = false;
        }

        /// <summary>
        /// creates a new machine by the user and adds it to the database
        /// </summary>
        /// <param name="newTitle">typtit</param>
        /// <param name="newInternalExternal">restyp1</param>
        /// <param name="newStationMachine">restyp2</param>
        /// <param name="newLimitUnlimit">rescap</param>
        /// <param name="newDescription">typetc</param>
        public Workshop(string newTitle)
        {
            this._title = newTitle;
            this._allMachinesInWorkshop = new List<Machine>();
        }
    }
}
