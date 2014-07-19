using System.Windows;
using System.Windows.Input;
using IMPOS.Annotations;
using IMPOS.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IMPOS.Views;
using Telerik.Windows.Controls;

namespace IMPOS.ViewModel
{
    public class AccessManagementViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private readonly AccessManagementView.ListLoaded _listLoaded;
        private readonly AccessManagementView.ListLoaded _staffListLoaded;
        private Organization _currentOrganization;
        private Staff _currentStaff;
        public List<Organization> Organizations { set; get; }

        public Staff CurrentStaff
        {
            set
            {
                if(AllDataCompleted()==false)return;
                _currentStaff = value;
                OnPropertyChanged("CurrentStaff");
                if (NewActivated)
                    TreeSelectionChangedAction();
                if (value == null)
                    DeleteEmployeeEnable = false;
                else
                    DeleteEmployeeEnable = true;
                PasswordRpt = null;
                OnPropertyChanged("DeleteEmployeeEnable");
            }
            get { return _currentStaff; }
        }

        public Organization CurrentOrganization
        {
            set
            {
                _currentOrganization = value;//CurrentOrganization.allStaff[].allSubSystemStatuses[].status
                OnPropertyChanged("CurrentOrganization");
                OnPropertyChanged("CurrentOrganization.allStaff");

                CurrentStaff = null;
                if (value != null)
                    DeleteEnable = true;
                else
                {
                    DeleteEnable = false;
                }
                OnPropertyChanged("DeleteEnable");
                //CurrentOrganization.allSubSystemStatuses
            }
            get { return _currentOrganization; }
        }

        public ItemStatus CurrentPurchaseItem { set; get; }
        public ICommand CrudClick { set; get; }
        public ICommand CrudEmployeeClick { set; get; }

        public AccessManagementViewModel(AccessManagementView.ListLoaded listLoaded, AccessManagementView.ListLoaded staffListLoaded)
        {
            _listLoaded = listLoaded;
            _staffListLoaded = staffListLoaded;
            Organizations = new List<Organization>();
            //SecurityManagementa = new SecurityManagements();
            SecurityManagements.loadOrganizationsFromDB();
            Organizations = SecurityManagements.allOrganizations;
            OnPropertyChanged("Organizations");
            CrudClick = new DelegateCommand(Crud);
            CrudEmployeeClick = new DelegateCommand(CrudEmployee);
            //CurrentOrganization.allPurchasedProductStatuses[].status
            //SecurityManagements.allSubSystems[].title

        }

        private void TreeSelectionChangedAction()
        {
            if (canceling)
            {
                canceling = false;
                return;
            }

            if (NewActivated && CurrentStaff != _beforeSelected)
            {

                if (_beforeSelected.password!=PasswordRpt)
                {
                    _currentStaff = _beforeSelected;
                    OnPropertyChanged("CurrentStaff");
                    MessageBox.Show(".رمز عبور با تکرار رمز عبور برابر نمی باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }
                if (string.IsNullOrEmpty(_beforeSelected.name))
                {
                    _currentStaff = _beforeSelected;
                    OnPropertyChanged("CurrentStaff");
                    MessageBox.Show(".نام و نام خانوادگی کارمند باید وارد شود", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }
                if (string.IsNullOrEmpty(_beforeSelected.username))
                {
                    _currentStaff = _beforeSelected;
                    OnPropertyChanged("CurrentStaff");
                    MessageBox.Show(".نام کاربری باید وارد شود", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }
                //if (string.IsNullOrEmpty(_beforeSelected.password))
                //{
                //    CurrentStaff = _beforeSelected;
                //    MessageBox.Show(".رمز عبور باید وارد شود", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                //    return;
                //}
                if (string.IsNullOrEmpty(_beforeSelected.code))
                {
                    _currentStaff = _beforeSelected;
                    OnPropertyChanged("CurrentStaff");
                    MessageBox.Show(".کد کارمند باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }
                if (_beforeSelected.code != null && _beforeSelected.code.Length> 20)
                {
                    _currentStaff = _beforeSelected;
                    OnPropertyChanged("CurrentStaff");
                    MessageBox.Show(".کد کارمند حداکثر 20 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }
                if (_beforeSelected.name != null && _beforeSelected.name.Length> 50)
                {
                    _currentStaff = _beforeSelected;
                    OnPropertyChanged("CurrentStaff");
                    MessageBox.Show(".نام کارمند حداکثر 50 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }

                if (_beforeSelected.job != null && _beforeSelected.job.Length> 50)
                {
                    _currentStaff = _beforeSelected;
                    OnPropertyChanged("CurrentStaff");
                    MessageBox.Show(".سمت کارمند حداکثر 50 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }

                if (_beforeSelected.tel != null && _beforeSelected.tel.Length> 50)
                {
                    _currentStaff = _beforeSelected;
                    OnPropertyChanged("CurrentStaff");
                    MessageBox.Show(".شماره تماس کارمند حداکثر 50 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }

                if (_beforeSelected.address != null && _beforeSelected.address.Length> 100)
                {
                    _currentStaff = _beforeSelected;
                    OnPropertyChanged("CurrentStaff");
                    MessageBox.Show(".نشانی کارمند حداکثر 100 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }

                if (_beforeSelected.description != null && _beforeSelected.description.Length> 200)
                {
                    _currentStaff = _beforeSelected;
                    OnPropertyChanged("CurrentStaff");
                    MessageBox.Show(".ملاحظات حداکثر 200 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }

                if (_beforeSelected.username != null && _beforeSelected.username.Length> 20)
                {
                    _currentStaff = _beforeSelected;
                    OnPropertyChanged("CurrentStaff");
                    MessageBox.Show(".نام کاربری حداکثر 20 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }

                if (_beforeSelected.password != null && _beforeSelected.password.Length> 20)
                {
                    _currentStaff = _beforeSelected;
                    OnPropertyChanged("CurrentStaff");
                    MessageBox.Show(".رمز عبور حداکثر 20 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }

            }
        }

        public bool NewActivated { get; set; }

        private void CrudEmployee(object obj)
        {
            switch (obj.ToString())
            {
                case "new":
                    {
                        //var win = new Views.InputBoxWindow("عنوان سازمان را وارد نمایید:", "نام سازمان");
                        //win.ShowDialog();
                        //if (win.DialogResult != true)
                        //    return;
                        //if (win.TextValue == null || string.IsNullOrEmpty(win.TextValue.Trim()))
                        //{
                        //    MessageBox.Show("عنوان سازمان باید وارد شود.", "خطا", MessageBoxButton.OK,
                        //        MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        //    return;
                        //}

                        if (CurrentOrganization == null)
                        {
                            MessageBox.Show("ابتدا یک سازمان را انتخاب کنید.", "خطا", MessageBoxButton.OK,
                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            return;
                        }
                        CurrentStaff = new Staff(CurrentOrganization.id);
                        int id = CurrentStaff.id;
                        CurrentOrganization.allStaff.Add(CurrentStaff);
                        _beforeSelected = CurrentStaff;
                        CurrentStaff.loadAgain();
                        _staffListLoaded.Invoke();
                        CancelEnable = true;
                        SaveEnable = false;
                        OnPropertyChanged("SaveEnable");
                        _currentStaff = CurrentOrganization.allStaff.Single(r => r.id == id);
                        OnPropertyChanged("CurrentStaff");
                        OnPropertyChanged("CancelEmployeeEnable");
                        NewActivated = true;
                        break;
                    }
                case "save":
                    {
                        if (!AllDataCompleted())
                            return;
                        SecurityManagements.saveAllOrganizations();
                        NewActivated = false;
                        SaveEnable = true;
                        OnPropertyChanged("SaveEnable");
                        break;
                    }
                case "cancel":
                    {
                        if (CurrentStaff == null)
                        {
                            MessageBox.Show("ابتدا یک سازمان را انتخاب نمایید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            return;
                        }
                        if (NewActivated)
                        {

                            canceling = true;
                            SecurityManagements.deleteStaffFromListByID(CurrentOrganization.id);
                            CancelEmployeeEnable = false;
                            OnPropertyChanged("CancelEmployeeEnable");
                            NewActivated = false;
                            SaveEnable = true;
                            CurrentOrganization.deleteOrganization();
                            OnPropertyChanged("SaveEnable");
                            CurrentStaff.deleteStaff();
                            //SecurityManagements.saveAllOrganizations();
                            CurrentOrganization.allStaff.Remove(CurrentStaff);
                            _staffListLoaded.Invoke();
                        }
                        break;
                    }
                case "del":
                    {
                        if (CurrentStaff == null)
                        {
                            MessageBox.Show("ابتدا یک سازمان را انتخاب نمایید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            return;
                        }
                        var msg = MessageBox.Show("آیا مطمئن به حذف کارمند انتخاب شده هستید؟", "حذف شود؟", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.RtlReading);
                        if (msg == MessageBoxResult.Yes)
                        {
                            CurrentStaff.deleteStaff();
                            SecurityManagements.deleteStaffFromListByID(CurrentOrganization.id);
                            //Organizations.Remove(CurrentOrganization);
                            CurrentOrganization.allStaff.Remove(CurrentStaff);
                            _staffListLoaded.Invoke();
                        }
                        break;
                    }
            }
        }

        public bool SaveEnable { get; set; }

        private bool AllDataCompleted()
        {
            if (canceling)
            {
                canceling = false;
                return false;
            }
            //if (NewActivated && CurrentStaff != _beforeSelected)
            //{
            if (CurrentStaff == null)
                return true;
            if (!string.IsNullOrEmpty(PasswordRpt) && CurrentStaff.password != PasswordRpt)
            {
                MessageBox.Show(".رمز عبور با تکرار رمز عبور برابر نمی باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                return false;
            }
            if (CurrentStaff.password != PasswordRpt && CurrentStaff.id==0)
                {
                    MessageBox.Show(".رمز عبور با تکرار رمز عبور برابر نمی باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }
            if (string.IsNullOrEmpty(CurrentStaff.name))
                {
                    MessageBox.Show(".نام و نام خانوادگی کارمند باید وارد شود", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }
            if (string.IsNullOrEmpty(CurrentStaff.username))
                {
                    MessageBox.Show(".نام کاربری باید وارد شود", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }
            if (string.IsNullOrEmpty(CurrentStaff.password)&&NewActivated)
                {
                    MessageBox.Show(".رمز عبور باید وارد شود", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }
            if (string.IsNullOrEmpty(CurrentStaff.code))
                {
                    MessageBox.Show(".کد کارمند باید وارد شود", "ورود تمامی اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }
            if (CurrentStaff.code != null && CurrentStaff.code.Length > 20)
                {
                    MessageBox.Show(".کد کارمند حداکثر 20 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }
            if (CurrentStaff.name != null && CurrentStaff.name.Length > 50)
                {
                    MessageBox.Show(".نام کارمند حداکثر 50 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }

            if (CurrentStaff.job != null && CurrentStaff.job.Length > 50)
                {
                    MessageBox.Show(".سمت کارمند حداکثر 50 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }

            if (CurrentStaff.tel != null && CurrentStaff.tel.Length > 50)
                {
                    MessageBox.Show(".شماره تماس کارمند حداکثر 50 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }

            if (CurrentStaff.address != null && CurrentStaff.address.Length > 100)
                {
                    MessageBox.Show(".نشانی کارمند حداکثر 100 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }

            if (CurrentStaff.description != null && CurrentStaff.description.Length > 200)
                {
                    MessageBox.Show(".ملاحظات حداکثر 200 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }

            if (CurrentStaff.username != null && CurrentStaff.username.Length > 20)
                {
                    MessageBox.Show(".نام کاربری حداکثر 20 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }

            if (CurrentStaff.password != null && CurrentStaff.password.Length > 20)
                {
                    MessageBox.Show(".رمز عبور حداکثر 20 کاراکتری می باشد", "خطا در ورود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return false;
                }
            return true;
            //}
        }

        public ItemStatus CurrentPurchasedProductStatus { set; get; }
        public ItemStatus CurrentMadeProductStatus { set; get; }
        public ResourceTypeStatus CurrentResourceTypeStatus { set; get; }
        public SubSystemStatus CurrentSubSystemStatus { set; get; }
        private bool _newActivated;
        private Staff _beforeSelected;
        private bool canceling;
        public bool DeleteEmployeeEnable { set; get; }
        public bool CancelEmployeeEnable { set; get; }
        public bool DeleteEnable { set; get; }
        public bool CancelEnable { set; get; }
        public string PasswordRpt { get; set; }

        private void Crud(object obj)
        {
            switch (obj.ToString())
            {
                case "new":
                    {
                        var win = new Views.InputBoxWindow("عنوان سازمان را وارد نمایید:", "نام سازمان");
                        win.ShowDialog();
                        if (win.DialogResult != true)
                            return;
                        if (win.TextValue == null || string.IsNullOrEmpty(win.TextValue.Trim()))
                        {
                            MessageBox.Show("عنوان سازمان باید وارد شود.", "خطا", MessageBoxButton.OK,
                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            return;
                        }
                        CurrentOrganization = new Organization(win.TextValue);
                        //var id = CurrentOrganization.id;

                        Organizations.Add(CurrentOrganization);
                        OnPropertyChanged("Organizations");
                        _listLoaded.Invoke();
                        CancelEnable = true;
                        OnPropertyChanged("CancelEnable");
                        break;
                    }
                case "save":
                    {
                        if (NewActivated)
                        {
                            if (!AllDataCompleted())
                            {
                                return;
                            }
                        }
                        SecurityManagements.saveAllOrganizations();
                        CancelEnable = false;
                        OnPropertyChanged("CancelEnable");
                        CancelEmployeeEnable = false;
                        OnPropertyChanged("CancelEmployeeEnable");
                        NewActivated = false;
                        break;
                    }
                case "cancel":
                    {
                        if (CurrentOrganization == null)
                        {
                            MessageBox.Show("ابتدا یک سازمان را انتخاب نمایید.", "خطا", MessageBoxButton.OK,MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            return;
                        }
                        if (_newActivated)
                        {
                            canceling = true;
                            SecurityManagements.deleteOrganizationFromListByID(CurrentOrganization.id);
                            CancelEnable = false;
                            //SecurityManagements.saveAllOrganizations();
                            OnPropertyChanged("CancelEnable");
                            Organizations.Remove(CurrentOrganization);
                            _listLoaded.Invoke();
                        }
                        break;
                    }
                case "del":
                    {
                        var msg = MessageBox.Show("آیا مطمئن به حذف سازمان انتخاب شده هستید؟", "حذف شود؟", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.RtlReading);
                        if (msg==MessageBoxResult.Yes)
                        {
                            CurrentOrganization.deleteOrganization();
                            SecurityManagements.deleteOrganizationFromListByID(CurrentOrganization.id);
                            SecurityManagements.saveAllOrganizations();
                            Organizations.Remove(CurrentOrganization);
                            _listLoaded.Invoke();
                        }
                        break;
                    }
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
