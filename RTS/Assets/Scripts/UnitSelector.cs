using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class UnitSelector : MonoBehaviour
{
    private const int FACTION = 0; // Player is always faction 0
    private Camera mainCamera;
    private LayerMask unitLayer;
    private LayerMask buildingLayer;
    private LayerMask selectableLayer;
    public List<UnitControllerAPI> selectedUnits = new List<UnitControllerAPI>();
    private List<ToggleSelectOutline> selectedUnitsOutlines = new List<ToggleSelectOutline>();
    public List<Transform> selectedUnitsTransforms = new List<Transform>();
    public FactoryScript selectedFactory;
    public GameObject FlagPrefab;
    private void Awake(){
        unitLayer = LayerMask.GetMask("UnitFaction0");
        buildingLayer = LayerMask.GetMask("Building");
        SetSelectLayerMask(new int[]{0,1,2,3});
        mainCamera = Camera.main;
    }

    private void Update(){
        if(InputListener.Instance.primaryDown){      
            UnitSelect();
            
        }

        if(InputListener.Instance.secondaryDown){
            UnitTarget();
        }
    }

    private void SetSelectLayerMask(int[] factions) {
        int count = factions.Length;
        string[] layers = new string[count*2 + 1];
        StringBuilder sb = new StringBuilder();
        int i;
        // Add unit factions
        for(i = 0; i < count; i++){
            sb.Clear();
            sb.Append("UnitFaction");
            sb.Append(factions[i]);
            layers[i] = sb.ToString();
        }
        // Add building factions
        for(i = 0; i < count; i++){
            sb.Clear();
            sb.Append("BuildingFaction");
            sb.Append(factions[i]);
            layers[count + i] = sb.ToString();
        }
        sb.Append("Building");
        sb.Clear();
        sb.Append("Map");
        layers[count * 2] = sb.ToString();

        foreach (var l in layers)
            Debug.Log(l);

        selectableLayer = LayerMask.GetMask(layers);
        layers = null;
    }

    private void UnitSelect(){
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0.1f, unitLayer);
        Transform unit = hit.transform;
        if(unit != null){
            UnitControllerAPI hitUnit = unit.GetComponent<UnitControllerAPI>();
            int hitFaction = hitUnit.FACTION;
            if(hitFaction == FACTION){ // Select Friendly Unit
                ToggleSelectOutline selector = unit.GetComponent<ToggleSelectOutline>();
                if(!InputListener.Instance.leftShift){
                    ClearSelectedUnits();
                }
                selector.ToggleOutline();
                AddSelectedUnit(hitUnit, selector);
            }else{
                // Dictate enemy unit select logic
                // Simply select
                // Or Target
                int numSelectedUnits = selectedUnits.Count;
                int i;
                for(i = 0; i < numSelectedUnits; i++){
                    selectedUnits[i].SetUnitTarget(hitUnit);
                }
            }
        }else{
            BuildingSelect();
        }         
    }

    private void BuildingSelect(){
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0.1f, buildingLayer);
        Transform building = hit.transform;
        if(building != null){
            Building hitBuilding = building.GetComponent<Building>();
            int faction = hitBuilding.FACTION;
            if(faction == FACTION){
                ToggleSelectOutline selector = hitBuilding.GetComponent<ToggleSelectOutline>();
                selector.ToggleOutline();
                ClearSelectedUnits();
                selectedFactory = hitBuilding.GetComponent<FactoryScript>();
                selectedUnitsTransforms.Add(hitBuilding.transform);
                UnitSelectEnter();
            }
            Debug.Log("Building");
            UIManager.Instance.OpenFactoryUI();
        }else{
            ClearSelectedUnits();
            UIManager.Instance.CloseFactoryUI();
        }
    }

    private void AddSelectedUnit(UnitControllerAPI selectedUnit, ToggleSelectOutline selectedOutline){
        selectedUnits.Add(selectedUnit);
        selectedUnitsOutlines.Add(selectedOutline);
        selectedUnitsTransforms.Add(selectedUnit.transform);
        UnitSelectEnter();
    }

    private void ClearSelectedUnits(){
        int size = selectedUnitsOutlines.Count;
        int i;
        for(i = 0; i < size; i++){
            if(selectedUnitsOutlines[i] == null){ i++;}
            else{ selectedUnitsOutlines[i].ToggleOutline(); }
        }
        if(selectedFactory != null){
            selectedFactory.GetComponent<ToggleSelectOutline>().ToggleOutline();
        }
        selectedFactory = null;
        selectedUnits.Clear();
        selectedUnitsOutlines.Clear();
        selectedUnitsTransforms.Clear();
    }

    private void UnitTarget(){
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0.1f, selectableLayer);
        Transform target = hit.transform;
        string targetTag;

        if(target != null){  // Did we select a unit
            targetTag = target.transform.tag;
            Debug.Log(targetTag);
            if(targetTag.Equals("Unit")){
                // Selected Unit
                Debug.Log("Unit");
                UnitControllerAPI targetUnit = target.GetComponent<UnitControllerAPI>();
                SetUnitTarget(targetUnit);
                return;
            }else if(targetTag.Equals("Building")){
                // Selected Building
                Debug.Log("Building");
            }else if(targetTag.Equals("Map")){
                // Selected Map
                Debug.Log("MAP");
                Vector3 targetPosition = target.position;
                SetPositionTarget(targetPosition);
            }
            else
            {
                Vector3 targetPosition = target.position;
                SetPositionTarget(targetPosition);
            }
        }else{
            Vector3 targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            SetPositionTarget(targetPosition);
            GameObject flag = Instantiate(FlagPrefab, targetPosition, Quaternion.identity);
            Destroy(flag, 2.5f);
        }
    }

    private void SetUnitTarget(UnitControllerAPI targetUnit){
        int faction = targetUnit.FACTION;
        if(faction == FACTION){
            return; // Don't target same faction
        }
        int numSelectedUnits = selectedUnits.Count;
        int i;
        for(i = 0; i < numSelectedUnits; i++){
            selectedUnits[i].SetUnitTarget(targetUnit);
        }
    }

    private void SetPositionTarget(Vector3 targetPosition){
        int numSelectedUnits = selectedUnits.Count;
        int i;
        for(i = 0; i < numSelectedUnits; i++){
            selectedUnits[i].SetPositionTarget(targetPosition);
        }
    }

    private void SetBuildingTarget(){ // SetBuildingTarget(Building buildingTarget)

    }

    public event Action OnUnitSelectEnter;
    public void UnitSelectEnter(){
        if(OnUnitSelectEnter != null){
            OnUnitSelectEnter();
        }
    }
}