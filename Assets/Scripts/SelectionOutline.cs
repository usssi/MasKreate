using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectionOutline : MonoBehaviour
{
    [Header("Settings")]
    public float thickness = 3f;
    public Color color = Color.magenta;
    public float margin = 0f;

    [Header("Cursors")]
    public Texture2D cursorResizeH;
    public Texture2D cursorResizeV;
    public Texture2D cursorResizeD_TLBR; // Top-Left to Bottom-Right
    public Texture2D cursorResizeD_TRBL; // Top-Right to Bottom-Left
    public Texture2D cursorRotate;
    public Texture2D cursorMove;
    public Vector2 cursorHotspot = new Vector2(16, 16); // Center of 32x32 cursor

    [Header("Internal References")]
    [SerializeField] private Image topLine;
    [SerializeField] private Image bottomLine;
    [SerializeField] private Image leftLine;
    [SerializeField] private Image rightLine;
    [SerializeField] private Image tlCorner;
    [SerializeField] private Image trCorner;
    [SerializeField] private Image blCorner;
    [SerializeField] private Image brCorner;
    [SerializeField] private Image rotateHandle;
    [SerializeField] private Image hGuideLine;
    [SerializeField] private Image vGuideLine;
    
    public SpriteEditorUI editorUI;

    private RectTransform target;
    private RectTransform myRectTransform;
    public bool IsInteracting { get; private set; } // To block ObjectSelector dragging

    // State for dragging
    private Vector2 initialMousePos;
    private Vector3 initialScale;
    private Vector2 initialSize;
    private float initialAngle;
    private float initialZRotation;

    private void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
        // Ensure we have the visual components
        if (topLine == null) CreateVisuals();
        UpdateVisuals();
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            FollowTarget();
        }
        else
        {
             // Hide if no target
             SetAlpha(0);
        }
    }

    public void SetTarget(RectTransform t)
    {
        target = t;
        if (target != null)
        {
            SetAlpha(color.a); // Restore visibility
            FollowTarget();
            
            // Render on top of everything in the same container
            transform.SetAsLastSibling();
        }
        else
        {
            // Reset cursor when deselecting
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void FollowTarget()
    {
        if (target == null) return;

        // Copy position and rotation
        transform.position = target.position;
        transform.rotation = target.rotation;
        transform.localScale = Vector3.one; // Keep outline scale constant so thickness doesn't change

        // Match size + margin
        // SizeDelta of target might be influenced by its anchors, but let's assume standard UI usage
        // Note: For complex anchors, we might need rect.width/height
        Vector2 targetSize = target.rect.size;
        // Adjust for target's scale so the box fits the visual size of the target
        targetSize.x *= Mathf.Abs(target.localScale.x);
        targetSize.y *= Mathf.Abs(target.localScale.y);

        myRectTransform.sizeDelta = targetSize + Vector2.one * (margin * 2);
    }

    private void CreateVisuals()
    {
        // Helper to create a line/handle
        Image CreateImage(string name, Color c)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(transform, false);
            Image img = obj.AddComponent<Image>();
            img.color = c;
            // Add GizmoHandle
            GizmoHandle handle = obj.AddComponent<GizmoHandle>();
            handle.outline = this;
            return img;
        }

        // Lines
        if(!topLine) { topLine = CreateImage("TopLine", color); topLine.GetComponent<GizmoHandle>().type = GizmoHandleType.Top; }
        if(!bottomLine) { bottomLine = CreateImage("BottomLine", color); bottomLine.GetComponent<GizmoHandle>().type = GizmoHandleType.Bottom; }
        if(!leftLine) { leftLine = CreateImage("LeftLine", color); leftLine.GetComponent<GizmoHandle>().type = GizmoHandleType.Left; }
        if(!rightLine) { rightLine = CreateImage("RightLine", color); rightLine.GetComponent<GizmoHandle>().type = GizmoHandleType.Right; }

        // Corners
        Color cornerColor = Color.clear; // Invisible catchers, or make them visible if desired
        if(!tlCorner) { tlCorner = CreateImage("TLCorner", cornerColor); tlCorner.GetComponent<GizmoHandle>().type = GizmoHandleType.CornerTL; }
        if(!trCorner) { trCorner = CreateImage("TRCorner", cornerColor); trCorner.GetComponent<GizmoHandle>().type = GizmoHandleType.CornerTR; }
        if(!blCorner) { blCorner = CreateImage("BLCorner", cornerColor); blCorner.GetComponent<GizmoHandle>().type = GizmoHandleType.CornerBL; }
        if(!brCorner) { brCorner = CreateImage("BRCorner", cornerColor); brCorner.GetComponent<GizmoHandle>().type = GizmoHandleType.CornerBR; }

        // Rotation Handle - Bottom Left circle
        if(!rotateHandle) { rotateHandle = CreateImage("RotateHandle", Color.yellow); rotateHandle.GetComponent<GizmoHandle>().type = GizmoHandleType.Rotate; }

        // Guide Lines (Blue)
        if (!hGuideLine) 
        {
            hGuideLine = CreateImage("HGuideLine", new Color(0, 0.5f, 1f, 1f));
            hGuideLine.raycastTarget = false;
            DestroyImmediate(hGuideLine.GetComponent<GizmoHandle>());
            hGuideLine.rectTransform.SetParent(transform.parent, false); // Stay at center of canvas
        }
        if (!vGuideLine)
        {
            vGuideLine = CreateImage("VGuideLine", new Color(0, 0.5f, 1f, 1f));
            vGuideLine.raycastTarget = false;
            DestroyImmediate(vGuideLine.GetComponent<GizmoHandle>());
            vGuideLine.rectTransform.SetParent(transform.parent, false); // Stay at center of canvas
        }


        // Setup Anchors for a box
        SetupLine(topLine.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, thickness / 2));
        SetupLine(bottomLine.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0, thickness / 2));
        SetupLine(leftLine.rectTransform, new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f), new Vector2(thickness / 2, 0));
        SetupLine(rightLine.rectTransform, new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 0.5f), new Vector2(thickness / 2, 0));

        // Setup Corners (e.g. 10x10 catchers)
        float cornerSize = 20f;
        SetupCorner(tlCorner.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(cornerSize, cornerSize)); // Anchor Top-Left, Pivot Bottom-Right (inside box)?? Actually catchers should extend out or be centered. Let's center them on corners.
        SetupCorner(trCorner.rectTransform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(cornerSize, cornerSize));
        SetupCorner(blCorner.rectTransform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(1, 1), new Vector2(cornerSize, cornerSize));
        SetupCorner(brCorner.rectTransform, new Vector2(1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(cornerSize, cornerSize));

        // Setup Rotation Handle (Bottom Left, distinct from resize corner)
        // Let's place it slightly outside BL corner
        SetupCorner(rotateHandle.rectTransform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0.5f, 0.5f), new Vector2(15, 15));
        rotateHandle.rectTransform.anchoredPosition = new Vector2(-20, -20);    
    }

    private void SetupCorner(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta)
    {
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot; // Using pivot to align corner
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = sizeDelta;
    }

    private void SetupLine(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta)
    {
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.sizeDelta = Vector2.zero; // Reset first
        
        // We want constant thickness. 
        // Top/Bottom: Stretch X, fixed Height
        // Left/Right: Stretch Y, fixed Width
        if (anchorMin.x == 0 && anchorMax.x == 1) // Horizontal
        {
            rt.sizeDelta = new Vector2(0, thickness);
        }
        else // Vertical
        {
            rt.sizeDelta = new Vector2(thickness, 0);
        }
    }

    public void UpdateVisuals()
    {
        if (topLine) { topLine.color = color; topLine.rectTransform.sizeDelta = new Vector2(0, thickness); }
        if (bottomLine) { bottomLine.color = color; bottomLine.rectTransform.sizeDelta = new Vector2(0, thickness); }
        if (leftLine) { leftLine.color = color; leftLine.rectTransform.sizeDelta = new Vector2(thickness, 0); }
        if (rightLine) { rightLine.color = color; rightLine.rectTransform.sizeDelta = new Vector2(thickness, 0); }
    }
    
    private void SetAlpha(float a)
    {
        Color c = color;
        c.a = a;
        if(topLine) topLine.color = c;
        if(bottomLine) bottomLine.color = c;
        if(leftLine) leftLine.color = c;
        if(rightLine) rightLine.color = c;
        
        // Ensure rotate handle alpha follows
        if(rotateHandle) 
        {
            Color rc = rotateHandle.color;
            rc.a = a;
            rotateHandle.color = rc;
        }
    }

    public void ShowGuides(bool horizontal, bool vertical, float thickness, float hLength, float vLength, Color guideColor)
    {
        if (hGuideLine)
        {
            hGuideLine.gameObject.SetActive(horizontal);
            if (horizontal)
            {
                hGuideLine.color = guideColor;
                hGuideLine.rectTransform.sizeDelta = new Vector2(hLength, thickness);
                hGuideLine.rectTransform.anchoredPosition = Vector2.zero;
                hGuideLine.rectTransform.rotation = Quaternion.identity;
            }
        }
        if (vGuideLine)
        {
            vGuideLine.gameObject.SetActive(vertical);
            if (vertical)
            {
                vGuideLine.color = guideColor;
                vGuideLine.rectTransform.sizeDelta = new Vector2(thickness, vLength);
                vGuideLine.rectTransform.anchoredPosition = Vector2.zero;
                vGuideLine.rectTransform.rotation = Quaternion.identity;
            }
        }
    }

    private void OnValidate()
    {
        UpdateVisuals();
    }
    // Interaction Interface for GizmoHandles
    public void OnHandleHoverEnter(GizmoHandle handle)
    {
        if (IsInteracting || target == null) return;
        
        Texture2D cursor = GetAdaptiveCursor(handle);
        Cursor.SetCursor(cursor, cursorHotspot, CursorMode.Auto);
    }

    private Texture2D GetAdaptiveCursor(GizmoHandle handle)
    {
        switch (handle.type)
        {
            case GizmoHandleType.Rotate: return cursorRotate;
            case GizmoHandleType.Body: return cursorMove;
        }

        // Calculate base angle for the handle
        float baseAngle = 0;
        switch (handle.type)
        {
            case GizmoHandleType.Right: baseAngle = 0; break;
            case GizmoHandleType.Top: baseAngle = 90; break;
            case GizmoHandleType.Left: baseAngle = 180; break;
            case GizmoHandleType.Bottom: baseAngle = 270; break;
            case GizmoHandleType.CornerTR: baseAngle = 45; break;
            case GizmoHandleType.CornerTL: baseAngle = 135; break;
            case GizmoHandleType.CornerBL: baseAngle = 225; break;
            case GizmoHandleType.CornerBR: baseAngle = 315; break;
        }

        // Visual angle = base handle angle + object rotation
        float visualAngle = Mathf.Repeat(baseAngle + target.localEulerAngles.z, 360);

        // Map angle to standard cursors
        if (visualAngle < 22.5f || visualAngle >= 337.5f || (visualAngle >= 157.5f && visualAngle < 202.5f))
            return cursorResizeH;
        if ((visualAngle >= 67.5f && visualAngle < 112.5f) || (visualAngle >= 247.5f && visualAngle < 292.5f))
            return cursorResizeV;
        if ((visualAngle >= 22.5f && visualAngle < 67.5f) || (visualAngle >= 202.5f && visualAngle < 247.5f))
            return cursorResizeD_TRBL;
        
        return cursorResizeD_TLBR;
    }

    public void OnHandleHoverExit(GizmoHandle handle)
    {
        if (IsInteracting) return;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnHandleDown(GizmoHandle handle, PointerEventData eventData)
    {
        IsInteracting = true;
        initialMousePos = eventData.position;
        if (target != null)
        {
            var transformer = target.GetComponent<SpriteTransformer>();
            if (transformer)
            {
                initialScale = target.localScale;
                // Use absolute CurrentWidth/Height for drag calculations
                initialSize = new Vector2(transformer.CurrentWidth, transformer.CurrentHeight);
                initialAngle = GetAngleToMouse(eventData.position);
                initialZRotation = transformer.zRotation;
            }
        }
    }

    public void OnHandleUp(GizmoHandle handle, PointerEventData eventData)
    {
        IsInteracting = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnHandleDrag(GizmoHandle handle, PointerEventData eventData)
    {
        if (target == null) return;
        var transformer = target.GetComponent<SpriteTransformer>();
        if (!transformer) return;

        // Project mouse delta into local space
        Vector2 screenDelta = eventData.position - initialMousePos;
        
        // Find the camera and canvas to convert screen delta to world/local units
        Canvas canvas = target.GetComponentInParent<Canvas>();
        Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) 
            ? canvas.worldCamera ?? Camera.main : null;

        Vector2 localInitial;
        Vector2 localCurrent;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(target.parent as RectTransform, initialMousePos, cam, out localInitial);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(target.parent as RectTransform, eventData.position, cam, out localCurrent);
        
        Vector2 worldDelta = localCurrent - localInitial;
        // Project that into the object's local rotation
        float rad = -target.localEulerAngles.z * Mathf.Deg2Rad;
        Vector2 localDelta = new Vector2(
            worldDelta.x * Mathf.Cos(rad) - worldDelta.y * Mathf.Sin(rad),
            worldDelta.x * Mathf.Sin(rad) + worldDelta.y * Mathf.Cos(rad)
        );

        switch (handle.type)
        {
            case GizmoHandleType.Top:
                if (editorUI) editorUI.UpdateFromGizmoHeight(initialSize.y + localDelta.y);
                else transformer.height = initialSize.y + localDelta.y;
                break;
            case GizmoHandleType.Bottom:
                if (editorUI) editorUI.UpdateFromGizmoHeight(initialSize.y - localDelta.y);
                else transformer.height = initialSize.y - localDelta.y;
                break;
            case GizmoHandleType.Right:
                if (editorUI) editorUI.UpdateFromGizmoWidth(initialSize.x + localDelta.x);
                else transformer.width = initialSize.x + localDelta.x;
                break;
            case GizmoHandleType.Left:
                if (editorUI) editorUI.UpdateFromGizmoWidth(initialSize.x - localDelta.x);
                else transformer.width = initialSize.x - localDelta.x;
                break;
            
            case GizmoHandleType.CornerTR:
                // For uniform scale, we take the dominant diagonal movement or just local X
                float scaleFactorTR = 1 + (localDelta.x * 0.01f);
                if (editorUI) editorUI.UpdateFromGizmoScale(initialScale.x * scaleFactorTR);
                else transformer.scale = initialScale.x * scaleFactorTR;
                break;
            case GizmoHandleType.CornerTL:
                float scaleFactorTL = 1 - (localDelta.x * 0.01f); 
                if (editorUI) editorUI.UpdateFromGizmoScale(initialScale.x * scaleFactorTL);
                else transformer.scale = initialScale.x * scaleFactorTL;
                break;
            case GizmoHandleType.CornerBR:
                float scaleFactorBR = 1 + (localDelta.x * 0.01f);
                if (editorUI) editorUI.UpdateFromGizmoScale(initialScale.x * scaleFactorBR);
                else transformer.scale = initialScale.x * scaleFactorBR;
                break;
            case GizmoHandleType.CornerBL:
                float scaleFactorBL = 1 - (localDelta.x * 0.01f);
                if (editorUI) editorUI.UpdateFromGizmoScale(initialScale.x * scaleFactorBL);
                else transformer.scale = initialScale.x * scaleFactorBL;
                break;

            case GizmoHandleType.Rotate:
                float currentAngle = GetAngleToMouse(eventData.position);
                float angleDiff = Mathf.DeltaAngle(initialAngle, currentAngle);
                float targetZ = initialZRotation + angleDiff;
                
                if (editorUI) editorUI.UpdateFromGizmoRotation(targetZ);
                else 
                {
                    targetZ = Mathf.Round(targetZ);
                    while (targetZ >= 360) targetZ -= 360;
                    while (targetZ < 0) targetZ += 360;
                    transformer.zRotation = targetZ;
                }
                break;
        }
    }

    private float GetAngleToMouse(Vector2 mousePosition)
    {
        if (target == null) return 0;

        Canvas canvas = target.GetComponentInParent<Canvas>();
        Camera cam = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        }

        Vector2 screenPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, mousePosition, cam, out _))
        {
            // Use WorldToScreenPoint to get the center of the target in screen space
            screenPos = RectTransformUtility.WorldToScreenPoint(cam, target.position);
        }
        else
        {
            // Fallback
            screenPos = mousePosition;
        }

        Vector2 direction = mousePosition - screenPos;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
