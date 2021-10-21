using System.Collections.Generic;
using UnityEngine;
namespace ARRC_DigitalTwin_Generator
{
    public class Phase
    {
        public string title;
        public bool isComplete;
        public float phaseProgress;
        public float totalSize;

        
        public virtual void Start()
        {
            isComplete = false;
            phaseProgress = 0;
        }

        public virtual void Enter() { }


        public virtual void Dispose() { }

    }
}