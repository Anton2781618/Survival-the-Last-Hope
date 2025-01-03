using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(Player))]
    public sealed class PlayerMovementModule : ModuleBase
    {
        private Animator _animator;
        private PlayerMovement _playerMovement;
        private bool _isTiredEffect = false;
        private bool _isRopeConnect = false;
        private bool _isRopeTired = false;

        public override void Initialize()
        {
            if (!_animator) _animator = Entity.GetCachedComponent<Animator>();

            if (_playerMovement == null) _playerMovement = new PlayerMovement(_animator);

            // Entity.LocalEvents.Subscribe<EventBase>(LocalEventBus.События.Состояния.Усталость, ApplayTiredEffect);
        }
        
        private void RopeTiredOn(EventBase empty)
        {
            if (_isRopeTired) return;

            _isRopeTired = true;
        }
        private void RopeTiredOff(EventBase empty)
        {
            if (!_isRopeTired) return;

            _isRopeTired = false;
        }


        private void ApplayTiredEffect(EventBase effectEvent)
        {
            _isTiredEffect = effectEvent.Enabled;
        }

        public void RopeConnected(EventBase data) => _isRopeConnect = true;
        public void RopeDisconnected(EventBase data) => _isRopeConnect = false;

        public override void UpdateMe()
        {
            if (_isRopeConnect && _playerMovement.MoveVector.magnitude > 0 && _isRopeTired == true)
            {
                // LocalEvents.Publish(LocalEventBus.События.Состояния.Эффекты.Эффект_изменился, new EffectEvent{Effect = new Effect( name: Effect.EffectName.Rope, duration: 0)});
            }
            else
            if (Input.GetKey(KeyCode.LeftShift) && _playerMovement.MoveVector.magnitude > 0)
            {
                if (!_isTiredEffect) _playerMovement.Sprint = true;

                // Entity.LocalEvents.Publish(LocalEventBus.События.Команды.Движение.Начать_спринт, new EventBase());
            }
            else
            if (_playerMovement.Sprint == true && (Input.GetKeyUp(KeyCode.LeftShift) || _playerMovement.MoveVector.magnitude == 0))
            {
                StopSprint();
            }
            
            _playerMovement.UpdateMe();
        }

        //метод закончить спринт
        public void StopSprint()
        {
            _playerMovement.Sprint = false;

            // Entity.LocalEvents.Publish(LocalEventBus.События.Команды.Движение.Закончить_спринт, new EventBase());
        }
    }
}