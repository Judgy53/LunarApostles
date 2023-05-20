using RoR2;
using RoR2.Projectile;
using EntityStates;
using EntityStates.VagrantMonster.Weapon;
using EntityStates.ScavMonster;
using EntityStates.LunarWisp;
using UnityEngine;

namespace LunarApostles
{
  public class FullHouse : BaseState
  {
    public static float baseDuration;
    public static string sound;
    public static float damageCoefficient;
    public static string attackSoundString;
    public static float timeToTarget = 3f;
    public static int projectileCount;
    private float duration;
    private float orbStopwatch;
    private float missileStopwatch;
    private ChildLocator childLocator;

    public override void OnEnter()
    {
      base.OnEnter();
      this.missileStopwatch = 0f;
      this.orbStopwatch = 0f;
      this.duration = 6 / this.attackSpeedStat;
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
        for (int i = 0; i < 16; i++)
        {
          float angle = i * Mathf.PI * 2 / 16;
          float x = Mathf.Cos(angle) * 5;
          float z = Mathf.Sin(angle) * 5;
          Vector3 pos = transform.position + new Vector3(x, 0, z);
          float angleDegrees = -angle * Mathf.Rad2Deg;
          Quaternion rot = Quaternion.Euler(0, angleDegrees, 0);
          ProjectileManager.instance.FireProjectile(JellyBarrage.projectilePrefab, pos, rot, this.gameObject, this.damageStat * FireEnergyCannon.damageCoefficient, FireEnergyCannon.force, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: 45);
        }
      }
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      this.missileStopwatch += Time.fixedDeltaTime;
      this.orbStopwatch += Time.fixedDeltaTime;
      if (this.orbStopwatch >= 2)
      {
        this.orbStopwatch -= 2;
        SpawnOrbs();
      }
      if (this.missileStopwatch >= 1f / (JellyBarrage.missileSpawnFrequency * 2))
      {
        this.missileStopwatch -= 1f / (JellyBarrage.missileSpawnFrequency * 2);
        Transform child = this.childLocator.FindChild(EnergyCannonState.muzzleName);
        if ((bool)(Object)child)
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
          if (Physics.Raycast(aimRay, out hitInfo, maxDistance, (int)LayerIndex.CommonMasks.bullet))
          {
            projectileRay.direction = hitInfo.point - projectileRay.origin;
          }
          EffectManager.SpawnEffect(LunarApostles.severPrefab, new EffectData { origin = projectileRay.origin, rotation = Util.QuaternionSafeLookRotation(projectileRay.direction) }, false);
          ProjectileManager.instance.FireProjectile(JellyBarrage.projectilePrefab, projectileRay.origin, Util.QuaternionSafeLookRotation(projectileRay.direction), this.gameObject, this.damageStat * SeekingBomb.bombDamageCoefficient, SeekingBomb.bombForce, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: 125);
        }
      }
      if ((double)this.fixedAge < (double)this.duration || !this.isAuthority)
        return;
      this.outer.SetNextStateToMain();
    }
  }
}