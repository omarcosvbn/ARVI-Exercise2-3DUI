using UnityEngine;
using UnityEngine.InputSystem;

namespace ARVI{
    public class GripControllerDisplay : MonoBehaviour{
        [SerializeField] string leftControllerName = "Left Controller";
        [SerializeField] string rightControllerName = "Right Controller";
        [SerializeField] InputActionReference leftGrabMoveAction;
        [SerializeField] InputActionReference rightGrabMoveAction;
        [SerializeField] Vector3 handLocalPosition = new Vector3(3.1f, -0.36f, 1.27f);

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
            else Debug.LogError($"Could not find '{leftControllerName}'. The left controller display cannot be created.", this);
            if (rightController != null) rightDisplay = CreateDisplay(rightController, "Controller (Right)");
            else Debug.LogError($"Could not find '{rightControllerName}'. The right controller display cannot be created.", this);

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
            if (leftDisplay != null) Destroy(leftDisplay);
            if (rightDisplay != null) Destroy(rightDisplay);
        }

        Transform FindController(string controllerName){
            Transform[] transforms = FindObjectsByType<Transform>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

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
            display.transform.localPosition = handLocalPosition;

            for (int i = 0; i < sourceParts.Length; i++){
                GameObject part = Instantiate(sourceParts[i], display.transform);
                part.name = sourceParts[i].name;
                part.SetActive(true);
            }

            display.SetActive(false);
            return display;
        }

        void EnableAction(InputActionReference actionReference){
            if (actionReference == null || actionReference.action == null){
                Debug.LogError("A Grab Move input action is not assigned.", this);
                return;
            }

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
