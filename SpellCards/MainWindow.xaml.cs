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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.VisualBasic.FileIO;

namespace SpellCards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Business.Config Config { get; set; }
        public ObservableCollection<Business.Class> Classes { get; set; }
        public ObservableCollection<Business.Character> Characters { get { if (CurrentSystem != null) return CurrentSystem.Characters; else return null; } }
        private Business.Class _currentClass;
        public Business.Class CurrentClass { get { return _currentClass; } set { _currentClass = value; OnPropertyChanged("CurrentClass"); OnPropertyChanged("IsClassSelected"); OnPropertyChanged("IsSpellSelected"); } }
        private Business.Character _currentCharacter;
        public Business.Character CurrentCharacter { get { return _currentCharacter; } set { _currentCharacter = value; OnPropertyChanged("CurrentCharacter"); OnPropertyChanged("IsCharacterSelected"); OnPropertyChanged("IsSpellSelected"); OnPropertyChanged("IsSpellAndCharacterSelected"); } }
        private Business.GameSystem _currentSystem;
        public Business.GameSystem CurrentSystem 
        { 
            get { return _currentSystem; } 
            set 
            { 
                _currentSystem = value;
                if (value != null)
                    Config.SelectedGameSystemID = value.ID;
                OnPropertyChanged("CurrentSystem"); 
                OnPropertyChanged("IsSystemSelected");
                OnPropertyChanged("IsClassSelected");
            }
        }
        private Business.Spell _currentClassSpell;
        private Business.Spell _currentCharacterSpell;
        private bool _isCharacterSpell;


        public Business.Spell CurrentClassSpell
        {
            get { return _isCharacterSpell ? null : _currentClassSpell; }
            set
            {
                _currentClassSpell = value;
                OnPropertyChanged("CurrentSpell");
                OnPropertyChanged("CurrentClassSpell");
                OnPropertyChanged("CurrentCharacterSpell");
                OnPropertyChanged("IsSpellSelected");
                OnPropertyChanged("IsSpellAndCharacterSelected");
            }
        }
        public Business.Spell CurrentCharacterSpell
        {
            get { return _isCharacterSpell ? _currentCharacterSpell : null; }
            set
            {
                _currentCharacterSpell = value;
                OnPropertyChanged("CurrentSpell");
                OnPropertyChanged("IsSpellSelected");
                OnPropertyChanged("CurrentClassSpell");
                OnPropertyChanged("CurrentCharacterSpell");
                OnPropertyChanged("IsSpellAndCharacterSelected");
            }
        }
        public Business.Spell CurrentSpell 
        { 
            get { return _isCharacterSpell ? _currentCharacterSpell : _currentClassSpell; } 
            set 
            {
                if (_isCharacterSpell)
                    _currentCharacterSpell = value;
                else
                    _currentClassSpell = value;
                OnPropertyChanged("CurrentSpell");
                OnPropertyChanged("IsSpellSelected");
                OnPropertyChanged("CurrentClassSpell");
                OnPropertyChanged("CurrentCharacterSpell");
                OnPropertyChanged("IsSpellAndCharacterSelected");
            } 
        }
        public bool IsCharacterSpell
        {
            get { return _isCharacterSpell; }
            set
            {
                _isCharacterSpell = value;
                OnPropertyChanged("IsCharacterSpell");
                OnPropertyChanged("CurrentSpell");
                OnPropertyChanged("IsSpellSelected");
                OnPropertyChanged("IsSpellAndCharacterSelected");
            }
        }
        private string _dataPath;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            _dataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpellCards\\");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(_dataPath))
                Directory.CreateDirectory(_dataPath);
            if (!File.Exists(System.IO.Path.Combine(_dataPath, "config.xml")))
            {
                //create default config with blank spell list
                using (StreamWriter ResourceFile = new StreamWriter(new FileStream(System.IO.Path.Combine(_dataPath, "config.xml"), FileMode.Create)))
                using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SpellCards.Config.config.xml"))
                using (var reader = new StreamReader(stream))
                {
                    ResourceFile.Write(reader.ReadToEnd());
                    ResourceFile.Flush();
                    ResourceFile.Close();
                }
            }
            //load the application config
            try
            {
                using (TextReader reader = new StreamReader(System.IO.Path.Combine(_dataPath, "config.xml")))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Business.Config));
                    Config = (Business.Config)serializer.Deserialize(reader);
                    reader.Close();
                    Config.GameSystems = new ObservableCollection<Business.GameSystem>(Config.GameSystems.OrderBy(p => p.Name));
                }
            }
            catch (Exception) { Config = new Business.Config(); }
                
            //load the selected system's classes
            LoadSystem();
            
            if (Classes == null)
                Classes = new ObservableCollection<Business.Class>(); 
            else if (Classes.Count > 0)
                CurrentClass = Classes[0];
            OnPropertyChanged("Config");
            OnPropertyChanged("Classes");
            OnPropertyChanged("CurrentClass");
            OnPropertyChanged("CurrentCharacter");
            OnPropertyChanged("IsSystemSelected");
            OnPropertyChanged("IsClassSelected");
        }

        public void LoadSystem()
        {
            if (Config.SelectedGameSystemID != null)
            {
                string path = System.IO.Path.Combine(_dataPath, Config.SelectedGameSystemID.ToString() + ".xml");
                if (File.Exists(path))
                {
                    try
                    {
                        using (TextReader reader = new StreamReader(System.IO.Path.Combine(_dataPath, Config.SelectedGameSystemID.ToString() + ".xml")))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(List<Business.Class>));
                            Classes = new ObservableCollection<Business.Class>(((List<Business.Class>)serializer.Deserialize(reader)).OrderBy(pn => pn.Name));
                            reader.Close();
                        }
                        if (Classes != null)
                            foreach (Business.Class c in Classes)
                            {
                                List<Business.Spell> l = c.Spells.ToList();
                                l.Sort();
                                c.Spells = new ObservableCollection<Business.Spell>(l);
                            }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error loading classes for the selected system.");
                    }
                }
                else { Classes = new ObservableCollection<Business.Class>(); }
                CurrentSystem = Config.GetSystemByID(Config.SelectedGameSystemID);
                if (CurrentSystem.Characters.Count > 0) CurrentCharacter = CurrentSystem.Characters[0];
            }
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                switch ((string)((FrameworkElement)sender).Tag)
                {
                    case "Exit":
                        Application.Current.Shutdown();
                        break;
                    case "Save":
                        SaveSystem();
                        break;
                    case "ImportCSV":
                        {
                            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                            dlg.DefaultExt = ".csv";
                            //dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif"; 

                            // Display OpenFileDialog by calling ShowDialog method 
                            Nullable<bool> result = dlg.ShowDialog();

                            // Get the selected file name and display in a TextBox 
                            if (result == true)
                            {
                                // Open document 
                                string filename = dlg.FileName;

                                TextFieldParser parser = new TextFieldParser(filename);
                                parser.TextFieldType = FieldType.Delimited;
                                parser.SetDelimiters(";");
                                while (!parser.EndOfData)
                                {
                                    //Process row
                                    string[] fields = parser.ReadFields();
                                    CurrentClass.Spells.Add(Business.Spell.NewSpell(fields, CurrentSystem, CurrentClass.Name));
                                }
                                parser.Close();
                            }
                        }
                        break;
                    case "Print":
                        {
                            PrintPreview pp = new PrintPreview(CurrentClass.Spells.ToList());
                            pp.Owner = this;
                            pp.ShowDialog();
                        }
                        break;
                    case "PrintAll":
                        {
                            List<Business.Spell> sps = new List<Business.Spell>();
                            foreach (Business.Class c in Classes)
                            {
                                sps.AddRange(c.Spells.Where(s => s.Print));
                            }
                            PrintPreview pp = new PrintPreview(sps);
                            pp.Owner = this;
                            pp.ShowDialog();
                        }
                        break;
                    case "PrintCharacter":
                        {
                            PrintPreview pp = new PrintPreview(CurrentCharacter.Spells.ToList());
                            pp.Owner = this;
                            pp.ShowDialog();
                        }
                        break;
                    case "PrintAllCharacters":
                        {
                            List<Business.Spell> sps = new List<Business.Spell>();
                            foreach (Business.Character c in CurrentSystem.Characters)
                            {
                                sps.AddRange(c.Spells.Where(s => s.Print));
                            }
                            PrintPreview pp = new PrintPreview(sps);
                            pp.Owner = this;
                            pp.ShowDialog();
                        }
                        break;
                    case "NewSpell":
                        CurrentClass.Spells.Add(Business.Spell.NewSpell(CurrentSystem, CurrentClass.Name));
                        break;
                    case "AddSpellToCharacter":
                        {
                            Business.Spell sp = Business.Spell.DeepCopy(CurrentSpell);
                            sp.Print = true;
                            sp.IsCharacterSpell = true;
                            CurrentCharacter.Spells.Add(sp);
                        }
                        break;
                    case "NewClass":
                        {
                            Business.Class c = Business.Class.NewClass();
                            Classes.Add(c);
                            CurrentClass = c;
                            OnPropertyChanged("Classes");
                        }
                        break;
                    case "DelClass":
                        if (MessageBox.Show("Are you sure you want to delete this class?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            Classes.Remove(CurrentClass);
                            if (Classes.Count > 0)
                                CurrentClass = Classes[0];
                            OnPropertyChanged("Classes");
                            OnPropertyChanged("CurrentClass");
                        }
                        break;
                    case "NewCharacter":
                        {
                            Business.Character c = Business.Character.NewCharacter();
                            Characters.Add(c);
                            CurrentCharacter = c;
                            OnPropertyChanged("Characteres");
                        }
                        break;
                    case "DelCharacter":
                        if (MessageBox.Show("Are you sure you want to delete this Character?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            Characters.Remove(CurrentCharacter);
                            if (Characters.Count > 0)
                                CurrentCharacter = Characters[0];
                            OnPropertyChanged("Characteres");
                            OnPropertyChanged("CurrentCharacter");
                        }
                        break;

                    case "CheckAll":
                        if (CurrentClass != null)
                        {
                            foreach (Business.Spell sp in CurrentClass.Spells)
                                sp.Print = true;
                        }
                        break;
                    case "UncheckAll":
                        if (CurrentClass != null)
                        {
                            foreach (Business.Spell sp in CurrentClass.Spells)
                                sp.Print = false;
                        }
                        break;
                    case "CheckAllChar":
                        if (CurrentCharacter != null)
                        {
                            foreach (Business.Spell sp in CurrentCharacter.Spells)
                                sp.Print = true;
                        }
                        break;
                    case "UncheckAllChar":
                        if (CurrentCharacter != null)
                        {
                            foreach (Business.Spell sp in CurrentCharacter.Spells)
                                sp.Print = false;
                        }
                        break;
                    case "ResetTitles":
                        if (CurrentClass != null)
                        {
                            if (MessageBox.Show("Are you sure you want to reset all spell header settings in this class to the game system default? This cannot be undone.", "Reset Class Header Settings", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                foreach (Business.Spell sp in CurrentClass.Spells)
                                {
                                    sp.Field3Title = CurrentSystem.Field3Title;
                                    sp.Field4Title = CurrentSystem.Field4Title;
                                    sp.Field1Title = CurrentSystem.Field1Title;
                                    sp.Field2Title = CurrentSystem.Field2Title;
                                    sp.ShowRow1 = CurrentSystem.ShowRow1;
                                    sp.ShowRow2 = CurrentSystem.ShowRow2;
                                    sp.ShowRow3 = CurrentSystem.ShowRow3;
                                }
                        }
                        break;
                    case "NewSystem":
                        {
                            if (IsSystemSelected) SaveSystem();

                            Business.GameSystem gs = Business.GameSystem.NewGameSystem();

                            EditGameSystem egs = new EditGameSystem(gs);
                            egs.Owner = this;
                            if (egs.ShowDialog() == true)
                            {
                                Config.GameSystems.Add(egs.CurrentSystem);
                                Config.GameSystems.OrderBy(p => p.Name);
                                Config.SelectedGameSystemID = egs.CurrentSystem.ID;
                                this.CurrentSystem = egs.CurrentSystem;

                                Classes = new ObservableCollection<Business.Class>();
                                CurrentClass = null;
                                CurrentSpell = null;
                                OnPropertyChanged("Config");
                                OnPropertyChanged("Classes");
                                SaveSystem();
                            }
                        }
                        break;
                    case "EditSystem":
                        {
                            Business.GameSystem gs = Config.GetSystemByID(Config.SelectedGameSystemID);

                            EditGameSystem egs = new EditGameSystem(gs);
                            egs.Owner = this;
                            if (egs.ShowDialog() == true)
                            {
                                Config.GameSystems.Remove(gs);
                                Config.GameSystems.Add(egs.CurrentSystem);
                                Config.GameSystems =  new ObservableCollection<Business.GameSystem>(Config.GameSystems.OrderBy(p => p.Name));
                                Config.SelectedGameSystemID = egs.CurrentSystem.ID;
                                this.CurrentSystem = egs.CurrentSystem;
                                OnPropertyChanged("Config");
                                SaveSystem();
                            }
                        }
                        break;
                    case "DeleteSystem":
                        {
                            Business.GameSystem gs = Config.GetSystemByID(Config.SelectedGameSystemID);
                            if (MessageBox.Show(this, "Are you sure you want to delete this game system and all related classes? This cannot be undone.", "Delete \"" + gs.Name + "\"", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                File.Delete(System.IO.Path.Combine(_dataPath, Config.SelectedGameSystemID.ToString() + ".xml"));
                                Config.GameSystems.Remove(gs);
                                SaveSystem();

                                if (Config.GameSystems.Count > 0)
                                {
                                    Config.SelectedGameSystemID = Config.GameSystems[0].ID;
                                    CurrentSystem = Config.GameSystems[0];
                                    LoadSystem();
                                    if (Classes.Count > 0)
                                        CurrentClass = Classes[0];
                                    OnPropertyChanged("Config");
                                    OnPropertyChanged("Classes");
                                    OnPropertyChanged("CurrentClass");
                                    OnPropertyChanged("IsSystemSelected");
                                    OnPropertyChanged("IsClassSelected");
                                }
                                else
                                {
                                    Classes = new ObservableCollection<Business.Class>();
                                    CurrentClass = null;
                                    CurrentSpell = null;
                                    CurrentSystem = null;
                                    OnPropertyChanged("Classes");
                                }
                                OnPropertyChanged("Config");
                            }

                        }
                        break;
                    case "ImportXML":
                        {
                            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                            dlg.DefaultExt = ".xml";
                            dlg.Filter = "XML Files (*.xml)|*.xml";
                            bool? result = dlg.ShowDialog();
                            if (result == true)
                            {
                                using (TextReader reader = new StreamReader(dlg.FileName))
                                {
                                    XmlSerializer serializer = new XmlSerializer(typeof(Business.SystemExport));
                                    Business.SystemExport se = (Business.SystemExport)serializer.Deserialize(reader);
                                    reader.Close();

                                    string name = Microsoft.VisualBasic.Interaction.InputBox("Name the new system:", "System Import", "New System");
                                    se.System.Name = name;
                                    se.System.ID = new Guid();

                                    Config.GameSystems.Add(se.System);
                                    Config.GameSystems.OrderBy(p => p.Name);
                                    Config.SelectedGameSystemID = se.System.ID;
                                    this.CurrentSystem = se.System;

                                    Classes = new ObservableCollection<Business.Class>(se.Classes);
                                    CurrentClass = null;
                                    CurrentSpell = null;
                                    OnPropertyChanged("Config");
                                    OnPropertyChanged("Classes");
                                    SaveSystem();
                                }
                            }
                        }
                        break;
                    case "ExportXML":
                        {
                            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                            dlg.DefaultExt = ".xml";
                            dlg.Filter = "XML Files (*.xml)|*.xml";
                            bool? result = dlg.ShowDialog();
                            if (result == true)
                            {
                                XmlSerializer writer = new XmlSerializer(typeof(Business.SystemExport));
                                using (FileStream file = File.Create(dlg.FileName))
                                {
                                    Business.SystemExport se = new Business.SystemExport();
                                    se.Classes = Classes.ToList();
                                    se.System = CurrentSystem;

                                    writer.Serialize(file, se);
                                    file.Close();
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        private bool SaveSystem()
        {
            try
            {
                using (TextWriter writer = new StreamWriter(System.IO.Path.Combine(_dataPath, "config.xml")))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Business.Config));
                    serializer.Serialize(writer, Config);
                    writer.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error saving the program config.");
                return false;
            }
            if (Config.SelectedGameSystemID != null)
            try
            {
                using (TextWriter writer = new StreamWriter(System.IO.Path.Combine(_dataPath, Config.SelectedGameSystemID.ToString() + ".xml")))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Business.Class>));
                    serializer.Serialize(writer, Classes.ToList());
                    writer.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error saving the current game system's spells.");
                return false;
            }
            
            return true;
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

        public bool IsSystemSelected { get { return CurrentSystem != null; } }
        public bool IsClassSelected { get { return IsSystemSelected && CurrentClass != null; } }
        public bool IsCharacterSelected { get { return IsSystemSelected && CurrentCharacter != null; } }
        public bool IsSpellSelected { get { return IsClassSelected && CurrentSpell != null; } }
        public bool IsSpellAndCharacterSelected { get { return IsCharacterSelected && CurrentSpell != null; } }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void CommandBinding_OpenSystem(object sender, ExecutedRoutedEventArgs e)
        {
            if (SaveSystem())
            {
                Config.SelectedGameSystemID = ((Business.GameSystem)e.Parameter).ID;
                LoadSystem();

                if (Classes.Count > 0)
                    CurrentClass = Classes[0];
                OnPropertyChanged("Classes");
                OnPropertyChanged("CurrentClass");
                OnPropertyChanged("CurrentSpell");
                OnPropertyChanged("IsClassSelected");
            }
        }

        private void ClassGroupBox_GotFocus(object sender, RoutedEventArgs e)
        {
            IsCharacterSpell = false;
        }

        private void CharacterGroupBox_GotFocus(object sender, RoutedEventArgs e)
        {
            IsCharacterSpell = true;
        }
    }
}
