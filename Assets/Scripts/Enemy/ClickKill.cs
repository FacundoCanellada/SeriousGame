using UnityEngine;
using UnityEngine.InputSystem;

public class KillOnClick : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Detectar clic o toque
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            DetectClick(Mouse.current.position.ReadValue());
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            DetectClick(Touchscreen.current.primaryTouch.position.ReadValue());
        }
    }

    void DetectClick(Vector2 position)
    {
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(position);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            Health health = GetComponent<Health>();
            if (health != null)
            {
                health.Damage(9999);
            }
        }
    }
}