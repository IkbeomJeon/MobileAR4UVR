using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using KCTM.Network.Data;

public class AnchorConverter : JsonConverter
{
    private bool IsContainLinkedAnchor;

    public AnchorConverter(bool IsContainLinkedAnchor)
    {
        this.IsContainLinkedAnchor = IsContainLinkedAnchor;
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(Anchor) == objectType;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        Anchor anchor = new Anchor();

        if(IsContainLinkedAnchor)
        {
            JObject obj = JObject.Load(reader);

            anchor.id = obj.GetValue("id").Value<long>();
            anchor.updatedtime = obj.GetValue("updatedtime").Value<long>();
            anchor.uploadedtime = obj.GetValue("uploadedtime").Value<long>();

            anchor.title = obj.GetValue("title").Value<string>();
            anchor.description = obj.GetValue("description").Value<string>();
            anchor.sharingtype = obj.GetValue("sharingtype").Value<string>();

            anchor.enablelike = obj.GetValue("enablelike").Value<bool>();
            if (obj.GetValue("likes") != null)
            {
                anchor.likes = JsonConvert.DeserializeObject<List<Like>>(obj.GetValue("likes").ToString());
            }

            anchor.enablecomment = obj.GetValue("enablecomment").Value<bool>();
            if (obj.GetValue("comments") != null)
            {
                anchor.comments = JsonConvert.DeserializeObject<List<Comment>>(obj.GetValue("comments").ToString());
            }

            if (obj.GetValue("point") != null)
            {
                anchor.point = JsonConvert.DeserializeObject<Point>(obj.GetValue("point").ToString());
            }

            if (obj.GetValue("user") != null)
            {
                anchor.user = JsonConvert.DeserializeObject<User>(obj.GetValue("user").ToString());
            }
            if (obj.GetValue("tags") != null)
            {
                anchor.tags = JsonConvert.DeserializeObject<List<Tag>>(obj.GetValue("tags").ToString());
            }

            if (obj.GetValue("contentinfos") != null)
            {
                anchor.contentinfos = JsonConvert.DeserializeObject<List<ContentInfo>>(obj.GetValue("contentinfos").ToString());
            }

            if(obj.GetValue("linkers") != null)
            {
                anchor.linkedAnchors = JsonConvert.DeserializeObject<List<Anchor>>(obj.GetValue("linkers").ToString(), new AnchorConverter(true));
            }
        }
        else
        {
            anchor = serializer.Deserialize<Anchor>(reader);
        }

        return anchor;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}