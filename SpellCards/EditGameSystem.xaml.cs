using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.VisualBasic.FileIO;

namespace SpellCards
{
    /// <summary>
    /// Interaction logic for EditGameSystem.xaml
    /// </summary>
    public partial class EditGameSystem : Window, INotifyPropertyChanged
    {
        private Business.GameSystem _currentSystem;
        public Business.GameSystem CurrentSystem { get { return _currentSystem; } set { _currentSystem = value; OnPropertyChanged("CurrentSystem"); } }

        public EditGameSystem(Business.GameSystem gs)
        {
            InitializeComponent();
            _currentSystem = gs;
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
