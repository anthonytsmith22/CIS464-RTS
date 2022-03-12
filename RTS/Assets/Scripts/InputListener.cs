using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    #region Singleton
    private static InputListener instance;
    public static InputListener Instance { get { return instance; } }
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Debug.LogWarning("More than one instance of InputListener found!");
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    private KeyCode primaryKey = KeyCode.Mouse0;
    private KeyCode secondaryKey = KeyCode.Mouse1;
    private KeyCode interactKey = KeyCode.E;
    private KeyCode menuKey = KeyCode.Escape;
    private KeyCode leftShiftKey = KeyCode.LeftShift;
    public bool primaryDown;
    public bool primaryUp;
    public bool secondaryDown;
    public float horizontal;
    public float vertical;
    public float horizonalMouse;
    public float verticalMouse;
    public bool leftShift;
    public bool menu;
    public bool interact;
    public bool w;
    public bool a;
    public bool s;
    public bool d;

    private void Update(){
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        horizonalMouse = Input.GetAxisRaw("Mouse X");
        verticalMouse = Input.GetAxisRaw("Mouse X");
        menu = Input.GetKeyDown(interactKey);
        interact = Input.GetKeyDown(interactKey);
        primaryDown = Input.GetKeyDown(primaryKey);
        primaryUp = Input.GetKeyUp(primaryKey);
        secondaryDown = Input.GetKeyDown(secondaryKey);
        leftShift = Input.GetKey(leftShiftKey);

        w = Input.GetKey(KeyCode.W);
        a = Input.GetKey(KeyCode.A);
        s = Input.GetKey(KeyCode.S);
        d = Input.GetKey(KeyCode.D);
    }
    
}