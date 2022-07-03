using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portals_of_Madness
{
    public class Sprite
    {
        public string baseImage { get; set; }
        public Image image { get; set; }

        public Sprite(string i)
        {
            var strings = i.Split(',');
            Random rand = new Random();
            int r = rand.Next(0, strings.Count() - 1);
            baseImage = strings[r];
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{baseImage}/base.png");
            }
            catch
            {
                Console.WriteLine($"Hát ez nincs itt haver: ../../Art/Sprites/Characters/{baseImage}/base.png");
            }
        }

        public void setImageToBase()
        {
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{baseImage}/base.png");
            }
            catch { }
        }

        public void setImageToAttack()
        {
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{baseImage}/attack.png");
            }
            catch { }
        }

        public void setImageToHurt()
        {
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{baseImage}/hurt.png");
            }
            catch { }
        }

        public void setImageToDead()
        {
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{baseImage}/dead.png");
            }
            catch { }
        }
    }
}
