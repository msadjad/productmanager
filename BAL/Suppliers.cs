using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;
using IMPOS.DQL;

namespace IMPOS.BLL
{
    public static class Suppliers
    {
        /// <summary>
        /// the version of the supplier
        /// </summary>
        private static int _supplierVersion = 0;

        /// <summary>
        /// the version of the supplier
        /// </summary>
        public static int supplierVersion
        {
            get
            {
                return _supplierVersion;
            }
            set
            {
                _supplierVersion = value;
            }
        }

        /// <summary>
        /// the list of suppliers
        /// </summary>
        static List<Supplier> _allSuppliers;

        /// <summary>
        /// the list of suppliers
        /// </summary>
        public static List<Supplier> allSuppliers
        {
            get
            {
                if (_allSuppliers == null)
                {
                    loadSupplierInfoFromDB();
                }
                return _allSuppliers;
            }
        }

        /// <summary>
        /// load all the supplier from the database
        /// </summary>
        public static void loadSupplierInfoFromDB()
        {
            var allSuppliersFromDB = SupplierQuery.getAllSuppliers();
            _allSuppliers = new List<Supplier>();
            int i = 0;
            foreach (var supplier in allSuppliersFromDB)
            {
                Supplier tempSupplier = new Supplier(supplier.supnum,
                    supplier.supcod,
                    supplier.suptit,
                    supplier.supadd,
                    supplier.supcon,
                    supplier.supetc);
                _allSuppliers.Add(tempSupplier);
            }
        }

        /// <summary>
        /// sorts the supplier by their title
        /// </summary>
        public static void sortSuppliersByName()
        {
            _allSuppliers.Sort(new Comparison<Supplier>((x, y) => string.Compare(x.title, y.title)));
            supplierVersion++;
        }

        /// <summary>
        /// deletes the supplier from the supplier list
        /// </summary>
        /// <param name="ID">the id of the supplier</param>
        public static void deleteFromListByID(int ID)
        {
            foreach (var s in _allSuppliers)
            {
                if (s.ID == ID)
                {
                    _allSuppliers.Remove(s);
                    break;
                }
            }
            _supplierVersion++;
        }

        /// <summary>
        /// add the new supplier to the list
        /// </summary>
        /// <param name="machine">the supplier just added</param>
        public static void addSupplierToList(Supplier supplier)
        {
            for (int i = 0; i < _allSuppliers.Count; i++)
                if (supplier.title.CompareTo(_allSuppliers[i].title) <= 0)
                {
                    _allSuppliers.Insert(i, supplier);
                    break;
                }
            _supplierVersion++;
        }

        /// <summary>
        /// saves all the changes made to supplier
        /// </summary>
        public static void saveAllSuppliers()
        {
            foreach(var supplier in _allSuppliers)
                if (supplier.hasChanged)
                {
                    supplier.saveChanges();
                }
        }

        /// <summary>
        /// reset all the changes made to supplier
        /// </summary>
        public static void cancelChanges()
        {
            foreach (var supplier in _allSuppliers)
                if (supplier.hasChanged)
                {
                    supplier.loadAgain();
                }
        }
    }

    public class Supplier
    {
        /// <summary>
        /// the supplier id(supplier number)
        /// </summary>
        private int _ID;
        /// <summary>
        /// the supplier code
        /// </summary>
        private string _code;
        /// <summary>
        /// the supplier title
        /// </summary>
        private string _title;
        /// <summary>
        /// the supplier address
        /// </summary>
        private string _address;
        /// <summary>
        /// the supplier tel
        /// </summary>
        private string _tel;
        /// <summary>
        /// the supplier description
        /// </summary>
        private string _description;
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
        /// supplier id(supplier number)
        /// </summary>
        public int ID
        {
            get { return _ID; }
        }

        /// <summary>
        /// supplier code
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
        /// supplier title
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
        /// supplier address
        /// </summary>
        public string  address
        {
            get{ return _address; }
            set 
            {
                if (value != _address)
                {
                    _address = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// supplier tel
        /// </summary>
        public string tel
        {
            get { return _tel; }
            set 
            { 
                _tel = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// supplier description
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
        /// Save the changes made to the Supplier to the database
        /// </summary>
        public void saveChanges()
        {
            SupplierQuery.setSupplierParametersByID(this._ID,
                this._code, 
                this._title, 
                this._address,
                this._tel,
                this._description);
            Suppliers.sortSuppliersByName();
            _hasChanged = false;
        }

        public void addSupplier()
        {
            this._ID = SupplierQuery.addNewSupplierWithParameters(
                this._code, 
                this._title, 
                this._address,
                this._tel,
                this._description);
            Suppliers.addSupplierToList(this);
            _hasChanged = false;
        }

        /// <summary>
        /// load the Supplier from the database again to revert changes
        /// </summary>
        public void loadAgain()
        {
            var s = SupplierQuery.getSupplierParametersByID(_ID);
            _ID = s.supnum;
            _code = s.supcod;
            _title = s.suptit;
            _address = s.supadd;
            _description = s.supetc;
            _tel = s.supcon;
            _hasChanged = false;
        }

        /// <summary>
        /// deletes this supplier from database
        /// </summary>
        public void deleteThisSupplier()
        {
            Suppliers.deleteFromListByID(_ID);
            SupplierQuery.deleteSupplierByID(_ID);
        }

        /// <summary>
        /// creates a new supplier form the one in the database
        /// </summary>
        /// <param name="newID">supnum</param>
        /// <param name="newCode">supcod</param>
        /// <param name="newTitle">suptit</param>
        /// <param name="newAddress">supadd</param>
        /// <param name="newTel">supcon</param>
        /// <param name="newDescription">supetc</param>
        public Supplier(int newID,
            string newCode,
            string newTitle,
            string newAddress,
            string newTel,
            string newDescription)
        {
            this._ID = newID;
            this._code = newCode;
            this._title = newTitle;
            this._address = newAddress;
            this._tel = newTel;
            this._description = newDescription;
            _hasChanged = false;
        } 

        /// <summary>
        /// creates a new supplier by the user and adds it to the database
        /// </summary>
        /// <param name="newID">supnum</param>
        /// <param name="newCode">supcod</param>
        /// <param name="newTitle">suptit</param>
        /// <param name="newAddress">supadd</param>
        /// <param name="newTel">supcon</param>
        /// <param name="newDescription">supetc</param>
        public Supplier(string newCode,
            string newTitle,
            string newAddress,
            string newTel,
            string newDescription)
        {
            this._code = newCode;
            this._title = newTitle;
            this._address = newAddress;
            this._tel = newTel;
            this._description = newDescription;
            _hasChanged = false;
        }
    }
}
