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
            baseImage = i;
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{baseImage}/base.png");
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }

        public void setImageToBase()
        {
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{baseImage}/base.png");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }

        public void setImageToAttack()
        {
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{baseImage}/attack.png");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }

        public void setImageToHurt()
        {
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{baseImage}/hurt.png");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }

        public void setImageToDead()
        {
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{baseImage}/dead.png");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }

        public Image getProfileImage()
        {
            try
            {
                return Image.FromFile($@"../../Art/Sprites/Characters/{baseImage}/profile.png");
            }
            catch
            {
                return Image.FromFile($@"../../Art/Sprites/Characters/{baseImage}/base.png");
            }
        }
    }
}
