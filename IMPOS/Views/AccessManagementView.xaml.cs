using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using IMPOS.Annotations;
using IMPOS.BLL;
using IMPOS.Common;
using IMPOS.ViewModel;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Docking;

namespace IMPOS.Views
{
    /// <summary>
    /// Interaction logic for MaterialManagementView.xaml
    /// </summary>
    public partial class AccessManagementView:INotifyPropertyChanged
    {
        private AccessManagementViewModel _accessManagementViewModel;
        public AccessManagementView()
        {
            InitializeComponent();
            serverInfoManager.appData = GetUserAppDataPath();
            serverInfoManager.readInfoFormFile();
            listLoaded = listLoadedCompelete;
            staffListLoaded = staffListLoadedCompelete;
            //IsHideDelegate = IsHideMethod;
            //#region Size Fitting
            //var sss = radDocking.SplitContainers.ToList();
            //var width = SystemParameters.WorkArea.Width / 4 - 2;
            //sss[0].Width = width;
            //sss[1].Width = width;
            //sss[2].Width = width * 2;
            ////sss[3].Width = width;
            //#endregion
            _accessManagementViewModel = new AccessManagementViewModel(listLoaded, staffListLoaded);
            DataContext = _accessManagementViewModel;

            MaterialStractureShowHide.IsChecked = true;
            
            MaterialContainsShowHide.IsChecked = true;
            MaterialInfoShowHide.IsChecked = true;
            OperationInfoShowHide.IsChecked = true;
            MaterialFathersShowHide.IsChecked = true;
        }



        private double _deferentWithWorkArea;
        private bool _notFirstRun;
        private void listLoadedCompelete()
        {
            OrgList.ItemsSource = null;
            OrgList.ItemsSource = _accessManagementViewModel.Organizations;
        }
        private void staffListLoadedCompelete()
        {
            StaffSubList.ItemsSource = null;
            if (_accessManagementViewModel.CurrentStaff != null)
                StaffSubList.ItemsSource = _accessManagementViewModel.CurrentStaff.allSubSystemStatuses;
            StaffList.ItemsSource = null;
            StaffList.ItemsSource = _accessManagementViewModel.CurrentOrganization.allStaff;
        }
        //private void IsHideMethod()
        //{
        //    MoveWindow.IsHidden = !_materialManagementViewModel.TransferWindowShowHide;
        //}

        public delegate void ListLoaded();
        public delegate void IsHide();
        public delegate void TreeItemLoadonlyForDropDelegate();
        public ListLoaded listLoaded;
        public ListLoaded staffListLoaded;
        public TreeItemLoadonlyForDropDelegate TreeItemLoadonlyForDrop;
        public IsHide IsHideDelegate;
        private double _firstColWidth;

        public string GetUserAppDataPath()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            //    {
            //        using (var isoStream = storage.OpenFile("RadDocking_LastLayoutAccessManagement.xml", FileMode.Open))
            //        {
            //            radDocking.LoadLayout(isoStream);
            //        }
            //    }
            //}
            //catch
            //{
            //}
            //var sss = radDocking.SplitContainers.ToList();
            //var width = SystemParameters.WorkArea.Width / 4 - 2;
            //sss[0].Width = width;
            //sss[1].Width = width;
            //sss[2].Width = width * 2;

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string xml;
            // Save your layout for example in the isolated storage.
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                using (var isoStream = storage.OpenFile("RadDocking_LastLayoutAccessManagement.xml", FileMode.Create))
                {
                    radDocking.SaveLayout(isoStream);
                    isoStream.Seek(0, SeekOrigin.Begin);
                    StreamReader reader = new StreamReader(isoStream);
                    xml = reader.ReadToEnd();
                    //isoStream.Seek(0, SeekOrigin.Begin);
                    //StreamWriter writer = new StreamWriter(isoStream);
                    //xml = xml.Replace("RadDocking><SplitContainers><RadSplitContainer", "RadDocking><SplitContainers><RadSplitContainer  Name=\"FirstCol\"");
                    //writer.Write(xml);
                    //writer.Close();
                    var sss = radDocking.SplitContainers.ToList();
                    var width = SystemParameters.WorkArea.Width / 4 - 2;
                    sss[0].Width = width;
                    sss[1].Width = width;
                    sss[2].Width = width * 2;
                    //sss[3].Width = with;

                }
            }
            // Return the generated XML
            //return xml;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            LoadLayout();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
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
            sss[2].Width = with * 2;
            //sss[3].Width = with;
            _lastwidth = with;

            #endregion
        }

        private double _lastwidth;
        #region Save And Load Dock Position

        private string SaveLayout()
        {
            string xml;
            // Save your layout for example in the isolated storage.
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                using (var isoStream = storage.OpenFile("RadDocking_DefaultAccessManagement.xml", FileMode.Create))
                {
                    radDocking.SaveLayout(isoStream);
                    isoStream.Seek(0, SeekOrigin.Begin);
                    StreamReader reader = new StreamReader(isoStream);
                    xml = reader.ReadToEnd();
                }
            }
            // Return the generated XML
            return xml;
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
                    using (var isoStream = storage.OpenFile("RadDocking_DefaultAccessManagement.xml", FileMode.Open))
                    {
                        radDocking.LoadLayout(isoStream);
                    }
                }
                #region Size Fitting

                //var with = SystemParameters.WorkArea.Width / 4 - 2;
                var sss = radDocking.SplitContainers.ToList();

                sss[0].Width = _lastwidth;
                sss[1].Width = _lastwidth;
                sss[2].Width = _lastwidth*2;
                //sss[3].Width = _lastwidth;
                #endregion

            }
            catch (Exception)
            {
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadLayout();
        }

        #endregion

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            //new MainWindow().Show();
        }





        private void MaterialManagementView_OnInitialized(object sender, EventArgs e)
        {
            try
            {
                // Load your layot from the isolated storage.
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    using (var isoStream = storage.OpenFile("RadDocking_LastLayoutAccessManagement.xml", FileMode.Open))
                    {
                        radDocking.LoadLayout(isoStream);
                    }
                }
            }
            catch (Exception)
            {
                SaveLayout();
            }
            #region Size Fitting

            var sss = radDocking.SplitContainers.ToList();
            var width = SystemParameters.WorkArea.Width / 4 - 2;
            StractureMaterial.CanUserClose = false;
            ((RadPane) FindName("StractureMaterial")).CanUserClose = false;
            sss[0].Width = width;
            sss[0].Width = width;
            sss[1].Width = width;
            sss[2].Width = width * 2;
            //sss[3].Width = width;
            #endregion
        }


        private void RadDocking_OnClose(object sender, StateChangeEventArgs e)
        {
            MaterialStractureShowHide.IsChecked = !MoveWindow.IsHidden;
            MaterialContainsShowHide.IsChecked = !MaterialContains.IsHidden;
            MaterialInfoShowHide.IsChecked = !MaterialInfo.IsHidden;
            OperationInfoShowHide.IsChecked = !OperationInfo.IsHidden;
            MaterialFathersShowHide.IsChecked = !MaterialFathers.IsHidden;
        }

        private void Cheked_OnClick(object sender, RoutedEventArgs e)
        {
            MaterialFathers.IsHidden = !(MaterialFathersShowHide.IsChecked??false);
            MoveWindow.IsHidden = !(MaterialStractureShowHide.IsChecked ?? false);
            MaterialContains.IsHidden = !(MaterialContainsShowHide.IsChecked ?? false);
            MaterialInfo.IsHidden = !(MaterialInfoShowHide.IsChecked ?? false);
            OperationInfo.IsHidden = !(OperationInfoShowHide.IsChecked ?? false);
            ImageAndFilm.IsHidden = !(OperationInfoShowHide.IsChecked ?? false);
            ImageAndFilm2.IsHidden = !(OperationInfoShowHide.IsChecked ?? false);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DataGrid_OnCurrentCellChanged(object sender, EventArgs e)
        {
            //purchaseGrid.SelectedItem = null;
            //purchaseGrid.SelectedItem = _accessManagementViewModel.CurrentPurchaseItem;
            //purchaseGrid.SelectedItem = ((List<ItemStatus>)purchaseGrid.ItemsSource)[1];
            //txtFockus.Focus();
            //purchaseGrid.SelectedItem = purchaseGrid.SelectedItem;
            //var value = (ItemStatus)purchaseGrid.SelectedItem;
            //if(value==null)return;
            //value.status = value.status;
        }
        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }
                DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                            cell.IsSelected = true;
                    }
                    else
                    {
                        DataGridRow row = FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                        {
                            row.IsSelected = true;
                        }
                    }
                }
            }
        }

        static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        } 
        private void ButtonStaffSave_OnClick(object sender, RoutedEventArgs e)
        {
            txtFockus.Focus();
        }

        private void PasswordBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (_accessManagementViewModel.CurrentStaff != null)
            {
                _accessManagementViewModel.CurrentStaff.password = PasswordBox.Password;
                _accessManagementViewModel.PasswordRpt = PasswordBoxrpt.Password;
            }
            else
            {
                MessageBox.Show("ابتدا یک کارمند را انتخاب نمایید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                PasswordBox.Password = "";
                PasswordBoxrpt.Password = "";
            }
        }
    }
}
