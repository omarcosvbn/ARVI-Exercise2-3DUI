using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ARVI{
    public class MediaButtonVisuals : MonoBehaviour{
        [SerializeField] float pressOffsetY = -0.005f;

        XRSimpleInteractable interactable;
        Transform buttonTransform;
        Vector3 restingPosition;

        void Awake(){
            interactable = GetComponent<XRSimpleInteractable>();
            buttonTransform = transform;
            restingPosition = buttonTransform.localPosition;
        }

        void OnEnable(){
            if (interactable == null) return;

            interactable.activated.AddListener(OnActivated);
        }

        void OnDisable(){
            if (interactable == null) return;

            interactable.activated.RemoveListener(OnActivated);
        }

        void OnActivated(ActivateEventArgs args){
            buttonTransform.localPosition = restingPosition + Vector3.up * pressOffsetY;
        }
    }
}
