using System;
using System.Drawing;

namespace Portals_of_Madness
{
    public class Sprite
    {
        public string BaseImage { get; set; }
        public Image Image { get; set; }

        public Sprite(string i)
        {
            BaseImage = i;
            try
            {
                Image = Image.FromFile($@"../../Art/Sprites/Characters/{BaseImage}/base.png");
            }
            catch
            {
                Console.WriteLine($"{BaseImage}/base not found!");
            }
        }

        public void SetImageToBase()
        {
            try
            {
                Image = Image.FromFile($@"../../Art/Sprites/Characters/{BaseImage}/base.png");
            }
            catch
            {
                Console.WriteLine($"{BaseImage}/base not found!");
            }
        }

        public void SetImageToAttack()
        {
            try
            {
                Image = Image.FromFile($@"../../Art/Sprites/Characters/{BaseImage}/attack.png");
            }
            catch
            {
                Console.WriteLine($"{BaseImage}/attack not found!");
            }
        }

        public void SetImageToHurt()
        {
            try
            {
                Image = Image.FromFile($@"../../Art/Sprites/Characters/{BaseImage}/hurt.png");
            }
            catch
            {
                Console.WriteLine($"{BaseImage}/hurt not found!");
            }
        }

        public void SetImageToDead()
        {
            try
            {
                Image = Image.FromFile($@"../../Art/Sprites/Characters/{BaseImage}/dead.png");
            }
            catch
            {
                Console.WriteLine($"{BaseImage}/dead not found!");
            }
        }
    }
}
