﻿using System;

namespace BaboNetwork
{
    public static class BaboNetSend
    {
        public static void requestSpawn(PlayerState ps) {
            net_clsv_spawn_request spawnRequest = new net_clsv_spawn_request(true);
            spawnRequest.playerID = ps.playerID;
            spawnRequest.weaponID = (byte)ps.getWeaponType();
            spawnRequest.meleeID = (byte)ps.getWeapon2Type();
            byte[] a = BaboUtils.stringToBaboBytes(ps.body.skin, false);
            Array.Copy(a, spawnRequest.skin, a.Length); //max 6 + \0

            spawnRequest.blueDecal = BaboUtils.toBaboColor(ps.body.blueDecal);
            spawnRequest.greenDecal = BaboUtils.toBaboColor(ps.body.greenDecal);
            spawnRequest.redDecal = BaboUtils.toBaboColor(ps.body.redDecal);
        }
    }
}
