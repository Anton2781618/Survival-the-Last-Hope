using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UIManager))]
    public class TextDisplayModule : ModuleBase
    {
        [SerializeField] private TextMesh text;

        public override void Initialize()
        {
            Entity.Globalevents.Add((EventsUI.Show_Text, (data) => ShowText((EventShowText)data)));
        }

        public override void UpdateMe()
        {
            
        }

        private void ShowText(EventShowText eventShowText)
        {
            text.gameObject.SetActive(eventShowText.Enabled);

            if(eventShowText.Enabled)
            {
                text.transform.position = eventShowText.Position;
                text.text = eventShowText.Text;            
            }
        }
    }
}