using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ARVI{
    public class MediaButtonVisuals : MonoBehaviour{
        [SerializeField] float pressOffsetY = -0.005f;

        XRSimpleInteractable interactable;
        Transform buttonTransform;
        Vector3 restingPosition;

        void Awake(){
            Initialize();
        }

        void OnEnable(){
            Initialize();
            if (interactable == null) return;

            interactable.selectEntered.AddListener(OnHandGrabbed);
            interactable.activated.AddListener(OnActivated);
        }

        void OnDisable(){
            if (interactable == null) return;

            interactable.selectEntered.RemoveListener(OnHandGrabbed);
            interactable.activated.RemoveListener(OnActivated);
        }

        void OnHandGrabbed(SelectEnterEventArgs args){
            if (args.interactorObject is NearFarInteractor){
                OnActivated(null);
            }
        }

        void OnActivated(ActivateEventArgs args){
            Initialize();
            buttonTransform.localPosition = restingPosition + Vector3.up * pressOffsetY;
        }

        public void ResetVisual(){
            Initialize();
            buttonTransform.localPosition = restingPosition;
        }

        void Initialize(){
            if (buttonTransform == null){
                buttonTransform = transform;
                restingPosition = buttonTransform.localPosition;
            }

            if (interactable == null){
                interactable = GetComponent<XRSimpleInteractable>();
            }
        }
    }
}
