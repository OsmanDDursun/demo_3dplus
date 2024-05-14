using TMPro;
using UnityEngine;

namespace App.Scripts.Ui.BuildingInfoPanel
{
    public class MetaTagView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _valueText;
        
        public void Initialize(string title, string value)
        {
            _titleText.text = title;
            _valueText.text = value;
        }
    }
}