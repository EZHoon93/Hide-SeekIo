namespace BehaviorDesigner.Runtime
{
    public class SharedTeam : SharedVariable<Define.Team>
    {
        public static implicit operator SharedTeam(Define.Team value) { return new SharedTeam { Value = value }; }
    }

}