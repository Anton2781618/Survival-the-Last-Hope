namespace States
{
    public interface IStateService
    {
        public float LerpTime {get; set;} 
        public void UpdateMe();
    }
}