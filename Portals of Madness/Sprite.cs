using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portals_of_Madness
{
    public class Sprite
    {
        protected string baseImage;
        protected string image;

        public Sprite(string image)
        {
            this.image = image;
        }

        public string getBaseImage()
        {
            return baseImage;
        }

        public string getImage()
        {
            return image;
        }

        public void setBaseImage(String image)
        {
            this.baseImage = image;
        }

        public void setImage(String image)
        {
            this.image = image + '1';
        }
    }
}
