using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KCTM.Network.Data;

namespace KCTM
{
    namespace Recommendation
    {
        [Serializable]
        public class TrajectorySegment
        {
            [SerializeField]
            public List<Segment> segments;

        }

        [Serializable]
        public class Segment
        {
            [SerializeField]
            public string anchorIds;
            [SerializeField]
            public List<String> points;

        }
    }
}
