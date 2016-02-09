using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaboNetwork
{
    public static class BaboNetSend
    {
        public static void requestSpawn(PlayerState ps)
        {
            net_clsv_spawn_request spawnRequest = new net_clsv_spawn_request(true);
            spawnRequest.playerID = ps.playerID;
            spawnRequest.weaponID = (byte)ps.nextSpawnWeapon;
            spawnRequest.meleeID = (byte)ps.nextSecondaryWeapon;
            byte[] a = BaboUtils.stringToBytes(ps.body.skin);
            Array.Copy(a, spawnRequest.skin, a.Length); //max 6 + \0
            
            spawnRequest.blueDecal = BaboUtils.toBaboColor(ps.body.blueDecal);
            spawnRequest.greenDecal = BaboUtils.toBaboColor(ps.body.greenDecal);
            spawnRequest.redDecal = BaboUtils.toBaboColor(ps.body.redDecal);
        }
    }
}
