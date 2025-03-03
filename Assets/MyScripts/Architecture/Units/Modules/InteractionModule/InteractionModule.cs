using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(Player))]
    public class InteractionModule : ModuleBase
    {
        //-------------------------------------------------------------------------------------
        [SerializeField] private float interactionRange = 100f;
        [SerializeField] private LayerMask interactionLayer;
        [SerializeField] private Camera _mainCamera; // Обычно камера или глаза персонажа
        
        //!-------------------------------------------------------------------------------------

        public override void Initialize()
        {
            Entity.LocalEvents.Subscribe<EventBase>(EventsInteraction.Try_Interact, OnTryInteract);
        }
        
        private void OnTryInteract(EventBase eventBase)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            
            // Проверяем, попали ли мы в интерактивный объект
            if (Physics.Raycast(ray, out hit, interactionRange, interactionLayer))
            {
                // Получаем компонент IInteractable
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                
                if (interactable != null)
                {
                    interactable.Interact(Entity);
                    
                }
            }
        }
    }
}