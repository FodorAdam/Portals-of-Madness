using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Portals_of_Madness
{
    public class GameEngine
    {
        GameForm gameForm;
        SelectionForm selectionForm;
        InfoForm infoForm;
        int nextMap;
        private List<Character> playerTeam;
        private List<Character> enemyTeam;
        private Mission mission;
        private List<Character> initiativeTeam;
        private bool playerTurn = false;
        private Character currentCharacter;
        private int currentID = -1;
        private int encounterNumber = 1;
        private int turn;
        private GameForm form;

        //When called by any form other than GameForm
        public GameEngine(){}

        //When called by GameForm after starting a new game
        public GameEngine(GameForm f)
        {
            nextMap = 0;
            Setup();
        }

        //When called by GameForm
        public GameEngine(GameForm f, List<Character> pT, int nM)
        {
            nextMap = nM;
            form = f;
            playerTeam = pT;
            Setup();
        }

        //Set the size of the form to the resolution, then maximize it
        public Point Resolution(Form f)
        {
            f.Location = new Point(0, 0);
            int h = Screen.PrimaryScreen.WorkingArea.Height;
            int w = Screen.PrimaryScreen.WorkingArea.Width;
            f.ClientSize = new Size(w, h);
            f.WindowState = FormWindowState.Maximized;

            return new Point(w, h);
        }

        //Used to close the current form and swap to another
        public void ChangeForm(Form f, string formChar)
        {
            f.Close();
            ShowOtherForm(formChar);
        }

        //Swaps to another form
        public void ShowOtherForm(string formChar)
        {
            switch (formChar)
            {
                case ("s"):
                    selectionForm = new SelectionForm();
                    selectionForm.ShowDialog();
                    break;
                case ("g"):
                    gameForm = new GameForm();
                    gameForm.ShowDialog();
                    break;
                case ("g+"):
                    gameForm = new GameForm(nextMap, playerTeam);
                    gameForm.ShowDialog();
                    break;
                case ("i"):
                    infoForm = new InfoForm();
                    infoForm.ShowDialog();
                    break;
                default:
                    break;
            }
        }

        //Sets the next map to get loaded
        public void NextMap(int n)
        {
            nextMap = n;
        }

        public void Setup()
        {
            turn = 0;
            encounterNumber = 1;
            playerTeam = new List<Character>();
            arrangeTeamMembers(playerTeam, mission.PlayerSide());
            if (mission.isEncounter())
            {
                encounterSetup();
            }
            else
            {
                showResult(true);
            }
        }

        //TODO: Sync this up with the GameForm images
        public void arrangeTeamMembers(List<Character> team, string side)
        {
            if ("left".Equals(side))
            {
                for (int i = 0; i < team.Count(); i++)
                {
                    //team[i].setX((int)(190 + Math.Pow(-1, i) * 40));
                    //team[i].setY(200 + i * 70);
                }
            }
            else
            {
                for (int i = 0; i < team.Count(); i++)
                {
                    //team[i].setX((int)(1240 + Math.Pow(-1, i) * (-1) * 40));
                    //team[i].setY(200 + i * 70);
                }
            }
        }

        //Resets the enemies, puts them in their place, creates the order in which the characters get to go and starts the game
        public void encounterSetup()
        {
            //TODO: Randomize the enemy teams according to the name in the XML file in Mission
            enemyTeam = new List<Character>();
            enemyTeam = mission.getEnemies();
            string enemySide = (mission.PlayerSide().Equals("left") ? "right" : "left");
            arrangeTeamMembers(enemyTeam, enemySide);
            createInitiative();
            manage();
        }

        public void nextEncounter()
        {
            if (mission.isEncounter())
            {
                encounterSetup();
            }
            else
            {
                showResult(true);
            }
        }

        //TODO: End screen after either winning or losing
        private void showResult(bool res)
        {
            foreach (Character c in playerTeam)
            {
                c.setCurrHealth(c.getMaxHealth());
                c.setAlive(true);
                c.setStunned(false);
                c.setCurrResource(0);
            }
        }

        public void manage()
        {
            //TODO: Ide kellene majd az AI meghívása
            if (bothTeamsAlive() == 0)
            {
                currentSelect();
                if (!playerTurn)
                {
                    manage();
                }
            }
            else if (bothTeamsAlive() == -1)
            {
                showResult(false);
            }
            else if (bothTeamsAlive() == 1)
            {
                mission.incrementEncounterNumber();
                nextEncounter();
            }
        }

        private int selectTarget()
        {
            Random rand = new Random();
            int target = rand.Next(playerTeam.Count());
            if (!playerTeam[target].isAlive())
            {
                target = selectTarget();
            }
            return target;
        }

        private void newTurn()
        {
            turn++;
            foreach (Character c in initiativeTeam)
            {
                if (c.isAlive())
                {
                    foreach (DoT dot in c.getDoTs())
                    {
                        c.setCurrHealth(c.getCurrHealth() - dot.amount);
                        dot.Tick();
                        if (dot.duration <= 0)
                        {
                            c.removeDoT(dot);
                        }
                    }

                    if (c.getCurrHealth() <= 0)
                    {
                        c.die();
                    }
                    if (c.getCurrHealth() > c.getMaxHealth())
                    {
                        c.setCurrHealth(c.getMaxHealth());
                    }
                    resourceGain(c);
                }
            }
        }

        private void resourceGain(Character c)
        {
            int gain = 3;

            switch (c.getResourceName())
            {
                case "mana":
                    gain = 4;
                    break;
                case "rage":
                    if (c.getCurrHealth() < c.getMaxHealth() * 0.3)
                    {
                        gain = 8;
                    }
                    else if (c.getCurrHealth() < c.getMaxHealth() * 0.7)
                    {
                        gain = 4;
                    }
                    else
                    {
                        gain = 2;
                    }
                    break;
                case "focus":
                    if (c.getCurrHealth() < c.getMaxHealth() * 0.3)
                    {
                        gain = 2;
                    }
                    else if (c.getCurrHealth() < c.getMaxHealth() * 0.7)
                    {
                        gain = 4;
                    }
                    else
                    {
                        gain = 8;
                    }
                    break;
                default:
                    break;
            }

            if (c.getCurrResource() + gain <= c.getMaxResource())
            {
                c.setCurrResource(c.getCurrResource() + gain);
            }
            else
            {
                c.setCurrResource(c.getMaxResource());
            }
        }

        private void currentSelect()
        {
            if (currentID == -1 || (currentID + 1) >= initiativeTeam.Count())
            {
                currentID = 0;
                newTurn();
            }
            else
            {
                currentID++;
            }
            if (!initiativeTeam[currentID].isAlive())
            {
                currentSelect();
            }
            if (initiativeTeam[currentID].isStunned())
            {
                initiativeTeam[currentID].setStunned(false);
                currentSelect();
            }
            setCurrentCharacter(initiativeTeam[currentID]);
            playerTurn = false;
            foreach (Character P in playerTeam)
            {
                if (P.getName().Equals(currentCharacter.getName()))
                {
                    playerTurn = true;
                }
            }
        }

        private int bothTeamsAlive()
        {
            bool playerAlive = false;
            bool enemyAlive = false;
            foreach (Character P in playerTeam)
            {
                if (P.getCurrHealth() > 0)
                {
                    playerAlive = true;
                }
            }
            foreach (Character E in enemyTeam)
            {
                if (E.getCurrHealth() > 0)
                {
                    enemyAlive = true;
                }
            }
            if (playerAlive && enemyAlive)
            {
                return 0;
            }
            else if (!playerAlive && enemyAlive)
            {
                return -1;
            }
            else if (playerAlive && !enemyAlive)
            {
                return 1;
            }
            return -1;
        }

        private void createInitiative()
        {
            currentID = -1;
            initiativeTeam = new List<Character>();
            initiativeTeam.AddRange(playerTeam);
            initiativeTeam.AddRange(enemyTeam);
            Character temp;
            for (int i = 0; i < initiativeTeam.Count() - 1; i++)
            {
                for (int j = i + 1; j < initiativeTeam.Count(); j++)
                {
                    if (initiativeTeam[j].getSpeed() <= initiativeTeam[i].getSpeed())
                    {
                        temp = initiativeTeam[i];
                        initiativeTeam[i] = initiativeTeam[j];
                        initiativeTeam[j] = temp;
                    }
                }
            }
        }

        public List<Character> getPlayerTeam()
        {
            return playerTeam;
        }

        public void setPlayerTeam(List<Character> playerTeam)
        {
            this.playerTeam = playerTeam;
        }

        public List<Character> getEnemyTeam()
        {
            return enemyTeam;
        }

        public void setEnemyTeam(List<Character> enemyTeam)
        {
            this.enemyTeam = enemyTeam;
        }

        public List<Character> getInitiativeTeam()
        {
            return initiativeTeam;
        }

        public void setInitiativeTeam(List<Character> initiativeTeam)
        {
            this.initiativeTeam = initiativeTeam;
        }

        public Character getCurrentCharacter()
        {
            return currentCharacter;
        }

        public void setCurrentCharacter(Character currentCharacter)
        {
            this.currentCharacter = currentCharacter;
            for (int i = 0; i < playerTeam.Count(); i++)
            {
                playerTeam[i].setActive(false);
            }
            currentCharacter.setActive(true);
            this.currentCharacter.setActive(true);
        }
    }
}
