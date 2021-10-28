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
        public class StoringInfo
        {
            [SerializeField]
            public long anchor_id;
            [SerializeField]
            public string user_id;
            [SerializeField]
            public double user_latitude;
            [SerializeField]
            public double user_longitude;
            [SerializeField]
            public double visited_duration;

            [SerializeField]
            public string visited_date;

            [SerializeField]
            public int user_like;

            public StoringInfo()
            {
                
            }

                public StoringInfo(VisitedContent v)
            {
                anchor_id = v.anchor.id;
                user_id = v.user_id;
                user_latitude = v.userLat;
                user_longitude = v.userLon;
                visited_date = v.visitedDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                visited_duration = v.userVisitedTime;
                user_like = v.liked;
            }

        }
    }
}
