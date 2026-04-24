using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaConverter.Models
{
    internal sealed class MediaData
    {
        private TimeSpan duration;

        private double framerate;

        private int primaryVideoIndex;

        public void setDuration(TimeSpan duration)
        {
            this.duration = duration; 
        }

        public void setFrameRate(double framerate) 
        {
            this.framerate = framerate;
        }

        public void setPrimaryVideoIndex(int primaryVideoIndex)
        {
            this.primaryVideoIndex = primaryVideoIndex;
        }

        public double getFrameRate()
        {
            return this.framerate;
        }

        public int getPrimaryVideoIndex()
        {
            return this.primaryVideoIndex;
        }


    }
}
