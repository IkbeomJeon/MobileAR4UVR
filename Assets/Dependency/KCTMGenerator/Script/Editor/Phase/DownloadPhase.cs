using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace ARRC_DigitalTwin_Generator
{
    class DownloadPhase : Phase
    {
        List<DownloadItem> items;
        ARRCGenerator gen;

        public DownloadPhase(string _title, ARRCGenerator gen)
        {
            title = _title;
            this.gen = gen;
        }

        public override void Start()
        {
            DownloadManager.Start(gen.items, 16);
            totalSize = DownloadManager.totalSizeMB;
        }

        public override void Enter()
        {
            bool result = DownloadManager.CheckComplete();
            phaseProgress = (float)DownloadManager.progress;

            if (result)
            {
                Dispose();
                isComplete = true;
            }
            //Repaint할 Window를 미리 등록하도록 수정해야함.
            //ARRCDigtalTwinGeneratorWindow.wnd.Repaint();
        }

        public override void Dispose()
        {
            DownloadManager.Dispose();
        }
    }
}
