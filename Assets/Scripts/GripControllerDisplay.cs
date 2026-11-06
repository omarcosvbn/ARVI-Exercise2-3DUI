using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace ARVI{
    public class GripControllerDisplay : MonoBehaviour{
        [SerializeField] string leftControllerName = "Left Controller";
        [SerializeField] string rightControllerName = "Right Controller";
        [SerializeField] InputActionReference leftGrabMoveAction;
        [SerializeField] InputActionReference rightGrabMoveAction;

        Transform leftController;
        Transform rightController;
        GameObject leftDisplay;
        GameObject rightDisplay;
        GameObject[] sourceParts;
        GameObject instructionCanvas;

        void Awake(){
            sourceParts = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++){
                sourceParts[i] = transform.GetChild(i).gameObject;
            }

            leftController = FindController(leftControllerName);
            rightController = FindController(rightControllerName);

            if (leftController != null) leftDisplay = CreateDisplay(leftController, "Controller (Left)");

            for (int i = 0; i < sourceParts.Length; i++){
                sourceParts[i].SetActive(false);
            }

            EnableAction(leftGrabMoveAction);
            CreateInstructionCanvas();
        }

        void Update(){
            SetDisplayActive(leftDisplay, IsActionPressed(leftGrabMoveAction));
        }

        void OnDestroy(){
            Destroy(leftDisplay);
            Destroy(rightDisplay);
            Destroy(instructionCanvas);
        }

        void CreateInstructionCanvas(){
            Camera camera = Camera.main;

            instructionCanvas = new GameObject("Controller Instruction Canvas");
            instructionCanvas.transform.SetParent(camera.transform, false);
            instructionCanvas.transform.localPosition = new Vector3(0f, -0.5f, 2f);
            instructionCanvas.transform.localScale = Vector3.one * 0.001f;

            Canvas canvas = instructionCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = camera;
            canvas.sortingOrder = 10;

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(700f, 100f);

            TextMeshProUGUI text = new GameObject("Instruction Text").AddComponent<TextMeshProUGUI>();
            text.transform.SetParent(canvas.transform, false);
            text.text = "Press [Grab] to take a controller";
            text.fontSize = 36f;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.outlineWidth = 0.2f;
            text.outlineColor = Color.black;

            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        Transform FindController(string controllerName){
            Transform[] transforms = FindObjectsByType<Transform>(FindObjectsInactive.Include);

            for (int i = 0; i < transforms.Length; i++){
                if (transforms[i].name == controllerName){
                    return transforms[i];
                }
            }

            return null;
        }

        GameObject CreateDisplay(Transform hand, string displayName){
            GameObject display = new GameObject(displayName);
            display.transform.SetParent(hand, false);

            for (int i = 0; i < sourceParts.Length; i++){
                GameObject part = Instantiate(sourceParts[i], display.transform);
                part.name = sourceParts[i].name;
                part.SetActive(true);
            }

            CenterDisplayOnHand(display.transform);
            display.SetActive(false);
            return display;
        }

        void CenterDisplayOnHand(Transform display){
            Renderer[] renderers = display.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0){
                display.localPosition = Vector3.zero;
                return;
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++){
                bounds.Encapsulate(renderers[i].bounds);
            }

            Vector3 localCenter = display.InverseTransformPoint(bounds.center);
            display.localPosition = -localCenter;
        }

        void EnableAction(InputActionReference actionReference){
            if (!actionReference.action.enabled){
                actionReference.action.Enable();
            }
        }

        bool IsActionPressed(InputActionReference actionReference){
            return actionReference != null && actionReference.action != null && actionReference.action.IsPressed();
        }

        void SetDisplayActive(GameObject display, bool isActive){
            if (display != null && display.activeSelf != isActive){
                display.SetActive(isActive);
            }
        }
    }
}
