using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {
    [SerializeField] private float cameraMovementSpeed = 1;
    [SerializeField] private InfoPanel infoPanel;
    [SerializeField] private LayerMask selectableLayerMask;

    public SelectedObject SelectedBuilding { get; set; }
    public List<ISelectable> selectedComponents;
    private GameObject selectedGameObject;

    private Camera myCamera;

    public void SelectBuilding(Building building) {
        selectedComponents.Clear();
        selectedGameObject = null;
        
        if (!ReferenceEquals(SelectedBuilding, null)) {
            Destroy(SelectedBuilding.gameObject);
        }

        var obj = Instantiate(building.Prefab);
        var selectedComp = obj.AddComponent<SelectedObject>();
        selectedComp.AssociatedBuilding = building;
        SelectedBuilding = selectedComp;
    }

    public void SelectObject(GameObject obj) {
        if (SelectedBuilding != null) {
            return;
        }
        
        selectedComponents.Clear();
        selectedComponents.AddRange(obj.GetComponents<ISelectable>());
        selectedGameObject = obj;
        infoPanel.ShowSelectedObjectsInfo(selectedComponents);
    }
    
    private void Awake() {
        myCamera = Camera.main;
        selectedComponents = new List<ISelectable>();
    }

    private void Update() {
        if (selectedGameObject == null) {
            selectedComponents.Clear();
            infoPanel.ShowSelectedObjectsInfo(selectedComponents);
        }
        
        foreach (var selected in selectedComponents) {
            if (selected.IsUpdated()) {
                infoPanel.ShowSelectedObjectsInfo(selectedComponents);
                break;
            }
        }
        
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(x, y) * cameraMovementSpeed * Time.deltaTime);
        
        var mousePos = myCamera.ScreenToWorldPoint(Input.mousePosition);
        
        if (!ReferenceEquals(SelectedBuilding, null)) {
            SelectedBuilding.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
            SelectedBuilding.CheckAccessibility(SessionManager.Instance.ResourceCount);

            if (Input.GetMouseButtonDown(0)) {
                if (!EventSystem.current.IsPointerOverGameObject() && SelectedBuilding.Instantiate()) {
                    AstarPath.active.Scan();
                    SelectedBuilding = null;
                }
            } else if (Input.GetMouseButtonDown(1)) {
                Destroy(SelectedBuilding.gameObject);
                SelectedBuilding = null;
            }

            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        if (Input.GetMouseButtonDown(0)) {
            var hit = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero, 0f, selectableLayerMask);

            if (hit.collider != null) {
                SelectObject(hit.collider.gameObject);
            }
        } else if (Input.GetMouseButtonDown(1)) {
            foreach (var selected in selectedComponents) {
                selected.OnRightClick(mousePos);
            }
        }
    }
}
