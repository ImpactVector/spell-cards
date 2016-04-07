using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SpellCards.Business
{
    [Serializable]
    public class Class : INotifyPropertyChanged, IComparable<Class>
    {
        private string _name;
        [XmlAttribute]
        public string Name { 
            get { return _name; } 
            set { 
                _name = value;
                foreach (Spell s in Spells)
                    s.ClassName = value;
                OnPropertyChanged("Name"); 
            } 
        }

        private ObservableCollection<Spell> _spells = new ObservableCollection<Spell>();
        public ObservableCollection<Spell> Spells { get { return _spells; } set { _spells = value; OnPropertyChanged("Spells"); } }

        public static Class NewClass()
        {
            Class c = new Class();

            c._name = "New Class";
            c.Spells = new ObservableCollection<Spell>();
            
            return c;
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

        public int CompareTo(Class other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }

    [Serializable]
    public class Spell : INotifyPropertyChanged, IComparable<Spell>
    {
        private string _name;
        private string _className;
        private string _school;
        private string _text;
        private string _field2Title;
        private string _field2;
        private string _field1Title;
        private string _field1;
        private string _keywords;
        private string _field3Title;
        private string _field3;
        private string _field4Title;
        private string _field4;
        private string _field5Title;
        private string _field5;
        private string _field6Title;
        private string _field6;
        private string _bottomText;
        private string _pageRef;
        private string _overrideFontSize = "";
        private int _level;

        private bool _print;
        private bool _isCharacterSpell;
        private bool _showRow1;
        private bool _showRow2;
        private bool _showRow3;
        [NonSerialized]
        private Color _borderColor;
        private string _borderColorString;

        [XmlAttribute]
        public int Level { get { return _level; } set { _level = value; OnPropertyChanged("Level"); } }
        [XmlAttribute]
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        [XmlAttribute]
        public string ClassName { get { return _className; } set { _className = value; OnPropertyChanged("ClassName"); } }
        [XmlAttribute]
        public string School { get { return _school; } set { _school = value; OnPropertyChanged("School"); } }
        [XmlAttribute, DefaultValue("Casting Time")]
        public string Field1Title { get { return _field1Title; } set { _field1Title = value; OnPropertyChanged("Field1Title"); } }
        [XmlAttribute, DefaultValue("")]
        public string Field1 { get { return _field1; } set { _field1 = value; OnPropertyChanged("Field1"); } }
        [XmlAttribute, DefaultValue("Range")]
        public string Field2Title { get { return _field2Title; } set { _field2Title = value; OnPropertyChanged("Field2Title"); } }
        [XmlAttribute, DefaultValue("")]
        public string Field2 { get { return _field2; } set { _field2 = value; OnPropertyChanged("Field2"); } }
        [XmlAttribute, DefaultValue("Components")]
        public string Field3Title { get { return _field3Title; } set { _field3Title = value; OnPropertyChanged("Field3Title"); } }
        [XmlAttribute, DefaultValue("")]
        public string Field3 { get { return _field3; } set { _field3 = value; OnPropertyChanged("Field3"); } }
        [XmlAttribute, DefaultValue("Duration")]
        public string Field4Title { get { return _field4Title; } set { _field4Title = value; OnPropertyChanged("Field4Title"); } }
        [XmlAttribute, DefaultValue("")]
        public string Field4 { get { return _field4; } set { _field4 = value; OnPropertyChanged("Field4"); } }
        [XmlAttribute, DefaultValue("")]
        public string Field5Title { get { return _field5Title; } set { _field5Title = value; OnPropertyChanged("Field5Title"); } }
        [XmlAttribute, DefaultValue("")]
        public string Field5 { get { return _field5; } set { _field5 = value; OnPropertyChanged("Field5"); } }
        [XmlAttribute, DefaultValue("")]
        public string Field6Title { get { return _field6Title; } set { _field6Title = value; OnPropertyChanged("Field6Title"); } }
        [XmlAttribute, DefaultValue("")]
        public string Field6 { get { return _field6; } set { _field6 = value; OnPropertyChanged("Field6"); } }
        [XmlAttribute, DefaultValue("")]
        public string Keywords { get { return _keywords; } set { _keywords = value; OnPropertyChanged("Keywords"); } }
        [XmlAttribute, DefaultValue("")]
        public string PageRef { get { return _pageRef; } set { _pageRef = value; OnPropertyChanged("PageRef"); } }
        [XmlText, DefaultValue("")]
        public string Text { get { return _text; } set { _text = value; OnPropertyChanged("Text"); } }
        [XmlAttribute, DefaultValue("")]
        public string BottomText { get { return _bottomText; } set { _bottomText = value; OnPropertyChanged("BottomText"); } }
        [XmlAttribute, DefaultValue("")]
        public string OverrideFontSize { get { return _overrideFontSize; } set { _overrideFontSize = value; OnPropertyChanged("OverrideFontSize"); } }

        //normally you might not persist this (especially here), but I hate reselecting my spells every time I start the program
        [XmlAttribute, DefaultValue(false)]
        public bool Print { get { return _print; } set { _print = value; OnPropertyChanged("Print"); } }
        [XmlAttribute, DefaultValue(true)]
        public bool ShowRow1 { get { return _showRow1; } set { _showRow1 = value; OnPropertyChanged("ShowRow1"); } }
        [XmlAttribute, DefaultValue(true)]
        public bool ShowRow2 { get { return _showRow2; } set { _showRow2 = value; OnPropertyChanged("ShowRow2"); } }
        [XmlAttribute, DefaultValue(false)]
        public bool ShowRow3 { get { return _showRow3; } set { _showRow3 = value; OnPropertyChanged("ShowRow3"); } }
        [XmlAttribute, DefaultValue(false)]
        public bool IsCharacterSpell { get { return _isCharacterSpell; } set { _isCharacterSpell = value; OnPropertyChanged("IsCharacterSpell"); } }

        [XmlIgnore]
        public int RowCount { get { return Convert.ToInt32(_showRow1) + Convert.ToInt32(_showRow2) + Convert.ToInt32(_showRow3); } }

        [XmlIgnore, DefaultValue("Black")]
        public Color BorderColor 
        { 
            get 
            {
                if (_borderColor == Color.FromArgb(0,0,0,0))
                    _borderColor = Color.FromRgb(byte.Parse(_borderColorString.Substring(1, 2), System.Globalization.NumberStyles.HexNumber)
                        , byte.Parse(_borderColorString.Substring(3, 2), System.Globalization.NumberStyles.HexNumber)
                        , byte.Parse(_borderColorString.Substring(5, 2), System.Globalization.NumberStyles.HexNumber));
                return _borderColor; 
            } 
            set 
            { 
                _borderColor = value;
                BorderColorString = String.Format("#{0:X2}{1:X2}{2:X2}", BorderColor.R, BorderColor.G, BorderColor.B);
                OnPropertyChanged("BorderColor"); 
            } 
        }

        //this was a complate pain to get working with SSRS
        [XmlAttribute, DefaultValue("#000000")]
        public string BorderColorString { 
            get { return _borderColorString; } 
            set { _borderColorString = value; _borderColor = Color.FromArgb(0, 0, 0, 0); OnPropertyChanged("BorderColorString"); } 
        }
        [XmlIgnore]
        public Color BorderTextColor { get { return 1 - (0.299 * BorderColor.R + 0.587 * BorderColor.G + 0.114 * BorderColor.B) / 255 < .5 ? Color.FromRgb(0, 0, 0) : Color.FromRgb(255, 255, 255); } }
        [XmlIgnore]
        public string BorderTextColorString { get { return String.Format("#{0:X2}{1:X2}{2:X2}", BorderTextColor.R, BorderTextColor.G, BorderTextColor.B); } }
        
        [XmlIgnore]
        public string TypeText 
        { 
            get 
            {
                if (_level == 0)
                    return _school + " Cantrip";
                else
                    return GetLevelText(_level) + " Level " + _school;
            } 
        }

        private static string GetLevelText(int level)
        {
            switch (level)
            {
                case 1:
                    return "1st";
                case 2:
                    return "2nd";
                case 3:
                    return "3rd";
                case 4:
                    return "4th";
                case 5:
                    return "5th";
                case 6:
                    return "6th";
                case 7:
                    return "7th";
                case 8:
                    return "8th";
                case 9:
                    return "9th";
                case 10:
                    return "10th";
                case 11:
                    return "11th";
                case 12:
                    return "12th";
                case 13:
                    return "13th";
                case 14:
                    return "14th";
                case 15:
                    return "15th";
                case 16:
                    return "16th";
                case 17:
                    return "17th";
                case 18:
                    return "18th";
                case 19:
                    return "19th";
                case 20:
                    return "20th";
                case 21:
                    return "21st";
                case 22:
                    return "22nd";
                case 23:
                    return "23rd";
                case 24:
                    return "24th";
                case 25:
                    return "25th";
                case 26:
                    return "26th";
                case 27:
                    return "27th";
                case 28:
                    return "28th";
                case 29:
                    return "29th";
                case 30:
                    return "30th";
            }
            return "";
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

        public Spell()
        {
            //the DefaultValue() attribute apparently doesn't set this stuff on deserialization
            _borderColorString = "#000000";
            _field1Title = "Casting Time";
            _field2Title = "Range";
            _field3Title = "Components";
            _field4Title = "Duration";
            _showRow1 = true;
            _showRow2 = true;
        }

        public static Spell NewSpell(GameSystem gs, string className)
        {
            Spell s = new Spell();
            s._name = "New Spell";
            s._level = 0;
            s._borderColorString = "#000000";

            s._field2Title = gs.Field2Title;
            s._field4Title = gs.Field4Title;
            s._field1Title = gs.Field1Title;
            s._field3Title = gs.Field3Title;
            s._field5Title = gs.Field5Title;
            s._field6Title = gs.Field6Title;
            s._showRow1 = gs.ShowRow1;
            s._showRow2 = gs.ShowRow2;
            s._showRow3 = gs.ShowRow3;
            s._className = className;
            return s;
        }

        public static Spell NewSpell(string[] props, GameSystem gs, string className)
        {
            Spell s = new Spell();
            int.TryParse(props[0], out s._level);
            s._name = props[1];
            s._school = props[2].EndsWith("Cantrip") ? props[2].Split(' ')[0] : props[2].Split(' ')[2];
            s._field1 = props[3];
            s._field2 = props[4];
            s._field3 = props[5];
            s._field4 = props[6];
            s._text = props[7];
            s._borderColorString = "#000000";

            s._field2Title = gs.Field2Title;
            s._field4Title = gs.Field4Title;
            s._field1Title = gs.Field1Title;
            s._field3Title = gs.Field3Title;
            s._field5Title = gs.Field5Title;
            s._field6Title = gs.Field6Title;
            s._showRow1 = gs.ShowRow1;
            s._showRow2 = gs.ShowRow2;
            s._showRow3 = gs.ShowRow3;
            s._className = className;
            return s;
        }


        public int CompareTo(Spell other)
        {
            int lev = this.Level.CompareTo(other.Level);
            int name = this.Name.CompareTo(other.Name);

            if (lev != 0)
                return lev;
            else
                return name;
        }

        //terribly inefficient, but it's the easiest way to do it since it's already set up for XML serialization
        public static Spell DeepCopy(Spell obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Spell));
                serializer.Serialize(stream, obj);
                stream.Position = 0;

                return (Spell)serializer.Deserialize(stream);
            }
        }
    }

    public class SpellComparer: IComparer<Spell>
    {
        public int Compare(Spell x, Spell y)
        {
            return x.CompareTo(y);
        }
    }
}
