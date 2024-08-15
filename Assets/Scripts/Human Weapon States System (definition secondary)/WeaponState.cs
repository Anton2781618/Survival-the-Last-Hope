// using ModestTree;
using UnityEngine;
using Weapons;

namespace States
{
    public abstract class WeaponState
    {
        protected HumanWeaponStateManager stateService;

        public bool IsComplete = false;

        public Vector3 FromPoint;
        protected Vector3 ToPoint;
        protected Quaternion FromRotation;
        public Quaternion ToRotation;

        public AnimationCurve lerpCurve;
        public Vector3 lerpOffset;
        public bool TransitionForward = true;
        internal float lerpRatio = 0;
        internal float stateTimer = 0;
        internal bool needMoveHeand = true;

        public void InitState(HumanWeaponStateManager stateService)
        {
            if(this.stateService == null) this.stateService = stateService;

            Init();
        }

        public virtual void Init()
        {
            IsComplete = false;

            FromPoint = stateService.playerModel.rigTargetRight.localPosition;

            FromRotation = stateService.playerModel.rigTargetRight.localRotation;

            lerpRatio = 0;

            stateTimer = 0;
        }

        internal void UpdateLerpRatio()
        {
            stateTimer += Time.deltaTime;

            if(stateTimer > stateService.LerpTime) stateTimer = stateService.LerpTime;

            lerpRatio = stateTimer / stateService.LerpTime;

            // Debug.Log(stateTimer + " / " + stateService.LerpTime + " = " + (stateTimer / stateService.LerpTime));
        }

        public abstract void Execute();

        public virtual void MoveHendsFromPoint()
        {
            if(!needMoveHeand) return;
            
            Vector3 positionOffset = TransitionForward ? lerpCurve.Evaluate(lerpRatio) * lerpOffset : Vector3.zero;
            
            //правая рука
            stateService.playerModel.rigTargetRight.localPosition = Vector3.Lerp(FromPoint, ToPoint, lerpRatio) + positionOffset;
            
            stateService.playerModel.rigTargetRight.localRotation = Quaternion.Lerp(FromRotation, ToRotation, lerpRatio);

            //левая рука
            stateService.playerModel.rigTargetLeft.transform.localPosition = stateService.CurrentWeapon.GetModel().WeaponStates[0].LeftHendPosition;

            stateService.playerModel.SetWeightLeftHand(TransitionForward ? lerpRatio : 1);
        }
    }

    public class StateWeaponChange : WeaponState
    {
        private WeaponBase newWeapon;

        public void SetNewWeapon(WeaponBase newWeapon)
        {
            this.newWeapon = newWeapon;
        }

        public override void Execute()
        {
            stateService.CurrentWeapon = newWeapon;

            stateService.inputSystem.weaponOn = true;

            stateService.inputSystem.weaponChange = false;
            
            // stateService.playerModel.rigTargetRight.localPosition = 
            // stateService.CurrentWeapon.TypeWeapon == Weapons.WeaponBase.WeaponType.Pistol ? stateService.playerModel.PistolHolster.localPosition : stateService.playerModel.RifleHolster.localPosition;
            
            // stateService.stateWeaponOn.FromPoint = stateService.playerModel.rigTargetRight.localPosition;
        }
    }
}