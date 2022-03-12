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
    private LayerMask selectableLayer;
    public List<UnitControllerAPI> selectedUnits = new List<UnitControllerAPI>();
    private List<ToggleSelectOutline> selectedUnitsOutlines = new List<ToggleSelectOutline>();
    public List<Transform> selectedUnitsTransforms = new List<Transform>();
    private void Awake(){
        unitLayer = LayerMask.GetMask("UnitFaction0");
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
        string[] layers = new string[count*2];
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
            ClearSelectedUnits();
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
            if(targetTag.Equals("Unit")){
                // Selected Unit
                UnitControllerAPI targetUnit = target.GetComponent<UnitControllerAPI>();
                SetUnitTarget(targetUnit);
                return;
            }else if(targetTag.Equals("Building")){
                // Selected Building
            }else if(targetTag.Equals("Map")){
                // Selected Map
                Vector3 targetPosition = target.position;
                SetPositionTarget(targetPosition);
            }
        }else{
            Vector3 targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            SetPositionTarget(targetPosition);
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
