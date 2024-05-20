using TMPro;
using UnityEngine;

namespace MemoryFishing.UI.Dialogue
{
    public class DialogueOption : MonoBehaviour
    {
        public class OnClickEventArgs : System.EventArgs
        {
            public int Index { get; private set; }
            public OnClickEventArgs(int index)
            {
                Index = index;
            }
        }

        public event System.EventHandler<OnClickEventArgs> OnClickEvent;
        public int Index { get; set; }
        [SerializeField] private TextMeshProUGUI optionUGUI;

        public void OnClick()
        {
            OnClickEvent?.Invoke(this, new OnClickEventArgs(Index));
        }

        public void SetText(string text)
        {
            optionUGUI.text = text;
        }
    }
}