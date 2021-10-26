using System;

[Serializable]
public class History
{
    public History()
    {

    }

    public History(long id, string contenttype, string contentdepth)
    {
        this.id = id;
        this.contenttype = contenttype;
        this.contentdepth = contentdepth;
    }

    public long id;
    public string contenttype;
    public string contentdepth;
}
