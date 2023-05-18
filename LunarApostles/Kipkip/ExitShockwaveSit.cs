using RoR2;
using EntityStates.ScavMonster;

namespace LunarApostles
{
  public class ExitShockwaveSit : BaseShockwaveSitState
  {
    public static float baseDuration;
    public static string soundString;
    private float duration;

    public override void OnEnter()
    {
      base.OnEnter();
      this.duration = ExitSit.baseDuration / this.attackSpeedStat;
      int num = (int)Util.PlaySound(ExitSit.soundString, this.gameObject);
      this.PlayCrossfade("Body", nameof(ExitSit), "Sit.playbackRate", this.duration, 0.1f);
      this.modelLocator.normalizeToFloor = false;
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      if ((double)this.fixedAge < (double)this.duration)
        return;
      this.outer.SetNextStateToMain();
    }
  }
}
