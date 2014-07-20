using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using IMPOS.BLL;
using Telerik.Windows.Controls;
using IMPOS.Annotations;
using IMPOS.Helpers;
using IMPOS.Views;

namespace IMPOS.ViewModel
{
    /// <summary>
    /// kharidani va tolidi
    /// </summary>
    public class ItemType
    {
        public string Title { set; get; }
        public int id { set; get; }
    }
    public class MaterialManagementViewModel:INotifyPropertyChanged
    {
        //fehrest va sakhtar 
        private readonly MaterialManagementView.fockesDelegate _hideDelegate;
        //hame maghadir ro check kone ke vared shode ya na ke kodoom select bood ghadim
        public int? BeforeSelection { set; get; }
        //vaghti new ro zad inha disable mishand
        public bool Disabled { set; get; }
        //baraye modiriate baz va baste shodan hast
        public string TreeHeight
        {
            set { _treeHeight = value; OnPropertyChanged(); }
            get { return _treeHeight; }
        }

        public string ListHeight
        {
            set { _listHeight = value;OnPropertyChanged(); }
            get { return _listHeight; }
        }

        private readonly MaterialManagementView.IsHideMethodForOperationSuplierDelegate _IsHideMethodForOperationSuplierDelegateV;
        private readonly MaterialManagementView.IsHide _isHideDelegate;
        private readonly MaterialManagementView.TreeItemLoaded _treeItemLoaded;
        private readonly MaterialManagementView.TreeItemLoadonlyForDropDelegate _treeItemLoadonlyForDrop;
        /// <summary>
        /// baraye inke panjare namayesh dade beshe ya na
        /// </summary>
        public bool _materialStracture;
        public bool _imageAndFilm;
        public bool _materialContains;
        public bool _operationInfo;
        public bool _materialInfo;
        //list fehrest
        public List<Item> Items { set; get; }
        //list panjare enteghal
        public List<Item> MoveItems { set; get; }

        //aya dogme faal bashad ya na
        public bool PasteActive
        {
            set { _pasteActive = value;OnPropertyChanged(); }
            get { return _pasteActive; }
        }

        public bool NewDisabling
        {
            get { return !_newActivated; }
        }

        public Item CurrentItem
        {
            set
            {
                //harchi ke to fehrest entekhab kard ro tooye tree peyda kon
                _currentItem = value;
                OnPropertyChanged();
                if (value == null)
                    return;
                var groupBack = Groups;
                _currentGroup = null;
                while (_currentGroup == null && groupBack.Count>0)
                {
                    CurrentGroup = groupBack.FirstOrDefault(r => r.ItemData.id == CurrentItem.id);
                    groupBack = groupBack.SelectMany(rr => rr.SubItemInTrees).ToList();
                }
                if (CurrentGroup == null)
                {
                    //nemiad hichvaght
                    MessageBox.Show(".کالای مورد نظر در ساختار درختی یافت نشد");
                }

            }
            get { return _currentItem; }
        }

        public ItemInTree CurrentGroup2
        {
            set
            {
                _currentGroup2 = value;OnPropertyChanged();
            }
            get { return _currentGroup2; }
        }

        public Item CurrentMoveItem
        {
            set
            {
                _currentItem = value;
                OnPropertyChanged();
                if (value == null)
                    return;
                var groupBack = Groups2;
                _currentGroup2 = null;
                while (_currentGroup2 == null)
                {
                    CurrentGroup2 = groupBack.FirstOrDefault(r => r.ItemData.id == CurrentItem.id);
                    groupBack = groupBack.SelectMany(rr => rr.SubItemInTrees).ToList();
                }
            }
            get { return _currentItem; }
        }

        public bool SupplierShowHide
        {
            set
            {
                _supplierShowHide = value;
            }
            get { return _supplierShowHide; }
        }

        public bool OperationShowHide
        {
            set
            {
                _operationShowHide = value;
            }
            get { return _operationShowHide; }
        }

        public List<ItemType> ItemTypes { set; get; }
        public ItemType CurrentItemType
        {
            set
            {
                _currentItemType = value;OnPropertyChanged();
                if (value==null)
                    return;
                if (CurrentGroup != null)
                {
                    if (CurrentGroup.ItemData != null)
                    {
                        CurrentGroup.ItemData.type = value.id;
                        //BLL.GetBeforeItemTypeData(CurrentGroup.ItemData.id,CurrentGroup.ItemData.)
                        if (CurrentGroup.ItemData.type == 2)
                        {
                            SupplierShowHide = false;
                            OperationShowHide = true;
                            KharidHeight = "*";
                            TolidHeight = "0";
                            HeaderOfMadPur = "اطلاعات تامین کننده";
                        }
                        else
                        {
                            OperationShowHide = false;
                            SupplierShowHide = true;
                            KharidHeight = "0";
                            TolidHeight = "*";
                            HeaderOfMadPur = "اطلاعات عملیات تولید";
                        }
                    }
                }
                _IsHideMethodForOperationSuplierDelegateV.Invoke();
            }
            get { return _currentItemType; }
        }
        
        public ICommand SearchTextChanged { set; get; }
        //taieed enteghal
        public ICommand TrasferClick { set; get; }
        //search enteghal
        public ICommand MoveSearchTextChanged { set; get; }
        //new save delete va ... in faal mishe
        public ICommand StructureClick { set; get; }
        //private Item item;
        private List<Item> _items;

        public string AjzayeKalaHeight
        {
            set { _ajzayeKalaHeight = value;OnPropertyChanged(); }
            get { return _ajzayeKalaHeight; }
        }

        public string SearchText
        {
            set
            {
                _searchText = value;
                OnPropertyChanged();
                //if (string.IsNullOrEmpty(value))
                //    return;
                // Items = _items.Where(r=>r.title.Contains(_searchText)).ToList();
                //OnPropertyChanged("Items");
            }
            get { return _searchText; }
        }

        public bool NewActivated
        {
            set { _newActivated = value; OnPropertyChanged(); OnPropertyChanged("NewDisabling"); OnPropertyChanged("CancleActivated"); }
            get { return _newActivated; }
        }

        public bool _cancleActivated = false;
        public bool CancleActivated
        {
            set { _cancleActivated = value; OnPropertyChanged(); OnPropertyChanged("NewDisabling"); }
            get
            {
                var tmpCancle = false;
                if (CurrentGroup != null && CurrentGroup.ItemData != null)
                    tmpCancle = CurrentGroup.ItemData.HasChanged;
                return _newActivated || _cancleActivated || tmpCancle;
            }
        }

        private bool _entriesInfoShowHide;
        //mostaghel ya joz ro moshakhas mikone
        private bool _isIndepend;
        private ItemInTree _currentGroup;
        private MeasuringType _currentMeasuringType;
        private List<MeasuringType> _measuringTypes;
        private string _tolidHeight;
        private string _kharidHeight;
        private ItemType _currentItemType;
        private Supplier _currentSupplier;
        private string _headerOfMadPur;
        private MachineType _currentMachineType;
        //baad az drag drop
        public ICommand DroppedEvent { set; get; }
        //public ICommand TreeSelectionChanged { set; get; }
        //public ObservableCollection<MaterialStraucture> MaterialStrauctures { set; get; }
        public MaterialManagementViewModel(MaterialManagementView.IsHideMethodForOperationSuplierDelegate IsHideMethodForOperationSuplierDelegateV, MaterialManagementView.fockesDelegate hideDelegate, MaterialManagementView.IsHide isHideDelegate, MaterialManagementView.TreeItemLoaded treeItemLoaded, MaterialManagementView.TreeItemLoadonlyForDropDelegate treeItemLoadonlyForDrop)
        {
            _hideDelegate = hideDelegate;
//CurrentGroup.ItemData.parents[].
            //CurrentBackColorOfData.BrushAssignedQuantity = Brushes.Red;
            //OnPropertyChanged("CurrentBackColorOfData");
            try
            {
                Disabled = false;
                OnPropertyChanged("Disabled");
                CurrentBackColorOfData = new BackColorOfData();
                DroppedEvent = new DelegateCommand(DroppedAction);
                TransferFehrest = true;
                PasteActive = false;
                SearchTextChanged = new DelegateCommand(SearchTextAction);
                TrasferClick = new DelegateCommand(TransferAction);
                MoveSearchTextChanged = new DelegateCommand(MoveSearchTextAction);
                StructureClick = new DelegateCommand(Structure);
                HeaderOfMadPur = "اطلاعات تامین کننده";
                ItemTypes = new List<ItemType>
                {
                    new ItemType{id = 2,Title = "تولیدی"},
                    new ItemType{id = 1,Title = "خریدنی"},
                };
                Suppliers = BLL.Suppliers.allSuppliers;
                MachineTypes = BLL.MachineTypes.allMachineTypes;
                MeasuringTypes = BLL.MeasuringTypes.measuringTypes;
                TolidHeight = "*";
                AjzayeKalaHeight = "0";
                KharidHeight = "0";
                TreeHeight = "*";
                ListHeight = "0";
                OnPropertyChanged("Suppliers");
                OnPropertyChanged("MachineTypes");
                OnPropertyChanged("MeasuringTypes");
                OnPropertyChanged("ItemTypes");
                CrudClick = new DelegateCommand(Crud);
                _IsHideMethodForOperationSuplierDelegateV = IsHideMethodForOperationSuplierDelegateV;
                _isHideDelegate = isHideDelegate;
                _treeItemLoaded = treeItemLoaded;
                _treeItemLoadonlyForDrop = treeItemLoadonlyForDrop;
                _items = BLL.Items.allItems;
                LoadGroupData(true);
                OnPropertyChanged("Items");

                _imageAndFilm = true;
                _materialInfo = true;
                _operationInfo = true;
                _materialContains = true;
                _entriesInfoShowHide = true;
                IsIndepend = true;
                _fehrestWindowShowHide = true;
                _materialStracture = false;
                ResetAllWindowClick = new DelegateCommand(ResetAllWindow);
                //TreeSelectionChanged = new DelegateCommand(TreeSelectionChangedAction);
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        //change is being canceled
        public void ChangeForCanceling()
        {
            OnPropertyChanged("CancleActivated");
        }

        private void DroppedAction(object obj)
        {
            try
            {
                var id = (int?)(((RadTreeViewDragEndedEventArgs)obj).TargetDropItem).Tag;
                if (id == null)
                {
                    MessageBox.Show(".فقط بر روی یک مورد دیگر می توانید ماوس را رها کنید", "خطا");
                    return;
                }
                var win = new InputBoxWindow("مقدار کالای انتقالی را وارد نمایید:", "مقدار؟");
                bool errorInData = false;
                win.Closing += (o, e) =>
                {
                    if (win.DialogResult != true) return;
                    try
                    {
                        var ddd = Convert.ToDouble(win.TextValue);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(".فیلد مقدار باید عددی وارد شود", "خطا", MessageBoxButton.OK,
                                    MessageBoxImage.Error, MessageBoxResult.OK,
                                    MessageBoxOptions.RightAlign);
                        win.DialogResult = false;
                        errorInData = true;
                    }
                };
                win.ShowDialog();
                if (win.DialogResult == true)
                {
                    AmountOfFather = double.Parse(win.TextValue);
                    CurrentMoveItem = MoveItems.Single(r => r.id == id);
                    TransferAction("fatherAdd");
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        public double? AmountOfFather
        {
            set { _amountOfFather = value;OnPropertyChanged(); }
            get { return _amountOfFather; }
        }

        private void TransferAction(object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "AddAjza":
                        {
                            if (CurrentGroup == null)
                            {
                                MessageBox.Show(".ابتدا یک مورد را از پنجره ساختارکالا/فهرست کالا انتخاب نمایید", "انتخاب یک مورد", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            if (CurrentChildItem == null)
                            {
                                MessageBox.Show(".ابتدا یک مورد را انتخاب نمایید", "انتخاب یک مورد", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            if (CurrentGroup.ItemData.id == CurrentChildItem.id)
                            {
                                MessageBox.Show(".یک کالا نمی تواند زیر مجوعه خودش باشد", "خطا", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            if (AmountOfChild == null || AmountOfChild <= 0)
                            {
                                MessageBox.Show(".مقدار واردی جزء معتبر نمی باشد", "خطای مقدار جزء", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            BLL.Items.addFatherForItem(CurrentChildItem.id, CurrentGroup.ItemData.id, AmountOfChild ?? 0);

                            var res = MessageBox.Show(".آیا می خواهید اطلاعات مجدداً بازیابی شوند", "بازیابی مجدد", MessageBoxButton.YesNo,
                                            MessageBoxImage.Question, MessageBoxResult.No,
                                            MessageBoxOptions.RightAlign);
                            if (res == MessageBoxResult.Yes)
                            {
                                BLL.Items.ClearAllItems();
                                _items = BLL.Items.allItems;
                                LoadGroupData();
                            }
                            else
                            {
                                var iii = new Item(CurrentChildItem.title);
                                iii.unit = CurrentChildItem.unit;
                                iii.childQuantity = AmountOfChild ?? 0;
                                CurrentGroup.SubItemInTrees.Add(new ItemInTree { SubItemInTrees = new List<ItemInTree>(), Key = CurrentChildItem.id, Name = CurrentChildItem.title, ItemData = iii });
                                //CurrentGroup.ItemData.childrens.Add(new Item(CurrentChildItem.title, AmountOfChild ?? 0, CurrentChildItem.unit));
                                var id = CurrentGroup.ItemData.id;
                                CurrentGroup = null;
                                OnPropertyChanged("CurrentGroup");
                                CurrentItem = Items.Single(r => r.id == id);
                                //OnPropertyChanged("CurrentGroup.ItemData.parents");
                                //OnPropertyChanged("CurrentGroup.ItemData");
                                OnPropertyChanged("CurrentGroup");
                            }
                            CurrentChildItem = null;
                            AmountOfChild = null;
                            break;
                        }
                    //taeed panjare enteghal
                    case "fatherAdd":
                        {
                            if (CurrentGroup == null)
                            {
                                MessageBox.Show(".ابتدا یک مورد را انتخاب نمایید", "انتخاب یک مورد", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            if (CurrentMoveItem == null)
                            {
                                MessageBox.Show(".ابتدا یک مورد را انتخاب نمایید", "انتخاب یک مورد", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            if (CurrentGroup.ItemData.id == CurrentMoveItem.id)
                            {
                                MessageBox.Show(".یک کالا نمی تواند زیر مجوعه خودش باشد", "خطا", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            if (AmountOfFather == null || AmountOfFather <= 0)
                            {
                                MessageBox.Show(".مقدار کالای وابسته معتبر نمی باشد", "مقدار کالای وابسته", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            var groups = new List<ItemInTree>();
                            var thisRes = CurrentGroup2.SubItemInTrees;
                            bool tanaghoz = false;
                            //thisRes = thisRes.ToList().Union(groups.SelectMany(ee => ee.SubItemInTrees).Where(r => r.ItemData.HasChanged).ToList());
                            while (thisRes.Count() != 0)
                            {
                                groups = groups.Union(thisRes).ToList();
                                if (groups.Count(r => r.ItemData.id == CurrentGroup.ItemData.id) > 0)
                                {
                                    tanaghoz = true;
                                }
                                thisRes = thisRes.SelectMany(ee => ee.SubItemInTrees).ToList();
                            }
                            if (!tanaghoz)
                            {
                                var thisRes2 = CurrentGroup2.ItemData.parents;
                                var groups2 = new List<Item>();
                                while (thisRes2.Count() != 0)
                                {
                                    groups2 = groups2.Union(thisRes2).ToList();
                                    if (groups2.Count(r => r.id == CurrentGroup.ItemData.id) > 0)
                                    {
                                        tanaghoz = true;
                                    }
                                    thisRes2 = thisRes.SelectMany(ee => ee.ItemData.parents).ToList();
                                    _treeItemLoadonlyForDrop.Invoke();
                                }
                            }
                            if (tanaghoz)
                            {
                                MessageBox.Show(".روابط کالای جز یا وابسته باعث به وجود آمدن حلقه یا موارد تکراری در ساختار درخت می شود", "خطا روابط کالاهای جز یا وابسته", MessageBoxButton.OK,
                                         MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            BLL.Items.addFatherForItem(CurrentGroup.ItemData.id, CurrentMoveItem.id, AmountOfFather ?? 0);

                            var res = MessageBox.Show(".آیا می خواهید اطلاعات مجدداً بازیابی شوند", "بازیابی مجدد", MessageBoxButton.YesNo,
                                            MessageBoxImage.Question, MessageBoxResult.No,
                                            MessageBoxOptions.RightAlign);
                            if (res == MessageBoxResult.Yes)
                            {
                                BLL.Items.ClearAllItems();
                                _items = BLL.Items.allItems;
                                LoadGroupData();
                            }
                            else
                            {
                                CurrentGroup.ItemData.parents.Add(new Item(CurrentMoveItem.title, AmountOfFather ?? 0, CurrentMoveItem.unit));
                                var id = CurrentGroup.ItemData.id;
                                CurrentGroup = null;
                                OnPropertyChanged("CurrentGroup");
                                CurrentItem = Items.Single(r => r.id == id);
                                OnPropertyChanged("CurrentGroup");
                            }
                            AmountOfFather = null;
                            break;
                        }
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private ItemInTree _beforeSelected;
        public double? AmountOfChild
        {
            set { _amountOfChild = value;OnPropertyChanged(); }
            get { return _amountOfChild; }
        }

        public Item CurrentChildItem
        {
            set { _currentChildItem = value;OnPropertyChanged(); }
            get { return _currentChildItem; }
        }

        //vaghti sabt zade shode check mikone hame data ha vared shode ya na
        private bool AllDataCompleted()
        {
            try
            {
                bool iseroor=false;
                //CurrentBackColorOfData.BrushAssignedQuantity = Brushes.BlueViolet;
                //CurrentBackColorOfData.ColorAssignedQuantity = Color.FromRgb(20, 250, 20);
                OnPropertyChanged("CurrentBackColorOfData");
                //if (NewActivated)
                {
                    if (CurrentGroup == null)
                        return true;
                    if (string.IsNullOrEmpty(CurrentGroup.ItemData.code))
                    {
                        CurrentBackColorOfData.BrushCode = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".کد کالا باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                    if (CurrentGroup.ItemData.assignedQuantity == null)
                    {
                        CurrentBackColorOfData.BrushAssignedQuantity = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".مقدار تخصیصی باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                    if (CurrentGroup.ItemData.type == null)
                    {
                        CurrentBackColorOfData.BrushType = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".نوع کالا باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }

                    if (CurrentGroup.ItemData.type == 1)
                    {
                        if (CurrentGroup.ItemData.supplier == null)
                        {
                            CurrentBackColorOfData.BrushSupplier = Brushes.LightPink;
                            iseroor = true;
                            //MessageBox.Show(".اطلاعات تامین کننده باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            //return false;
                        }
                        if (CurrentGroup.ItemData.supplyTime == null)
                        {
                            CurrentBackColorOfData.BrushSupplyTime = Brushes.LightPink;
                            iseroor = true;
                            //MessageBox.Show(".زمان تهیه به روز باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            //return false;
                        }
                        if (CurrentGroup.ItemData.supplyTime < 0)
                        {
                            CurrentBackColorOfData.BrushSupplyTime = Brushes.LightPink;
                            iseroor = true;
                            //MessageBox.Show(".زمان حمل و نقل باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            //return false;
                        }
                    }
                    else
                    {
                        if (CurrentGroup.ItemData.machineType == null)
                        {
                            CurrentBackColorOfData.BrushMachineType = Brushes.LightPink;
                            iseroor = true;
                            //MessageBox.Show(".نوع منبع تولیدی باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            //return false;
                        }
                        //if (string.IsNullOrEmpty(CurrentGroup.ItemData.operationDescription))//todo: this field not in view but in table "madite"
                        //{
                        //    CurrentGroup = CurrentGroup;
                        //    MessageBox.Show(".شرح عملیات باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //    return;
                        //}
                        if (string.IsNullOrEmpty(CurrentGroup.ItemData.operationName))
                        {
                            CurrentBackColorOfData.BrushOperationName = Brushes.LightPink;
                            iseroor = true;
                            //MessageBox.Show(".عنوان عملیات باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            //return false;
                        }
                        if (CurrentGroup.ItemData.prepareTime == null)
                        {
                            CurrentBackColorOfData.BrushPrepareTime = Brushes.LightPink;
                            iseroor = true;
                            //MessageBox.Show(".زمان آماده سازی باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            //return false;
                        }
                        if (CurrentGroup.ItemData.buildUnitTime == null)
                        {
                            CurrentBackColorOfData.BrushBuildUnitTime = Brushes.LightPink;
                            iseroor = true;
                            //MessageBox.Show(".زمان تولید یک واحد باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            //return false;
                        }
                        if (CurrentGroup.ItemData.shipmentTime == null)
                        {
                            CurrentBackColorOfData.BrushShipmentTime = Brushes.LightPink;
                            iseroor = true;
                            //MessageBox.Show(".زمان حمل و نقل باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            //return false;
                        }
                        if (CurrentGroup.ItemData.transmitGroupSize==null)//equal with: _beforeSelected.ItemData.transmitGroupSize==0
                        {
                            CurrentBackColorOfData.BrushTransmitGroupSize = Brushes.LightPink;
                            iseroor = true;
                            //MessageBox.Show(".اندازه دسته انتقالی باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            //return false;
                        }
                        if (CurrentGroup.ItemData.buildUnitTime < 0)
                        {
                            CurrentBackColorOfData.BrushBuildUnitTime = Brushes.LightPink;
                            iseroor = true;
                            //MessageBox.Show(".زمان تولید یک واحد باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            //return false;
                        }
                        if (CurrentGroup.ItemData.shipmentTime < 0)
                        {
                            CurrentBackColorOfData.BrushShipmentTime = Brushes.LightPink;
                            iseroor = true;
                            //MessageBox.Show(".زمان حمل و نقل باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            //return false;
                        }
                        if (CurrentGroup.ItemData.prepareTime < 0)
                        {
                            CurrentBackColorOfData.BrushPrepareTime = Brushes.LightPink;
                            iseroor = true;
                            //MessageBox.Show(".زمان آماده سازی باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            //return false;
                        }
                    }
                    if (CurrentGroup.ItemData.size == null)
                    {
                        CurrentBackColorOfData.BrushSize = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".سفارش دهی متغیر یا ثابت باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                    if (string.IsNullOrEmpty(CurrentGroup.ItemData.title))
                    {
                        CurrentBackColorOfData.BrushTitle = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".عنوان کالا باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                    if (CurrentGroup.ItemData.unit == null)
                    {
                        CurrentBackColorOfData.BrushUnit = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".واحد اندازه گیری کالا باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                    if (CurrentGroup.ItemData.wastedQuantity == null)
                    {
                        CurrentBackColorOfData.BrushWastedQuantity = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".مقدار ضایعات باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                    if (CurrentGroup.ItemData.code.Length > 20)
                    {
                        CurrentBackColorOfData.BrushCode = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".کد کالا باید حداکثر 20 رقمی باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                    if (NewActivated && CurrentGroup.ItemData.id == 0 && BLL.Items.IsItemCodeUniqe(CurrentGroup.ItemData.code) == false)
                    {
                        CurrentBackColorOfData.BrushId = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".کد کالا باید منحصر به فرد باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                    if (CurrentGroup.ItemData.title.Length > 50)
                    {
                        CurrentBackColorOfData.BrushTitle = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".عنوان کالا باید حداکثر 50 حرفی باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                    if (CurrentGroup.ItemData.quentity < 0)
                    {
                        CurrentBackColorOfData.BrushQuentity = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".مقدار سفارش باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                    if (CurrentGroup.ItemData.assignedQuantity < 0)
                    {
                        CurrentBackColorOfData.BrushAssignedQuantity = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".مقدار تخصیصی باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                    if (CurrentGroup.ItemData.wastedQuantity < 0)
                    {
                        CurrentBackColorOfData.BrushWastedQuantity = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".مقدار ضایعات باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                    if (CurrentGroup.ItemData.transmitGroupSize < 0)
                    {
                        CurrentBackColorOfData.BrushTransmitGroupSize = Brushes.LightPink;
                        iseroor = true;
                        //MessageBox.Show(".دسته انتقالی باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //return false;
                    }
                }
                if (iseroor)
                {
                    MessageBox.Show(".لطفا اطلاعات را تصحیح و یا تکمیل نمایید", "خطا در اطلاعات", MessageBoxButton.OK,
                        MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    OnPropertyChanged("CurrentBackColorOfData");
                    return false;
                }
                else
                {
                    CurrentBackColorOfData=new BackColorOfData();
                    OnPropertyChanged("CurrentBackColorOfData");
                    return true;
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
            return false;
        }

        private bool canceling;
        private void TreeSelectionChangedAction()
        {
            try
            {
                if (canceling)
                {
                    canceling = false;
                    return;
                }
                if (NewActivated && CurrentGroup != _beforeSelected)
                {
                    if (string.IsNullOrEmpty(_beforeSelected.ItemData.code))
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".کد کالا باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                    if (_beforeSelected.ItemData.assignedQuantity == null)
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".مقدار تخصیصی باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                    //if (_beforeSelected.ItemData.availableNow==null)
                    //{
                    //    CurrentGroup = _beforeSelected;
                    //    MessageBox.Show(".موجودی فعلی باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    //    return;
                    //}
                    //if (string.IsNullOrEmpty(_beforeSelected.ItemData.description))
                    //{
                    //    CurrentGroup = _beforeSelected;
                    //    MessageBox.Show(".توضیحات کالا باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    //    return;
                    //}
                    //if (_beforeSelected.ItemData.emergencyStorage==null)
                    //{
                    //    CurrentGroup = _beforeSelected;
                    //    MessageBox.Show(".مقدار ذخیره احتیاطی باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    //    return;
                    //}

                    if (_beforeSelected.ItemData.type == null)
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".نوع کالا باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }

                    if (_beforeSelected.ItemData.type == 1)
                    {
                        if (_beforeSelected.ItemData.supplier == null)
                        {
                            CurrentGroup = _beforeSelected;
                            MessageBox.Show(".اطلاعات تامین کننده باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            return;
                        }
                        if (_beforeSelected.ItemData.supplyTime == null)
                        {
                            CurrentGroup = _beforeSelected;
                            MessageBox.Show(".زمان تهیه به روز باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            return;
                        }
                        if (_beforeSelected.ItemData.supplyTime < 0)
                        {
                            CurrentGroup = _beforeSelected;
                            MessageBox.Show(".زمان حمل و نقل باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            return;
                        }
                    }
                    else
                    {
                        if (_beforeSelected.ItemData.machineType == null)
                        {
                            CurrentGroup = _beforeSelected;
                            MessageBox.Show(".نوع منبع تولیدی باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            return;
                        }
                        //if (string.IsNullOrEmpty(_beforeSelected.ItemData.operationDescription))//todo: this field not in view but in table "madite"
                        //{
                        //    CurrentGroup = _beforeSelected;
                        //    MessageBox.Show(".شرح عملیات باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        //    return;
                        //}
                        if (string.IsNullOrEmpty(_beforeSelected.ItemData.operationName))
                        {
                            CurrentGroup = _beforeSelected;
                            MessageBox.Show(".عنوان عملیات باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            return;
                        }
                        if (_beforeSelected.ItemData.prepareTime == null)
                        {
                            CurrentGroup = _beforeSelected;
                            MessageBox.Show(".زمان آماده سازی باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            return;
                        }
                        if (_beforeSelected.ItemData.buildUnitTime == null)
                        {
                            CurrentGroup = _beforeSelected;
                            MessageBox.Show(".زمان تولید یک واحد باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            return;
                        }
                        if (_beforeSelected.ItemData.shipmentTime == null)
                        {
                            CurrentGroup = _beforeSelected;
                            MessageBox.Show(".زمان حمل و نقل باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            return;
                        }
                        if (Math.Abs(_beforeSelected.ItemData.transmitGroupSize ?? 0) < 0.000001)//equal with: _beforeSelected.ItemData.transmitGroupSize==0
                        {
                            CurrentGroup = _beforeSelected;
                            MessageBox.Show(".اندازه دسته انتقالی باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            return;
                        }
                        if (_beforeSelected.ItemData.buildUnitTime < 0)
                        {
                            CurrentGroup = _beforeSelected;
                            MessageBox.Show(".زمان تولید یک واحد باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            return;
                        }
                        if (_beforeSelected.ItemData.shipmentTime < 0)
                        {
                            CurrentGroup = _beforeSelected;
                            MessageBox.Show(".زمان حمل و نقل باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            return;
                        }
                        if (_beforeSelected.ItemData.prepareTime < 0)
                        {
                            CurrentGroup = _beforeSelected;
                            MessageBox.Show(".زمان آماده سازی باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            return;
                        }
                    }
                    if (_beforeSelected.ItemData.size == null)
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".سفارش دهی متغیر یا ثابت باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                    if (string.IsNullOrEmpty(_beforeSelected.ItemData.title))
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".عنوان کالا باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                    if (_beforeSelected.ItemData.unit == null)
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".واحد اندازه گیری کالا باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                    if (_beforeSelected.ItemData.wastedQuantity == null)
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".مقدار ضایعات باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                    if (_beforeSelected.ItemData.code.Length > 20)
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".کد کالا باید حداکثر 20 رقمی باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                    if (NewActivated && _beforeSelected.ItemData.id == 0 && BLL.Items.IsItemCodeUniqe(_beforeSelected.ItemData.code) == false)
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".کد کالا باید منحصر به فرد باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                    if (_beforeSelected.ItemData.title.Length > 50)
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".عنوان کالا باید حداکثر 50 حرفی باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                    if (_beforeSelected.ItemData.quentity < 0)
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".مقدار سفارش باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                    if (_beforeSelected.ItemData.assignedQuantity < 0)
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".مقدار تخصیصی باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                    if (_beforeSelected.ItemData.wastedQuantity < 0)
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".مقدار ضایعات باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                    if (_beforeSelected.ItemData.transmitGroupSize < 0)
                    {
                        CurrentGroup = _beforeSelected;
                        MessageBox.Show(".دسته انتقالی باید بزرگتر با مساوی صفر باشد", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        return;
                    }
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        //private object _copyOrCutItem;
        private void Structure(object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "add":
                        {
                            AjzayeKalaHeight = AjzayeKalaHeight == "0" ? "Auto" : "0";
                            break;
                        }
                    case "moveWindow":
                        {
                            TransferWindowShowHide = !TransferWindowShowHide;
                            _isHideDelegate.Invoke();
                            //ListHeight="*";
                            //TreeHeight="*";
                            //Items = _items.Where(r => r.type==2).ToList();
                            //OnPropertyChanged("Items");
                            break;
                        }
                    case "move":
                        {
                            //ListHeight="*";
                            //TreeHeight="*";
                            TransferWindowShowHide = true;
                            _isHideDelegate.Invoke();
                            //_copyOrCutItem = CurrentGroup;
                            _isCopy = false;
                            PasteActive = true;
                            break;
                        }
                    case "copy":
                        {
                            //ListHeight = "*";
                            //TreeHeight="*";
                            TransferWindowShowHide = true;
                            _isHideDelegate.Invoke();
                            //_copyOrCutItem = CurrentGroup;
                            _isCopy = true;
                            PasteActive = true;
                            break;
                        }
                    case "paste":
                        {
                            if (CurrentGroup == null)
                            {
                                MessageBox.Show(".اطلاعات رونوشت یا انتقال از بین رفته است", "خطا در اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            if (CurrentMoveItem == null)
                            {
                                MessageBox.Show(".ابتدا یک مورد را از پنجره انتقال انتخاب نمایید", "انتخاب یک مورد", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            if (CurrentGroup.ItemData.id == CurrentMoveItem.id)
                            {
                                MessageBox.Show(".یک کالا نمی تواند زیر مجوعه خودش باشد", "خطا", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            if (CurrentGroup.ItemData.parents.Count(r => r.id == CurrentMoveItem.id) != 0)
                            {
                                MessageBox.Show(".این مورد در این مسیر وجود دارد", "خطا", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            if (CurrentGroup == null)
                            {
                                MessageBox.Show(".ابتدا یک مورد را از پنجره ساختارکالا/فهرست کالا انتخاب نمایید", "انتخاب یک مورد", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            var win = new InputBoxWindow("مقدار کالای انتقالی را وارد نمایید:", "مقدار؟");
                            bool errorInData = false;
                            //agar addad vared nakard copy anjam nashe
                            win.Closing += (o, e) =>
                            {
                                if (win.DialogResult != true) return;
                                try
                                {
                                    var ddd = Convert.ToDouble(win.TextValue);
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show(".فیلد مقدار باید عددی وارد شود", "خطا", MessageBoxButton.OK,
                                             MessageBoxImage.Error, MessageBoxResult.OK,
                                             MessageBoxOptions.RightAlign);
                                    win.DialogResult = false;
                                    errorInData = true;
                                }
                            };
                            win.ShowDialog();
                            if (win.DialogResult == true)
                            {
                                var groups = new List<ItemInTree>();
                                var thisRes = CurrentGroup2.SubItemInTrees;
                                bool tanaghoz = false;
                                //thisRes = thisRes.ToList().Union(groups.SelectMany(ee => ee.SubItemInTrees).Where(r => r.ItemData.HasChanged).ToList());
                                while (thisRes.Count() != 0)
                                {
                                    groups = groups.Union(thisRes).ToList();
                                    if (groups.Count(r => r.ItemData.id == CurrentGroup.ItemData.id) > 0)
                                    {
                                        tanaghoz = true;
                                    }
                                    thisRes = thisRes.SelectMany(ee => ee.SubItemInTrees).ToList();
                                }
                                if (!tanaghoz)
                                {
                                    var thisRes2 = CurrentGroup2.ItemData.parents;
                                    var groups2 = new List<Item>();
                                    while (thisRes2.Count() != 0)
                                    {
                                        groups2 = groups2.Union(thisRes2).ToList();
                                        if (groups2.Count(r => r.id == CurrentGroup.ItemData.id) > 0)
                                        {
                                            tanaghoz = true;
                                        }
                                        thisRes2 = thisRes.SelectMany(ee => ee.ItemData.parents).ToList();
                                    }
                                }
                                if (tanaghoz)
                                {
                                    MessageBox.Show(".روابط کالای جز یا وابسته باعث به وجود آمدن حلقه یا موارد تکراری در ساختار درخت می شود", "خطا روابط کالاهای جز یا وابسته", MessageBoxButton.OK,
                                             MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                    return;
                                }
                                if (_isCopy)
                                {
                                    BLL.Items.addFatherForItem(CurrentGroup.ItemData.id, CurrentMoveItem.id, double.Parse(win.TextValue));
                                }
                                else
                                {
                                    if (CurrentGroup2.fatherId != null)
                                        BLL.Items.deleteFather(CurrentGroup.ItemData.id, CurrentGroup.fatherId ?? 0);
                                    BLL.Items.addFatherForItem(CurrentGroup.ItemData.id, CurrentMoveItem.id, double.Parse(win.TextValue));
                                }
                                var res = MessageBox.Show(".آیا می خواهید اطلاعات مجدداً بازیابی شوند", "بازیابی مجدد", MessageBoxButton.YesNo,
                                                MessageBoxImage.Question, MessageBoxResult.No,
                                                MessageBoxOptions.RightAlign);
                                if (res == MessageBoxResult.Yes)
                                {
                                    BLL.Items.ClearAllItems();
                                    _items = BLL.Items.allItems;
                                    LoadGroupData();
                                }
                                OnPropertyChanged("Groups");
                                OnPropertyChanged("CurrentGroup");
                            }
                            if (!errorInData)
                                PasteActive = false;
                            break;
                        }
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private bool _isCopy;
        //TransferList
        //TransferTree
        //inke fehrest faale ya sakhtar
        public bool TransferFehrest
        {
            set { _transferFehrest = value; OnPropertyChanged(); OnPropertyChanged("TransferSakhtar"); TransferList = _transferFehrest ? "*" : "0"; TransferTree = _transferFehrest ? "0" : "*"; }
            get { return _transferFehrest; }
        }

        public bool TransferSakhtar
        {
            get { return !_transferFehrest; }
        }

        private void SearchTextAction(object obj)
        {
            try
            {
                Items = _items.Where(r => r.title.Contains(obj.ToString())).OrderBy(r => r.title).ToList();
                OnPropertyChanged("Items");
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private void MoveSearchTextAction(object obj)
        {
            try
            {
                MoveItems = _items.Where(r => r.type == 2 && r.title.Contains(obj.ToString())).OrderBy(r => r.title).ToList();
                OnPropertyChanged("MoveItems");
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }
        //dige estefade nemishe
        public string TransferList
        {
            set { _transferList = value;OnPropertyChanged(); }
            get { return _transferList; }
        }

        public string TransferTree
        {
            set { _transferTree = value;OnPropertyChanged(); }
            get { return _transferTree; }
        }

        public int _parentId;
        private string _treeHeight;
        private string _listHeight;
        private string _searchText;
        private Item _currentItem;
        private bool _pasteActive;
        private bool _newActivated;
        private double _amount;
        private string _transferList;
        private string _transferTree;
        private bool _transferFehrest;
        private bool _transferSakhtar;
        private bool _transferWindowShowHide;
        private string _ajzayeKalaHeight;
        private double? _amountOfFather;
        private Item _currentChildItem;
        private double? _amountOfChild;
        private ItemInTree _currentGroup2;
        //dige estefade nemishe
        private RadTreeViewItem _currentGroup22;

        public void Crud(object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "new":
                        {
                            if (IsDepend)
                            {
                                if (CurrentGroup == null)
                                {
                                    MessageBox.Show(".ابتدا یک مورد را از پنجره ساختارکالا/فهرست کالا انتخاب نمایید", "انتخاب یک مورد", MessageBoxButton.OK,
                                                    MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                    return;
                                }
                                if (CurrentGroup.ItemData.type == 1)
                                {
                                    MessageBox.Show(".کالای انتخابی خریدنی است و نمیتواند دارای کالای جزیی باشد", "خطای نوع کالا", MessageBoxButton.OK,
                                                    MessageBoxImage.Error, MessageBoxResult.OK,
                                                    MessageBoxOptions.RightAlign);
                                    return;
                                }
                                var res = new InputBoxWindow("مقدار :", "مقدار مورد نیاز برای ساخت کالا");
                                res.ShowDialog();
                                res.Closing += (o, e) =>
                                {
                                    if (res.DialogResult != true) return;
                                    try
                                    {
                                        var ddd = Convert.ToDouble(res.TextValue);
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show(".فیلد مقدار باید عددی وارد شود", "خطا", MessageBoxButton.OK,
                                                 MessageBoxImage.Error, MessageBoxResult.OK,
                                                 MessageBoxOptions.RightAlign);
                                        res.DialogResult = false;
                                    }
                                };
                                if (res.DialogResult == true)
                                {
                                    Amount = Convert.ToDouble(res.TextValue);
                                    PasteActive = false;
                                    NewActivated = true;
                                    var item = new ItemInTree { Name = "", ItemData = new Item(""), SubItemInTrees = new List<ItemInTree>() };
                                    item.ItemData.size = false;
                                    CurrentGroup.SubItemInTrees.Add(item);
                                    CurrentGroup.ItemData.childrens.Add(item.ItemData);
                                    _parentId = CurrentGroup.ItemData.id;
                                    _beforeSelected = item;
                                    _treeItemLoaded.Invoke();
                                }
                                return;
                            }
                            {
                                _parentId = -1;
                                PasteActive = false;
                                NewActivated = true;
                                var item = new ItemInTree { Name = "", ItemData = new Item(""), SubItemInTrees = new List<ItemInTree>() };
                                item.ItemData.size = false;
                                Groups.Add(item);
                                _beforeSelected = item;
                                Disabled = true;
                                _currentGroup = new ItemInTree { ItemData = new Item("") };
                                OnPropertyChanged("Disabled");
                                _treeItemLoaded.Invoke();
                            }
                            break;
                        }
                    case "save":
                        {
                            if (!AllDataCompleted())
                                return;
                            var groups = Groups.Where(r => r.ItemData.HasChanged).ToList();
                            var thisRes = Groups.Where(r => r.ItemData.HasChanged);
                            //thisRes = thisRes.ToList().Union(groups.SelectMany(ee => ee.SubItemInTrees).Where(r => r.ItemData.HasChanged).ToList());
                            while (thisRes.Count() != 0)
                            {
                                groups = groups.Union(thisRes).ToList();
                                thisRes = thisRes.SelectMany(ee => ee.SubItemInTrees).Where(r => r.ItemData.HasChanged);
                            }
                            BLL.Items.AllItemSaveChanges(groups.Select(r => r.ItemData).ToList(), _parentId, Amount);

                            //var res = MessageBox.Show(".آیا می خواهید اطلاعات مجدداً بازیابی شوند", "بازیابی مجدد", MessageBoxButton.YesNo,
                            //                MessageBoxImage.Question, MessageBoxResult.No,
                            //                MessageBoxOptions.RightAlign);
                            //if (res==MessageBoxResult.Yes)
                            //{
                            if (CurrentGroup != null)
                                BeforeSelection = CurrentGroup.ItemData.id;
                            else
                                BeforeSelection = null;
                            BLL.Items.ClearAllItems();
                            _items = BLL.Items.allItems;
                            LoadGroupData();

                            //}
                            OnPropertyChanged("Groups");
                            OnPropertyChanged("CurrentGroup");
                            NewActivated = false;
                            //_treeItemLoaded.Invoke();
                            break;
                        }
                    case "reload":
                        {
                            BLL.Items.ClearAllItems();
                            _items = BLL.Items.allItems;
                            LoadGroupData();
                            break;
                        }
                    case "cancel":
                        {
                            if (CurrentGroup == null)
                            {
                                MessageBox.Show(".ابتدا یک مورد را از پنجره ساختارکالا/فهرست کالا انتخاب نمایید", "انتخاب یک مورد", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            canceling = true;
                            if (CurrentGroup.Key == 0)
                                Groups.Remove(CurrentGroup);
                            if (NewActivated == false)
                            {
                                CurrentGroup.ItemData.loadAgain();
                                CurrentGroup.Name = CurrentGroup.ItemData.title;
                                CurrentGroup.Key = CurrentGroup.ItemData.id;
                                OnPropertyChanged("CurrentGroup");

                                CurrentItemType = ItemTypes.SingleOrDefault(r => r.id == CurrentGroup.ItemData.type);
                                if (CurrentGroup.ItemData.type == 2)
                                {
                                    KharidHeight = "*";
                                    TolidHeight = "0";
                                    OperationShowHide = false;
                                    SupplierShowHide = true;
                                    HeaderOfMadPur = "اطلاعات تامین کننده";

                                }
                                else
                                {
                                    OperationShowHide = true;
                                    SupplierShowHide = false;
                                    KharidHeight = "0";
                                    TolidHeight = "*";
                                    HeaderOfMadPur = "اطلاعات عملیات تولید";
                                }
                                if (CurrentGroup.ItemData.machineType != null)
                                    CurrentMachineType = MachineTypes.SingleOrDefault(r => r.ID == CurrentGroup.ItemData.machineType.ID);
                                if (CurrentGroup.ItemData.unit != null)
                                    CurrentMeasuringType = MeasuringTypes.SingleOrDefault(r => r.ID == CurrentGroup.ItemData.unit.ID);
                                if (CurrentGroup.ItemData.type != null)
                                    CurrentItemType = ItemTypes.SingleOrDefault(r => r.id == CurrentGroup.ItemData.type);
                                if (CurrentGroup.ItemData.supplier != null)
                                    CurrentSupplier = Suppliers.SingleOrDefault(r => r.ID == CurrentGroup.ItemData.supplier.ID);
                                OnPropertyChanged("CurrentItemType");
                                OnPropertyChanged("CurrentMachineType");
                                OnPropertyChanged("CurrentMeasuringType");
                                OnPropertyChanged("CurrentSupplier");
                                CancleActivated = false;
                            }
                            else
                            {
                                _treeItemLoaded.Invoke();

                                NewActivated = false;
                                if (IsDepend)
                                {
                                    Crud("reload");
                                    return;
                                }
                            }
                            break;
                        }
                    case "search":
                        {
                            //TreeHeight = ListHeight == "*" ? "*" : "0";
                            //ListHeight = ListHeight == "*" ? "0" : "*";
                            if (MaterialStractureShowHide == false)
                            {
                                FehrestWindowShowHide = false;
                                MaterialStractureShowHide = true;
                                _hideDelegate.Invoke();
                            }
                            else
                            {
                                FehrestWindowShowHide = true;
                                MaterialStractureShowHide = false;
                            }
                            //_isHideDelegate.Invoke();
                            break;
                        }
                    case "del":
                        {
                            if (CurrentGroup == null)
                            {
                                MessageBox.Show(".ابتدا یک مورد را انتخاب نمایید", "انتخاب یک مورد", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            if (CurrentGroup.SubItemInTrees.Count > 0)
                            {
                                MessageBox.Show(".این مورد شامل اجزا می باشد. ابتدا آنها را حذف نمایید", "شامل اجزا", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            if (CurrentGroup.ItemData.parents.Count > 0)
                            {
                                MessageBox.Show(".این مورد شامل کالای وابسته می باشد. ابتدا این کالا را مستقل نمایید", "شامل اجزا", MessageBoxButton.OK,
                                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                return;
                            }
                            var res = MessageBox.Show("آیا مایل به حذف گزینه انتخابی در درخت هستید؟", "حذف شود؟", MessageBoxButton.YesNo,
                                            MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.RightAlign);
                            if (res == MessageBoxResult.Yes)
                            {
                                if (CurrentGroup.Key == 0)
                                {
                                    Crud("cancel");
                                    return;
                                }
                                BLL.Items.deleteItem(CurrentGroup.Key);
                                Groups.Remove(CurrentGroup);
                                CurrentGroup = null;
                                _treeItemLoaded.Invoke();
                            }
                            break;
                        }
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        //dar sakhtare drakhti gharar midahad
        private void LoadGroupData(bool? first=null)
        {
            try
            {
                Groups = new List<ItemInTree>();
                Groups2 = new List<ItemInTree>();
                var orginalFathers = _items.Where(r => r.parents.Select(ee => ee.id).Count() == 0).OrderBy(r => r.id).ToList();
                Items = _items;
                OnPropertyChanged("Items");
                MoveItems = _items.Where(r => r.type == 2).OrderBy(r => r.id).ToList();
                LoadItems(orginalFathers, null, Groups, false);
                var orginalFathersMade = _items.Where(r => r.type == 2 && r.parents.Select(ee => ee.id).Count() == 0).OrderBy(r => r.id).ToList();
                LoadItems(orginalFathersMade, null, Groups2, true);
                _treeItemLoadonlyForDrop.Invoke();
                if (first != true)
                    _treeItemLoaded.Invoke();
                OnPropertyChanged("MoveItems");
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        protected double Amount
        {
            get { return _amount; }
            set { _amount = value;OnPropertyChanged(); }
        }

        private void ResetAllWindow(object obj)
        {
            try
            {
                _materialStracture = true;
                _imageAndFilm = true;
                _materialInfo = true;
                _operationInfo = true;
                _materialContains = true;
                _entriesInfoShowHide = true;
                _isHideDelegate.Invoke();
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        public string TolidHeight
        {
            set { _tolidHeight = value; OnPropertyChanged(); }
            get { return _tolidHeight; }
        }

        public string KharidHeight
        {
            set { _kharidHeight = value;OnPropertyChanged(); }
            get { return _kharidHeight; }
        }

        public List<MachineType> MachineTypes { set; get; }
        public BackColorOfData CurrentBackColorOfData { set; get; }
        public MachineType CurrentMachineType
        {
            set
            {
                _currentMachineType = value;
                OnPropertyChanged();

                if (CurrentGroup != null)
                {
                    if (CurrentGroup.ItemData != null)
                    {
                        CurrentGroup.ItemData.machineType = value;
                    }
                }
            }
            get { return _currentMachineType; }
        }

        //sabet ya motaghayer 
        public bool SizeNot { get { try { return  !(CurrentGroup.ItemData.size??true); } catch { return false; } } }
        
        public ICommand ResetAllWindowClick { set; get; }
        
        public ICommand CrudClick { set; get; }
        
        public IList<ItemInTree> Groups { set; get; }
        
        public IList<ItemInTree> Groups2 { set; get; }
        
        public ItemInTree CurrentGroup
        {
            set
            {
                _currentSupplier = null;
                _currentMachineType = null;
                _currentItemType = null;
                _currentMeasuringType = null;
                OnPropertyChanged("CurrentMeasuringType");
                OnPropertyChanged("CurrentMachineType");
                OnPropertyChanged("CurrentSupplier");
                OnPropertyChanged("CurrentItemType");
                _currentGroup = value;
                OnPropertyChanged();
                OnPropertyChanged("CurrentGroup.ItemData");
                OnPropertyChanged("SizeNot");
                if (CurrentGroup == null)
                {
                    Disabled = false;
                    OnPropertyChanged("Disabled");
                    return;
                }
                CurrentBackColorOfData = new BackColorOfData();
                Disabled = true;
                OnPropertyChanged("CancleActivated");
                OnPropertyChanged("Disabled");
                if (CurrentGroup.ItemData == null) return;
                TreeSelectionChangedAction();
                //NewActivated = CurrentGroup.ItemData.id == 0;
                var val = BLL.Items.getProStrByParentId(CurrentGroup.Key);
                CurrentGroup.SubItemInTrees.ToList().ForEach(t =>
                    {
                        var val2 = val.SingleOrDefault(r => r.chinum == t.ItemData.id);
                        if (val2 != null)
                        {
                            t.ItemData.childQuantity = val2.strrat ?? 0;
                            t.ItemData.childDescription = val2.stretc;
                        }
                    });
                if (CurrentGroup.Key != 0)
                {
                    val = BLL.Items.getProStrByChildId(CurrentGroup.Key);
                    CurrentGroup.ItemData.parents.ToList().ForEach(t =>
                        {
                            var val2 = val.SingleOrDefault(r => r.parnum == t.id);
                            if (val2 != null)
                            {
                                t.parentQuantity = val2.strrat ?? 0;
                                t.parentDescription = val2.stretc;
                            }
                        });
                }
                if (MachineTypes!=null && MachineTypes.Count>0)
                {
                    if (CurrentGroup.ItemData.machineType != null)
                        CurrentMachineType = MachineTypes.SingleOrDefault(r => r.ID == CurrentGroup.ItemData.machineType.ID);
                }
                if (ItemTypes!=null && ItemTypes.Count>0)
                {
                    CurrentItemType = ItemTypes.SingleOrDefault(r => r.id == CurrentGroup.ItemData.type);
                    if (CurrentGroup.ItemData.type == 2)
                    {
                        KharidHeight = "*";
                        TolidHeight = "0";
                        OperationShowHide = false;
                        SupplierShowHide = true;
                        HeaderOfMadPur = "اطلاعات تامین کننده";

                    }
                    else
                    {
                        OperationShowHide = true;
                        SupplierShowHide = false;
                        KharidHeight = "0";
                        TolidHeight = "*";
                        HeaderOfMadPur = "اطلاعات عملیات تولید";
                    }
                }
                _IsHideMethodForOperationSuplierDelegateV.Invoke();
                //_isHideDelegate.Invoke();
                _currentMeasuringType = null;

                if (MeasuringTypes != null && MeasuringTypes.Count > 0)
                {
                    if (CurrentGroup.ItemData.unit != null)
                        CurrentMeasuringType = MeasuringTypes.SingleOrDefault(r => r.ID == CurrentGroup.ItemData.unit.ID);
                }
                _currentItemType = null;

                if (ItemTypes != null && ItemTypes.Count > 0)
                {
                    if (CurrentGroup.ItemData.type != null)
                        CurrentItemType = ItemTypes.SingleOrDefault(r => r.id == CurrentGroup.ItemData.type);
                }
                if (Suppliers != null && Suppliers.Count > 0)
                {
                    if (CurrentGroup.ItemData.supplier != null)
                        CurrentSupplier = Suppliers.SingleOrDefault(r => r.ID == CurrentGroup.ItemData.supplier.ID);
                }
            }
            get { return _currentGroup; }
        }

        public string HeaderOfMadPur
        {
            set { _headerOfMadPur = value; OnPropertyChanged(); }
            get { return _headerOfMadPur; }
        }

        public MeasuringType CurrentMeasuringType
        {
            set 
            { 
                _currentMeasuringType = value;
                OnPropertyChanged();
                if (CurrentGroup!=null)
                {
                    if (CurrentGroup.ItemData!=null)
                    {
                        CurrentGroup.ItemData.unit = value;
                    }
                    //_IsHideMethodForOperationSuplierDelegateV.Invoke();
                    _isHideDelegate.Invoke();
                }
            }
            get { return _currentMeasuringType; }
        }
        
        public List<Supplier> Suppliers { set; get; }
        public RadTreeViewItem CurrentGroup22
        {
            set
            {
                _currentGroup22 = value;
                if (value == null)
                {
                    return;
                }
                CurrentMoveItem = MoveItems.Single(r => r.id ==(int) CurrentGroup22.Tag);
            }
            get { return _currentGroup22; }
        }

        public Supplier CurrentSupplier
        {
            set
            {
                _currentSupplier = value; OnPropertyChanged();
                if (CurrentGroup != null)
                {
                    if (CurrentGroup.ItemData != null)
                    {
                        CurrentGroup.ItemData.supplier = value;
                    }
                }
            }
            get { return _currentSupplier; }
        }

        public List<MeasuringType> MeasuringTypes
        {
            set
            {
                _measuringTypes = value;OnPropertyChanged();
            }
            get { return _measuringTypes; }
        }

        public bool IsDepend
        {
            get { return !_isIndepend; }
        }

        public bool IsIndepend
        {
            set
            {
                _isIndepend = value;
                OnPropertyChanged();
                OnPropertyChanged("IsDepend");
            }
            get { return _isIndepend; }
        }

        //private bool tikNow;
        //private bool _imageAndFilmShow;

        public bool EntriesInfoShowHide
        {
            set
            {
                _entriesInfoShowHide = value;
                OnPropertyChanged();
                _isHideDelegate.Invoke();
            }
            get { return _entriesInfoShowHide; }
        }

        public bool ImageAndFilmShowHide
        {
            set
            {
                _imageAndFilm = value;
                OnPropertyChanged();
                //tikNow = true;
                _isHideDelegate.Invoke();
            }
            get { return _imageAndFilm; }
        }

        public bool MaterialContainsShowHide
        {
            set
            {
                _materialContains = value;
                OnPropertyChanged();
                _isHideDelegate.Invoke();
            }
            get { return _materialContains; }
        }

        public bool TransferWindowShowHide
        {
            set { _transferWindowShowHide = value; OnPropertyChanged();}
            get { return _transferWindowShowHide; }
        }

        public bool OperationInfoShowHide
        {
            set
            {
                _operationInfo = value;
                OnPropertyChanged();
                _isHideDelegate.Invoke();
            }
            get { return _operationInfo; }
        }

        public bool MaterialInfoShowHide
        {
            set
            {
                _materialInfo = value;
                OnPropertyChanged();
                _isHideDelegate.Invoke();
            }
            get { return _materialInfo; }
        }


        //public bool ImageAndFilmShow
        //{
        //    set { _imageAndFilmShow = value; _imageAndFilm = !value;OnPropertyChanged("ImageAndFilmShowHide"); }
        //    get { return _imageAndFilmShow; }
        //}

        public bool MaterialStractureShowHide
        {
            set
            {
                _materialStracture = value;
                OnPropertyChanged();
                _isHideDelegate.Invoke();
            }
            get { return _materialStracture; }
        }

        private bool _fehrestWindowShowHide;
        private bool _supplierShowHide;
        private bool _operationShowHide;

        public bool FehrestWindowShowHide
        {
            set
            {
                _fehrestWindowShowHide = value;
                OnPropertyChanged();
                _isHideDelegate.Invoke();
            }
            get { return _fehrestWindowShowHide; }
        }

        public void LoadItems(List<Item> items, ItemInTree parent, IList<ItemInTree> grp,bool typeMade)
        {
            try
            {
                foreach (var item in items)
                {
                    if (item == null)
                        continue;
                    int? fId;
                    if (parent == null)
                        fId = null;
                    else
                        fId = parent.ItemData.id;
                    var grp1 = new ItemInTree { Key = item.id, Name = item.title, SubItemInTrees = new List<ItemInTree>(), ItemData = item, fatherId = fId };
                    if (item.childrens != null)
                    {
                        if (typeMade)
                        {
                            if (item.childrens != null)
                            {
                                var item2 = item.childrens.Where(r => r != null && r.type == 2).ToList();
                                if (item2.Count > 0)
                                {
                                    LoadItems(item2, grp1, grp, true);
                                }
                            }
                        }
                        else
                            LoadItems(item.childrens, grp1, grp, false);
                    }
                    if (parent == null)
                        grp.Add(grp1);
                    else
                        parent.SubItemInTrees.Add(grp1);
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        public void Load()
        {
            try
            {
                var grp1 = new ItemInTree { Key = 1, Name = "Group 1", SubItemInTrees = new List<ItemInTree>(), };// Entries = new List<Entry>() };
                var grp2 = new ItemInTree { Key = 2, Name = "Group 2", SubItemInTrees = new List<ItemInTree>(), };// Entries = new List<Entry>() };
                var grp3 = new ItemInTree { Key = 3, Name = "Group 3", SubItemInTrees = new List<ItemInTree>(), };// Entries = new List<Entry>() };
                var grp4 = new ItemInTree { Key = 4, Name = "Group 4", SubItemInTrees = new List<ItemInTree>(), };// Entries = new List<Entry>() };

                //grp1
                grp1.SubItemInTrees.Add(new ItemInTree { Key = 1, Name = "Entry number 1", SubItemInTrees = new List<ItemInTree>() });
                grp1.SubItemInTrees.Add(new ItemInTree { Key = 2, Name = "Entry number 2", SubItemInTrees = new List<ItemInTree>() });
                grp1.SubItemInTrees.Add(new ItemInTree { Key = 3, Name = "Entry number 3", SubItemInTrees = new List<ItemInTree>() });

                //grp2
                grp2.SubItemInTrees.Add(new ItemInTree { Key = 4, Name = "Entry number 4", SubItemInTrees = new List<ItemInTree>() });
                grp2.SubItemInTrees.Add(new ItemInTree { Key = 5, Name = "Entry number 5", SubItemInTrees = new List<ItemInTree>() });
                grp2.SubItemInTrees.Add(new ItemInTree { Key = 6, Name = "Entry number 6", SubItemInTrees = new List<ItemInTree>() });

                //grp3
                grp3.SubItemInTrees.Add(new ItemInTree { Key = 7, Name = "Entry number 7", SubItemInTrees = new List<ItemInTree>() });
                grp3.SubItemInTrees.Add(new ItemInTree { Key = 8, Name = "Entry number 8", SubItemInTrees = new List<ItemInTree>() });
                grp3.SubItemInTrees.Add(new ItemInTree { Key = 9, Name = "Entry number 9", SubItemInTrees = new List<ItemInTree>() });

                //grp4
                grp4.SubItemInTrees.Add(new ItemInTree { Key = 10, Name = "Entry number 10", SubItemInTrees = new List<ItemInTree>() });
                grp4.SubItemInTrees.Add(new ItemInTree { Key = 11, Name = "Entry number 11", SubItemInTrees = new List<ItemInTree>() });
                grp4.SubItemInTrees.Add(new ItemInTree { Key = 12, Name = "Entry number 12", SubItemInTrees = new List<ItemInTree>() });

                grp4.SubItemInTrees.Add(grp1);
                grp2.SubItemInTrees.Add(grp4);

                Groups.Add(grp1);
                Groups.Add(grp2);
                Groups.Add(grp3);
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

