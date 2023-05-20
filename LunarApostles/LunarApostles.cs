using BepInEx;
using RoR2;
using RoR2.Networking;
using RoR2.CharacterAI;
using R2API;
using EntityStates;
using EntityStates.ScavMonster;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace LunarApostles
{
  [BepInPlugin("com.Nuxlar.LunarApostles", "LunarApostles", "1.0.0")]

  public class LunarApostles : BaseUnityPlugin
  {
    public static List<GameObject> timeCrystals;
    public static bool activatedMass;
    public static bool activatedDesign;
    public static bool activatedBlood;
    public static bool activatedSoul;
    public static bool completedPillar = false;

    public static GameObject timeCrystal = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/WeeklyRun/TimeCrystalBody.prefab").WaitForCompletion();
    public static GameObject severPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/moon/MoonExitArenaOrbEffect.prefab").WaitForCompletion();
    public static GameObject wispBomb = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarWisp/LunarWispTrackingBomb.prefab").WaitForCompletion();
    public static GameObject bloodSiphon = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/moon2/BloodSiphonNearbyAttachment.prefab").WaitForCompletion();
    public static MeteorStormController meteorStormController = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Meteor/MeteorStorm.prefab").WaitForCompletion().GetComponent<MeteorStormController>();

    private SceneDef limbo = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/limbo/limbo.asset").WaitForCompletion();
    private static GameObject kipkipBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar1Body.prefab").WaitForCompletion();
    private static GameObject wipwipBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar2Body.prefab").WaitForCompletion();
    private static GameObject twiptwipBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar3Body.prefab").WaitForCompletion();
    private static GameObject guraguraBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar4Body.prefab").WaitForCompletion();
    private static GameObject kipkipMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar1Master.prefab").WaitForCompletion();
    private static GameObject wipwipMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar2Master.prefab").WaitForCompletion();
    private static GameObject twiptwipMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar3Master.prefab").WaitForCompletion();
    private static GameObject guraguraMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar4Master.prefab").WaitForCompletion();

    public void Awake()
    {
      limbo.suppressNpcEntry = true;
      // Setup
      SetupSkillStates();
      // Hooks
      On.RoR2.SceneDirector.Start += SceneDirector_Start;
      On.RoR2.CharacterBody.Start += CharacterBody_Start;
      On.RoR2.HoldoutZoneController.Start += HoldoutZoneController_Start;
      On.EntityStates.MoonElevator.MoonElevatorBaseState.OnEnter += MoonElevatorBaseState_OnEnter;
      On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.OnEnter += FadeOut_OnEnter;

      GivePickupsOnStart[] pickups = kipkipMaster.GetComponents<GivePickupsOnStart>();
      foreach (GivePickupsOnStart pickup in pickups)
        GameObject.Destroy(pickup);
      pickups = wipwipMaster.GetComponents<GivePickupsOnStart>();
      foreach (GivePickupsOnStart pickup in pickups)
        GameObject.Destroy(pickup);
      pickups = twiptwipMaster.GetComponents<GivePickupsOnStart>();
      foreach (GivePickupsOnStart pickup in pickups)
        GameObject.Destroy(pickup);
      pickups = guraguraMaster.GetComponents<GivePickupsOnStart>();
      foreach (GivePickupsOnStart pickup in pickups)
        GameObject.Destroy(pickup);

      // Body Changes
      kipkipBody.GetComponent<CharacterBody>().baseMaxHealth = 1900;
      kipkipBody.GetComponent<CharacterBody>().levelMaxHealth = 600;
      wipwipBody.GetComponent<CharacterBody>().baseMaxHealth = 1900;
      wipwipBody.GetComponent<CharacterBody>().levelMaxHealth = 600;
      twiptwipBody.GetComponent<CharacterBody>().baseMaxHealth = 1900;
      twiptwipBody.GetComponent<CharacterBody>().levelMaxHealth = 600;
      guraguraBody.GetComponent<CharacterBody>().baseMaxHealth = 1900;
      guraguraBody.GetComponent<CharacterBody>().levelMaxHealth = 600;
      //  Master Changes
      kipkipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Primary).First<AISkillDriver>().maxDistance = 160f;
      kipkipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Secondary).First<AISkillDriver>().maxUserHealthFraction = 0.95f;
      kipkipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Utility).First<AISkillDriver>().maxUserHealthFraction = 0.85f;
      wipwipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Primary).First<AISkillDriver>().maxDistance = 160f;
      wipwipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Secondary).First<AISkillDriver>().maxUserHealthFraction = 0.95f;
      wipwipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Utility).First<AISkillDriver>().maxUserHealthFraction = 0.85f;
      twiptwipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Primary).First<AISkillDriver>().maxDistance = 160f;
      twiptwipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Secondary).First<AISkillDriver>().maxUserHealthFraction = 0.95f;
      twiptwipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Utility).First<AISkillDriver>().maxUserHealthFraction = 0.85f;
      guraguraMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Primary).First<AISkillDriver>().maxDistance = 160f;
      guraguraMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Secondary).First<AISkillDriver>().maxUserHealthFraction = 0.95f;
      guraguraMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Utility).First<AISkillDriver>().maxUserHealthFraction = 0.85f;
    }

    private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
    {
      orig(self);
      if (SceneManager.GetActiveScene().name == "moon2" && completedPillar && self.isPlayerControlled)
      {
        // 308 -139 398 525.0092 -156.221 603.6622
        SetPosition(new Vector3(308, -139, 398), self);
        completedPillar = false;
      }
    }

    private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
    {
      if (SceneManager.GetActiveScene().name != "limbo")
      {
        wipwipBody.GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(PrepEnergyCannon));
        wipwipBody.GetComponent<SkillLocator>().secondary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(ThrowSack));
        wipwipBody.GetComponent<SkillLocator>().utility.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(EnterSit));
      }
      if (SceneManager.GetActiveScene().name == "limbo")
      {
        LunarApostles.timeCrystals = new();
        if (activatedMass)
        {
          wipwipBody.GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(PrepSeveredCannon));
          wipwipBody.GetComponent<SkillLocator>().secondary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(OrbBarrage));
          wipwipBody.GetComponent<SkillLocator>().utility.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(EnterShockwaveSit));
        }
        if (activatedDesign)
        {
          wipwipBody.GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(PrepBlunderbuss));
          wipwipBody.GetComponent<SkillLocator>().secondary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(ArtilleryBarrage));
          wipwipBody.GetComponent<SkillLocator>().utility.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(EnterMineSit));
        }
        if (activatedBlood)
        {
          wipwipBody.GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(PrepStarCannon));
          wipwipBody.GetComponent<SkillLocator>().secondary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(StarFall));
          wipwipBody.GetComponent<SkillLocator>().utility.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(EnterDrainSit));
        }
        if (activatedSoul)
        {
          wipwipBody.GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(PrepLuckyCannon));
          wipwipBody.GetComponent<SkillLocator>().secondary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(FullHouse));
          wipwipBody.GetComponent<SkillLocator>().utility.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(EnterCrystalSit));
        }
      }

      orig(self);
      if (SceneManager.GetActiveScene().name == "moon2")
      {
        activatedMass = false;
        activatedDesign = false;
        activatedBlood = false;
        activatedSoul = false;
      }
    }

    private void MoonElevatorBaseState_OnEnter(On.EntityStates.MoonElevator.MoonElevatorBaseState.orig_OnEnter orig, EntityStates.MoonElevator.MoonElevatorBaseState self)
    {
      orig(self);
      self.outer.SetNextState(new EntityStates.MoonElevator.Ready());
    }

    private void FadeOut_OnEnter(On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.orig_OnEnter orig, EntityStates.Missions.LunarScavengerEncounter.FadeOut self)
    {
      orig(self);
      activatedMass = false;
      activatedDesign = false;
      activatedBlood = false;
      activatedSoul = false;
      completedPillar = true;
      SetScene("moon2");
    }

    private void HoldoutZoneController_Start(On.RoR2.HoldoutZoneController.orig_Start orig, RoR2.HoldoutZoneController self)
    {
      orig(self);
      if (self.name.Contains("MoonBattery"))
      {
        if (self.name.Contains("Mass"))
          activatedMass = true;
        if (self.name.Contains("Design"))
          activatedDesign = true;
        if (self.name.Contains("Blood"))
          activatedBlood = true;
        if (self.name.Contains("Soul"))
          activatedSoul = true;
        SetScene("limbo");
      }
      // moon pillar MoonBatteryDesign MoonBatteryBlood MoonBatterySoul MoonBatteryMass (some number)
    }

    private void SetPosition(Vector3 newPosition, CharacterBody body)
    {
      if (!(bool)(Object)body.characterMotor)
        return;
      body.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity);
    }

    private void SetScene(string sceneName)
    {
      if (!(bool)(UnityEngine.Object)NetworkManagerSystem.singleton)
        throw new ConCommandException("set_scene failed: NetworkManagerSystem is not available.");
      SceneCatalog.GetSceneDefForCurrentScene();
      SceneDef defFromSceneName = SceneCatalog.GetSceneDefFromSceneName(sceneName);
      if (!(bool)(UnityEngine.Object)defFromSceneName)
        throw new ConCommandException("\"" + sceneName + "\" is not a valid scene.");
      int num = !(bool)(UnityEngine.Object)NetworkManager.singleton ? 1 : (NetworkManager.singleton.isNetworkActive ? 1 : 0);
      if (NetworkManager.singleton.isNetworkActive)
      {
        if (defFromSceneName.isOfflineScene)
          throw new ConCommandException("Cannot switch to scene \"" + sceneName + "\": Cannot switch to offline-only scene while in a network session.");
      }
      else if (!defFromSceneName.isOfflineScene)
        throw new ConCommandException("Cannot switch to scene \"" + sceneName + "\": Cannot switch to online-only scene while not in a network session.");
      if (NetworkServer.active)
      {
        Debug.LogFormat("Setting server scene to {0}", (object)sceneName);
        NetworkManagerSystem.singleton.ServerChangeScene(sceneName);
      }
      else
      {
        if (NetworkClient.active)
          throw new ConCommandException("Cannot change scene while connected to a remote server.");
        Debug.LogFormat("Setting offline scene to {0}", (object)sceneName);
        NetworkManagerSystem.singleton.ServerChangeScene(sceneName);
      }
    }

    private void SetupSkillStates()
    {
      // Kipkip
      ContentAddition.AddEntityState<BaseShockwaveSitState>(out _);
      ContentAddition.AddEntityState<EnterShockwaveSit>(out _);
      ContentAddition.AddEntityState<ExitShockwaveSit>(out _);
      ContentAddition.AddEntityState<ShockwaveSit>(out _);
      ContentAddition.AddEntityState<SeveredCannonState>(out _);
      ContentAddition.AddEntityState<PrepSeveredCannon>(out _);
      ContentAddition.AddEntityState<FireSeveredCannon>(out _);
      ContentAddition.AddEntityState<OrbBarrage>(out _);
      // Wipwip
      ContentAddition.AddEntityState<BaseMineSitState>(out _);
      ContentAddition.AddEntityState<EnterMineSit>(out _);
      ContentAddition.AddEntityState<ExitMineSit>(out _);
      ContentAddition.AddEntityState<MineSit>(out _);
      ContentAddition.AddEntityState<BlunderbussState>(out _);
      ContentAddition.AddEntityState<PrepBlunderbuss>(out _);
      ContentAddition.AddEntityState<FireBlunderbuss>(out _);
      ContentAddition.AddEntityState<ArtilleryBarrage>(out _);
      // Twiptwip
      ContentAddition.AddEntityState<BaseDrainSitState>(out _);
      ContentAddition.AddEntityState<EnterDrainSit>(out _);
      ContentAddition.AddEntityState<ExitDrainSit>(out _);
      ContentAddition.AddEntityState<DrainSit>(out _);
      ContentAddition.AddEntityState<StarCannonState>(out _);
      ContentAddition.AddEntityState<PrepStarCannon>(out _);
      ContentAddition.AddEntityState<FireStarCannon>(out _);
      ContentAddition.AddEntityState<StarFall>(out _);
      // Guragura
      ContentAddition.AddEntityState<BaseCrystalSitState>(out _);
      ContentAddition.AddEntityState<EnterCrystalSit>(out _);
      ContentAddition.AddEntityState<ExitCrystalSit>(out _);
      ContentAddition.AddEntityState<CrystalSit>(out _);
      ContentAddition.AddEntityState<LuckyCannonState>(out _);
      ContentAddition.AddEntityState<PrepLuckyCannon>(out _);
      ContentAddition.AddEntityState<FireLuckyCannon>(out _);
      ContentAddition.AddEntityState<FullHouse>(out _);
    }

  }
}