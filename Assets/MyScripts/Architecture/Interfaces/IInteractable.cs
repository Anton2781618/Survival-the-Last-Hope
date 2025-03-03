namespace ModularEventArchitecture
{
    public interface IInteractable
    {
        // Базовые методы взаимодействия
        void Interact(GameEntity interactor);
        bool CanInteract(GameEntity interactor);
    }
}