using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorState : MonoBehaviour
{
    public bool lockCursor = true;

    void Awake()
    {

        Cursor.visible = (false);

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.M))
        {
            lockCursor = !lockCursor;
        }

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
