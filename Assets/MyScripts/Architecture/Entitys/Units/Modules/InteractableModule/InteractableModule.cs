using System.Collections;
using System.Collections.Generic;
using ModularEventArchitecture;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UnitEntity))]
    [RequireComponent(typeof(BoxCollider)), RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(OutlineSystem.Outline))]
    public class InteractableModule : ModuleBase, IInteractable
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private OutlineSystem.Outline _outline;

        public Collider GetCollider() => _collider;
        public Rigidbody GetRigidbody() => _rigidbody;
        public OutlineSystem.Outline GetOutline() => _outline;

        public override void Initialize()
        {

        }
        
        public virtual void Interact(GameEntity interactor)
        {
            Debug.Log(interactor.name + " is interacting with " + this.GetType());
        }

        public void OnMouseEnter()
        {
            _outline.enabled = true;
        }

        public void OnMouseExit()
        {
            _outline.enabled = false;
        }
    }
}