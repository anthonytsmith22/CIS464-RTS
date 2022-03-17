using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{
    private float minX, maxX, minY, maxY;
    Transform Map;
    [SerializeField] Transform FreeLookCameraPoint;
    [SerializeField] Transform CameraStartPosition;
    [SerializeField] UnitSelector selector;
    List<Transform> SelectedUnits = new List<Transform>();
    [SerializeField] float cameraMoveSpeed = 3f;
    float minCameraMoveSpeed;
    float maxCameraMoveSpeed;
    CinemachineVirtualCamera cmCam;
    [SerializeField] float startingOrthoSize = 5f;
    float minOrthoSize = 2f;
    float maxOrthoSize = 16;
    private float currentOrthoSize;
    
    private void Awake(){
        cmCam = GetComponent<CinemachineVirtualCamera>();
        minCameraMoveSpeed = cameraMoveSpeed;
        maxCameraMoveSpeed = cameraMoveSpeed * 5f;
    }
    private void Start(){
        SetupClamp();
        GetSelectedUnits();
        selector.OnUnitSelectEnter += GetSelectedUnitsViewPosition;
        cmCam.m_Lens.OrthographicSize = startingOrthoSize;
        currentOrthoSize = startingOrthoSize;
        FreeLookCameraPoint.position = CameraStartPosition.position;
    }

    private void Update(){
        FreeMoveCamera();
        ResizeCamera();
    }

    private void ResizeCamera(){
        float scrollDelta = Input.mouseScrollDelta.y;
        float scrollChange = scrollDelta * 0.5f;
        currentOrthoSize -= scrollChange;
        currentOrthoSize = Mathf.Clamp(currentOrthoSize, minOrthoSize, maxOrthoSize);

        // Get camera move speed
        float orthoRatio = currentOrthoSize / (maxOrthoSize - minOrthoSize);
        cameraMoveSpeed = (maxCameraMoveSpeed - minCameraMoveSpeed) * orthoRatio + minCameraMoveSpeed;
        cameraMoveSpeed = Mathf.Clamp(cameraMoveSpeed, minCameraMoveSpeed, maxCameraMoveSpeed); 
        cmCam.m_Lens.OrthographicSize = currentOrthoSize;
    }

    private void FreeMoveCamera(){
        float horizontal = InputListener.Instance.horizontal;
        float vertical = InputListener.Instance.vertical;

        Vector3 moveCam = new Vector3(horizontal, vertical);
        Vector3 camPosition = FreeLookCameraPoint.position + moveCam * cameraMoveSpeed * Time.deltaTime;
        camPosition = ClampPosition(camPosition);
        FreeLookCameraPoint.position = camPosition;
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
        FreeLookCameraPoint.position = centerPosition;
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
        Vector3 position = FreeLookCameraPoint.position;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
    }

    private Vector3 ClampPosition(Vector3 position){
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        return position;
    }
}
