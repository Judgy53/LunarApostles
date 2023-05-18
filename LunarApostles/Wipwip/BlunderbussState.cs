using UnityEngine;
using EntityStates;
using EntityStates.ScavMonster;

namespace LunarApostles
{
  public class BlunderbussState : BaseState
  {
    public static string muzzleName;
    protected Transform muzzleTransform;

    public override void OnEnter()
    {
      base.OnEnter();
      this.muzzleTransform = this.FindModelChild(EnergyCannonState.muzzleName);
    }

    public override void FixedUpdate() => base.FixedUpdate();

    public override void OnExit() => base.OnExit();

    public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.PrioritySkill;
  }
}