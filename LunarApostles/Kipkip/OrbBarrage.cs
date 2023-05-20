using RoR2;
using RoR2.Projectile;
using EntityStates;
using EntityStates.ScavMonster;
using EntityStates.LunarWisp;
using UnityEngine;

namespace LunarApostles
{
  public class OrbBarrage : BaseState
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
      this.duration = ThrowSack.baseDuration / this.attackSpeedStat;
      int num = (int)Util.PlayAttackSpeedSound(ThrowSack.sound, this.gameObject, this.attackSpeedStat);
      this.PlayAnimation("Body", nameof(ThrowSack), "ThrowSack.playbackRate", this.duration);
      Transform modelTransform = this.GetModelTransform();
      if (!(bool)(Object)modelTransform)
        return;
      this.childLocator = modelTransform.GetComponent<ChildLocator>();
      if (!(bool)(Object)this.childLocator)
        return;
      SpawnOrbs();
    }

    private void SpawnOrbs()
    {
      Transform child = this.childLocator.FindChild(EnergyCannonState.muzzleName);
      if ((bool)(Object)child)
      {
        for (int i = 0; i < 12; i++)
        {
          Ray aimRay = this.GetAimRay();
          Ray projectileRay = new Ray();
          projectileRay.direction = aimRay.direction;
          float maxDistance = 1000f;
          float randX = UnityEngine.Random.Range(-25f, 25f);
          float randY = UnityEngine.Random.Range(10f, 25f);
          float randZ = UnityEngine.Random.Range(-25f, 25f);
          Vector3 randVector = new Vector3(randX, randY, randZ);
          Vector3 position = child.position + randVector;
          projectileRay.origin = position;
          RaycastHit hitInfo;
          {
            if (Physics.Raycast(aimRay, out hitInfo, maxDistance, (int)LayerIndex.CommonMasks.bullet))
            {
              projectileRay.direction = hitInfo.point - projectileRay.origin;
              EffectManager.SpawnEffect(LunarApostles.severPrefab, new EffectData { origin = projectileRay.origin, rotation = Util.QuaternionSafeLookRotation(projectileRay.direction) }, false);
              ProjectileManager.instance.FireProjectile(LunarApostles.wispBomb, projectileRay.origin, Util.QuaternionSafeLookRotation(projectileRay.direction), this.gameObject, this.damageStat * SeekingBomb.bombDamageCoefficient, SeekingBomb.bombForce, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: 0);
            }
          }
        }
      }
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      if ((double)this.fixedAge < (double)this.duration || !this.isAuthority)
        return;
      this.outer.SetNextStateToMain();
    }

    public override void OnExit()
    {
      base.OnExit();
      ProjectileSimple[] projectiles = GameObject.FindObjectsOfType<ProjectileSimple>();
      if (projectiles.Length > 0)
      {
        foreach (ProjectileSimple projectile in projectiles)
          if (projectile.name == "LunarWispTrackingBomb(Clone)")
          {
            projectile.desiredForwardSpeed = 45;
          }
      }
    }
  }
}