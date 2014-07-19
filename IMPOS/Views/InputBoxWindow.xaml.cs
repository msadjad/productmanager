using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace IMPOS.Views
{
    /// <summary>
    /// Interaction logic for InputBoxWindow.xaml
    /// </summary>
    public partial class InputBoxWindow : INotifyPropertyChanged
    {
        private string _textCaption;
        private string _textValue;

        public string TextCaption
        {
            set { _textCaption = value; OnPropertyChanged(); }
            get { return _textCaption; }
        }

        public string TextValue
        {
            set { _textValue = value;OnPropertyChanged(); }
            get { return _textValue; }
        }

        public InputBoxWindow(string textCaption,string title,string defualtTextValue=null)
        {
            InitializeComponent();
            DataContext = this;
            TextValue = defualtTextValue;
            TextCaption = textCaption;
            Title = title;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonBase2_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
