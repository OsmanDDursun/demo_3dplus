using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace App.Scripts.Ui
{
    public class UiButtonBase : Button
    {
        public UnityEvent<bool> OnPointerEnterEvent = new();
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            OnPointerEnterEvent.Invoke(true);
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            OnPointerEnterEvent.Invoke(false);
        }
    }
}