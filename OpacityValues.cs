using System;
using System.Collections.Generic;
using System.Text;

namespace NEA
{
    class OpacityValues
    {
        public double eVis { get; private set; }
        public double BVis { get; private set; }
        public double GVis { get; private set; }
        public double DVis { get; private set; }
        public double AVis { get; private set; }
        public double EVis { get; private set; }

        public OpacityValues()
        {
            Initialize(1.0);
        }

        public void Initialize(double value)
        {
            eVis = value;
            BVis = value;
            GVis = value;
            DVis = value;
            AVis = value;
            EVis = value;
        }

        public void ChangeOpacity(char exceptionString)
        {
            Initialize(0.25);
            if (exceptionString == 'E')
            {
                EVis = 1.0;
            }
            else if (exceptionString == 'A')
            {
                AVis = 1.0;
            }
            else if (exceptionString == 'D')
            {
                DVis = 1.0;
            }
            else if (exceptionString == 'G')
            {
                GVis = 1.0;
            }
            else if (exceptionString == 'B')
            {
                BVis = 1.0;
            }
            else if (exceptionString == 'e')
            {
                eVis = 1.0;
            }
        }
    }
}
