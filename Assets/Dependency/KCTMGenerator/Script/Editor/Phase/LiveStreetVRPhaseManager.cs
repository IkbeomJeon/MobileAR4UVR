using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ARRC_DigitalTwin_Generator
{
    public class LiveStreetVRPhaseManager
    {
        public Phase activePhase;
        int activePhaseIndex;
        List<Phase> requiredPhases;

        StreetViewGenerator streetVewGen;
        VWorldBuildingGenerator vworldBuildingGen;
        KCTMAnchorGenerator kctmAnchorGenerator;

        static LiveStreetVRPhaseManager instance;
        public static LiveStreetVRPhaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LiveStreetVRPhaseManager();
                }
                return instance;
            }
        }

        public void Init(double latFrom, double lonFrom, double latTo, double lonTo, int zoom, EditorWindow wnd)
        {
            activePhaseIndex = -1;

            requiredPhases = new List<Phase>();
            streetVewGen = new StreetViewGenerator();
            vworldBuildingGen = new VWorldBuildingGenerator();
            kctmAnchorGenerator = new KCTMAnchorGenerator();

            //StreetView 관련
            if (ARRCDigtalTwinGeneratorWindow.Instance.bGenerateStreetView)
            {
                requiredPhases.Add(new RunCoroutinePhase(streetVewGen, wnd, streetVewGen.MakeDownloadList_StreetViewXMLList(latFrom, lonFrom, latTo, lonTo, zoom)));
                requiredPhases.Add(new DownloadPhase("Downloading StreetView XML list...", streetVewGen));
                requiredPhases.Add(new RunCoroutinePhase(streetVewGen, wnd, streetVewGen.MakeDownloadList_StreetViewImage(zoom)));
                requiredPhases.Add(new DownloadPhase("Downloading StreetView Images...", streetVewGen));
                requiredPhases.Add(new RunCoroutinePhase(streetVewGen, wnd, streetVewGen.GenerateStreetViews()));
            }

            if (ARRCDigtalTwinGeneratorWindow.Instance.bGenerateBuilding)
            {
                //VWorld Building 모델
                requiredPhases.Add(new RunCoroutinePhase(vworldBuildingGen, wnd, vworldBuildingGen.MakeDownloadItemList_XDODataList(latFrom, lonFrom, latTo, lonTo)));
                requiredPhases.Add(new DownloadPhase("Downloading VWorld XDO List...", vworldBuildingGen));
                requiredPhases.Add(new RunCoroutinePhase(vworldBuildingGen, wnd, vworldBuildingGen.MakeDownloadItemList_XDOData()));
                requiredPhases.Add(new DownloadPhase("Downloading VWorld XDO...", vworldBuildingGen));
                requiredPhases.Add(new RunCoroutinePhase(vworldBuildingGen, wnd, vworldBuildingGen.WriteOBJFiles()));
                requiredPhases.Add(new DownloadPhase("Downloading VWorld Obj's textures...", vworldBuildingGen));
                requiredPhases.Add(new RunCoroutinePhase(vworldBuildingGen, wnd, vworldBuildingGen.GenerateGameObjects_Building()));
            }
            if (ARRCDigtalTwinGeneratorWindow.Instance.bGenerateKCTMMetadata)
            {
                requiredPhases.Add(new RunCoroutinePhase(kctmAnchorGenerator, wnd, kctmAnchorGenerator.MakeDownloadList(latFrom, lonFrom, latTo, lonTo)));
                requiredPhases.Add(new DownloadPhase("Downloading & generating KCTM anchors...", kctmAnchorGenerator));
                //requiredPhases.Add(new RunCoroutinePhase(kctmAnchorGenerator, wnd, kctmAnchorGenerator.GenerateAnchorObjects()));
            }


            //// Test
            //GameObject prefab = Resources.Load("Terrain") as GameObject;
            //GameObject terrainObj = GameObject.Instantiate(prefab);
            //GameObject prefab2 = Resources.Load("POI Example") as GameObject;
            //GameObject poiObj = GameObject.Instantiate(prefab2);
        }

        public bool StartNextPhase()
        {
            activePhaseIndex++;
            if (requiredPhases == null || activePhaseIndex >= requiredPhases.Count)
            {
                //Debug.Log("No active phase");
                activePhase = null;
                return true;
            }

            activePhase = requiredPhases[activePhaseIndex];
            activePhase.Start();
            return false;
        }

        public void DisposeAllPhase()
        {
            if (requiredPhases == null)
                return;

            foreach (Phase p in requiredPhases)
                p.Dispose();

            requiredPhases = null;
            activePhase = null;
            activePhaseIndex = -1;

            streetVewGen = null;
            vworldBuildingGen = null;
        }
    }
}