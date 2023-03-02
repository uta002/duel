using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorManager : MonoBehaviour
{
    bool isHide;
    private void Awake()
    {
        LockCursor();
    }


    private void Update()
    {

        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }
        if (Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }
        */
    }

    public static void LockCursorS()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
