using UnityEngine;
using UnityEngine.UI;


namespace Suncheon.UI
{
    public class CloseButton : MonoBehaviour, IButton
    {
        [SerializeField] protected GameObject ui_OpenObject;
        [SerializeField] protected GameObject ui_CloseObject;
        [SerializeField] private ButtonControl buttonControl;

        protected virtual void Awake()
        {
            Button btn = GetComponent<Button>();
            btn.onClick.AddListener(() => Click());
        }

        public virtual void Click()
        {
            Close();
        }

        public virtual void Close()
        {    
            if (UIInteractionManager.Instance != null && ui_CloseObject)
            {
                UIInteractionManager.Instance.ClosePopUp(ui_CloseObject);
            }
            else
            {
                ui_CloseObject.SetActive(false);
            }

            if(ui_OpenObject != null)
            {
                UIInteractionManager.Instance.OpenPopUp(ui_OpenObject);
            }

            if(buttonControl != null)
            {
                buttonControl.OnBackGroundFadeIn();
                buttonControl.OnBtnImageFadeIn();
            }

        }

        public virtual void Open()
        {

        }
    }
}
