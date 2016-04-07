using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SpellCards.Business
{
    [Serializable]
    public class Config : INotifyPropertyChanged
    {
        private Guid _selectedGameSystemId;

        private ObservableCollection<GameSystem> _gameSystems = new ObservableCollection<GameSystem>();
        public ObservableCollection<GameSystem> GameSystems { get { return _gameSystems; } set { _gameSystems = value; OnPropertyChanged("GameSystem"); } }
        
        [XmlAttribute]
        public Guid SelectedGameSystemID { get { return _selectedGameSystemId; } set { _selectedGameSystemId = value; OnPropertyChanged("SelectedGameSystemID"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public GameSystem GetSystemByID(Guid id)
        {
            return GameSystems.FirstOrDefault(p => p.ID == id);
        }
    }

    [Serializable]
    public class GameSystem : INotifyPropertyChanged, IComparable<GameSystem>
    {
        private Guid _id;
        private string _name;
        private string _field2Title;
        private string _field1Title;
        private string _field3Title;
        private string _field4Title;
        private string _field5Title;
        private string _field6Title;
        private bool _showRow1;
        private bool _showRow2;
        private bool _showRow3;

        [XmlAttribute]
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        [XmlAttribute]
        public Guid ID { get { return _id; } set { _id = value; OnPropertyChanged("ID"); } }
        [XmlAttribute]
        public string Field1Title { get { return _field1Title; } set { _field1Title = value; OnPropertyChanged("Field1Title"); } }
        [XmlAttribute]
        public string Field2Title { get { return _field2Title; } set { _field2Title = value; OnPropertyChanged("Field2Title"); } }
        [XmlAttribute]
        public string Field3Title { get { return _field3Title; } set { _field3Title = value; OnPropertyChanged("Field3Title"); } }
        [XmlAttribute]
        public string Field4Title { get { return _field4Title; } set { _field4Title = value; OnPropertyChanged("Field4Title"); } }
        [XmlAttribute]
        public string Field5Title { get { return _field5Title; } set { _field5Title = value; OnPropertyChanged("Field5Title"); } }
        [XmlAttribute]
        public string Field6Title { get { return _field6Title; } set { _field6Title = value; OnPropertyChanged("Field6Title"); } }
        [XmlAttribute]
        public bool ShowRow1 { get { return _showRow1; } set { _showRow1 = value; OnPropertyChanged("ShowRow1"); } }
        [XmlAttribute]
        public bool ShowRow2 { get { return _showRow2; } set { _showRow2 = value; OnPropertyChanged("ShowRow2"); } }
        [XmlAttribute]
        public bool ShowRow3 { get { return _showRow3; } set { _showRow3 = value; OnPropertyChanged("ShowRow3"); } }

        private ObservableCollection<Character> _characters = new ObservableCollection<Character>();
        public ObservableCollection<Character> Characters { get { return _characters; } set { _characters = value; OnPropertyChanged("Characters"); } }

        public GameSystem() { }

        public static GameSystem NewGameSystem()
        {
            GameSystem gs = new GameSystem();

            gs.ID = Guid.NewGuid();
            gs._name = "New System";

            return gs;
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

        public int CompareTo(GameSystem other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }

    public class Character : INotifyPropertyChanged
    {
        private string _name;
        [XmlAttribute]
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }

        private ObservableCollection<Spell> _spells = new ObservableCollection<Spell>();
        public ObservableCollection<Spell> Spells { get { return _spells; } set { _spells = value; OnPropertyChanged("Spells"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public static Character NewCharacter()
        {
            Character c = new Character();

            c._name = "New Character";
            c.Spells = new ObservableCollection<Spell>();

            return c;
        }
    }
}
