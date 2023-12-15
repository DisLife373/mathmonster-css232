using UnityEngine;
using UnityEngine.InputSystem;


public class InputHandler : MonoBehaviour
{
    private Camera _mainCamera;

    void Awake()
    {
        _mainCamera = Camera.main;
        
    }

    public void OnClick(InputAction.CallbackContext _context) {
        if (!_context.started) return;

        var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(pos: (Vector3)Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;

        Debug.Log(rayHit.collider.gameObject.name);
    }
}
