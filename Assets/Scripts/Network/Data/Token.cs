using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KCTM.Network.Data;

namespace KCTM
{
    namespace Network
    {
        namespace Data
        {
            [Serializable]
            public class Token
            {
                [SerializeField]
                public string token { get; set; }

                [SerializeField]
                public double weight { get; set; }
            }
        }
    }
}