using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Portals_of_Madness
{
    public class Mission
    {
        public string name { get; set; }
        public List<Character> Enemies { get; set; }
        public string background { get; set; }
        public Encounters encounters { get; set; }
        public int encounterNumber { get; set; }

        public Mission(int number)
        {
            Enemies = new List<Character>();
            string dir = Environment.CurrentDirectory;
            encounters = Deserializer(typeof(Encounters), $"../../Missions/mission{number}.xml") as Encounters;
            encounterNumber = 1;
        }

        //Deserializes the XML file
        public Object Deserializer(Type datatype, string path)
        {
            Object obj = null;
            XmlSerializer des = new XmlSerializer(datatype);
            if (File.Exists(path))
            {
                TextReader sr = new StreamReader(path);
                obj = des.Deserialize(sr);
                sr.Close();
            }
            return obj;
        }

        //Checks if there is an encounter remaining
        public bool isEncounter()
        {
            Enemies = new List<Character>();
            if(encounters.encounter[encounterNumber] != null)
            {
                return true;
            }
            return false;
        }

        //Returns the side the player is on
        public string PlayerSide()
        {
            return encounters.side;
        }

        public void incrementEncounterNumber()
        {
            ++encounterNumber;
        }
    }

    [Serializable]
    public class Encounters
    {
        public string side { get; set; }
        public Encounter[] encounter { get; set; }
    }

    public class Encounter
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool optional { get; set; }
        public string startdialog { get; set; }
        public Fights fights { get; set; }
        public string enddialog { get; set; }
    }

    public class Fights
    {
        public Fight[] fight { get; set; }
    }

    public class Fight
    {
        public int id { get; set; }
		public string dialog { get; set; }
        public string enemies { get; set; }
        public int amount { get; set; }
        public string type { get; set; }
    }
}
