using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationTest : MonoBehaviour
{
    Matrix4x4 mat_Realworld2ARworld;
    // Start is called before the first frame update
    void Start()
    {
        mat_Realworld2ARworld = transform.localToWorldMatrix.inverse;
        Quaternion rotation = Quaternion.identity;
        rotation = Quaternion.Euler(0, -5, 0);
        Matrix4x4 mat = Matrix4x4.identity;

        mat.SetTRS(Vector3.zero, rotation, new Vector3(1, 1, 1));

        // positionMat.SetColumn(3, new Vector4(addedPos.x, addedPos.y, addedPos.z, 1));

        mat_Realworld2ARworld *= mat.inverse;

        transform.position = mat_Realworld2ARworld.GetColumn(3);
        transform.rotation = mat_Realworld2ARworld.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
