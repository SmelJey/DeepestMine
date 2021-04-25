using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {
    [SerializeField] private float cameraMovementSpeed = 1;
    [SerializeField] private InfoPanel infoPanel;

    public SelectedObject SelectedBuilding { get; set; }
    public List<ISelectable> selectedObject;

    private Camera myCamera;

    public void SelectBuilding(Building building) {
        selectedObject.Clear();
        
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
        
        selectedObject.Clear();
        selectedObject.AddRange(obj.GetComponents<ISelectable>());
        infoPanel.ShowSelectedObjectsInfo(selectedObject);
    }
    
    private void Awake() {
        myCamera = Camera.main;
        selectedObject = new List<ISelectable>();
    }

    private void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(x, y) * cameraMovementSpeed * Time.deltaTime);
        
        var mousePos = myCamera.ScreenToWorldPoint(Input.mousePosition);
        
        if (!ReferenceEquals(SelectedBuilding, null)) {
            SelectedBuilding.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
            SelectedBuilding.CheckAccessibility(SessionManager.Instance.ResourceCount);

            if (Input.GetMouseButtonDown(0)) {
                if (!EventSystem.current.IsPointerOverGameObject() && SelectedBuilding.Instantiate()) {
                    SelectedBuilding = null;
                }
            } else if (Input.GetMouseButtonDown(1)) {
                Destroy(SelectedBuilding.gameObject);
                SelectedBuilding = null;
            }

            return;
        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            var hit = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero, 0f);

            if (hit.collider != null) {
                SelectObject(hit.collider.gameObject);
            }
        }
    }
}
