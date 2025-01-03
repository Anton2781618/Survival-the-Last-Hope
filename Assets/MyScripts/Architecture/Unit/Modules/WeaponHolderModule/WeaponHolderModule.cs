using System;
using System.Collections;
using System.Collections.Generic;
using Entitys.Player.Events;
using Entitys.Weapon.Modules.WeaponModelModule;
using States;
using Units;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static Weapons.WeaponModel;

// Получает события от PlayerInputModule
// Проверяет возможность действия (можно ли сейчас целиться/стрелять)
// Передает команды в LocalEvents оружия
namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UnitEntity))]
    public class WeaponHolderModule : ModuleBase
    {
        //--------------- Текущее состояние анимации ----------------
        [ReadOnly] [SerializeField] private AnimationState _currentState;

        //--------------- Ссылки на объекты ----------------
        [SerializeField] private WeaponEntity _currentWeapon;
        [SerializeField] private HumanModel _humanModel;
        private Weapons.WeaponModel _weaponModel;

        //------------- Корутины ----------------
        private Coroutine _currCoruteine;
        private Coroutine _currentAnimationCoroutine;

        private enum AnimationState
        {
            Holster,
            HeandOn,
            Aim,
            Fire,
        }

        private void StopCurrentCoroutines()
        {
            if (_currCoruteine != null)
            {
                StopCoroutine(_currCoruteine);
                _currCoruteine = null;
            }

            if (_currentAnimationCoroutine != null)
            {
                StopCoroutine(_currentAnimationCoroutine);
                _currentAnimationCoroutine = null;
            }
        }
        
        public override void Initialize()
        {
            Entity.LocalEvents.Subscribe<EventBase>(EventsAnimationWeapon.DrawWeaponStart, OnDrawWeapon);
            Entity.LocalEvents.Subscribe<EventBase>(EventsAnimationWeapon.AimWeapon, OnWeaponAim);
            Entity.LocalEvents.Subscribe<EventBase>(EventsAnimationWeapon.OffAim, OnOffAim);
            Entity.LocalEvents.Subscribe<EventBase>(EventsAnimationWeapon.Fire, OnStartFire);
            Entity.LocalEvents.Subscribe<EventBase>(EventsAnimationWeapon.StopFire, OnStopFire);

            _weaponModel = _currentWeapon.GetModule<WeaponModelModule>().WeaponModel;

            _currCoruteine = StartCoroutine(HolsterWeapon());
        }


        public override void UpdateMe()
        {
            
        }

        private void OnStartFire(EventBase eventBase)
        {
            if(_currentState != AnimationState.Aim) return;

            _currentWeapon.LocalEvents.Publish(EventsWeapon.StartFire, new EventBase());

            // _currentState = AnimationState.Fire;
        }
        private void OnStopFire(EventBase eventBase)
        {
            // if(_currentState == AnimationState.Fire) OnOffAim();

            _currentWeapon.LocalEvents.Publish(EventsWeapon.StopFire, new EventBase());
        }

        private void OnDrawWeapon(EventBase eventBase)
        {
            StopCurrentCoroutines();

            if(_currentState != AnimationState.Holster)
            {
                _currCoruteine = StartCoroutine(HolsterWeapon());
            }
            else
            {
                _currCoruteine = StartCoroutine(WeaponOn());
            }
        }
        
        private void OnWeaponAim(EventBase eventBase)
        {
            if(_currentState != AnimationState.HeandOn) return;

            StopCurrentCoroutines();
            _currCoruteine = StartCoroutine(WeaponAim());
        }

        private void OnOffAim(EventBase eventBase = null)
        {
            if(_currentState != AnimationState.Aim) return;
            
            StopCurrentCoroutines();
            _currCoruteine = StartCoroutine(OffAim());
        }

        //убрать оружие в кабуру
        public IEnumerator HolsterWeapon()
        {
            _currentState = AnimationState.Holster;

            _currentAnimationCoroutine = StartCoroutine(StartAnimation(new StateWeaponOff())); //перемещает риг в кабуру по кривой и вращает его в кабуре
            
            yield return _currentAnimationCoroutine;
        }

        //достать оружие
        public IEnumerator WeaponOn()
        {
            _currentState = AnimationState.HeandOn;

            _currentAnimationCoroutine = StartCoroutine(StartAnimation(new StateHeandOnWeapon())); //увеличивает вес рига на слое aim
            
            yield return _currentAnimationCoroutine;

            StateWeaponOn stateWeaponOn = new StateWeaponOn();

            stateWeaponOn.Setup(_weaponModel.AnimationsLayers[(int)WeaponPositionsStates.OrReady]);

            _currentAnimationCoroutine = StartCoroutine(StartAnimation(stateWeaponOn));
            
            yield return _currentAnimationCoroutine;
            
        }

        //прицелиться
        public IEnumerator WeaponAim()
        {
            _currentState = AnimationState.Aim;
            StateWeaponAim stateWeapon = new StateWeaponAim();

            stateWeapon.Setup(_weaponModel.AnimationsLayers[(int)WeaponPositionsStates.Aim]);

            _currentAnimationCoroutine = StartCoroutine(StartAnimation(stateWeapon)); //увеличивает вес рига на слое aim
            yield return _currentAnimationCoroutine;
            
        }

        //перестать прицелеваться
        public IEnumerator OffAim()
        {
            _currentState = AnimationState.HeandOn;

            StateWeaponOn stateWeaponOn = new StateWeaponOn();

            stateWeaponOn.Setup(_weaponModel.AnimationsLayers[(int)WeaponPositionsStates.OffAim]);

            _currentAnimationCoroutine = StartCoroutine(StartAnimation(stateWeaponOn));
            yield return _currentAnimationCoroutine;

        }

        public IEnumerator StartAnimation(WeaponState weaponState)
        {
            weaponState.CurrentWeapon = _currentWeapon.GetModule<WeaponModelModule>();
            weaponState.UnitModel = _humanModel;

            weaponState.Init();

            while (!weaponState.IsComplete)
            {
                yield return null;
                weaponState.Execute();
            }            

            Debug.Log("IsComplete");
        }       
    }
}
