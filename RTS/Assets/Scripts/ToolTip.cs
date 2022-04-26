using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{

    public string Text = "No tooltip text set";
    public GameObject TooltipUI;
    public RectTransform Rect;
    public Vector2 Offset;

    // Start is called before the first frame update
    void Start()
    {
        // create tooltip instance
        Show(false);
        Rect = TooltipUI.GetComponent<RectTransform>();
    }

    public void Show(bool show)
    {
        TooltipUI.transform.position = Input.mousePosition;
        TooltipUI.GetComponent<Text>().text = Text;
        TooltipUI.SetActive(show);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
