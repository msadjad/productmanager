using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using IMPOS.Annotations;
using IMPOS.BLL;
using IMPOS.Common;
using IMPOS.Helpers;
using IMPOS.ViewModel;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Docking;

namespace IMPOS.Views
{
    /// <summary>
    /// Interaction logic for MaterialManagementView.xaml
    /// </summary>
    public partial class MaterialManagementView:INotifyPropertyChanged
    {
        //public double FirstColWidth
        //{
        //    set { _firstColWidth = value;OnPropertyChanged();}
        //    get { return _firstColWidth; }
        //}
        private MaterialManagementViewModel _materialManagementViewModel;
        public MaterialManagementView()
        {
            try
            {

                InitializeComponent();
                this.DataContextChanged += MaterialManagementView_DataContextChanged;
                serverInfoManager.appData = GetUserAppDataPath();
                //MessageBox.Show(serverInfoManager.appData);
                serverInfoManager.readInfoFormFile();
                IsHideDelegate = IsHideMethod;
                treeItemLoaded = GetTreeItem;
                TreeItemLoadonlyForDrop = LoadTreeDataOnlyForDrop;
                fockes = FocusMethod;
                IsHideMethodForOperationSuplierDelegateV = IsHideMethodForOperationSuplier;
                _materialManagementViewModel = new MaterialManagementViewModel(IsHideMethodForOperationSuplierDelegateV, fockes, IsHideDelegate, treeItemLoaded, TreeItemLoadonlyForDrop);
                DataContext = _materialManagementViewModel;
                #region Size Fitting
                var sss = radDocking.SplitContainers.ToList();
                var width = SystemParameters.WorkArea.Width / 4 - 2;
                sss[0].Width = width;
                sss[1].Width = width;
                sss[2].Width = width;
                sss[3].Width = width;
                #endregion

                MoveWindow.IsHidden = true;
                ffff.Background = Brushes.Blue;
                //_materialManagementViewModel.IsIndepend = ;
                //_materialManagementViewModel.MaterialStractureShowHide = !MaterialInfo.IsHidden;
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        //private static bool _fistRunHide;
        //void IsFistRunHide()
        //{
        //    _fistRunHide = true;
        //}
        void MaterialManagementView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                tree2.Items.Clear();
                LoadItems(_materialManagementViewModel.Groups2.ToList(), null);
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message,"Error description",MessageBoxButton.OK);
            }
        }

        private void LoadTreeDataOnlyForDrop()
        {
            try
            {
                if (!_notFirstRun)
                    return;
                tree2.Items.Clear();
                LoadItems(_materialManagementViewModel.Groups2.ToList(), null);
                //var fff = new RadTreeViewItem();
                //vm.Groups2.ToList().ForEach(ttt =>
                //    {
                //        fff.Header = ttt.ItemData.title;
                //        fff.Tag = ttt.ItemData.id;
                //        tree2.Items.Add(fff);
                //    });
                //fff.Tag = 33;
                //fff.Header = "111111111122222222222222";
            
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }
        public void LoadItems(List<Helpers.ItemInTree> items, RadTreeViewItem parent)
        {
            try
            {
                foreach (var item in items)
                {
                    if (item == null)
                        continue;
                    var grp1 = new RadTreeViewItem { Tag = item.ItemData.id, Header = item.ItemData.title };
                    if (item.SubItemInTrees != null)
                        LoadItems(item.SubItemInTrees.ToList(), grp1);
                    if (parent == null)
                        tree2.Items.Add(grp1);
                    else
                        parent.Items.Add(grp1);
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }


        private double _deferentWithWorkArea;
        private bool _notFirstRun;

        private void IsHideMethodForOperationSuplier()
        {
            try
            {
                if (OperationInfoShowHide.IsChecked ?? false)
                {
                    OperationInfo.IsHidden = _materialManagementViewModel.OperationShowHide;
                    suplierInfo.IsHidden = _materialManagementViewModel.SupplierShowHide;
                }
                else
                {
                    OperationInfo.IsHidden = _materialManagementViewModel.OperationShowHide;
                    suplierInfo.IsHidden = _materialManagementViewModel.SupplierShowHide;
                    
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private void IsHideMethod()
        {
            try
            {
                //var vm = (MaterialManagementViewModel) DataContext;
                //StractureMaterial.IsHidden = !vm.MaterialStractureShowHide;
                //ImageAndFilm.IsHidden = !vm.ImageAndFilmShowHide;
                //EntriesInfo.IsHidden = !vm.EntriesInfoShowHide;
                //MaterialContains.IsHidden = !vm.MaterialContainsShowHide;
                //MaterialInfo.IsHidden = !vm.MaterialInfoShowHide;
                //OperationInfo.IsHidden = !vm.OperationInfoShowHide;
                FehrestMaterial.IsHidden = _materialManagementViewModel.FehrestWindowShowHide;
                StractureMaterial.IsHidden = _materialManagementViewModel.MaterialStractureShowHide;
                MaterialStractureShowHide.IsChecked = !_materialManagementViewModel.MaterialStractureShowHide;
                MoveWindow.IsHidden = !_materialManagementViewModel.TransferWindowShowHide;
                //if (_fistRunHide)
                //{
                //    //object tt = _materialManagementViewModel.CurrentItemType;
                //    //MadeOrPurchase.SelectedItem = null;
                //    //var ttt = (List<ItemType>)MadeOrPurchase.ItemsSource;
                //    //MadeOrPurchase.SelectedItem = ttt.Single(r => r.id == ((ItemType)tt).id);
                //}
                //_fistRunHide = false;
                //var tt= OperationInfo.TitleTemplate;
                //var dt = (TextBlock)(tt.Resources.["s22"]);
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        public delegate void fockesDelegate();
        public delegate void TreeItemLoaded();
        public delegate void IsHide();
        public delegate void IsHideMethodForOperationSuplierDelegate();
        public delegate void TreeItemLoadonlyForDropDelegate();
        public fockesDelegate fockes;
        public TreeItemLoaded treeItemLoaded;
        public TreeItemLoadonlyForDropDelegate TreeItemLoadonlyForDrop;
        public IsHide IsHideDelegate;
        public IsHideMethodForOperationSuplierDelegate IsHideMethodForOperationSuplierDelegateV;

        public void GetTreeItem()
        {
            try
            {

                int? id = null;
                if (_materialManagementViewModel.CurrentGroup != null && _materialManagementViewModel.CurrentGroup.ItemData != null)
                    id = _materialManagementViewModel.CurrentGroup.ItemData.id;
                _truePath = "";
                if (id != null)
                {
                    foreach (ItemInTree itemInTree in _materialManagementViewModel.Groups)
                    {
                        findItem((int)id, itemInTree.ItemData.id.ToString(), itemInTree);
                    }
                    if (_truePath != null)
                    {
                        var pathes = _truePath.Split(';');
                        ItemInTree item = null;
                        foreach (var pathe in pathes)
                        {
                            if (pathe.Length == 0)
                                continue;
                            if (item == null)
                                item = _materialManagementViewModel.Groups.First(r => r.ItemData.id == int.Parse(pathe));
                            else
                                item = item.SubItemInTrees.First(r => r.ItemData.id == int.Parse(pathe));
                            item.IsExpanded = true;
                        }
                        if (_materialManagementViewModel.NewActivated && _materialManagementViewModel.IsDepend)
                        {
                            item = item.SubItemInTrees.FirstOrDefault(r => r.ItemData.id == 0);
                            if (item != null) item.IsSelected = true;
                        }
                        else
                            if (_materialManagementViewModel.NewActivated && _materialManagementViewModel.IsIndepend)
                            {
                                item = _materialManagementViewModel.Groups.FirstOrDefault(r => r.ItemData.id == 0);
                                if (item != null) item.IsSelected = true;
                            }
                            else if (item.ItemData.id == id)
                                item.IsSelected = true;
                            else
                            {
                                item = item.SubItemInTrees.FirstOrDefault(r => r.ItemData.id == id);
                                if (item != null) item.IsSelected = true;
                            }
                    }
                }

                tree.ItemsSource = null;
                tree.ItemsSource = _materialManagementViewModel.Groups;
                if (_materialManagementViewModel.CurrentGroup != null)
                    FathersList.ItemsSource = _materialManagementViewModel.CurrentGroup.ItemData.parents;
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private string _truePath;
        void findItem(int id, string path,ItemInTree items)
        {

            try
            {
                if (items.ItemData.id == id)
                {
                    _truePath = path;
                    return;
                }
                foreach (var item in items.SubItemInTrees)
                {
                    if (item.ItemData.id == id)
                    {
                        _truePath = path;
                        return;
                    }
                    findItem(id, path + ";" + item.ItemData.id, item);
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            } 
        }

        public string GetUserAppDataPath()
        {
            try
            {
                string path = string.Empty;
                Assembly assm;
                Type at;
                object[] r;

                // Get the .EXE assembly
                assm = Assembly.GetEntryAssembly();
                // Get a 'Type' of the AssemblyCompanyAttribute
                at = typeof(AssemblyCompanyAttribute);
                // Get a collection of custom attributes from the .EXE assembly
                r = assm.GetCustomAttributes(at, false);
                // Get the Company Attribute
                AssemblyCompanyAttribute ct =
                              ((AssemblyCompanyAttribute)(r[0]));
                // Build the User App Data Path
                path = Environment.GetFolderPath(
                            Environment.SpecialFolder.ApplicationData);
                path += @"\IMPOS" + ct.Company;
                path += @"\" + assm.GetName().Version.ToString();

                return path;
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
            return null;
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                string xml;
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    using (var isoStream = storage.OpenFile("RadDocking_LastLayout.xml", FileMode.Create))
                    {
                        radDocking.SaveLayout(isoStream);
                        isoStream.Seek(0, SeekOrigin.Begin);
                        StreamReader reader = new StreamReader(isoStream);
                        xml = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                #region Size Fitting

                if (!_notFirstRun)
                {
                    _deferentWithWorkArea = e.NewSize.Width - SystemParameters.WorkArea.Width;
                    _notFirstRun = true;
                }
                var sss = radDocking.SplitContainers.ToList();
                var with = (e.NewSize.Width - _deferentWithWorkArea) / 4 - 2;
                sss[0].Width = with;
                sss[1].Width = with;
                sss[2].Width = with;
                sss[3].Width = with;
                _lastwidth = with;

                #endregion
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private double _lastwidth;
        #region Save And Load Dock Position

        private string SaveLayout()
        {
            try
            {
                //suplierInfo.IsHidden = true;
                string xml;
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    using (var isoStream = storage.OpenFile("RadDocking_Default.xml", FileMode.Create))
                    {
                        radDocking.SaveLayout(isoStream);
                        isoStream.Seek(0, SeekOrigin.Begin);
                        StreamReader reader = new StreamReader(isoStream);
                        xml = reader.ReadToEnd();
                    }
                }
                return xml;
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(SaveLayout());
        }

        private void LoadLayout()
        {
            try
            {
                // Load your layot from the isolated storage.
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    using (var isoStream = storage.OpenFile("RadDocking_Default.xml", FileMode.Open))
                    {
                        radDocking.LoadLayout(isoStream);
                    }
                }
                #region Size Fitting

                //var with = SystemParameters.WorkArea.Width / 4 - 2;
                var sss = radDocking.SplitContainers.ToList();

                sss[0].Width = _lastwidth;
                sss[1].Width = _lastwidth;
                sss[2].Width = _lastwidth;
                sss[3].Width = _lastwidth;


                ImageAndFilmShowHide.IsChecked = !ImageAndFilm.IsHidden;
                MaterialStractureShowHide.IsChecked = !StractureMaterial.IsHidden;
                EntriesInfoShowHide.IsChecked = !EntriesInfo.IsHidden;
                MaterialContainsShowHide.IsChecked = !MaterialContains.IsHidden;
                MaterialInfoShowHide.IsChecked = !MaterialInfo.IsHidden;
                OperationInfoShowHide.IsChecked = !MaterialInfo.IsHidden;
                MaterialFathersShowHide.IsChecked = !MaterialFathers.IsHidden;
                _materialManagementViewModel.FehrestWindowShowHide = FehrestMaterial.IsHidden;
                _materialManagementViewModel.MaterialStractureShowHide = StractureMaterial.IsHidden;
                _materialManagementViewModel.TransferWindowShowHide = MoveWindow.IsHidden;

                _materialManagementViewModel._imageAndFilm = ImageAndFilm.IsHidden;
                _materialManagementViewModel._materialInfo = MaterialInfo.IsHidden;
                _materialManagementViewModel._operationInfo = (!OperationInfo.IsHidden) || (!suplierInfo.IsHidden);
                _materialManagementViewModel._materialContains = MaterialContains.IsHidden;
                _materialManagementViewModel.EntriesInfoShowHide = EntriesInfo.IsHidden;
                #endregion

            }
            catch (Exception eeeeee)
            {
                MessageBox.Show(eeeeee.Message, "Error Description", MessageBoxButton.OK);
            }
        }

        #endregion

        private void treeViewProductStructure_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                long ID = ((Helpers.ItemInTree) ((RadTreeView) sender).SelectedItem).ItemData.id;
                BindImages(ID);
                btnAddImage.Visibility = Visibility.Visible;
                btnDelImage.Visibility = Visibility.Visible;
            }
            catch (NullReferenceException)
            {

            }
            catch (Exception eeeeee)
            {
                MessageBox.Show(eeeeee.Message, "Error Description", MessageBoxButton.OK);
            }
        }


        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("آیا مطمئن به حذف سند انتخابی هستید؟", "حذف شود؟", MessageBoxButton.YesNo,
                        MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.RtlReading) == MessageBoxResult.Yes)
                {
                    var items = (ImageEntity)(LsImageGallery).SelectedItem;
                    File.Delete(items.OrginalNameImagePath);
                    BindImages(((Helpers.ItemInTree) (tree).SelectedItem).ItemData.id);
                }
            }
            catch (Exception eeeeee)
            {
                MessageBox.Show(eeeeee.Message, "Error Description", MessageBoxButton.OK);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "JPG Files (*.jpg)|*.jpg|MP4 Files (*.MP4)|*.MP4|Avi Files (*.avi)|*.avi|3gp Files (*.3gp)|*.3gp|Asx Files (*.asx)|*.asx|Asf Files (*.asf)|*.asf|Wmv Files (*.wmv)|*.wmv|mkv Files (*.mkv)|*.mkv|flv Files (*.flv)|*.flv|mpv Files (*.mpv)|*.mpv|mov Files (*.mov)|*.mov|mp4 Files (*.mp4)|*.mp4|mpg Files (*.mpg)|*.mpg|vob Files (*.vob)|*.vob";
                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();


                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    long ID = ((Helpers.ItemInTree) (tree).SelectedItem).ItemData.id;
                    string filename = dlg.FileName;
                    ImageView.addImage("d:\\ProductImages", ID, filename);
                    BindImages(ID);
                }
            }
            catch (Exception eeeeee)
            {
                MessageBox.Show(eeeeee.Message, "Error Description", MessageBoxButton.OK);
            }
        }

        private void BindImages(long id)
        {
            try
            {
                // Store Data in List Object
                List<ImageEntity> ListImageObj = ImageView.GetAllImagesData("d:\\ProductImages", id);

                // Check List Object Count
                if (ListImageObj.Count > 0)
                {
                    // Bind Data in List Box Control.
                    LsImageGallery.DataContext = ListImageObj;

                }
                else
                    LsImageGallery.DataContext = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void MaterialManagementView_OnInitialized(object sender, EventArgs e)
        {
            try
            {
                //Load your layot from the isolated storage.
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    using (var isoStream = storage.OpenFile("RadDocking_LastLayout.xml", FileMode.Open))
                    {
                        radDocking.LoadLayout(isoStream);

                        ImageAndFilmShowHide.IsChecked = !ImageAndFilm.IsHidden;
                        MaterialStractureShowHide.IsChecked = !StractureMaterial.IsHidden;
                        EntriesInfoShowHide.IsChecked = !EntriesInfo.IsHidden;
                        MaterialContainsShowHide.IsChecked = !MaterialContains.IsHidden;
                        MaterialInfoShowHide.IsChecked = !MaterialInfo.IsHidden;
                        OperationInfoShowHide.IsChecked = !MaterialInfo.IsHidden;
                        MaterialFathersShowHide.IsChecked = !MaterialFathers.IsHidden;
                        _materialManagementViewModel.FehrestWindowShowHide = FehrestMaterial.IsHidden;
                        _materialManagementViewModel.MaterialStractureShowHide = StractureMaterial.IsHidden;
                        _materialManagementViewModel.TransferWindowShowHide = MoveWindow.IsHidden;

                        _materialManagementViewModel._imageAndFilm = ImageAndFilm.IsHidden;
                        _materialManagementViewModel._materialInfo = MaterialInfo.IsHidden;
                        _materialManagementViewModel._operationInfo = (!OperationInfo.IsHidden) || (!suplierInfo.IsHidden);
                        _materialManagementViewModel._materialContains = MaterialContains.IsHidden;
                        _materialManagementViewModel.EntriesInfoShowHide = EntriesInfo.IsHidden;
                    }
                }
            }
            catch (Exception)
            {
                SaveLayout();
            }

            try
            {
            #region Size Fitting

            var sss = radDocking.SplitContainers.ToList();
            var width = SystemParameters.WorkArea.Width / 4 - 2;
            sss[0].Width = width;
            sss[1].Width = width;
            sss[2].Width = width;
            sss[3].Width = width;
            #endregion
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }


        private void RadDocking_OnClose(object sender, StateChangeEventArgs e)
        {
            try
            {
                ImageAndFilmShowHide.IsChecked = !ImageAndFilm.IsHidden;
                MaterialStractureShowHide.IsChecked = !StractureMaterial.IsHidden;
                EntriesInfoShowHide.IsChecked = !EntriesInfo.IsHidden;
                MaterialContainsShowHide.IsChecked = !MaterialContains.IsHidden;
                MaterialInfoShowHide.IsChecked = !MaterialInfo.IsHidden;
                OperationInfoShowHide.IsChecked = !(OperationInfo.IsHidden && suplierInfo.IsHidden);
                MaterialFathersShowHide.IsChecked = !MaterialFathers.IsHidden;
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private void Cheked_OnClick(object sender, RoutedEventArgs e)
        {

            try
            {
                ImageAndFilm.IsHidden = !(ImageAndFilmShowHide.IsChecked ?? false);
                MaterialFathers.IsHidden = !(MaterialFathersShowHide.IsChecked ?? false);

                StractureMaterial.IsHidden = !(MaterialStractureShowHide.IsChecked ?? false);
                EntriesInfo.IsHidden = !(EntriesInfoShowHide.IsChecked ?? false);
                MaterialContains.IsHidden = !(MaterialContainsShowHide.IsChecked ?? false);
                MaterialInfo.IsHidden = !(MaterialInfoShowHide.IsChecked ?? false);
                if (OperationInfoShowHide.IsChecked ?? false)
                {
                    if (_materialManagementViewModel.CurrentGroup != null)
                    {
                        if (_materialManagementViewModel.CurrentGroup.ItemData != null)
                        {
                            if (_materialManagementViewModel.CurrentGroup.ItemData.type == 2)
                            {
                                suplierInfo.IsHidden = !(OperationInfoShowHide.IsChecked ?? false);
                            }
                            else
                                OperationInfo.IsHidden = !(OperationInfoShowHide.IsChecked ?? false);
                        }
                        else
                        {
                            OperationInfo.IsHidden = !(OperationInfoShowHide.IsChecked ?? false);
                            suplierInfo.IsHidden = (OperationInfoShowHide.IsChecked ?? false);
                        }
                    }
                    else
                    {
                        OperationInfo.IsHidden = !(OperationInfoShowHide.IsChecked ?? false);
                        suplierInfo.IsHidden = (OperationInfoShowHide.IsChecked ?? false);
                    }
                }
                else
                {
                    OperationInfo.IsHidden = !(OperationInfoShowHide.IsChecked ?? false);
                    suplierInfo.IsHidden = !(OperationInfoShowHide.IsChecked ?? false);
                }

            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ButtonBase_SaveClick(object sender, RoutedEventArgs e)
        {
            try
            {

                lastFocuse.Focus();
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private void radDocking_Show(object sender, StateChangeEventArgs e)
        {
            try
            {
                OperationInfoShowHide.IsChecked = !(OperationInfo.IsHidden && suplierInfo.IsHidden);
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private void Tree_OnDrop(object sender, DragEventArgs e)
        {
            try
            {
            MessageBox.Show(".رها کردن فقط در ساختار درختی پنجره انتقال مقدور است", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error, MessageBoxResult.OK,
                        MessageBoxOptions.RightAlign);
            _materialManagementViewModel.Crud("reload");
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        /*check for sample
         * private void hourCheck_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            int iValue = -1;

            if (Int32.TryParse(textBox.Text, out iValue) == false || (iValue < 0 || iValue > 23))
            {
                TextChange textChange = e.Changes.ElementAt<TextChange>(0);
                int iAddedLength = textChange.AddedLength;
                int iOffset = textChange.Offset;

                textBox.Text = textBox.Text.Remove(iOffset, iAddedLength);
                textBox.CaretIndex = iOffset;
            }
        }
         */

        private void itemCode_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                if (tree.SelectedItem != null && ((ItemInTree)tree.SelectedItem).ItemData.code != ((System.Windows.Controls.TextBox)sender).Text)
                {
                    int sw = 0;
                    for (int i = 0; i < Items.allItems.Count; i++)
                    {
                        if (Items.allItems[i].code == ((System.Windows.Controls.TextBox)sender).Text)
                        {
                            MessageBox.Show("کد کالای انتخابی تکراری است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                            e.Handled = true;
                            sw = 1;
                        }
                    }
                    if (sw == 0)
                    {
                        _materialManagementViewModel.ChangeForCanceling();
                        saveItem.IsEnabled = true;
                    }
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private void itemTitle_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                if (tree.SelectedItem != null && ((ItemInTree)tree.SelectedItem).ItemData.title != ((System.Windows.Controls.TextBox)sender).Text)
                {
                    _materialManagementViewModel.ChangeForCanceling();
                    saveItem.IsEnabled = true;
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tree.SelectedItem != null && ((ItemInTree)tree.SelectedItem).ItemData.HasChanged)
                {
                    _materialManagementViewModel.ChangeForCanceling();
                    saveItem.IsEnabled = true;
                }
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }


        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _materialManagementViewModel.ChangeForCanceling();
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                _materialManagementViewModel.ChangeForCanceling();
            }
            catch (Exception eeeee)
            {
                MessageBox.Show(eeeee.Message, "Error description", MessageBoxButton.OK);
            }
        }

        private void ButtonSearch_OnClick(object sender, RoutedEventArgs e)
        {
            sasa.Focus();
        }
        private void FocusMethod()
        {
            sasa.Focus();
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            LoadLayout();
        }

        private void LsImageGallery_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process myProcess = new Process();
            myProcess.StartInfo.FileName = ((ImageEntity)((System.Windows.Controls.ListBox)sender).SelectedItem).OrginalNameImagePath; //not the full application path
            //myProcess.StartInfo.Arguments = "/A \"page=2=OpenActions\" C:\\example.pdf";
            myProcess.Start();
            //if (((ImageEntity)((System.Windows.Controls.ListBox)sender).SelectedItem).ImagePath.ToLower().Contains("movie.png"))
            //    File.Open(((ImageEntity)((System.Windows.Controls.ListBox)sender).SelectedItem).OrginalNameImagePath,FileMode.Open);
            ;
        }

        private void FehrestMaterial_OnLoaded(object sender, RoutedEventArgs e)
        {
            sasa.Focus();
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            //_materialManagementViewModel.CurrentGroup.ItemData.HasChanged = true;
        }
    }
}
