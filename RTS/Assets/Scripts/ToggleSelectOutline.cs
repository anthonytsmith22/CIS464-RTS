using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSelectOutline : MonoBehaviour
{
    [SerializeField] SpriteRenderer outlineRenderer;

    private bool isOutlined = false;

    public void ToggleOutline(){
        isOutlined = !isOutlined;
        outlineRenderer.enabled = isOutlined;
    }
}
