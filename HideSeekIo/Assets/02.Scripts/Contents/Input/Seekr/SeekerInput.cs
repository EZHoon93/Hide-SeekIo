using System.Collections;

using UnityEngine;

public class SeekerInput : InputBase
{
    //public Vector2 SkillVector { get; private set; }
    //public Vector2 LastSkillVector { get; private set; }


    //bool isSkill;


    
    public override void OnPhotonInstantiate()
    {
        //isSkill = false;
        if (this.IsMyCharacter())
        {
            InputManager.Instacne.SetActiveSeekerController(true);
        }
    }


    //protected void Update()
    //{
    //    if (this.IsMyCharacter() == false) return;
     
    //}
    public void UpdateInputUser()
    {
        MoveVector = InputManager.Instacne.MoveVector;

        //if (InputManager.Instacne.AttackTouch)
        //{
        //    AttackVector = InputManager.Instacne.AttackVector;
        //    isAttack = true;
        //    if (AttackVector.sqrMagnitude == 0)
        //    {
        //        isAttack = false;
        //    }

        //}
        //else
        //{
        //    if (isAttack)
        //    {
        //        LastAttackVector = AttackVector;
        //        isAttack = false;
        //    }
        //    else
        //    {
        //        AttackVector = Vector2.zero;
        //        LastAttackVector = Vector2.zero;
        //    }

        //}
    }

    //void UpdateSkillInput()
    //{
    //    if (InputManager.Instacne.SkillTouch)
    //    {
    //        SkillVector = InputManager.Instacne.SkillVector;
    //        isSkill = true;
    //        if (SkillVector.sqrMagnitude == 0)
    //        {
    //            isSkill = false;
    //        }

    //    }
    //    else
    //    {
    //        //스킬 밝사
    //        if (isSkill)
    //        {
    //            LastSkillVector = SkillVector;
    //            InputManager.Instacne.SkillSucess();
    //            isSkill = false;
    //        }
    //        else
    //        {
    //            SkillVector = Vector2.zero;
    //            LastSkillVector = Vector2.zero;
    //        }

    //    }
    //}

    public override void Stop(float newTime)
    {
        //MoveVector = Vector2.zero;
        //IsStop = true;
        //stopTime = newTime;
    }
}
