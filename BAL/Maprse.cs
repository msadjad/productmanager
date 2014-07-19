using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DQL;
using IMPOS.DAL;

namespace IMPOS.BLL
{
    public static class Maprses
    {
        /// <summary>
        /// the version of the maprse
        /// </summary>
        private static int _maprseVersion = 0;

        /// <summary>
        /// the version of the maprse
        /// </summary>
        public static int maprseVersion
        {
            get
            {
                return _maprseVersion;
            }
            set
            {
                _maprseVersion = value;
            }
        }

        /// <summary>
        /// the list of maprses
        /// </summary>
        static List<Maprse> _allMaprses;

        /// <summary>
        /// the list of maprses
        /// </summary>
        public static List<Maprse> allMaprses
        {
            get
            {
                if (_allMaprses == null)
                {
                    loadMaprseInfoFromDB();
                }
                return _allMaprses;
            }
        }

        /// <summary>
        /// load all the maprse type from the database
        /// </summary>
        public static void loadMaprseInfoFromDB()
        {
            var allMaprsesFromDB = MaprseQuery.getAllMaprses();
            _allMaprses = new List<Maprse>();
            int i = 0;
            foreach (var maprse in allMaprsesFromDB)
            {
                Maprse tempMaprse = new Maprse(
                    maprse.mpsnum,
                    maprse.mpscod,
                    (int)maprse.itenum,
                    (int)maprse.mpsqua,
                    (DateTime)maprse.mpsdue,
                    (int)maprse.mpspri,
                    (int)maprse.mpscom,
                    (int)maprse.cusnum,
                    (int)maprse.mpssta);
                _allMaprses.Add(tempMaprse);
            }
        }

        /// <summary>
        /// deletes the maprse from the maprse list
        /// </summary>
        /// <param name="ID">the id of the maprse</param>
        public static void deleteFromListByID(int ID)
        {
            foreach (var m in _allMaprses)
            {
                if (m.ID == ID)
                {
                    _allMaprses.Remove(m);
                    break;
                }
            }
            _maprseVersion++;
        }

        /// <summary>
        /// add the new maprse to the list
        /// </summary>
        /// <param name="maprse">the maprse just added</param>
        public static void addMaprseToList(Maprse maprse)
        {
            _allMaprses.Add(maprse);
            _maprseVersion++;
        }

        /// <summary>
        /// saves all the changes made to maprses
        /// </summary>
        public static void saveAllMaprses()
        {
            foreach (var maprse in _allMaprses)
                if (maprse.hasChanged)
                {
                    maprse.saveChanges();
                }
        }

        /// <summary>
        /// reset all the changes made to maprses
        /// </summary>
        public static void cancelChanges()
        {
            foreach (var maprse in _allMaprses)
                if (maprse.hasChanged)
                {
                    maprse.loadAgain();
                }
        }
    }

    public class Maprse
    {
        /// <summary>
        /// the maprse id(maprse number) mpsnum
        /// </summary>
        private int _ID;
        /// <summary>
        /// the maprse code mpscod
        /// </summary>
        private string _code;
        /// <summary>
        /// the item mpsitm
        /// </summary>
        private Item _item;
        /// <summary>
        /// the amount of the item in the order mpsqua
        /// </summary>
        private int _quantity;
        /// <summary>
        /// the due date of the order mpsdue
        /// </summary>
        private DateTime _dueDate;
        /// <summary>
        /// the priority of the order mpspri
        /// </summary>
        private int _priority;
        /// <summary>
        /// the completeness of the amount mpscom
        /// </summary>
        private int _completeAmount;
        /// <summary>
        /// the customer that has orderd mpscus
        /// </summary>
        private Customer _customer;
        /// <summary>
        /// the maprse status mpssta
        /// </summary>
        private int _status;

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
        /// maprse id(maprse number) mpsnum
        /// </summary>
        public int ID
        {
            get { return _ID; }
        }
        /// <summary>
        /// maprse code mpscod
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
        /// the item mpsitm
        /// </summary>
        public Item item
        {
            get { return _item; }
            set
            {
                _item = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// the maprse status mpssta
        /// </summary>
        public int status
        {
            get { return _status; }
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
        /// the customer that has orderd mpscus
        /// </summary>
        public Customer customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// the completeness of the amount mpscom
        /// </summary>
        public int completeAmount
        {
            get { return _completeAmount; }
            set
            {
                _completeAmount = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// the priority of the order mpspri
        /// </summary>
        public int priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// the due date of the order mpsdue
        /// </summary>
        public DateTime dueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// the amount of the item in the order mpsqua
        /// </summary>
        public int quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// Save the changes made to the Maprse to the database
        /// </summary>
        public void saveChanges()
        {
            MaprseQuery.setMaprseParametersByID(this._ID,
                _code,
                _item.id,
                _quantity,
                _dueDate,
                _priority,
                _completeAmount,
                _customer.ID,
                _status);
            _hasChanged = false;
        }

        public void addMaprse()
        {
            this._ID = MaprseQuery.addNewMaprseWithParameters(
                _code,
                _item.id,
                _quantity,
                _dueDate,
                _priority,
                _completeAmount,
                _customer.ID,
                _status);
            Maprses.addMaprseToList(this);
            _hasChanged = false;
        }

        /// <summary>
        /// load the Maprse from the database again to revert changes
        /// </summary>
        public void loadAgain()
        {
            var m = MaprseQuery.getMaprseParametersByID(_ID);
            code = m.mpscod;
            item = Items.getItemById(m.itenum); 
            quantity = (int)m.mpsqua;
            dueDate = (DateTime)m.mpsdue;
            priority = (int)m.mpspri;
            completeAmount = (int)m.mpscom;
            customer = Customers.getCustomerByID((int)m.cusnum);
            status = (int)m.mpssta;
            _hasChanged = false;
        }

        /// <summary>
        /// deletes this maprse from database
        /// </summary>
        public void deleteThisMaprse()
        {
            Maprses.deleteFromListByID(_ID);
            MaprseQuery.deleteMaprseByID(_ID);
        }

        /// <summary>
        /// creates a new maprse form the one in the database
        /// </summary>
        /// <param name="id">mpsnum</param>
        /// <param name="newCode">mpscod</param>
        /// <param name="newItem">mpsitm</param>
        /// <param name="newQuantity">mpsqua</param>
        /// <param name="newDueDate">mpsdue</param>
        /// <param name="newPriority">mpspre</param>
        /// <param name="newCompleteAmount">mpscom</param>
        /// <param name="newCustomer">mpscus</param>
        /// <param name="newStatus">mpssta</param>  

        public Maprse(int newID,
            string newCode,
            int newItem,
            int newQuantity,
            DateTime newDueDate,
            int newPriority,
            int newCompleteAmount,
            int newCustomer,
            int newStatus)
        {
            _ID = newID;
            _code = newCode;
            _item = Items.getItemById(newItem);
            _quantity = newQuantity;
            _dueDate = newDueDate;
            _priority = newPriority;
            _completeAmount = newCompleteAmount;
            _customer = Customers.getCustomerByID(newCustomer);
            _status = newStatus;
            _hasChanged = false;
        }

        /// <summary>
        /// creates a new maprse by the user and adds it to the database
        /// </summary>
        /// <param name="newCode">mpscod</param>
        /// <param name="newItem">mpsitm</param>
        /// <param name="newQuantity">mpsqua</param>
        /// <param name="newDueDate">mpsdue</param>
        /// <param name="newPriority">mpspre</param>
        /// <param name="newCompleteAmount">mpscom</param>
        /// <param name="newCustomer">mpscus</param>
        /// <param name="newStatus">mpssta</param>  

        public Maprse(
            string newCode,
            int newItem,
            int newQuantity,
            DateTime newDueDate,
            int newPriority,
            int newCompleteAmount,
            int newCustomer,
            int newStatus)
        {
            _code = newCode;
            _item = Items.getItemById(newItem);
            _quantity = newQuantity;
            _dueDate = newDueDate;
            _priority = newPriority;
            _completeAmount = newCompleteAmount;
            _customer = Customers.getCustomerByID(newCustomer);
            _status = newStatus;
            _hasChanged = false;
        }
    }
}

