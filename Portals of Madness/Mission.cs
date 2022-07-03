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
        public List<AICharacter> Enemies { get; set; }
        public List<AICharacter> EveryEnemy { get; set; }
        public string background { get; set; }
        public Encounters encounters { get; set; }
        public int encounterNumber { get; set; }
        public int fightNumber { get; set; }

        public Mission(int number)
        {
            Enemies = new List<AICharacter>();
            EveryEnemy = new List<AICharacter>();
            GDBBackupModel gdbm = new GDBBackupModel();
            foreach (var character in gdbm.AICharactersDatabase)
            {
                EveryEnemy.Add(character.convToAICharacter());
            }

            string path = $@"../../Missions/Mission{number}.xml";
            try
            {
                encounters = Deserializer(path);
            }
            catch { }
            encounterNumber = 0;
            fightNumber = 0;
        }

        public void LoadNextEnemies()
        {
            Enemies.Clear();
            if (encounters.encounter[encounterNumber].fights.fight[fightNumber].type.Equals("normal")){
                for (int i = 0; i < encounters.encounter[encounterNumber].fights.fight[fightNumber].amount; i++)
                {
                    Enemies.Add(SelectAICharacter());
                }
            }
            else
            {
                Enemies.AddRange(selectBossFight());
            }
            ++fightNumber;
        }

        public AICharacter SelectAICharacter()
        {
            switch (encounters.encounter[encounterNumber].fights.fight[fightNumber].enemies)
            {
                case "prisonPack":
                    return EveryEnemy.Where(a => a.name == "Crazed Prisoner").Select(a => a).First();
                case "crazedCitizenPack":
                    break;
                case "guardPack":
                    return EveryEnemy.Where(a => a.name == "Guard").Select(a => a).First();
                default:
                    break;
            }
            return null;
        }

        public List<AICharacter> selectBossFight()
        {
            List<AICharacter> characters = new List<AICharacter>();
            switch (encounters.encounter[encounterNumber].fights.fight[fightNumber].enemies)
            {
                case "prisonBoss":
                    characters.Add(EveryEnemy.Where(a => a.name == "Crazed Prisoner").Select(a => a).First());
                    characters.Add(EveryEnemy.Where(a => a.name == "The Warden").Select(a => a).First());
                    characters.Add(EveryEnemy.Where(a => a.name == "Crazed Prisoner").Select(a => a).First());
                    break;
                case "ratThief":
                    break;
                case "townCouncil":
                    break;
                default:
                    break;
            }
            return characters;
        }

        //Deserializes the XML file
        public Encounters Deserializer(string path)
        {
            Encounters obj = new Encounters();
            XmlSerializer des = new XmlSerializer(typeof(Encounters));
            try
            {
                TextReader sr = new StreamReader(path);
                obj = (Encounters)des.Deserialize(sr);
                sr.Close();
            }
            catch { }
            return obj;
        }

        //Checks if there is an encounter remaining
        public bool IsEncounter()
        {
            Enemies = new List<AICharacter>();
            return encounterNumber < encounters.encounter.Length;
        }

        //Returns the side the player is on
        public string PlayerSide()
        {
            return encounters.side;
        }

        public void IncrementEncounterNumber()
        {
            ++encounterNumber;
            fightNumber = 1;
        }
    }

    [XmlRoot("Encounters")]
    public class Encounters
    {
        [XmlElement("side")]
        public string side { get; set; }

        [XmlElement("Encounter")]
        public Encounter[] encounter { get; set; }
    }

    public class Encounter
    {
        [XmlElement("id")]
        public int id { get; set; }

        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("background1")]
        public string background1 { get; set; }

        [XmlElement("optional")]
        public bool optional { get; set; }

        [XmlElement("startdialog")]
        public string startdialog { get; set; }

        [XmlElement("Fights")]
        public Fights fights { get; set; }

        [XmlElement("enddialog")]
        public string enddialog { get; set; }
    }

    public class Fights
    {
        [XmlElement("Fight")]
        public Fight[] fight { get; set; }
    }

    public class Fight
    {
        [XmlElement("id")]
        public int id { get; set; }

        [XmlElement("dialog")]
        public string dialog { get; set; }

        [XmlElement("enemies")]
        public string enemies { get; set; }

        [XmlElement("amount")]
        public int amount { get; set; }

        [XmlElement("type")]
        public string type { get; set; }
    }
}
