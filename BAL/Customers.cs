using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;
using IMPOS.DQL;

namespace IMPOS.BLL
{
    public static class Customers
    {
        /// <summary>
        /// the version of the customer
        /// </summary>
        private static int _customerVersion = 0;

        /// <summary>
        /// the version of the customer
        /// </summary>
        public static int customerVersion
        {
            get
            {
                return _customerVersion;
            }
            set
            {
                _customerVersion = value;
            }
        }

        /// <summary>
        /// the list of customers
        /// </summary>
        static List<Customer> _allCustomers;

        /// <summary>
        /// the list of customers
        /// </summary>
        public static List<Customer> allCustomers
        {
            get
            {
                if (_allCustomers == null)
                {
                    loadCustomerInfoFromDB();
                }
                return _allCustomers;
            }
        }

        /// <summary>
        /// load all the customer from the database
        /// </summary>
        public static void loadCustomerInfoFromDB()
        {
            var allCustomersFromDB = CustomerQuery.getAllCustomers();
            _allCustomers = new List<Customer>();
            int i = 0;
            foreach (var customer in allCustomersFromDB)
            {
                Customer tempCustomer = new Customer(customer.cusnum,
                    customer.cuscod,
                    customer.cusnam,
                    customer.cusadd,
                    customer.connum,
                    customer.cusetc);
                _allCustomers.Add(tempCustomer);
            }
        }

        /// <summary>
        /// returns the customer info by its id
        /// </summary>
        /// <param name="id">the id of the customer</param>
        /// <returns></returns>
        public static Customer getCustomerByID(int id)
        {
            foreach (var c in allCustomers)
            {
                if (c.ID == id)
                    return c;
            }
            return null;
        }

        /// <summary>
        /// sorts the customer by their title
        /// </summary>
        public static void sortCustomersByName()
        {
            _allCustomers.Sort(new Comparison<Customer>((x, y) => string.Compare(x.title, y.title)));
            customerVersion++;
        }

        /// <summary>
        /// deletes the customer from the customer list
        /// </summary>
        /// <param name="ID">the id of the customer</param>
        public static void deleteFromListByID(int ID)
        {
            foreach (var s in _allCustomers)
            {
                if (s.ID == ID)
                {
                    _allCustomers.Remove(s);
                    break;
                }
            }
            _customerVersion++;
        }

        /// <summary>
        /// add the new customer to the list
        /// </summary>
        /// <param name="customer">the customer just added</param>
        public static void addCustomerToList(Customer customer)
        {
            for (int i = 0; i < _allCustomers.Count; i++)
                if (customer.title.CompareTo(_allCustomers[i].title) <= 0)
                {
                    _allCustomers.Insert(i, customer);
                    break;
                }
            _customerVersion++;
        }

        /// <summary>
        /// saves all the changes made to customer
        /// </summary>
        public static void saveAllCustomers()
        {
            foreach(var customer in _allCustomers)
                if (customer.hasChanged)
                {
                    customer.saveChanges();
                }
        }

        /// <summary>
        /// reset all the changes made to customer
        /// </summary>
        public static void cancelChanges()
        {
            foreach (var customer in _allCustomers)
                if (customer.hasChanged)
                {
                    customer.loadAgain();
                }
        }
    }

    public class Customer
    {
        /// <summary>
        /// the customer id(customer number)
        /// </summary>
        private int _ID;
        /// <summary>
        /// the customer code
        /// </summary>
        private string _code;
        /// <summary>
        /// the customer title
        /// </summary>
        private string _title;
        /// <summary>
        /// the customer address
        /// </summary>
        private string _address;
        /// <summary>
        /// the customer tel
        /// </summary>
        private string _tel;
        /// <summary>
        /// the customer description
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
        /// customer id(customer number)
        /// </summary>
        public int ID
        {
            get { return _ID; }
        }

        /// <summary>
        /// customer code
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
        /// customer title
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
        /// customer address
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
        /// customer tel
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
        /// customer description
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
        /// Save the changes made to the Customer to the database
        /// </summary>
        public void saveChanges()
        {
            CustomerQuery.setCustomerParametersByID(this._ID,
                this._code, 
                this._title, 
                this._address,
                this._tel,
                this._description);
            Customers.sortCustomersByName();
            _hasChanged = false;
        }

        public void addCustomer()
        {
            this._ID = CustomerQuery.addNewCustomerWithParameters(
                this._code, 
                this._title, 
                this._address,
                this._tel,
                this._description);
            Customers.addCustomerToList(this);
            _hasChanged = false;
        }

        /// <summary>
        /// load the Customer from the database again to revert changes
        /// </summary>
        public void loadAgain()
        {
            var s = CustomerQuery.getCustomerParametersByID(_ID);
            _ID = s.cusnum;
            _code = s.cuscod;
            _title = s.cusnam;
            _address = s.cusadd;
            _description = s.cusetc;
            _tel = s.connum;
            _hasChanged = false;
        }

        /// <summary>
        /// deletes this customer from database
        /// </summary>
        public void deleteThisCustomer()
        {
            Customers.deleteFromListByID(_ID);
            CustomerQuery.deleteCustomerByID(_ID);
        }

        /// <summary>
        /// creates a new customer form the one in the database
        /// </summary>
        /// <param name="newID">supnum</param>
        /// <param name="newCode">supcod</param>
        /// <param name="newTitle">suptit</param>
        /// <param name="newAddress">supadd</param>
        /// <param name="newTel">supcon</param>
        /// <param name="newDescription">supetc</param>
        public Customer(int newID,
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
        /// creates a new customer by the user and adds it to the database
        /// </summary>
        /// <param name="newID">cusnum</param>
        /// <param name="newCode">cuscod</param>
        /// <param name="newTitle">cusnam</param>
        /// <param name="newAddress">cusadd</param>
        /// <param name="newTel">connum</param>
        /// <param name="newDescription">cusetc</param>
        public Customer(string newCode,
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
