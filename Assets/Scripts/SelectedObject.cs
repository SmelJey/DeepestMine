using System.Collections.Generic;
using UnityEngine;

public class SelectedObject : MonoBehaviour {
    public Building AssociatedBuilding { get; set; }
    private Collider2D myCollider2D;
    private SpriteRenderer mySpriteRenderer;

    private void Awake() {
        myCollider2D = GetComponent<Collider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool CheckAccessibility(Dictionary<Resource, int> resources) {
        var color = Color.green;
        if (!SessionManager.Instance.CheckCost(AssociatedBuilding.Costs)) {
            color = Color.red;
        }
        
        var res = new Collider2D[1];
        if (myCollider2D.OverlapCollider(new ContactFilter2D(), res) > 0) {
            color = Color.red;
        }

        mySpriteRenderer.color = color;
        return color == Color.green;
    }

    public bool Instantiate() {
        if (!CheckAccessibility(SessionManager.Instance.ResourceCount)) {
            return false;
        }
        
        mySpriteRenderer.color = Color.white;
        SessionManager.Instance.Buy(AssociatedBuilding.Costs);
        if (AssociatedBuilding.name == "Factory") {
            SessionManager.Instance.AddFactory(gameObject);
        }
        
        Destroy(this);
        return true;
    }
}
