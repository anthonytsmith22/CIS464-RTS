using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{
    private float minX, maxX, minY, maxY;
    Transform Map;
    [SerializeField] Transform FreeLockCameraPoint;
    [SerializeField] UnitSelector selector;
    List<Transform> SelectedUnits = new List<Transform>();
    [SerializeField] float cameraMoveSpeed = 2f;
    CinemachineVirtualCamera cmCam;
    [SerializeField] float startingOrthoSize = 5f;
    [SerializeField] float minOrthoSize = 2f;
    [SerializeField] float maxOrthoSize = 8f;
    private float currentOrthoSize;
    
    private void Awake(){
        cmCam = GetComponent<CinemachineVirtualCamera>();
    }
    private void Start(){
        SetupClamp();
        GetSelectedUnits();
        selector.OnUnitSelectEnter += GetSelectedUnitsViewPosition;
        cmCam.m_Lens.OrthographicSize = startingOrthoSize;
        currentOrthoSize = startingOrthoSize;
    }

    private void Update(){
        FreeMoveCamera();
        ResizeCamera();
    }

    private void ResizeCamera(){
        float scrollDelta = Input.mouseScrollDelta.y * 0.5f;
        currentOrthoSize -= scrollDelta;
        currentOrthoSize = Mathf.Clamp(currentOrthoSize, minOrthoSize, maxOrthoSize);
        cmCam.m_Lens.OrthographicSize = currentOrthoSize;
    }

    private void FreeMoveCamera(){
        float horizontal = InputListener.Instance.horizontal;
        float vertical = InputListener.Instance.vertical;

        Vector3 moveCam = new Vector3(horizontal, vertical);
        Vector3 camPosition = FreeLockCameraPoint.position + moveCam * cameraMoveSpeed * Time.deltaTime;
        camPosition = ClampPosition(camPosition);
        FreeLockCameraPoint.position = camPosition;
    }

    private void GetSelectedUnits(){
        SelectedUnits = selector.selectedUnitsTransforms;
    }

    private void GetSelectedUnitsViewPosition(){
        int numUnits = SelectedUnits.Count;
        int i;
        Vector3 avgPosition = Vector3.zero;
        for(i = 0; i < numUnits; i++){
            avgPosition += SelectedUnits[i].position;
        }
        Vector3 centerPosition = new Vector3(avgPosition.x / numUnits, avgPosition.y / numUnits, 0f);
        FreeLockCameraPoint.position = centerPosition;
        ClampPosition();
    }

    private void SetupClamp(){ // Set mix, max X and Z
        Map = GameObject.Find("map").transform;
        
        float centerX = Map.position.x;
        float centerY = Map.position.y;

        float scaleX = Map.localScale.x;
        float scaleY = Map.localScale.y;

        minX = centerX - (scaleX/2);
        maxX = centerX + (scaleX/2);

        minY = centerY - (scaleY/2);
        maxY = centerY + (scaleY/2);
    }

    private void ClampPosition(){
        Vector3 position = FreeLockCameraPoint.position;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
    }

    private Vector3 ClampPosition(Vector3 position){
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        return position;
    }
}
