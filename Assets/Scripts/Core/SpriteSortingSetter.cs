using UnityEngine;

/// <summary>
/// Utility to enforce SpriteRenderer sorting layer/order (and optional Z depth)
/// to avoid render-order issues like sprites appearing behind backgrounds.
/// Attach this to your Enemy, Player, Background, etc.
/// </summary>
public class SpriteSortingSetter : MonoBehaviour
{
    [Header("Sorting")]
    [Tooltip("Sorting Layer name (create in Project Settings > Tags and Layers > Sorting Layers)." )]
    public string sortingLayerName = "Characters";

    [Tooltip("Order within the sorting layer. Higher renders on top.")]
    public int sortingOrder = 0;

    [Tooltip("Apply to all child SpriteRenderers as well.")]
    public bool applyToChildren = true;

    [Header("Optional Z-Depth")]
    [Tooltip("Also force the object's Z position (useful for orthographic cameras)." )]
    public bool setZDepth = false;

    [Tooltip("Z value to set when setZDepth is enabled.")]
    public float zDepth = 0f;

    private void Awake()
    {
        Apply();
    }

    private void OnValidate()
    {
        // Apply in editor when values change for quick feedback
        if (!Application.isPlaying)
        {
            Apply();
        }
    }

    public void Apply()
    {
        if (setZDepth)
        {
            var p = transform.position;
            p.z = zDepth;
            transform.position = p;
        }

        if (applyToChildren)
        {
            var renderers = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sr in renderers)
            {
                SetSorting(sr);
            }
        }
        else
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                SetSorting(sr);
            }
        }
    }

    private void SetSorting(SpriteRenderer sr)
    {
        if (!string.IsNullOrEmpty(sortingLayerName))
        {
            // If the sorting layer doesn't exist, Unity will fallback to Default silently.
            sr.sortingLayerName = sortingLayerName;
        }
        sr.sortingOrder = sortingOrder;
    }
}
