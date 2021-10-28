using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KCTM
{
    namespace Network
    {
        namespace Data
        {
            [Serializable]
            public class ContentInfo
            {
                public Content content;
                public string coordinatetype;
                public double positionx;
                public double positiony;
                public double positionz;

                public double rotationx;
                public double rotationy;
                public double rotationz;

                public double scalex = 1;
                public double scaley = 1;
                public double scalez = 1;
            }

            [Serializable]
            public class Anchor : Data
            {
                public User user;
                public string sharingtype;
                public string description;
                public string title;
                public bool enablelike;
                public bool enablecomment;
                /// <value>Optional</value>
                public List<Comment> comments;
                /// <value>Optional</value>
                public List<Like> likes;
                public Point point;
                /// <value>Optional</value>
                public List<AnchorLinker> linkers;
                public List<Anchor> linkedAnchors;
                /// <value>Optional</value>
                public List<Tag> tags;
                public List<ContentInfo> contentinfos;

                public string contenttype;
                public string contentdepth;
                public int age;
                public int experiencetime;
                public int season;
                public int weather;
                public int prefertime;
                public int companion;

                public string status = "new";

            }
        }
    }
}

