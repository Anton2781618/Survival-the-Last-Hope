namespace ModularEventArchitecture
{
    public interface IInteractable
    {
        // Базовые методы взаимодействия
        public void Interact(GameEntity interactor);
        public void OnMouseEnter();
        public void OnMouseExit();
    }
}