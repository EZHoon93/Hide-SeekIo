using UnityEngine;

// 버프를 받을 수 있는 오브젝
public interface IBuffable
{
    void OnApplyBuff(Define.BuffType buffType, float durationTime = -1);
}