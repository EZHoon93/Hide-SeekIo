using System.Collections;

using UnityEngine;

public interface IMakeRunEffect
{
    Define.MoveHearState HearState { get; set; }
    bool IsLocal();

}
