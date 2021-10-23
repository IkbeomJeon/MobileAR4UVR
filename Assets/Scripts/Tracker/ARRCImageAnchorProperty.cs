using System;
using UnityEngine;

// Custom serializable class
public class ReadOnlyAttribute : PropertyAttribute
{

}

[Serializable]
public class ARRCImageAnchorProperty : MonoBehaviour
{
    [ReadOnly] public string guid;
    [ReadOnly] public Vector2 size;
    
   //[ReadOnly] public Texture texture;
}
