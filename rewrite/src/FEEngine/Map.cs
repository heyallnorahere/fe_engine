using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEEngine
{
    public class Map
    {
        public int Width
        {
            get
            {
                return mWidth;
            }
            set
            {
                mWidth = value;
            }
        }
        public int Height
        {
            get
            {
                return mHeight;
            }
            set
            {
                mHeight = value;
            }
        }
        public Map(int width, int height)
        {
            mWidth = width;
            mHeight = height;
        }
        private int mWidth, mHeight;
    }
}
