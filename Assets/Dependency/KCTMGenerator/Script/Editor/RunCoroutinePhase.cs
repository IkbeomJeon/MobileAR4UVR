using EditorCoroutines;
using System;
using System.Collections;
using UnityEditor;

namespace ARRC_DigitalTwin_Generator
{
    class RunCoroutinePhase : Phase
    {

        EditorWindow editorWnd;
        ARRCGenerator gen;
        IEnumerator func;
        public RunCoroutinePhase(ARRCGenerator gen, EditorWindow editorWnd, IEnumerator coroutineFunc)
        {
            this.editorWnd = editorWnd;
            this.gen = gen;
            func = coroutineFunc;
        }

        public override void Start()
        {
            phaseProgress = 0;
            //totalSize = gen.totalSize;

            editorWnd.StartCoroutine(func);
            title = gen.currentState;
        }

        public override void Enter()
        {
            bool result = gen.CheckComplete();
            phaseProgress = gen.progress;

            if (result)
            {
                Dispose();
                isComplete = true;
            }

            editorWnd.Repaint();
        }

        public override void Dispose()
        {
            gen.isComplete = true;
        }
    }
}
