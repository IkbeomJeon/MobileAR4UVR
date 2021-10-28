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
        public class Transition
        {
            [SerializeField]
            public string user_id;
            [SerializeField]
            public string anchor_trajectory_ids;
            [SerializeField]
            public List<AnchorRecom> anchor_recoms;
            [SerializeField]
            public int recomType;

        }

        [Serializable]
        public class AnchorRecom
        {
            [SerializeField]
            public long anchor_id;

            [SerializeField]
            public double weight;
        }
    }
}
