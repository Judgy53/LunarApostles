using RoR2;
using RoR2.Navigation;
using EntityStates;
using UnityEngine;
using System.Collections.Generic;

namespace LunarApostles
{
  public class CrystalSit : BaseCrystalSitState
  {

    public override void OnEnter()
    {
      base.OnEnter();
      SpawnCrystals(this.characterBody);
      this.outer.SetNextState((EntityState)new ExitCrystalSit());
    }

    private void SpawnCrystals(CharacterBody body)
    {
      body.AddBuff(RoR2Content.Buffs.Immune);
      int crystalCount = 3;

      for (int i = 0; i < crystalCount; i++)
      {
        NodeGraph groundNodes = SceneInfo.instance.groundNodes;
        if (!(bool)(Object)groundNodes)
          return;
        List<NodeGraph.NodeIndex> withFlagConditions = groundNodes.GetActiveNodesForHullMaskWithFlagConditions(HullMask.Golem, NodeFlags.None, NodeFlags.NoCharacterSpawn);
        NodeGraph.NodeIndex nodeIndex = withFlagConditions[Random.Range(0, withFlagConditions.Count)];
        Vector3 position;
        groundNodes.GetNodePosition(nodeIndex, out position);
        LunarApostles.timeCrystals.Add(GameObject.Instantiate(LunarApostles.timeCrystal, position, Quaternion.identity));
      }

    }
  }
}