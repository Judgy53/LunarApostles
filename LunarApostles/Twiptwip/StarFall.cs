using RoR2;
using RoR2.Projectile;
using RoR2.Navigation;
using EntityStates;
using EntityStates.VagrantMonster.Weapon;
using EntityStates.ScavMonster;
using EntityStates.TitanMonster;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LunarApostles
{
  public class StarFall : BaseState
  {
    public static float baseDuration;
    public static string sound;
    public static float damageCoefficient;
    public static string attackSoundString;
    public static float timeToTarget = 3f;
    public static int projectileCount;
    private float duration;
    private ChildLocator childLocator;

    public override void OnEnter()
    {
      base.OnEnter();
      this.duration = (ThrowSack.baseDuration * 2) / this.attackSpeedStat;
      int num = (int)Util.PlayAttackSpeedSound(ThrowSack.sound, this.gameObject, this.attackSpeedStat);
      this.PlayAnimation("Body", nameof(ThrowSack), "ThrowSack.playbackRate", this.duration);
      Transform modelTransform = this.GetModelTransform();
      if (!(bool)(UnityEngine.Object)modelTransform)
        return;
      this.childLocator = modelTransform.GetComponent<ChildLocator>();
      if (!(bool)(UnityEngine.Object)this.childLocator)
        return;
      // get random idx to grab a random player
      int playerCount = PlayerCharacterMasterController.instances.Count;
      System.Random r = new System.Random();
      int rIdx = r.Next(0, playerCount - 1);
      PlayerCharacterMasterController player = PlayerCharacterMasterController.instances[rIdx];

      Vector3 position = new Vector3(player.body.footPosition.x, 100f, player.body.footPosition.z);
      FireStarFormation(this.GetAimRay(), position);
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      if ((double)this.fixedAge < (double)this.duration || !this.isAuthority)
        return;
      this.outer.SetNextStateToMain();
    }

    private void FireStarFormation(Ray aimRay, Vector3 pos)
    {
      float num3 = UnityEngine.Random.Range(0.0f, 360f);
      for (int index3 = 0; index3 < 6; ++index3)
      {
        for (int index4 = 0; index4 < 6; ++index4)
        {
          Vector3 vector3 = Quaternion.Euler(0.0f, num3 + 60f * (float)index3, 0.0f) * Vector3.forward;
          Vector3 position = pos + vector3 * FireGoldFist.distanceBetweenFists * (float)index4;
          ProjectileManager.instance.FireProjectile(JellyBarrage.projectilePrefab, position, Util.QuaternionSafeLookRotation(Vector3.down), this.gameObject, this.damageStat * FireEnergyCannon.damageCoefficient, FireEnergyCannon.force, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: 60);
        }
      }
    }

  }
}