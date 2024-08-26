namespace AntonStateMachine
{
    public class PlayerStateMachie : StateManager<PlayerStateMachie.WeaponStates>
    {
        public override void InitializeStates()
        {
            states.Add(WeaponStates.NoWeapon, new StateNoWeapon(this, WeaponStates.NoWeapon));

            states.Add(WeaponStates.WeaponOn, new StateWeaponOn(this, WeaponStates.WeaponOn));

            currentState = states[WeaponStates.NoWeapon];
        }

        public enum WeaponStates
        {
            NoWeapon,
            WeaponOn,
            Aim
        }
    }
}
