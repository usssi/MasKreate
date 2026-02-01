using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ObjectSelector : MonoBehaviour
{
    public SpriteEditorUI editorUI;
    public SelectionOutline selectionOutline; // Reference to the outline script
    public LayerMask selectionLayer;

    [Header("Movement Settings")]
    public bool lockXAxis = false;
    public bool lockYAxis = false;
    
    [Header("Snapping Settings")]
    public float snapThreshold = 5f;
    public float guideLineThickness = 2f;
    public float vLineLength = 2000f;
    public float hLineLength = 2000f;
    public Color guideLineColor = new Color(0, 0.5f, 1f, 1f);
    public RectTransform snappingBackground; // Reference for custom center/size

    public bool IsDragging { get; private set; }
    private Transform dragTarget;
    private Vector3 dragOffset;

    private void Update()
    {
        if (Mouse.current == null) return;

        // Mouse Down: Try to select and start drag
        // Check if we are interacting with the gizmo handles first
        if (selectionOutline != null && selectionOutline.IsInteracting) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TrySelectObject();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            TryDeleteObject();
        }

        if (IsDragging && dragTarget != null)
        {
            Vector3 currentMousePos = GetMouseWorldPosition(dragTarget);
            Vector3 targetPos = new Vector3(currentMousePos.x + dragOffset.x, currentMousePos.y + dragOffset.y, dragTarget.position.z);

            // Apply Axis Locking
            if (lockXAxis) targetPos.x = dragTarget.position.x;
            if (lockYAxis) targetPos.y = dragTarget.position.y;

            // Snapping logic
            bool snappedX = false;
            bool snappedY = false;
            RectTransform rt = dragTarget as RectTransform;

            float currentHLength = hLineLength;
            float currentVLength = vLineLength;
            Vector3? guideCenter = null;

            if (rt != null)
            {
                // Determine snap center
                Vector3 worldSnapCenter = (snappingBackground != null) ? snappingBackground.position : (rt.parent != null ? rt.parent.position : Vector3.zero);
                
                if (snappingBackground != null)
                {
                    currentHLength = snappingBackground.rect.width;
                    currentVLength = snappingBackground.rect.height;
                    guideCenter = snappingBackground.position;
                }

                // Temporary move to check snap
                dragTarget.position = targetPos;
                
                // Calculate local offset from the worldSnapCenter
                Vector3 worldOffset = dragTarget.position - worldSnapCenter;
                
                // Convert world offset to local units (relative to parent scale)
                Vector3 localOffset = rt.parent != null ? rt.parent.InverseTransformVector(worldOffset) : worldOffset;

                // We check if the local anchored position would be close to our target (usually 0,0 relative to snap center)
                // If snappingBackground is used, we snap to its center
                if (Mathf.Abs(localOffset.x) < (snapThreshold / 100f)) // Threshold depends on scale, but let's approximate
                {
                    // Since localOffset is world-space vector converted to local, 
                    // we can just adjust the world position to align with snap center
                    targetPos.x = worldSnapCenter.x;
                    snappedX = true;
                }
                if (Mathf.Abs(localOffset.y) < (snapThreshold / 100f))
                {
                    targetPos.y = worldSnapCenter.y;
                    snappedY = true;
                }

                dragTarget.position = targetPos; // Sync back
            }

            // Show Guide Lines
            if (selectionOutline != null)
            {
                selectionOutline.ShowGuides(snappedY, snappedX, guideLineThickness, currentHLength, currentVLength, guideLineColor, guideCenter);
            }

            // Direct Movement (No Smoothing)
            dragTarget.position = targetPos;
        }
        else
        {
            // Ensure guides are hidden when not dragging or target is null
            if (selectionOutline != null && !IsDragging)
            {
                selectionOutline.ShowGuides(false, false, guideLineThickness, hLineLength, vLineLength, guideLineColor);
            }
        }

        // Mouse Up: Stop dragging
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            IsDragging = false;
            dragTarget = null;
            if (selectionOutline != null)
            {
                selectionOutline.ShowGuides(false, false, guideLineThickness, hLineLength, vLineLength, guideLineColor);
            }
        }
    }

    private Vector3 GetMouseWorldPosition(Transform target)
    {
        RectTransform rt = target as RectTransform;
        RectTransform plane = (rt != null && rt.parent != null) ? rt.parent as RectTransform : rt;
        
        Canvas canvas = target.GetComponentInParent<Canvas>();
        Camera cam = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        }

        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(plane, Mouse.current.position.ReadValue(), cam, out worldPoint);
        return worldPoint;
    }

    private void TrySelectObject()
    {
        // Use EventSystem to find UI elements under the mouse
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        Debug.Log($"[ObjectSelector] Clicked. Raycast results count: {results.Count}");

        foreach (var result in results)
        {
            GameObject obj = result.gameObject;
            Debug.Log($"[ObjectSelector] Hit UI: {obj.name}");
            
            // 1. Check for Target
            SpriteTransformer transformer = obj.GetComponent<SpriteTransformer>();
            if (transformer != null && transformer.isInteractable)
            {
                Debug.Log($"[ObjectSelector] Selecting Target: {obj.name}");
                if (editorUI != null) editorUI.SetTarget(transformer);
                if (selectionOutline != null) 
                {
                    selectionOutline.editorUI = editorUI;
                    selectionOutline.SetTarget(transformer.GetComponent<RectTransform>());
                }
                
                // Start Drag
                IsDragging = true;
                dragTarget = transformer.transform;
                dragOffset = dragTarget.position - GetMouseWorldPosition(dragTarget);
                
                return; // Found target, stop.
            }

            // 2. Check for Interactive UI (Slider, Button, InputField, etc.)
            // If we hit a UI control, we assume the user is using the editor, NOT trying to deselect.
            // Use GetComponentInParent because the raycast might hit "Handle" or "Text" which are children of the Selectable.
            var selectable = obj.GetComponentInParent<UnityEngine.UI.Selectable>();
            if (selectable != null)
            {
                Debug.Log($"[ObjectSelector] Hit Interactive UI ({selectable.name}). Ignoring selection change.");
                return; // Stop, do NOT fall through to deselect.
            }
        }

        // Fallback: If we processed all hits and found neither a Target nor a UI Control...
        Debug.Log("[ObjectSelector] Clicked outside (No Target, No Control). Deselecting.");
        if (editorUI != null) editorUI.SetTarget(null);
        if (selectionOutline != null) selectionOutline.SetTarget(null);
    }

    private void TryDeleteObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            GameObject obj = result.gameObject;
            SpriteTransformer transformer = obj.GetComponent<SpriteTransformer>();
            
            if (transformer != null && transformer.isInteractable)
            {
                // Check if this is the currently selected target
                bool isSelected = false;
                
                if (selectionOutline != null)
                {
                    // Access the private 'target' field via reflection to verify if it's the selected one
                    var targetField = selectionOutline.GetType().GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    RectTransform selectedTarget = targetField?.GetValue(selectionOutline) as RectTransform;
                    
                    if (selectedTarget == transformer.GetComponent<RectTransform>())
                    {
                        isSelected = true;
                    }
                }

                if (isSelected)
                {
                    Debug.Log($"[ObjectSelector] Deleting Selected Object: {obj.name}");
                    
                    // Clear selection 
                    if (editorUI != null) editorUI.SetTarget(null);
                    if (selectionOutline != null) selectionOutline.SetTarget(null);

                    Destroy(obj);
                }
                
                return; // Stop after first SpriteTransformer hit, even if not selected
            }

            var selectable = obj.GetComponentInParent<UnityEngine.UI.Selectable>();
            if (selectable != null) return; // Don't delete if we hit UI
        }
    }
}
