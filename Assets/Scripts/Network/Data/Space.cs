using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCTM
{
    namespace Network
    {
        namespace Data
        {
            public class Space : Data
            {
                public User user;
                public Point centerpoint;
                public string name;

                public List<Point> points;
            }
        }
    }
}
