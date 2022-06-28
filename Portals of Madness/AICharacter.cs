using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portals_of_Madness
{
    public class AICharacter : Character
    {
        private readonly string aiType;

        public AICharacter(string im,
            int l, string n, string c,
            double bHP, double hpM,
            string rN, int mR,
            double pAt, double pAtM, double mAt, double mAtM,
                double pAr, double pArM, double mAr, double mArM, List<string> weak,
            Ability ab1, Ability ab2, Ability ab3,
            int ini, string aiT) : base(im, l, n, c, bHP, hpM, rN, mR,
            pAt, pAtM, mAt, mAtM, pAr, pArM, mAr, mArM, weak, ab1, ab2, ab3, ini)
        {
            aiType = aiT;
        }

        public void act()
        { 
            
        }
    }
}
