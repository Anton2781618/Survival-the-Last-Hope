// using ModestTree;

using Entitys.Weapon.Modules.WeaponModelModule;
using Units;
using UnityEngine;
using Weapons;

namespace States
{
    public abstract class WeaponState
    {
        //--------- сервис ----------------
        protected HumanWeaponStateManager stateService;
        public HumanModel UnitModel;
        public WeaponModelModule CurrentWeapon;
        
        //--------- позиция и поворот ----------------
        public Vector3 FromPointRight;
        public Quaternion FromRotationRight;
        public Vector3 FromPointLeft;
        public Quaternion FromRotationLeft;

        //--------- кривая лерпа ----------------
        internal float lerpRatio = 0;
        internal float stateTimer = 0;
        internal float LerpTime = 0.4f;

        //--------- флаги ----------------
        public bool IsComplete = false;
        internal bool needMoveHeand = true;
        private int _stagesCount = 0;
        private int _currentStgeIndex = 0;

        //--------- объект с точкой куда перемещать конечности
        private WeaponPositions _weaponPositions;
        private AnimationPoint _currentStage;

        public void Setup(WeaponPositions weaponPositions)
        {
            _weaponPositions = weaponPositions;

            _stagesCount = _weaponPositions.AnimationsPoints.Count - 1;

            _currentStage = _weaponPositions.AnimationsPoints[_currentStgeIndex];
        }

        public void InitState(HumanWeaponStateManager stateService)
        {
            if(this.stateService == null) this.stateService = stateService;

            Init();
        }

        public virtual void Init()
        {
            IsComplete = false;

            lerpRatio = 0;

            stateTimer = 0;

            FromPointRight = UnitModel.rigTargetRight.localPosition;
            FromRotationRight = UnitModel.rigTargetRight.localRotation;

            FromPointLeft = UnitModel.rigTargetLeft.localPosition;
            FromRotationLeft = UnitModel.rigTargetLeft.localRotation;
        }

        internal void UpdateLerpRatio()
        {
            stateTimer += Time.deltaTime;

            if(stateTimer > LerpTime) stateTimer = LerpTime;

            lerpRatio = stateTimer / LerpTime;

            // Debug.Log(stateTimer + " / " + LerpTime + " = " + (stateTimer / LerpTime));
        }

        public abstract void Execute();

        public virtual void MoveHendsFromPoint()
        {
            Vector3 positionOffsetRight = _currentStage.LerpCurveRightHend.Evaluate(lerpRatio) * _currentStage.LerpOffsetRightHend;
            Vector3 positionOffsetLeft = _currentStage.LerpCurveLeftHend.Evaluate(lerpRatio) * _currentStage.LerpOffsetLeftHend;
            
            //правая рука
            UnitModel.rigTargetRight.localPosition = Vector3.Lerp(FromPointRight, _currentStage.RightHendPosition, lerpRatio) + positionOffsetRight;
            
            UnitModel.rigTargetRight.localRotation = Quaternion.Lerp(FromRotationRight, _currentStage.RightHendRotation, lerpRatio);


            //левая рука
            UnitModel.rigTargetLeft.localPosition = Vector3.Lerp(FromPointLeft, _currentStage.LeftHendPosition, lerpRatio) + positionOffsetLeft;
            
            UnitModel.rigTargetLeft.localRotation = Quaternion.Lerp(FromRotationLeft, _currentStage.LeftHendRotation, lerpRatio);
            // UnitModel.rigTargetLeft.transform.localPosition = _currentStage.LeftHendPosition;

            if(_currentStage.UseLeftWeight) UnitModel.SetWeightLeftHand(lerpRatio);
            
            UpdateLerpRatio();

            if(lerpRatio >= 0.99f && _currentStgeIndex < _stagesCount)
            {
                _currentStgeIndex ++;

                _currentStage = _weaponPositions.AnimationsPoints[_currentStgeIndex];

                Init();
            }
        }

        public Vector3 ConvertePoint(Vector3 point) => UnitModel.objectForLocalSpace.worldToLocalMatrix.MultiplyPoint(point);
    }
}