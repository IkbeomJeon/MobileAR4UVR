using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCTM
{
    namespace Network
    {
        namespace Data
        {
            public class Content : Data
            {
                public User user;
                public string type;
                public string uri;
                public string filename;
                public string mediatype;
                public List<ContentLinker> contentlinkers;
            }
        }
    }
}