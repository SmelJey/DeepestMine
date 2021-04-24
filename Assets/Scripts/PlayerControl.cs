using UnityEngine;

public class PlayerControl : MonoBehaviour {
    [SerializeField] private float cameraMovementSpeed = 1;
    
    private void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        
        transform.Translate(new Vector3(x, y) * cameraMovementSpeed * Time.deltaTime);
        Debug.Log($"{x}, {y}");
    }
}
