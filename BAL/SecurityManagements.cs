using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BLL.Annotations;
using IMPOS.DAL;
using IMPOS.DQL;

namespace IMPOS.BLL
{
    public class SubSystem
    {
        /// <summary>
        /// the id of the subsystem
        /// </summary>
        private int _id;
        /// <summary>
        /// the title of the subsystem
        /// </summary>
        private string _title;
        /// <summary>
        /// the sort id of the subsystem
        /// </summary>
        private int _sortID;
        /// <summary>
        /// the id of the sub system
        /// </summary>
        public int id
        {
            get { return _id; }
        }
        /// <summary>
        /// the title of the sub system
        /// </summary>
        public string title
        {
            get { return _title; }
        }
        /// <summary>
        /// the sort id of the sub system
        /// </summary>
        public int sortID
        {
            get { return _sortID; }
        }
        /// <summary>
        /// creates a new Sub System
        /// </summary>
        /// <param name="newId">the id of the sub system</param>
        /// <param name="newTitle">the title of the sub system</param>
        /// <param name="newSortID">the sort id of the sort id</param>
        public SubSystem(int newId, string newTitle, int newSortID)
        {
            _id = newId;
            _title = newTitle;
            _sortID = newSortID;
        }
    }

    
    public class SubSystemStatus
    {
        /// <summary>
        /// if has changed is true it means that the variables have changed.
        /// </summary>
        public bool _hasChanged = false;
        /// <summary>
        /// the ID of the organization or staff id
        /// </summary>
        private int _organizationOrStaffID;
        /// <summary>
        /// The sub system information
        /// </summary>
        private SubSystem _subSystem;
        /// <summary>
        /// the status of the sub system
        /// </summary>
        private bool _status;
        /// <summary>
        /// organization=0 or staff=1
        /// </summary>
        private bool _organizationOrStaff;
        /// <summary>
        /// the id of the organization or staff id
        /// </summary>
        public int organizationOrStaffID
        {
            get
            {
                return _organizationOrStaffID;
            }
        }
        /// <summary>
        /// the subsystem information
        /// </summary>
        public SubSystem subSystem
        {
            get
            {
                return _subSystem;
            }
        }
        /// <summary>
        /// the status of the subsystem for the organization
        /// </summary>
        public bool status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// shows that the information here is for an organization or an staff
        /// organization = false and staff = true
        /// </summary>
        public bool organizationOrStaff
        {
            get 
            { 
                return _organizationOrStaff; 
            }
        }
        /// <summary>
        /// loads the data again from the database
        /// </summary>
        public void loadAgain()
        {
            if(organizationOrStaff == false)
                _status = SecurityManagementQueries.loadTheSubSystemForOrganizationStatusAgain(organizationOrStaffID, subSystem.id, status);
            else if(organizationOrStaff == true)
                _status = SecurityManagementQueries.loadTheSubSystemForStaffStatusAgain(organizationOrStaffID, subSystem.id, status);
            _hasChanged = false;
        }
        /// <summary>
        /// saves changes to the database
        /// </summary>
        public void saveChanges()
        {
            if(_hasChanged == true)
            {
                if(organizationOrStaff == false)
                    SecurityManagementQueries.changeTheStatusOfTheSubSystemForOrganization(organizationOrStaffID, subSystem.id, status); 
                else if(organizationOrStaff == true)
                    SecurityManagementQueries.changeTheStatusOfTheSubsystemForStaff(organizationOrStaffID, subSystem.id, status); 
                _hasChanged = false;
            }
        }

        /// <summary>
        /// the constructor of the class, creates aa new subsystem status
        /// </summary>
        /// <param name="newOrganizationID">the organization id</param>
        /// <param name="newSubSystem">the subsystem</param>
        /// <param name="newStatus">the status of the subsystem</param>
        /// <param name="newOrganizationOrStaff">is it organization=false or staff=true</param>
        public SubSystemStatus(int newOrganizationOrStaffID,SubSystem newSubSystem, bool newStatus, bool newOrganizationOrStaff)
        {
            _organizationOrStaffID = newOrganizationOrStaffID;
            _subSystem = newSubSystem;
            _status = newStatus;
            _organizationOrStaff = newOrganizationOrStaff;
        }
    }

    public class ItemStatus:INotifyPropertyChanged
    {
        /// <summary>
        /// if the status has changed=true or not=false
        /// </summary>
        public bool _hasChanged = false;
        /// <summary>
        /// the ID of the organization
        /// </summary>
        private int _organizationID;
        /// <summary>
        /// The item information
        /// </summary>
        private Item _item;
        /// <summary>
        /// the status of the item
        /// 0 -> not accessable
        /// 1 -> not editable
        /// 2 -> editable
        /// </summary>
        private int _status;
        /// <summary>
        /// is the item made=false or purchased=true
        /// </summary>
        private bool _madeOrPurchaced;

        private bool _statusZero;
        private bool _statusOne;
        private bool _statusTwo;

        /// <summary>
        /// the id of the organization
        /// </summary>
        public int organizationID
        {
            get
            {
                return _organizationID;
            }
        }
        /// <summary>
        /// the item information
        /// </summary>
        public Item item
        {
            get
            {
                return _item;
            }
        }
        /// <summary>
        /// the status of the item for the organization
        /// 0 -> not accessable
        /// 1 -> not editable
        /// 2 -> editable
        /// </summary>
        public int status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                _hasChanged = true;
                OnPropertyChanged("status");
            }
        }

        //public bool statusZero
        //{
        //    set
        //    {
        //        _statusZero = value;
        //        if (value)
        //        {
        //            status = 0;
        //            statusOne = false;
        //            statusTwo = false;
        //            OnPropertyChanged("statusZero");
        //            OnPropertyChanged("statusOne");
        //            OnPropertyChanged("statusTwo");
        //        }
        //    }
        //    get { return _statusZero; }
        //}

        //public bool statusOne
        //{
        //    set
        //    {
        //        _statusOne = value;

        //        if (value)
        //        {
        //            status = 1;
        //            statusZero = false;
        //            statusTwo = false;
        //            OnPropertyChanged("statusZero");
        //            OnPropertyChanged("statusOne");
        //            OnPropertyChanged("statusTwo");
        //        }
        //    }
        //    get { return _statusOne; }
        //}

        //public bool statusTwo
        //{
        //    set
        //    {
        //        _statusTwo = value;

        //        if (value)
        //        {
        //            status = 2;
        //            statusZero = false;
        //            statusOne = false;
        //            OnPropertyChanged("statusZero");
        //            OnPropertyChanged("statusOne");
        //            OnPropertyChanged("statusTwo");
        //        }
        //    }
        //    get { return _statusTwo; }
        //}

        /// <summary>
        /// saves the changes made to item status to the database
        /// </summary>
        public void saveChanges()
        {
            if(_hasChanged)
            {
                SecurityManagementQueries.changeTheStatusOfTheItem(organizationID, _item.id, status, _madeOrPurchaced);
                _hasChanged = false;
            }
        }
        /// <summary>
        /// loads the item status from the database again
        /// </summary>
        public void loadAgain()
        {
            if(_hasChanged)
            {
                _status = SecurityManagementQueries.loadTheItemForOrganizationStatusAgain(organizationID, _item.id, status, _madeOrPurchaced);
                _hasChanged = false;
            }
        }

        /// <summary>
        /// the constructor of the class, creates aa new item status
        /// </summary>
        /// <param name="newOrganizationID">the id of the organization</param>
        /// <param name="newItem">the item</param>
        /// <param name="newStatus">the status of the item</param>
        /// <param name="newMadeOrPurchased">if the item is made=0 or the item is purchased=1</param>
        public ItemStatus(int newOrganizationID, Item newItem, int newStatus, bool newMadeOrPurchased)
        {
            _organizationID = newOrganizationID;
            _item = newItem;
            _status = newStatus;
            _madeOrPurchaced = newMadeOrPurchased;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ResourceTypeStatus
    {
        public bool _hasChanged = false;
        /// <summary>
        /// the id of the organization
        /// </summary>
        private int _organizationID;
        /// <summary>
        /// the machine type
        /// </summary>
        private MachineType _machineType;
        /// <summary>
        /// the status of the accessiblity
        /// </summary>
        private bool _status;

        /// <summary>
        /// the id of the organizaiton
        /// </summary>
        public int organizationID
        {
            get
            {
                return _organizationID;
            }
        }
        /// <summary>
        /// the machine type
        /// </summary>
        public MachineType machineType
        {
            get { return _machineType; }
        }
        /// <summary>
        /// the status of the accessiblity
        /// </summary>
        public bool status
        {
            get 
            { 
                return _status; 
            }
            set 
            { 
                _status = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// saves changes made to resourse type status to the database
        /// </summary>
        public void saveChanges()
        {
            if(_hasChanged)
            {
                SecurityManagementQueries.changeTheStatusOfTheResourceType(_organizationID, _machineType.ID, _status); 
                _hasChanged = false;
            }
        }
        /// <summary>
        /// loads the information of the resource type from the databse
        /// </summary>
        public void loadAgain()
        {
            if(_hasChanged)
            {
                _status = SecurityManagementQueries.loadTheStatusOfTheResourceTypeAgain(_organizationID, _machineType.ID, _status); 
                _hasChanged = false;
            }
        }
        /// <summary>
        /// creates a new resource type
        /// </summary>
        /// <param name="newOrganizationID">the ID of the organization</param>
        /// <param name="newMachineType">the type of the machine</param>
        /// <param name="newStatus">the </param>
        public ResourceTypeStatus(int newOrganizationID, MachineType newMachineType, bool newStatus)
        {
            _organizationID = newOrganizationID;
            _machineType = newMachineType;
            _status = newStatus;
        }
    }

    public class Staff
    {
        /// <summary>
        /// the code of the sub section
        /// </summary>
        private int _subSectionCode;
        /// <summary>
        /// the id if the staff
        /// </summary>
        private int _id;
        /// <summary>
        /// the code of the staff
        /// </summary>
        private string _code;
        /// <summary>
        /// the name of the staff
        /// </summary>
        private string _name;
        /// <summary>
        /// the address of the staff
        /// </summary>
        private string _address;
        /// <summary>
        /// the tel of the staff
        /// </summary>
        private string _tel;
        /// <summary>
        /// the username of the staff
        /// </summary>
        private string _username;
        /// <summary>
        /// the password of the staff
        /// </summary>
        private string _password;
        /// <summary>
        /// the job of the staff
        /// </summary>
        private string _job;
        /// <summary>
        /// the description of the staff
        /// </summary>
        private string _description;
        /// <summary>
        /// the test planning permission of the staff
        /// </summary>
        private bool _testPlanningPermission;
        /// <summary>
        /// the test planning DB name of the staff
        /// </summary>
        private string _testDBName;
        /// <summary>
        /// the list of all Subsystem statuses
        /// </summary>
        private List<SubSystemStatus> _allSubSystemStatuses;
        /// <summary>
        /// has the info changed or not
        /// </summary>
        public bool _hasChanged;
        /// <summary>
        /// the sub section code
        /// </summary>
        public int subSectionCode
        {
            set { _subSectionCode = value; }
            get { return _subSectionCode; }
        }
        /// <summary>
        /// the id if the staff
        /// </summary>
        public int id
        {
            set { _id=value; }
            get { return _id; }
        }
        /// <summary>
        /// the code of the staff
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
        /// the name of the staff
        /// </summary>
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// the address of the staff
        /// </summary>
        public string address
        {
            get { return _address; }
            set
            {
                _address = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// the tel of the staff
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
        /// the username of the staff
        /// </summary>
        public string username
        {
            get { return _username; }
            set
            {
                _username = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// the password of the staff
        /// </summary>
        public string password
        {
            get { return _password; }
            set
            {
                _password = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// the job of the staff
        /// </summary>
        public string job
        {
            get { return _job; }
            set
            {
                _job = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// the description of the staff
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
        /// the test planning permission of the staff
        /// </summary>
        public bool testPlanningPermission
        {
            get { return _testPlanningPermission; }
            set
            {
                _testPlanningPermission = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// the test planning DB name of the staff
        /// </summary>
        public string testDBName
        {
            get { return _testDBName; }
            set
            {
                _testDBName = value;
                _hasChanged = true;
            }
        }
        /// <summary>
        /// the list of all Subsystem statuses
        /// </summary>
        public List<SubSystemStatus> allSubSystemStatuses
        {
            set { _allSubSystemStatuses=value; }
            get
            {
                if (_allSubSystemStatuses == null)
                {
                    _allSubSystemStatuses = new List<SubSystemStatus>();
                    var allSubSystemsStatus = SecurityManagementQueries.getAllStaffSubSystemRelation(_id);
                    foreach (var subSystem in SecurityManagements.allSubSystems)
                    {
                        bool isFound = false;
                        for(int i=0 ; i<allSubSystemsStatus.Count() ; i++)
                        {
                            if(subSystem.id == allSubSystemsStatus[i].subnum)
                            {
                                isFound = true;
                                break;
                            }
                        }
                        if(isFound)
                        {
                            _allSubSystemStatuses.Add(new SubSystemStatus(_id,subSystem,true,true));
                        }
                        else
                        {
                            _allSubSystemStatuses.Add(new SubSystemStatus(_id,subSystem,false,true));
                        }
                    }
                }
                return _allSubSystemStatuses;
            }
        }

        /// <summary>
        /// sets the list of the sub system statuses to allSubSystemStatuses
        /// </summary>
        /// <param name="newList">the list of allSubSystemStatuses</param>
        public void setAllSubSystemStatuses(List<SubSystemStatus> newList)
        {
            _allSubSystemStatuses = newList;
        }
        
        /// <summary>
        /// saves the changes made to staff
        /// </summary>
        public void saveChanges()
        {
            if(_allSubSystemStatuses==null)
                return;
            for(int i=0 ; i<_allSubSystemStatuses.Count() ; i++)
            {
                _allSubSystemStatuses[i].saveChanges();
            }
            if(_hasChanged == true)
            {
                SecurityManagementQueries.changeTheStaffInfo(_id,_code,_name,_address
                    ,_tel,_username,_password,_job,_description,_testPlanningPermission
                    ,_testDBName);
                _hasChanged = false;
            }
        }

        /// <summary>
        /// loads the information of the staff from the database again to revert changes
        /// </summary>
        public void loadAgain()
        {
            for(int i=0 ; i<_allSubSystemStatuses.Count() ; i++)
            {
                _allSubSystemStatuses[i].loadAgain();
            }
            if(_hasChanged == true)
            {
                var staff = SecurityManagementQueries.getStaffInfoByID(_id);
                _id = staff.stanum;
                _name = staff.stanam;
                _address = staff.staadd;
                _tel = staff.statel;
                _username = staff.staid;
                _password = staff.stapas;
                _job = staff.stajob;
                _description = staff.staetc;
                _testPlanningPermission = staff.plarig==0?false:true;
                _hasChanged = false;
            }
            
            _allSubSystemStatuses = new List<SubSystemStatus>();
            var allSubSystemsStatus = SecurityManagementQueries.getAllStaffSubSystemRelation(_id);
            foreach (var subSystem in SecurityManagements.allSubSystems)
            {
                bool isFound = false;
                for(int i=0 ; i<allSubSystemsStatus.Count() ; i++)
                {
                    if(subSystem.id == allSubSystemsStatus[i].subnum)
                    {
                        isFound = true;
                        break;
                    }
                }
                if(isFound)
                {
                    _allSubSystemStatuses.Add(new SubSystemStatus(_id,subSystem,true,true));
                }
                else
                {
                    _allSubSystemStatuses.Add(new SubSystemStatus(_id,subSystem,false,true));
                }
            }
        }

        /// <summary>
        /// adds the newly added staff to the database
        /// </summary>
        public void addStaff(int orgId)
        {
            //_subSectionCode = orgId;
            _id = SecurityManagementQueries.createANewStaff(code,orgId,name,address,tel,username,password,job,description,testPlanningPermission);
            SecurityManagements.addStaffToSectionList(_subSectionCode,this);
            _hasChanged = false;
        }

        /// <summary>
        /// deletes this staff from the database
        /// </summary>
        public void deleteStaff()
        {
            SecurityManagementQueries.deleteStaff(_id);
            SecurityManagements.deleteStaffFromSectionList(_subSectionCode,this);
        }

        /// <summary>
        /// creates a new Staff from an object from the database
        /// </summary>
        /// <param name="newId">the id of the staff</param>
        /// <param name="newName">the name of the staff</param>
        /// <param name="newAddress">the address of the staff</param>
        /// <param name="newTel">the tel of the staff</param>
        /// <param name="newUsername">the name of the staff</param>
        /// <param name="newPassword">the password of the staff</param>
        /// <param name="newJob">the job of the staff</param>
        /// <param name="newDescription">the descriptio of the staff</param>
        /// <param name="newTestPlanningPermission">the testplanning of the staff</param>
        public Staff(int    newId,string code,
                     string newName,
                     string newAddress,
                     string newTel,
                     string newUsername,
                     string newPassword,
                     string newJob,
                     string newDescription,
                     bool   newTestPlanningPermission)
        {
            _code = code;
            _id = newId;
            _name = newName;
            _address = newAddress;
            _tel = newTel;
            _username = newUsername;
            _password = newPassword;
            _job = newJob;
            _description = newDescription;
            testPlanningPermission = newTestPlanningPermission;
        }

        
        /// <summary>
        /// creates a new Staff from a new object
        /// </summary>
        /// <param name="newName">the name of the staff</param>
        /// <param name="newAddress">the address of the staff</param>
        /// <param name="newTel">the tel of the staff</param>
        /// <param name="newUsername">the name of the staff</param>
        /// <param name="newPassword">the password of the staff</param>
        /// <param name="newJob">the job of the staff</param>
        /// <param name="newDescription">the descriptio of the staff</param>
        /// <param name="newTestPlanningPermission">the testplanning of the staff</param>
        public Staff(string newName,string code,
                     string newAddress,
                     string newTel,
                     string newUsername,
                     string newPassword,
                     string newJob,
                     string newDescription,
                     bool   newTestPlanningPermission)
        {
            _name = newName;
            _code = code;
            _address = newAddress;
            _tel = newTel;
            _username = newUsername;
            _password = newPassword;
            _job = newJob;
            _description = newDescription;
            testPlanningPermission = newTestPlanningPermission;
        }
        public Staff(int organizationId)
        {
            addStaff(organizationId);
            
        }
    }

    public class Organization
    {
        /// <summary>
        /// the id if the oragnization
        /// </summary>
        private int _id;
        /// <summary>
        /// the name of the organization
        /// </summary>
        private string _title;
        /// <summary>
        /// the list of all Subsystem statuses
        /// </summary>
        private List<SubSystemStatus> _allSubSystemStatuses;
        /// <summary>
        /// the list of all Made Product statuses
        /// </summary>
        private List<ItemStatus> _allMadeProductStatuses;
        /// <summary>
        /// the list of all Purchased Product statuses
        /// </summary>
        private List<ItemStatus> _allPurchaseProductStatuses;
        /// <summary>
        /// the list of all resource type statuses
        /// </summary>
        private List<ResourceTypeStatus> _allResourceTypeStatuses;
        /// <summary>
        /// the list of all staff in the organization
        /// </summary>
        private List<Staff> _allStaff;
        /// <summary>
        /// has the info changed or not
        /// </summary>
        private bool _hasChanged;
        /// <summary>
        /// the id of the organization
        /// </summary>
        public int id
        {
            get{ return _id; }
        }
        /// <summary>
        /// the title of the organization
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
        /// the list of all Subsystem statuses
        /// </summary>
        public List<SubSystemStatus> allSubSystemStatuses
        {
            set
            {
                _allSubSystemStatuses = value;
                _hasChanged = true;
            }
            get
            {
                if (_allSubSystemStatuses == null)
                {
                    loadAllSubSystemStatuses();
                }
                return _allSubSystemStatuses;
            }
        }

        /// <summary>
        /// the list of all machine type statuses
        /// </summary>
        public List<ResourceTypeStatus> allResourceTypeStatuses
        {
            set
            {
                _allResourceTypeStatuses = value;
                _hasChanged = true;
            }
            get
            {
                if (_allResourceTypeStatuses == null)
                {
                    loadAllResourceTypeStatuses();
                }
                return _allResourceTypeStatuses;
            }
        }
        /// <summary>
        /// the list of all the staff
        /// </summary>
        public List<Staff> allStaff
        {
            set
            {
                _allStaff = value;
                _hasChanged = true;
            }
            get
            {
                if(_allStaff == null)
                {
                    loadAllTheStaff();
                }
                return _allStaff;
            }
        }
        /// <summary>
        /// the list of all made products statuses
        /// </summary>
        public List<ItemStatus> allMadeProductStatuses
        {
            set
            {
                _allMadeProductStatuses = value;
                _hasChanged = true;
            }
            get
            {
                if (_allMadeProductStatuses == null)
                {
                    loadAllMadeProductStatuses();
                }
                return _allMadeProductStatuses;
            }
        }
        /// <summary>
        /// the list of all purchased products statuses
        /// </summary>
        public List<ItemStatus> allPurchasedProductStatuses
        {
            set 
            {
                _allPurchaseProductStatuses = value;
                _hasChanged = true;
            }
            get
            {
                if (_allPurchaseProductStatuses == null)
                {
                    loadAllPurchasedProductStatuses();
                }
                return _allPurchaseProductStatuses;
            }
        }
        /// <summary>
        /// loads all the sub system statuses from the database
        /// </summary>
        private void loadAllSubSystemStatuses()
        {
            _allSubSystemStatuses = new List<SubSystemStatus>();
            var allSubSystemsStatusHere = SecurityManagementQueries.getAllOrganizationSubSystemRelation(_id);
            foreach (var subSystem in SecurityManagements.allSubSystems)
            {
                bool isFound = false;
                for(int i=0 ; i<allSubSystemsStatusHere.Count() ; i++)
                {
                    if(subSystem.id == allSubSystemsStatusHere[i].subnum)
                    {
                        isFound = true;
                        break;
                    }
                }
                if(isFound)
                {
                    _allSubSystemStatuses.Add(new SubSystemStatus(_id,subSystem,true,false));
                }
                else
                {
                    _allSubSystemStatuses.Add(new SubSystemStatus(_id,subSystem,false,false));
                }
            }
        }

        /// <summary>
        /// loads all the rousource type from the database
        /// </summary>
        private void loadAllResourceTypeStatuses()
        {
 	        _allResourceTypeStatuses = new List<ResourceTypeStatus>();
            var allResourceTypeStatusesHere = SecurityManagementQueries.getAllOrganizationResourceTypeRelation(_id);
            foreach (var resourceType in MachineTypes.allMachineTypes)
            {
                bool isFound = false;
                for(int i=0 ; i<allResourceTypeStatusesHere.Count() ; i++)
                {
                    if(resourceType.ID == allResourceTypeStatusesHere[i].typnum)
                    {
                        isFound = true;
                        break;
                    }
                }
                if(isFound)
                {
                    _allResourceTypeStatuses.Add(new ResourceTypeStatus(_id,resourceType,true));
                }
                else
                {
                    _allResourceTypeStatuses.Add(new ResourceTypeStatus(_id,resourceType,false));
                }
            }
        }

        /// <summary>
        /// loads all the staff from the database
        /// </summary>
        private void loadAllTheStaff()
        {
 	        _allStaff = new List<Staff>();
            var allStaffhere = SecurityManagementQueries.getAllOrganizationStaff(_id);
            foreach (var staff in allStaffhere)
            {
                _allStaff.Add(new Staff(staff.stanum,staff.stacod, staff.stanam, staff.staadd,staff.statel,staff.staid
                    ,staff.stapas,staff.stajob,staff.staetc,staff.plarig == 0? false:true));
            }
        }
        /// <summary>
        /// loads all the made products statuses from the database
        /// </summary>
        private void loadAllMadeProductStatuses()
        {
            _allMadeProductStatuses = new List<ItemStatus>();
            var allProductsStatusesHere = SecurityManagementQueries.getAllOrganizationItemRelation(_id);
            foreach (var item in Items.allItems)
            {
                if(item.type == 2) //made items
                {
                    int statusHere = -1;
                    for(int i=0 ; i<allProductsStatusesHere.Count() ; i++)
                    {
                        if(item.id == allProductsStatusesHere[i].itenum)
                        {
                            statusHere = (int)allProductsStatusesHere[i].itacmo;
                            break;
                        }
                    }
                    if(statusHere!=-1)
                    {
                        _allMadeProductStatuses.Add(new ItemStatus(_id,item,statusHere,false));
                    }
                    else
                    {
                        _allMadeProductStatuses.Add(new ItemStatus(_id,item,0,false));
                    }
                }
            }
        }
        /// <summary>
        /// loads all the purchased products statuses from the database
        /// </summary>
        private void loadAllPurchasedProductStatuses()
        {
            _allPurchaseProductStatuses = new List<ItemStatus>();
            var allProductsStatusesHere = SecurityManagementQueries.getAllOrganizationItemRelation(_id);
            foreach (var item in Items.allItems)
            {
                if(item.type == 1) //purchased items
                {
                    int statusHere = -1;
                    for(int i=0 ; i<allProductsStatusesHere.Count() ; i++)
                    {
                        if(item.id == allProductsStatusesHere[i].itenum)
                        {
                            statusHere = (int)allProductsStatusesHere[i].itacmo;
                            break;
                        }
                    }
                    if(statusHere!=-1)
                    {
                        _allPurchaseProductStatuses.Add(new ItemStatus(_id,item,statusHere,false));
                    }
                    else
                    {
                        _allPurchaseProductStatuses.Add(new ItemStatus(_id,item,0,false));
                    }
                }
            }
        }
        /// <summary>
        /// saves the changes made to organization
        /// </summary>
        public void saveChanges()
        {
            if (_hasChanged)
                SecurityManagementQueries.changeTheOrganizationInfo(_id, _title);
            foreach(var madeProductStatus in allMadeProductStatuses)
            {
                if(madeProductStatus._hasChanged)
                    madeProductStatus.saveChanges();
            }
            foreach(var purchaseProductStatus in allPurchasedProductStatuses)
            {
                if(purchaseProductStatus._hasChanged)
                purchaseProductStatus.saveChanges();
            }
            foreach(var resourceTypeStatus in allResourceTypeStatuses)
            {
                if (resourceTypeStatus._hasChanged)
                resourceTypeStatus.saveChanges();
            }
            foreach(var subSystemStatus in allSubSystemStatuses)
            {
                if (subSystemStatus._hasChanged)
                subSystemStatus.saveChanges();
            }
            foreach(var staff in allStaff)
            {
                if (staff._hasChanged)
                staff.saveChanges();
            }
            _hasChanged = false;
        }

        /// <summary>
        /// loads the information of the organization from the database again to revert changes
        /// </summary>
        public void loadAgain()
        {
            if(_hasChanged == true)
            {
                var organization = SecurityManagementQueries.getOrganizaitonInfoByID(_id);
                _title = organization.sectit;
                loadAllMadeProductStatuses();
                loadAllPurchasedProductStatuses();
                loadAllResourceTypeStatuses();
                loadAllSubSystemStatuses();
                loadAllTheStaff();
                _hasChanged = false;
            }
        }

        /// <summary>
        /// adds the newly added organization to the database
        /// </summary>
        public void addOrganization()
        {
            _id = SecurityManagementQueries.createANewOrganization(title);
            loadAllMadeProductStatuses();
            loadAllPurchasedProductStatuses();
            loadAllResourceTypeStatuses();
            loadAllSubSystemStatuses();
            loadAllTheStaff();
            _hasChanged = false;
        }

        /// <summary>
        /// deletes this organization from the database
        /// </summary>
        public void deleteOrganization()
        {
            SecurityManagementQueries.deleteOragnization(_id);
        }

        /// <summary>
        /// adds a staff to the organization
        /// </summary>
        /// <param name="newStaff">adds the staff to organization</param>
        public void addStaff(Staff newStaff)
        {
            newStaff.setAllSubSystemStatuses(_allSubSystemStatuses);
            _allStaff.Add(newStaff);
        }

        /// <summary>
        /// deletes a staff from the organization
        /// </summary>
        /// <param name="staff">removes the staff from the organization</param>
        public void deleteStaff(Staff staff)
        {
            _allStaff.Remove(staff);
        }

        /// <summary>
        /// creates a new Organization from an object from the database
        /// </summary>
        /// <param name="newId">the Id of the organization</param>
        /// <param name="newTitle">the title of the organization</param>
        public Organization(int newId,
            string newTitle)
        {
            _id = newId;
            _title = newTitle;
            loadAllMadeProductStatuses();
            loadAllPurchasedProductStatuses();
            loadAllResourceTypeStatuses();
            loadAllSubSystemStatuses();
            loadAllTheStaff();
            _hasChanged = false;
            //_saveIndb = true;
        }

        //private bool _saveIndb ;
        /// <summary>
        /// creates a new Staff from a new object
        /// </summary>
        /// <param name="newTitle">the title of the organization</param>
        public Organization(string newTitle)
        {
            _title = newTitle;
            _hasChanged = false;
            //_saveIndb = false;
            _id = SecurityManagementQueries.changeTheOrganizationInfo(_id, _title);
        }
    }

    public static class SecurityManagements
    {
        /// <summary>
        /// the version of the security management
        /// </summary>
        private static int _securityManagementVersion = 0;

        /// <summary>
        /// the version of the security management
        /// </summary>
        public static int securityMangementVersion
        {
            get
            {
                return _securityManagementVersion;
            }
        }

        /// <summary>
        /// the list of sub systems
        /// </summary>
        static List<SubSystem> _allSubSystems;
        /// <summary>
        /// the list of the organizations
        /// </summary>
        static List<Organization> _allOrganizations;

        /// <summary>
        /// the list of sub systems
        /// </summary>
        public static List<SubSystem> allSubSystems
        {
            get
            {
                if (_allSubSystems == null)
                {
                    loadSubSystemInfoFromDB();
                }
                return _allSubSystems;
            }
        }
        /// <summary>
        /// the list of the organizations
        /// </summary>
        public static List<Organization> allOrganizations
        {
            get
            {
                if(_allOrganizations == null)
                {
                    loadOrganizationsFromDB();
                }
                return _allOrganizations;
            }
        }
        /// <summary>
        /// load all the subsystems from the database
        /// </summary>
        public static void loadSubSystemInfoFromDB()
        {
            var allSubSystemFromDB = SecurityManagementQueries.getAllSubSystems();
            _allSubSystems = new List<SubSystem>();
            int i = 0;
            foreach (var subsystem in allSubSystemFromDB)
            {
                
                SubSystem tempSubSystem = new SubSystem(subsystem.subnum,subsystem.subtit,(int)subsystem.subseq);
                _allSubSystems.Add(tempSubSystem);
            }
        }
        /// <summary>
        /// load all the Organiztions from the database
        /// </summary>
        public static void loadOrganizationsFromDB()
        {
            var allOrganizationsFromDB = SecurityManagementQueries.getAllOrganizations();
            _allOrganizations = new List<Organization>();
            int i = 0;
            foreach (var organization in allOrganizationsFromDB)
            {
                
                Organization tempOrganization = new Organization(organization.secnum,organization.sectit);
                _allOrganizations.Add(tempOrganization);
            }
        }
        /// <summary>
        /// returns the organization info by its id
        /// </summary>
        /// <param name="id">the id of the organization</param>
        /// <returns>an organization</returns>
        public static Organization getOrganizationByID(int id)
        {
            foreach (var c in allOrganizations)
            {
                if (c.id == id)
                    return c;
            }
            return null;
        }

        /// <summary>
        /// sorts the organization by their title
        /// </summary>
        public static void sortOrganizationByTitle()
        {
            _allOrganizations.Sort(new Comparison<Organization>((x, y) => string.Compare(x.title, y.title)));
            _securityManagementVersion++;
        }

        /// <summary>
        /// deletes the organization from the organization list
        /// </summary>
        /// <param name="ID">the id of the organization</param>
        public static void deleteOrganizationFromListByID(int organizationID)
        {
            foreach (var s in _allOrganizations)
            {
                if (s.id == organizationID)
                {
                    _allOrganizations.Remove(s);
                    break;
                }
            }
            _securityManagementVersion++;
        }

        /// <summary>
        /// add the new organization to the list
        /// </summary>
        /// <param name="organization">the organizaiton just added</param>
        public static void addOrganizationToList(Organization organization)
        {
            for (int i = 0; i < _allOrganizations.Count; i++)
                if (organization.title.CompareTo(_allOrganizations[i].title) <= 0)
                {
                    _allOrganizations.Insert(i, organization);
                    break;
                }
            _securityManagementVersion++;
        }

        /// <summary>
        /// saves all the changes made to organization
        /// </summary>
        public static void saveAllOrganizations()
        {
            foreach(var organization in _allOrganizations)
                organization.saveChanges();
        }

        /// <summary>
        /// reset all the changes made to organization
        /// </summary>
        public static void cancelChanges()
        {
            foreach (var organization in _allOrganizations)
                organization.loadAgain();
        }

        internal static void addStaffToSectionList(int _subSectionCode, Staff staff)
        {
            foreach (var subSection in allOrganizations)
            {
                if (subSection.id == _subSectionCode)
                {
                    subSection.addStaff(staff);
                }
            }
        }

        internal static void deleteStaffFromSectionList(int _subSectionCode, Staff staff)
        {
            foreach (var subSection in allOrganizations)
            {
                if (subSection.id == _subSectionCode)
                {
                    subSection.deleteStaff(staff);
                }
            }
        }

        public static void deleteStaffFromListByID(int id)
        {
            SecurityManagementQueries.deleteStaff(id);
        }
    }
}
