using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DQL;
using IMPOS.DAL;

namespace IMPOS.BLL
{
    public static class Items
    {

        private static Hashtable _itemIDs;

        private static List<Item> _allItems;
        /// <summary>
        /// the version of the item
        /// </summary>
        private static int _itemVersion = 0;

        public static bool IsItemCodeUniqe(string code)
        {
            return ItemQuery.IsItemCodeUniqe(code);
        }

        public static void ClearAllItems()
        {
            _allItems = null;
        }
        public static void addFatherForItem(int id, int fatherid, double amount)
        {
            ItemQuery.addFatherForItem(fatherid,id,amount);
        }

        public static List<Item> allItems
        {
            get
            {
                if (_allItems == null)
                {
                    loadItemInfoFromDB();
                }
                return _allItems;
            }
        }

        /// <summary>
        /// the version of the item
        /// </summary>
        public static int itemVersion
        {
            get
            {
                return _itemVersion;
            }
            set
            {
                _itemVersion = value;
            }
        }

        public static void loadItemInfoFromDB()
        {
            //DateTime start = DateTime.Now;
            var allItemsFromDB = ItemQuery.getAllItems();
            _itemIDs=new Hashtable();
            _allItems = new List<Item>();
            foreach (var item in allItemsFromDB)
            {
                var tempItem = new Item(item);
                _allItems.Add(tempItem);
                _itemIDs.Add(item.itenum,tempItem);
            }
            //var defe1 = DateTime.Now - start;
            ItemQuery.LoadProStr();
            foreach (var item in _allItems)
            {
                item.loadChildrenAndParentList();
            }
            //var defe = DateTime.Now - start;
        }

        internal static void addItemToList(Item item)
        {
 	        _allItems.Add(item);
            _itemVersion++;
        }

        public static Item getItemById(int? id)
        {
            return (Item) _itemIDs[id];
        }
        /*
        /// <summary>
        /// the version of the item
        /// </summary>
        private static int _itemVersion = 0;

        /// <summary>
        /// the version of the item
        /// </summary>
        public static int itemVersion
        {
            get
            {
                return _itemVersion;
            }
            set
            {
                _itemVersion = value;
            }
        }

        /// <summary>
        /// the list of items
        /// </summary>
        static List<Item> _allItems;

        /// <summary>
        /// the list of items
        /// </summary>
        public static List<Item> allItems
        {
            get
            {
                if (_allItems == null)
                {
                    loadItemInfoFromDB();
                }
                return _allItems;
            }
        }

        /// <summary>
        /// load all the item type from the database
        /// </summary>
        public static void loadItemInfoFromDB()
        {
            var allItemsFromDB = ItemQuery.getAllItems();
            _allItems = new List<Item>();
            int i = 0;
            foreach (var item in allItemsFromDB)
            {
                Item tempItem = new Item(
                    item.mpsnum,
                    item.mpscod,
                    (int)item.itenum,
                    (int)item.mpsqua,
                    (DateTime)item.mpsdue,
                    (int)item.mpspri,
                    (int)item.mpscom,
                    (int)item.cusnum,
                    (int)item.mpssta);
                _allItems.Add(tempItem);
            }
        }

        /// <summary>
        /// add the new item to the list
        /// </summary>
        /// <param name="item">the item just added</param>
        public static void addItemToList(Item item)
        {
            _allItems.Add(item);
            _itemVersion++;
        }

        /// <summary>
        /// saves all the changes made to items
        /// </summary>
        public static void saveAllItems()
        {
            foreach (var item in _allItems)
                if (item.hasChanged)
                {
                    item.saveChanges();
                }
        }

        /// <summary>
        /// reset all the changes made to items
        /// </summary>
        public static void cancelChanges()
        {
            foreach (var item in _allItems)
                if (item.hasChanged)
                {
                    item.loadAgain();
                }
        }
        */

        /// <summary>
        /// deletes the item from the item list
        /// </summary>
        /// <param name="ID">the id of the item</param>
        public static void deleteFromListByID(int _id)
        {
            foreach (var m in _allItems)
            {
                if (m.id == _id)
                {
                    _allItems.Remove(m);
                    break;
                }
            }
            _itemVersion++;
        }

        public static List<PROSTR> getProStrByParentId(int father)
        {
            return ItemQuery.GetProStrByParentId(father);
        }

        public static List<PROSTR> getProStrByChildId(int child)
        {
            return ItemQuery.GetProStrByChildId(child);
        }

        public static void deleteItem(int id)
        {
            ItemQuery.deleteItemByID(id);
        }

        public static void AllItemSaveChanges(List<Item> items, int parentId, double amount)
        {
            items.Where(r => r.HasChanged).ToList().ForEach(ee =>
                {
                    ee.machineType = ee.machineType ?? new MachineType(-1, "", false, false, false, "");
                    ee.supplier = ee.supplier ?? new Supplier(-1, "", "", "", "", "");
                    if (ee.id == 0)
                    {
                        var id = ItemQuery.addNewItemWithParameters(
                            ee.title, ee.code, ee.unit.ID, ee.type ?? -1, ee.description, ee.size ?? false, ee.quentity,
                            ee.availableNow ?? -1,
                            ee.assignedQuantity ?? -1, ee.emergencyStorage ?? -1, ee.wastedQuantity ?? -1,
                            ee.operationName, ee.machineType.ID,
                            ee.prepareTime ?? -1, ee.buildUnitTime ?? -1, ee.shipmentTime ?? -1,
                            ee.transmitGroupSize??-1 , ee.operationDescription,
                            ee.supplier.ID, ee.supplyTime ?? -1);
                        ee.itemSaved(id);
                        if (parentId != -1)
                            ee.addFatherForItem(id, parentId, amount);
                    }
                    else
                    {
                        ItemQuery.setItemParametersByID(ee.id,
                                                        ee.title,
                                                        ee.code,
                                                        ee.unit.ID,
                                                        ee.type ?? -1,
                                                        ee.description,
                                                        ee.size ?? false,
                                                        ee.quentity,
                                                        ee.availableNow ?? -1,
                                                        ee.assignedQuantity ?? -1,
                                                        ee.emergencyStorage ?? -1,
                                                        ee.wastedQuantity ?? -1,
                                                        ee.operationName,
                                                        ee.machineType.ID,
                                                        ee.prepareTime ?? -1,
                                                        ee.buildUnitTime ?? -1,
                                                        ee.shipmentTime ?? -1,
                                                        ee.transmitGroupSize??-1,
                                                        ee.operationDescription,
                                                        ee.supplier.ID,
                                                        ee.supplyTime ?? -1);
                        ee.itemSaved(ee.id);
                    }


                });

        //_hasChanged = false;
        }

        public static void deleteFather(int id, int fatherId)
        {
            ItemQuery.deleteProStr(id, fatherId);
        }
    }

    public class Item
    {
        #region Field and properties

        public bool HasChanged { get { return _hasChanged; } }
        /// <summary>
        /// the id of the item
        /// </summary>
        private int _id;

        /// <summary>
        /// the title of the item
        /// </summary>
        private string _title;

        /// <summary>
        /// the code of the item
        /// </summary>
        private string _code;

        /// <summary>
        /// the measuring type of the Item
        /// </summary>
        private MeasuringType _unit;

        /// <summary>
        /// the type of the item
        /// 2 for made Items
        /// 1 for purchase Items
        /// </summary>
        private int? _type;

        /// <summary>
        /// the description of the item
        /// </summary>
        private string _description;

        /// <summary>
        /// is the order type dynamic(true) or static(false) سفارش دهی متغیر یا ثابت
        /// </summary>
        private bool? _size;

        /// <summary>
        /// the amount of the order مقدار سفارش دهی
        /// </summary>
        private double _quentity;

        /// <summary>
        /// the amount of the available item موجودی فعلی
        /// </summary>
        private double? _availableNow;

        /// <summary>
        /// the amount that is assigned to the item مقدار تخصیصی
        /// </summary>
        private double? _assignedQuantity;

        /// <summary>
        /// the amount that is stored for emergency مقدار ذخیره احتیاطی
        /// </summary>
        private double? _emergencyStorage;

        /// <summary>
        /// the amount of waste of the item مقدار ضایعات
        /// </summary>
        private double? _wastedQuantity;

        /// <summary>
        /// the childrens of the item از این مواد ساخته میشود
        /// </summary>
        private List<Item> _childrens;

        /// <summary>
        /// the parents of the item (با هم ترکیب میشوند و آن را درست میکنند(
        /// </summary>
        private List<Item> _parents;

        /// <summary>
        /// the name of the operation عنوان عملیات
        /// </summary>
        private string _operationName;

        /// <summary>
        /// the type of the machine نوع منبع تولیدی
        /// </summary>
        private MachineType _machineType;

        /// <summary>
        /// the time needed for prepration زمان آماده سازی
        /// </summary>
        private double? _prepareTime;

        /// <summary>
        /// the time needed to build one unit زمان تولید یک واحد
        /// </summary>
        private double? _buildUnitTime;

        /// <summary>
        /// the time needed to ship the item زمان حمل و نقل
        /// </summary>
        private double? _shipmentTime;

        /// <summary>
        /// the size of the transmit group اندازه دسته انتقالی
        /// </summary>
        private double? _transmitGroupSize;

        /// <summary>
        /// the description on the operation شرح عملیات
        /// </summary>
        private string _operationDescription;

        /// <summary>
        /// the info of the supplier اطلاعات تامین کننده
        /// </summary>
        private Supplier _supplier;

        /// <summary>
        /// the time of the supply زمان تهیه به روز
        /// </summary>
        private double? _supplyTime;

        /// <summary>
        /// has any parameter changed
        /// </summary>
        private bool _hasChanged;


        /// <summary>
        /// the id of the item
        /// </summary>
        public int id
        {
            get { return _id; }
        }

        /// <summary>
        /// the title of the item
        /// </summary>
        public string title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the code of the item
        /// </summary>
        public string code
        {
            get { return _code; }
            set
            {
                if (_code != value)
                {
                    _code = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the measuring type of the item
        /// </summary>
        public MeasuringType unit
        {
            get { return _unit; }
            set
            {
                if (_unit != value)
                {
                    _unit = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the type of the item
        /// </summary>
        public int? type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    _hasChanged = true;
                }
            }
        }

        //public int? fatherId { set; get; }

        /// <summary>
        /// the description of the item
        /// </summary>
        public string description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// is the order type dynamic(true) or static(false) سفارش دهی متغیر یا ثابت
        /// </summary>
        public bool? size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the amount of the order مقدار سفارش دهی
        /// </summary>
        public double quentity
        {
            get { return _quentity; }
            set
            {
                if (_quentity != value)
                {
                    _quentity = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the amount of the available item موجودی فعلی
        /// </summary>
        public double? availableNow
        {
            get { return _availableNow; }
            set
            {
                if (_availableNow != value)
                {
                    _availableNow = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the amount that is assigned to the item مقدار تخصیصی
        /// </summary>
        public double? assignedQuantity
        {
            get { return _assignedQuantity; }
            set
            {
                if (_assignedQuantity != value)
                {
                    _assignedQuantity = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the amount that is stored for emergency مقدار ذخیره احتیاطی
        /// </summary>
        public double? emergencyStorage
        {
            get { return _emergencyStorage; }
            set
            {
                if (_emergencyStorage != value)
                {
                    _emergencyStorage = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the amount of waste of the item مقدار ضایعات
        /// </summary>
        public double? wastedQuantity
        {
            get { return _wastedQuantity; }
            set
            {
                if (_wastedQuantity != value)
                {
                    _wastedQuantity = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the childrens of the item
        /// </summary>
        public List<Item> childrens
        {
            get { return _childrens; }
        }

        /// <summary>
        /// the parents of the item
        /// </summary>
        public List<Item> parents
        {
            get { return _parents; }
        }

        /// <summary>
        /// the name of the operation عنوان عملیات
        /// </summary>
        public string operationName
        {
            get { return _operationName; }
            set
            {
                if (_operationName != value)
                {
                    _operationName = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the type of the machine نوع منبع تولیدی
        /// </summary>
        public MachineType machineType
        {
            get { return _machineType; }
            set
            {
                if (_machineType != value)
                {
                    _machineType = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the time needed for prepration زمان آماده سازی
        /// </summary>
        public double? prepareTime
        {
            get { return _prepareTime; }
            set
            {
                if (_prepareTime != value)
                {
                    _prepareTime = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the time needed to build one unit زمان تولید یک واحد
        /// </summary>
        public double? buildUnitTime
        {
            get { return _buildUnitTime; }
            set
            {
                if (_buildUnitTime != value)
                {
                    _buildUnitTime = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the time needed to ship the item زمان حمل و نقل
        /// </summary>
        public double? shipmentTime
        {
            get { return _shipmentTime; }
            set
            {
                if (_shipmentTime != value)
                {
                    _shipmentTime = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the size of the transmit group اندازه دسته انتقالی
        /// </summary>
        public double? transmitGroupSize
        {
            get { return _transmitGroupSize; }
            set
            {
                if (_transmitGroupSize != value)
                {
                    _transmitGroupSize = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the description on the operation شرح عملیات
        /// </summary>
        public string operationDescription
        {
            get { return _operationDescription; }
            set
            {
                if (_operationDescription != value)
                {
                    _operationDescription = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the info of the supplier اطلاعات تامین کننده
        /// </summary>
        public Supplier supplier
        {
            get { return _supplier; }
            set
            {
                if (_supplier != value)
                {
                    _supplier = value;
                    _hasChanged = true;
                }
            }
        }

        /// <summary>
        /// the time of the supply زمان تهیه به روز
        /// </summary>
        public double? supplyTime
        {
            get { return _supplyTime; }
            set
            {
                if (_supplyTime != value)
                {
                    _supplyTime = value;
                    var rrr = new PROSTR().strrat;
                    var rrr2 = new PROSTR().stretc;
                    _hasChanged = true;
                }
            }
        }
        
        public double childQuantity { set; get; }
        public double parentQuantity { set; get; }
        public string childDescription { set; get; }
        public string parentDescription { set; get; }

        #endregion

        /// <summary>
        /// Save the changes made to the Maprse to the database
        /// </summary>
        public void saveChanges()
        {
            ItemQuery.setItemParametersByID(_id,
                                        _title,
                                        _code,
                                        _unit.ID,
                                        _type ?? -1,
                                        _description,
                                        _size ?? false,
                                        _quentity,
                                        _availableNow ?? -1,
                                        _assignedQuantity ?? -1,
                                        _emergencyStorage ?? -1,
                                        _wastedQuantity ?? -1,
                                        _operationName,
                                        _machineType.ID,
                                        _prepareTime ?? -1,
                                        _buildUnitTime ?? -1,
                                        _shipmentTime ?? -1,
                                        _transmitGroupSize??0,
                                        _operationDescription,
                                        _supplier.ID,
                                        _supplyTime ?? -1);
        
            
            _hasChanged = false;
        }

        public void addFatherForItem(int id, int fatherId, double amount)
        {
            ItemQuery.addFatherForItem(fatherId, id, amount);
        }

        public void addItem()
        {
            this._id = ItemQuery.addNewItemWithParameters(
                                        _title,
                                        _code,
                                        _unit.ID,
                                        _type ?? -1,
                                        _description,
                                        _size ?? false,
                                        _quentity,
                                        _availableNow ?? -1,
                                        _assignedQuantity ?? -1,
                                        _emergencyStorage ?? -1,
                                        _wastedQuantity ?? -1,
                                        _operationName,
                                        _machineType.ID,
                                        _prepareTime ?? -1,
                                        _buildUnitTime ?? -1,
                                        _shipmentTime ?? -1,
                                        _transmitGroupSize??0,
                                        _operationDescription,
                                        _supplier.ID,
                                        _supplyTime ?? -1);
            Items.addItemToList(this);
            _hasChanged = false;
        }
        public void itemSaved(int id)
        {
            _id = id;
            _hasChanged = false;
        }
        /// <summary>
        /// load the Item from the database again to revert changes
        /// </summary>
        public void loadAgain()
        {
            var item = ItemQuery.getItemByID(_id);
            title = item.itetit;
            code = item.itecod;
            for(int i=0 ; i<MeasuringTypes.measuringTypes.Count ; i++)
                if(MeasuringTypes.measuringTypes[i].ID == item.itemea)
                    unit = MeasuringTypes.measuringTypes[i];
            type = (int)item.itetyp ;
            description = item.iteetc;
            size = (item.itesiz == 1)?false:true;
            quentity = (double)item.itequa;
            availableNow = (double)item.iteinv;
            assignedQuantity = (double)item.aloamo;
            emergencyStorage = (double)item.itesaf;
            wastedQuantity = (item.scrinv!=null)?(double)item.scrinv:0; //TODO: this must be changed to double
            if(type==2)
            {
                var made = ItemQuery.getMadeItemByID(_id);
                operationName       = made.madtit; 
                for(int i=0 ; i<MachineTypes.allMachineTypes.Count ; i++)
                    if(MachineTypes.allMachineTypes[i].ID == made.mactyp)
                        machineType = MachineTypes.allMachineTypes[i];  
                prepareTime         = (double)made.settim;  
                buildUnitTime       = (double)made.protim;  
                shipmentTime        = (double)made.tratim;  
                transmitGroupSize   = (double)made.trasiz;  
                operationDescription= made.maddes;  
            }
            else
            {
                var purchase = ItemQuery.getPurchaseItemByID(_id);
                for(int i=0 ; i<Suppliers.allSuppliers.Count ; i++)
                {
                    if(purchase.supnum == Suppliers.allSuppliers[i].ID)
                        supplier = Suppliers.allSuppliers[i];
                }
                supplyTime = (double)purchase.purlt;
            }
            _hasChanged = false;
        }

        /// <summary>
        /// deletes this item from database
        /// </summary>
        public bool deleteThisItem()
        {
            if (_childrens.Count() == 0 && _parents.Count() == 0)
            {
                Items.deleteFromListByID(_id);
                ItemQuery.deleteItemByID(_id);
                return true;
            }
            return false;
        }

        /// <summary>
        /// creates a new item by the user and adds it to the database
        /// </summary>
        public Item(
           string newTitle,
           string newCode,
           int newUnit,
           int newType,
           string newDescription,
           bool newSize,
           double newQuentity,
           double newAvailableNow,
           double newAssignedQuantity,
           double newEmergencyStorage,
           double newWastedQuantity,
           string newOperationName,
           int newMachineType,
           double newPrepareTime,
           double newBuildUnitTime,
           double newShipmentTime,
           double newTransmitGroupSize,
           string newOperationDescription,
           int newSupplier,
           double newSupplyTime)
        {
            _title = newTitle;
            _code = newCode;
            for (int i = 0; i < MeasuringTypes.measuringTypes.Count; i++)
            {
                if (newUnit == MeasuringTypes.measuringTypes[i].ID)
                    _unit = MeasuringTypes.measuringTypes[i];
            }
            _type = newType;
            _description = newDescription;
            _size = newSize;
            _quentity = newQuentity;
            _availableNow = newAvailableNow;
            _assignedQuantity = newAssignedQuantity;
            _emergencyStorage = newEmergencyStorage;
            _wastedQuantity = newWastedQuantity;
            _operationName = newOperationName;
            for (int i = 0; i < MachineTypes.allMachineTypes.Count; i++)
            {
                if (newMachineType == MachineTypes.allMachineTypes[i].ID)
                    _machineType = MachineTypes.allMachineTypes[i];
            }
            _prepareTime = newPrepareTime;
            _buildUnitTime = newBuildUnitTime;
            _shipmentTime = newShipmentTime;
            _transmitGroupSize = newTransmitGroupSize;
            _operationDescription = newOperationDescription;
            for (int i = 0; i < Suppliers.allSuppliers.Count; i++)
            {
                if (newSupplier == Suppliers.allSuppliers[i].ID)
                    _supplier = Suppliers.allSuppliers[i];
            }
            _supplyTime = newSupplyTime;
            _hasChanged = false;
        }

        public Item(int newID,
           string newTitle,
           string newCode,
           int newUnit,
           int newType,
           string newDescription,
           bool newSize,
           double newQuentity,
           double newAvailableNow,
           double newAssignedQuantity,
           double newEmergencyStorage,
           double newWastedQuantity,
           string newOperationName,
           int newMachineType,
           double newPrepareTime,
           double newBuildUnitTime,
           double newShipmentTime,
           double newTransmitGroupSize,
           string newOperationDescription,
           int newSupplier,
           double newSupplyTime)
        {
            _id = newID;
            _title = newTitle;
            _code = newCode;
            for (int i = 0; i < MeasuringTypes.measuringTypes.Count; i++)
            {
                if (newUnit == MeasuringTypes.measuringTypes[i].ID)
                    _unit = MeasuringTypes.measuringTypes[i];
            }
            _type = newType;
            _description = newDescription;
            _size = newSize;
            _quentity = newQuentity;
            _availableNow = newAvailableNow;
            _assignedQuantity = newAssignedQuantity;
            _emergencyStorage = newEmergencyStorage;
            _wastedQuantity = newWastedQuantity;
            _operationName = newOperationName;
            for (int i = 0; i < MachineTypes.allMachineTypes.Count; i++)
            {
                if (newMachineType == MachineTypes.allMachineTypes[i].ID)
                    _machineType = MachineTypes.allMachineTypes[i];
            }
            _prepareTime = newPrepareTime;
            _buildUnitTime = newBuildUnitTime;
            _shipmentTime = newShipmentTime;
            _transmitGroupSize = newTransmitGroupSize;
            _operationDescription = newOperationDescription;
            for (int i = 0; i < Suppliers.allSuppliers.Count; i++)
            {
                if (newSupplier == Suppliers.allSuppliers[i].ID)
                    _supplier = Suppliers.allSuppliers[i];
            }
            _supplyTime = newSupplyTime;
            _childrens=new List<Item>();//todo:edit add this line
        }


        public static List<PROSTR> getParentsByID(int id)
        {
            return (from i in ItemQuery.Prostrs
                    where i.chinum == id
                    select i).OrderBy(i => i.parnum).ToList();
        }

        public static List<PROSTR> getChildrenByID(int id)
        {
            return (from i in ItemQuery.Prostrs
                    where i.parnum == id
                    select i).OrderBy(ii => ii.chinum).ToList();
        }

        public void loadChildrenAndParentList()
        {
            List<PROSTR> tempChildren = getChildrenByID(_id);
            _childrens=new List<Item>();
            //childrens.AddRange(tempChildren);
            foreach (var child in tempChildren)
            {
                childrens.Add(Items.getItemById(child.chinum));
            }
            var tempParents = getParentsByID(_id);
            _parents=new List<Item>();
            foreach (var parent in tempParents)
            {
                parents.Add(Items.getItemById(parent.parnum));
            }
        }

        public Item(string name)
        {
            _title = name;
        }

        public Item(string tempTitle,double tempAmount,MeasuringType unit)
        {
            _title = tempTitle;
            quentity = tempAmount;
            _unit = unit;
        }

        public Item(ITEMS item)
        {
            _id = item.itenum;
            _title = item.itetit;
            _code = item.itecod;
            for (int i = 0; i < MeasuringTypes.measuringTypes.Count; i++)
                if (MeasuringTypes.measuringTypes[i].ID == item.itemea)
                    _unit = MeasuringTypes.measuringTypes[i];
            _type = item.itetyp??0;
            _description = item.iteetc;
            _size = (item.itesiz == 1) ? false : true;
            _quentity = item.itequa??0;
            _availableNow = item.iteinv;
            _assignedQuantity = item.aloamo;
            _emergencyStorage = item.itesaf;
            _wastedQuantity = (item.scrinv != null) ? (double)item.scrinv : 0; //TODO: this must be changed to double
            if (_type == 2)
            {
                var made = ItemQuery.getMadeItemByID(_id);
                _operationName = made.madtit;
                for (int i = 0; i < MachineTypes.allMachineTypes.Count; i++)
                    if (MachineTypes.allMachineTypes[i].ID == made.mactyp)
                        _machineType = MachineTypes.allMachineTypes[i];
                _prepareTime = made.settim;
                _buildUnitTime = made.protim;
                _shipmentTime = made.tratim;
                _transmitGroupSize = made.trasiz??-1;
                _operationDescription = made.maddes;
            }
            else
            {
                var purchase = ItemQuery.getPurchaseItemByID(_id);
                for (int i = 0; i < Suppliers.allSuppliers.Count; i++)
                {
                    if (purchase.supnum == Suppliers.allSuppliers[i].ID)
                        _supplier = Suppliers.allSuppliers[i];
                }
                _supplyTime = purchase.purlt;
            }
            _hasChanged = false;
        }
    }
}
