using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;

namespace LunarApostles
{
  public class BaseDrainSitState : BaseState
  {
    public override void OnEnter()
    {
      base.OnEnter();
      if (!NetworkServer.active || !(bool)(Object)this.characterBody)
        return;
      this.characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
    }

    public override void OnExit()
    {
      if (NetworkServer.active && (bool)(Object)this.characterBody)
        this.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
      base.OnExit();
    }
  }
}
