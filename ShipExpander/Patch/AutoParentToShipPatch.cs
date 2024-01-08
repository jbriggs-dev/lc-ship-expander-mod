﻿using HarmonyLib;
using ShipExpander.Core;
using ShipExpander.Helper;
using Unity.Netcode;
using UnityEngine;

namespace ShipExpander.Patch;

// Some objects within the game use this class to stay with the ship when it moves.
//  This patch will add the new ship offset for where the inside of the ship sits in 
//  the game work.
#if DEBUG
[HarmonyDebug]
#endif
[HarmonyPatch(typeof(AutoParentToShip))]
public class AutoParentToShipPatch
{
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    static void HarmonyPostFixAwake(ref Vector3 ___positionOffset, ref Vector3 ___startingPosition)
    {
        Vector3 originalPosition = ___positionOffset;
        ___positionOffset += new Vector3(0f, ConstantVariables.InsideShipHeight, 0f);
        Core.SELogger.Log("AutoParentToShipPatch",
            $"Adjusting offset for AutoParentToShip object from {originalPosition} -> {___positionOffset}");
        ___startingPosition = ___positionOffset;
    }
    
    
    [HarmonyPatch("Awake")]
    [HarmonyPrefix]
    static void HarmonyPreFix(ref AutoParentToShip __instance)
    {
        var insideShipGO = GameObjectHelper.FindObjectByName("insideShip");
        if (insideShipGO == null) return;
        
        var networkObject = __instance.GetComponent<NetworkObject>();
        networkObject.TrySetParent(insideShipGO);
        
        
    }
}