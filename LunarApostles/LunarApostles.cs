using BepInEx;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace LunarApostles
{
  [BepInPlugin("com.Nuxlar.LunarApostles", "LunarApostles", "1.0.0")]

  public class LunarApostles : BaseUnityPlugin
  {
    public static List<GameObject> timeCrystals;
    private static GameObject kipkipBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar1Body.prefab").WaitForCompletion();
    private static GameObject kipkipMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar1Master.prefab").WaitForCompletion();
    private static GameObject wipwipMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar2Master.prefab").WaitForCompletion();
    private static GameObject twiptwipMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar3Master.prefab").WaitForCompletion();
    private static GameObject guraguraMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar4Master.prefab").WaitForCompletion();

    public void Awake()
    {
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
      new TriJawCannon();
      new CircularToss();
      new CrystalSit();
    }

  }
}