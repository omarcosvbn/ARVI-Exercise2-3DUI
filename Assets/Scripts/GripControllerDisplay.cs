using UnityEngine;
using UnityEngine.InputSystem;

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

        void Awake(){
            sourceParts = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++){
                sourceParts[i] = transform.GetChild(i).gameObject;
            }

            leftController = FindController(leftControllerName);
            rightController = FindController(rightControllerName);

            if (leftController != null) leftDisplay = CreateDisplay(leftController, "Controller (Left)");
            if (rightController != null) rightDisplay = CreateDisplay(rightController, "Controller (Right)");

            for (int i = 0; i < sourceParts.Length; i++){
                sourceParts[i].SetActive(false);
            }

            EnableAction(leftGrabMoveAction);
            EnableAction(rightGrabMoveAction);
        }

        void Update(){
            SetDisplayActive(leftDisplay, IsActionPressed(leftGrabMoveAction));
            SetDisplayActive(rightDisplay, IsActionPressed(rightGrabMoveAction));
        }

        void OnDestroy(){
            Destroy(leftDisplay);
            Destroy(rightDisplay);
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
