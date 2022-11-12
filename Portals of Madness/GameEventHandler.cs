using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portals_of_Madness
{
    public class GameEventHandler
    {
        public List<string> Flags { get; set; }
        private Controller Cont { get; set; }

        public GameEventHandler(Controller c)
        {
            Flags = new List<string>();
            Cont = c;
            Cont.XMLOperations.SetUpAllCharacters();
        }

        public void MissionCompleteEvents(int missionNumber)
        {

        }

        public void EncounterCompleteEvents(int missionNumber, int encounterNumber)
        {
            string flag;
            switch (missionNumber)
            {
                case 0:
                    flag = "03optional";
                    if (encounterNumber == 3 && !Flags.Contains(flag))
                    {
                        Flags.Add(flag);
                    }
                    break;
                default:
                    break;
            }
        }

        public bool AddToTeam(string s, List<Character> team)
        {
            Character tmp = Cont.XMLOperations.AllCharacters.Where(a => a.ID == s).Select(a => a).First();
            bool b = false;
            foreach (Character c in team)
            {
                if(tmp.ID == c.ID)
                {
                    b = true;
                }
            }
            if (!b)
            {
                team.Add(tmp);
            }
            return !b;
        }

        public bool RemoveFromTeam(string s, List<Character> team)
        {
            int index = -1;
            for (int i=0; i < team.Count; i++)
            {
                if (s == team[i].ID)
                {
                    index = i;
                }
            }
            if(index != -1)
            {
                team.RemoveAt(index);
                return true;
            }
            return false;
        }

        public bool EncounterStartEvents(int missionNumber, int encounterNumber, List<Character> playerTeam, List<Character> AITeam)
        {
            switch (missionNumber)
            {
                case 0:
                    if (encounterNumber == 4 && Flags.Contains("03optional"))
                    {
                        AddToTeam("clumsyPl", playerTeam);
                        return true;
                    }
                    if (encounterNumber == 5 && !Flags.Contains("03optional"))
                    {
                        RemoveFromTeam("maxwellEn", AITeam);
                        AddToTeam("maxwellPl", playerTeam);
                        return true;
                    }
                    break;
                case 2:
                    if (encounterNumber == 4)
                    {
                        AddToTeam("redPl", playerTeam);
                        return true;
                    }
                    if (encounterNumber == 6)
                    {
                        RemoveFromTeam("redPl", playerTeam);
                        RemoveFromTeam("maxwellPl", playerTeam);
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}
