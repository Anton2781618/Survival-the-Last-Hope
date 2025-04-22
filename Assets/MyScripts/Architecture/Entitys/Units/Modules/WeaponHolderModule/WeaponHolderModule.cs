using System.Collections;
using Entitys.Player.Events;
using Entitys.Weapon.Modules.WeaponModelModule;
using States;
using Units;
using UnityEngine;
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
        private WeaponDataModule _weaponDataModule;
        private Weapons.WeaponModel _weaponModel;

        //------------- Корутины -----------------------------------
        private Coroutine _currCoruteine;
        private Coroutine _currentAnimationCoroutine;
        //-----------------------------------------------------------
        [SerializeField] private Transform _hand;

        //-----------------------------------------------------------
        [ReadOnly] [SerializeField] private string currentAnimationName = "";
        [ReadOnly] [SerializeField] private int currentAnimationStage = 0;
        private int indexAnimation = 0;
        //----------------------------------------------------------

        private enum AnimationState
        {
            Holster,
            HeandOn,
            Aim,
            Fire,
            Reload,
        }
        //!-----------------------------------------------------------

        //----------------Сервисная часть------------------
        [Tools.Button("Weapon_Aim")]
        private void Weapon_Aim() => OnWeaponAim();

        [Tools.Button("Off_Aim")]
        private void OnOff_Aim()
        {
            _currentState = AnimationState.Aim;
            OnOffAim();
        }

        [Tools.Button("Reload")]
        private void On_Reload()
        {
            OnReload();
        }

        [Tools.Button("Следующая анимация")]
        private void SetNextAnimation()
        {

            //выбрать следуующий AnimationsLayers 
            indexAnimation = (indexAnimation + 1) % _weaponModel.AnimationsLayers.Count;

            currentAnimationName = _weaponModel.AnimationsLayers[indexAnimation].Name;
        }

        [Tools.Button("Следующий Этап")]
        private void SetNextStage()
        {
            currentAnimationStage = (currentAnimationStage + 1) % _weaponModel.AnimationsLayers[indexAnimation].AnimationsPoints.Count;
        }

        [Tools.Button("перейти к этапу анимации")]
        private void TransitionTo()
        {
            StopCurrentCoroutines();
            _currCoruteine = StartCoroutine(Reloadddd(indexAnimation, currentAnimationStage));
        }
        //-------------------------------------------------

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
            Entity.LocalEvents.Subscribe<EventBase>(EventsAnimationWeapon.Draw_Weapon_Start, OnDrawWeapon);
            Entity.LocalEvents.Subscribe<EventBase>(EventsAnimationWeapon.AimWeapon, OnWeaponAim);
            Entity.LocalEvents.Subscribe<EventBase>(EventsAnimationWeapon.Off_Aim, OnOffAim);
            Entity.LocalEvents.Subscribe<EventBase>(EventsAnimationWeapon.Fire, OnStartFire);
            Entity.LocalEvents.Subscribe<EventBase>(EventsAnimationWeapon.Stop_Fire, OnStopFire);
            Entity.LocalEvents.Subscribe<EventBase>(EventsAnimationWeapon.Reload, OnReload);

            Entity.LocalEvents.Subscribe<SetupWeaponEventData>(EventsAnimationWeapon.Setup_Weapon, OnSetupWeapon);
        }

        //заспавнить оружие в руках
        private void OnSetupWeapon(SetupWeaponEventData setupWeaponEventData)
        {
            _currentWeapon = setupWeaponEventData.Slot.ClothingItem.GetComponent<WeaponEntity>();

            _weaponDataModule = _currentWeapon.GetModule<WeaponDataModule>();
                Debug.Log(_weaponDataModule == null);
            _weaponDataModule.InventoryItem = setupWeaponEventData.InventoryItem;
            
            _weaponModel = _weaponDataModule.WeaponModel;


            _weaponDataModule.DrawWeapon(_hand);

            _currentState = AnimationState.HeandOn;

            StateWeaponOn stateWeaponOn = new StateWeaponOn();

            stateWeaponOn.Setup(_weaponModel.AnimationsLayers[(int)WeaponPositionsStates.OrReady]);

            _currentAnimationCoroutine = StartCoroutine(StartAnimation(stateWeaponOn));
            

            // _weaponDataModule.HolsterWeapon(_humanModel.RifleHolster);
        }
    
        //старт стрельбы
        private void OnStartFire(EventBase eventBase = null)
        {
            if(_currentState != AnimationState.Aim) return;

            _currentWeapon.LocalEvents.Publish(EventsWeapon.StartFire, new EventBase());

            // _currentState = AnimationState.Fire;
        }
        private void OnStopFire(EventBase eventBase = null)
        {
            if(!_currentWeapon) return;

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

        

        private void OnReload(EventBase eventBase = null)
        {
            // if(_currentState != AnimationState.Reload) return;

            StopCurrentCoroutines();
            _currCoruteine = StartCoroutine(Reload());
        }

        private void OnWeaponAim(EventBase eventBase = null)
        {
            if(_currentState != AnimationState.HeandOn) return;

            StopCurrentCoroutines();
            _currCoruteine = StartCoroutine(WeaponAim());
        }

        private void OnOffAim(EventBase eventBase = null)
        {
            if(_currentState != AnimationState.Aim) return;

            OnStopFire();
            
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

            StateWeaponOn stateWeapon = new StateWeaponOn();

            stateWeapon.Setup(_weaponModel.AnimationsLayers[(int)WeaponPositionsStates.OffAim]);

            _currentAnimationCoroutine = StartCoroutine(StartAnimation(stateWeapon));
            yield return _currentAnimationCoroutine;

        }

        private IEnumerator Reload()
        {
            _currentState = AnimationState.Reload;

            StateWeaponReload stateWeapon = new StateWeaponReload();

            stateWeapon.Setup(_weaponModel.AnimationsLayers[(int)WeaponPositionsStates.Reload]);

            _currentAnimationCoroutine = StartCoroutine(StartAnimation(stateWeapon));

            yield return _currentAnimationCoroutine;

            _currentState = AnimationState.HeandOn;
        }
        private IEnumerator Reloadddd(int indexAnimLayer, int stage)
        {
            _currentState = AnimationState.Reload;

            StateWeaponReload stateWeapon = new StateWeaponReload();

            stateWeapon._currentStageIndex = stage;

            stateWeapon.Setup(_weaponModel.AnimationsLayers[indexAnimLayer]);

            stateWeapon.TransitToNextStage = false;


            _currentAnimationCoroutine = StartCoroutine(StartAnimation(stateWeapon));

            yield return _currentAnimationCoroutine;

            _currentState = AnimationState.HeandOn;
        }

        public IEnumerator StartAnimation(WeaponState weaponState)
        {
            weaponState.CurrentWeapon = _currentWeapon.GetModule<WeaponDataModule>();
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