using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRC_DigitalTwin_Generator
{
    class ARRCGenerator
    {
        public List<DownloadItem> items;

        public bool isComplete;
        public float progress;
        public float totalSize;

        public string currentState;

        public virtual bool CheckComplete() { return isComplete; }

    }
}
