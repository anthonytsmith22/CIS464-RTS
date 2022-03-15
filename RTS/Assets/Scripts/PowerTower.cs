using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerTower : MonoBehaviour
{

    public LineRenderer LineRenderer;

    public PowerCollider Collider;

    public bool Connected { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        LineRenderer.positionCount = 2;
        
        UpdateConnections();
    }

    public void UpdateConnections()
    {
        
        Vector3 target = new Vector3();

        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.GetMask("Building");



        Vector3[] pos = new Vector3[2] {
            new Vector3(0.0f, 0.0f, 0.0f),
            target
        };

        pos[0] += transform.position;
        pos[1] += transform.position;

        LineRenderer.SetPositions(pos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
