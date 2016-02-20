using System;
using System.Text;
using UnityEngine;

namespace BaboNetwork
{
    public class BaboNetPacketProcessor
    {
        private string myMac = "";
        public static ushort GAME_VERSION_CL = 21100;
        private GameState gameState = null;

        private AddPacketCallback sendPacketCallback = null;
        private bool isDownloadingMap = false;

        private void doSendPacket(BaboRawPacket rawPacket) {
            if (sendPacketCallback != null)
                sendPacketCallback(rawPacket);
        }

        public BaboNetPacketProcessor(GameState gameState, AddPacketCallback doSendPacket) {
            this.gameState = gameState;
            this.sendPacketCallback = doSendPacket;

            myMac = BaboUtils.GetMac();
        }

        public bool processPacket(BaboRawPacket rawPacket) {
            if (rawPacket == null) {
                if (Debug.isDebugBuild)
                    Debug.Log("Trying to process null packet");
                return false;
            }
            object parsedPacket = rawPacket.packetToStruct();
            switch ((BaboPacketTypeID)rawPacket.typeID) {
                case BaboPacketTypeID.NET_SVCL_PICKUP_ITEM:
                    doPickupItem((net_svcl_pickup_item)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_FLAME_STICK_TO_PLAYER:
                    doFlameStickToPlayer((net_svcl_flame_stick_to_player)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_CONSOLE:
                    doConsole(rawPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_ADMIN_ACCEPTED:
                    doAdminAccepted();
                    break;
                case BaboPacketTypeID.NET_SVCL_AUTOBALANCE:
                    doAutoBalance();
                    break;
                case BaboPacketTypeID.NET_SVCL_END_VOTE:
                    break;
                case BaboPacketTypeID.NET_SVCL_UPDATE_VOTE:
                    doUpdateVote((net_svcl_update_vote)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_VOTE_RESULT:
                    doVoteResult((net_svcl_vote_result)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_MSG:
                    doMsg((net_svcl_msg)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_PLAYER_UPDATE_STATS:
                    doPlayerUpdateStats((net_svcl_player_update_stats)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_BAD_CHECKSUM_ENTITY:
                    doBadChecksumEntity((net_svcl_bad_checksum_entity)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_BAD_CHECKSUM_INFO:
                    doBadChecksumInfo((net_svcl_bad_checksum_info)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_CLSV_SVCL_PLAYER_INFO:
                    doPlayerInfo((net_clsv_svcl_player_info)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_CLSV_SVCL_CHAT:
                    doChat((net_clsv_svcl_chat)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_CLSV_SVCL_TEAM_REQUEST:
                    doTeamRequest((net_clsv_svcl_team_request)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_CLSV_SVCL_PLAYER_COORD_FRAME:
                    doPlayerCoordFrame((net_clsv_svcl_player_coord_frame)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_CREATE_MINIBOT:
                    doCreateMinibot((net_svcl_create_minibot)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_MINIBOT_COORD_FRAME:
                    doMinibotCoordFrame((net_svcl_minibot_coord_frame)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_CLSV_SVCL_PLAYER_CHANGE_NAME:
                    doPlayerChangeName((net_clsv_svcl_player_change_name)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_CLSV_SVCL_PLAYER_PROJECTILE:
                    doPlayerProjectile((net_clsv_svcl_player_projectile)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_CLSV_SVCL_PLAYER_SHOOT_MELEE:
                    doPlayerShootMelee((net_clsv_svcl_player_shoot_melee)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_CLSV_SVCL_VOTE_REQUEST:
                    doVoteRequest((net_clsv_svcl_vote_request)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_CLSV_MAP_REQUEST:
                    break;
                case BaboPacketTypeID.NET_SVCL_MAP_CHUNK:
                    doMapChunk((net_svcl_map_chunk)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_MAP_LIST:
                    doMapList((net_svcl_map_list)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_CLSV_SVCL_PLAYER_UPDATE_SKIN:
                    doPlayerUpdateSkin((net_clsv_svcl_player_update_skin)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_HASH_SEED:
                    doHashSeed((net_svcl_hash_seed)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_EXPLOSION:
                    doExplosion((net_svcl_explosion)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_PLAYER_HIT:
                    doPlayerHit((net_svcl_player_hit)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_PLAY_SOUND:
                    doPlaySound((net_svcl_play_sound)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_GAMEVERSION:
                    doGameVersion((net_svcl_gameversion)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_SYNCHRONIZE_TIMER:
                    doSyncTimer((net_svcl_synchronize_timer)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_CHANGE_FLAG_STATE:
                    doChangeFlagState((net_svcl_change_flag_state)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_DROP_FLAG:
                    doDropFlag((net_svcl_drop_flag)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_FLAG_ENUM:
                    doFlagEnum((net_svcl_flag_enum)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_GAME_STATE:
                    doGameState((net_svcl_game_state)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_CHANGE_GAME_TYPE:
                    doChangeGameType((net_svcl_change_game_type)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_MAP_CHANGE:
                    doMapChange((net_svcl_map_change)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_SERVER_INFO:
                    doServerInfo((net_svcl_server_info)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_SERVER_DISCONNECT:
                    doServerDisconnect();
                    break;
                case BaboPacketTypeID.NET_SVCL_PLAYER_DISCONNECT:
                    doPlayerDisconnect((net_svcl_player_disconnect)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_PLAYER_ENUM_STATE:
                    doPlayerEnumState((net_svcl_player_enum_state)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_PING:
                    doPing((net_svcl_ping)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_PLAYER_PING:
                    doPlayerPing((net_svcl_player_ping)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_PLAYER_SPAWN:
                    doPlayerSpawn((net_svcl_player_spawn)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_SV_CHANGE:
                    doSvChange((net_svcl_sv_change)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_PLAYER_SHOOT:
                    doSvclPlayerShoot((net_svcl_player_shoot)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_DELETE_PROJECTILE:
                    doDeleteProjectile((net_svcl_delete_projectile)parsedPacket);
                    break;
                case BaboPacketTypeID.NET_SVCL_NEWPLAYER:
                    doNewPlayer((net_svcl_newplayer)parsedPacket);
                    break;
                default:
                    Debug.LogFormat("Unknown packet ID {0}", rawPacket.typeID);
                    return false;
            }
            return true;
        }


        private void doCreateMinibot(net_svcl_create_minibot parsedPacket) {
            //throw new NotImplementedException();
        }

        private void doMinibotCoordFrame(net_svcl_minibot_coord_frame parsedPacket) {
            //throw new NotImplementedException();
        }

        private void doSvclPlayerShoot(net_svcl_player_shoot playerShoot) {
            if (gameState.thisPlayer == null) //my player must be here at this moment
                return;
            PlayerState whoShoot;
            if (!gameState.players.TryGetValue(playerShoot.playerID, out whoShoot))
                return;

            BaboWeapon shootWeapon = (BaboWeapon)playerShoot.weaponID;
            Vector3 p1 = BaboUtils.fromBaboPosition(playerShoot.p1, gameState.map.wShift, gameState.map.hShift);
            Vector3 p2 = BaboUtils.fromBaboPosition(playerShoot.p2, gameState.map.wShift, gameState.map.hShift);
            //Vector3 normal = BaboUtils.vectorFromArray(playerShoot.normal) / 120.0f;
            if (shootWeapon == BaboWeapon.WEAPON_MINIBOT_WEAPON)
                return; //TODO
            if (gameState.thisPlayer.playerID == playerShoot.hitPlayerID) { //shoot to me
                Vector3 direction = Vector3.Normalize(p2 - p1);
                gameState.thisPlayer.currentCF.vel += direction * gameState.serverVars.weaponsVars.vars[shootWeapon].damage * 2;
            }
            whoShoot.firedShowDelay = 2;
            gameState.spawnImpact(p1, p2, shootWeapon, gameState.thisPlayer.teamID, playerShoot.nuzzleID);
        }

        private void doPlayerHit(net_svcl_player_hit playerHit) {
            PlayerState pHit;
            if (!gameState.players.TryGetValue(playerHit.playerID, out pHit))
                return;
            PlayerState pFromHit;
            if (!gameState.players.TryGetValue(playerHit.fromID, out pFromHit))
                return;
            BaboWeapon fromWeapon = (BaboWeapon)playerHit.weaponID;

            if (pHit == gameState.thisPlayer) {
                pHit.currentCF.vel += BaboUtils.vectorFromArray(playerHit.vel) / 10f;
                switch (fromWeapon) {
                    case BaboWeapon.WEAPON_BAZOOKA:
                    case BaboWeapon.WEAPON_GRENADE:
                    case BaboWeapon.NUKE:
                        float realDamage = gameState.serverVars.weaponsVars.vars[fromWeapon].damage;
                        float viewShakeAmount = 2 - (playerHit.damage / realDamage);
                        gameState.viewShake += viewShakeAmount;
                        break;
                }
            }
            pHit.hit((BaboWeapon)playerHit.weaponID, pFromHit, playerHit.damage);
        }

        private void doPlayerShootMelee(net_clsv_svcl_player_shoot_melee parsedPacket) {
            PlayerState ps;
            if (!gameState.players.TryGetValue(parsedPacket.playerID, out ps))
                return;
            ps.secondaryWeapon.shoot();
        }

        private void doPlayerCoordFrame(net_clsv_svcl_player_coord_frame parsedPacket) {
            PlayerState ps;
            if (!gameState.players.TryGetValue(parsedPacket.playerID, out ps))
                return;
            CoordFrame cf = new CoordFrame();
            cf.frameID = parsedPacket.frameID;

            cf.position = BaboUtils.fromBaboPosition(parsedPacket.position, gameState.map.wShift, gameState.map.hShift);
            cf.mousePosOnMap = BaboUtils.fromBaboPosition(parsedPacket.mousePos, gameState.map.wShift, gameState.map.hShift);
            cf.vel = BaboUtils.vectorFromArray(parsedPacket.vel);
            ps.setCoordFrame(cf, parsedPacket.camPosZ);
        }

        private void doExplosion(net_svcl_explosion explosion) {
            /*if (explosion.playerID == gameState.thisPlayer.playerID)
			{
				game->thisPlayer->rocketInAir = false;
				if (game->thisPlayer->meleeWeapon->weaponID == WEAPON_NUCLEAR &&
					game->thisPlayer->minibot && explosion.radius >= 4.0f)
				{
					dksPlay3DSound(dksCreateSoundFromFile("main/sounds/BaboCreve3.wav", false), -1, 5, game->thisPlayer->minibot->currentCF.position, 64);
					game->spawnBloodMinibot(game->thisPlayer->minibot->currentCF.position, .5f);
					delete game->thisPlayer->minibot;
					game->thisPlayer->minibot = 0;
				}
			}*/
            gameState.spawnExplosion(BaboUtils.fromBaboPosition(explosion.position, gameState.map.wShift, gameState.map.hShift), BaboUtils.vectorFromArray(explosion.normal), explosion.radius);
        }

        private void doPlayerProjectile(net_clsv_svcl_player_projectile parsedPacket) {
            PlayerState ps;
            if (!gameState.players.TryGetValue(parsedPacket.playerID, out ps))
                return;
            if (gameState.thisPlayer == ps) {

            }
            Vector3 position = BaboUtils.fromBaboPosition(parsedPacket.position, gameState.map.wShift, gameState.map.hShift);
            Vector3 velocity = BaboUtils.vectorFromArray(parsedPacket.vel) / 10f;
            ProjectileState projectile = new ProjectileState(gameState, position, velocity,
                (BaboProjectileType)parsedPacket.projectileType, (BaboWeapon)parsedPacket.weaponID);
            projectile.nuzzleID = parsedPacket.nuzzleID;
            projectile.playerID = parsedPacket.playerID;
            projectile.uniqueID = parsedPacket.uniqueID;
            gameState.projectiles.Add(projectile);

            if (Debug.isDebugBuild)
                Debug.LogFormat("Projectile {0} id: {1} pos: {2} vel: {3}", projectile.typeID.ToString(),
                    projectile.uniqueID, projectile.position.ToString(), projectile.vel.ToString());
        }

        /*private void doProjectileCoordFrame(net_svcl_projectile_coord_frame parsedPacket) {
            if (Debug.isDebugBuild)
                Debug.LogFormat("Projectile coord: {0} {1}", parsedPacket.uniqueID, ((BaboProjectileType)parsedPacket.projectileID).ToString());

            ProjectileState ps;
			if (!gameState.projectiles.TryGetValue(parsedPacket.uniqueID, out ps))
				return;
			ps.frameID = parsedPacket.frameID;
			ps.position = BaboUtils.fromBaboPosition(parsedPacket.position, gameState.map.wShift, gameState.map.hShift);
			ps.vel = BaboUtils.vectorFromArray(parsedPacket.vel);
		}*/

        private void doDeleteProjectile(net_svcl_delete_projectile parsedPacket) {
            ProjectileState ps = gameState.projectiles.Find(p => p.uniqueID == parsedPacket.projectileID);
            if (ps == null)
                return;
            gameState.projectiles.Remove(ps);
            ps.destroy();
            if (Debug.isDebugBuild)
                Debug.LogFormat("Projectile destroy {0}", parsedPacket.projectileID);
        }

        private void doFlameStickToPlayer(net_svcl_flame_stick_to_player parsedPacket) {
            //projectileID here is not ID but index in list of projectiles (omg)
            if (parsedPacket.projectileID >= gameState.projectiles.Count)
                return;
            //ok, lets pray it is right projectile
            gameState.projectiles[parsedPacket.projectileID].stickToPlayer = parsedPacket.playerID;
        }

        private void doPickupItem(net_svcl_pickup_item parsedPacket) {
            PlayerState ps;
            if (!gameState.players.TryGetValue(parsedPacket.playerID, out ps))
                return;
            switch ((BaboGrabbableItem)parsedPacket.itemType) {
                case BaboGrabbableItem.ITEM_LIFE_PACK:
                    ps.life += .5f;

                    if (gameState.thisPlayer == ps) {
                        //dksPlaySound(gameVar.sfx_lifePack, -1, 255);
                    }
                    break;
                case BaboGrabbableItem.ITEM_WEAPON:
                    ps.setWeaponType((BaboWeapon)parsedPacket.itemFlag);
                    if (gameState.thisPlayer == ps) {
                        //dksPlaySound(gameVar.sfx_equip, -1, 255);
                    }
                    break;
                case BaboGrabbableItem.ITEM_GRENADE:
                    ps.nades++;

                    if (gameState.thisPlayer == ps) {
                        //dksPlaySound(gameVar.sfx_equip, -1, 255);
                    }
                    break;
            }
        }

        private void doSvChange(net_svcl_sv_change parsedPacket) {
            gameState.serverVars.setRawVar(BaboUtils.bytesToString(parsedPacket.svChange));
        }

        private void doUpdateVote(net_svcl_update_vote parsedPacket) {
            gameState.voting.yes = parsedPacket.nbYes;
            gameState.voting.no = parsedPacket.nbNo;
        }

        private void doVoteResult(net_svcl_vote_result parsedPacket) {
            gameState.voting.inProgress = false;
            if (parsedPacket.passed)
                gameState.eventMessages.Add(l10n.votePassed);
            else
                gameState.eventMessages.Add(l10n.voteFailed);
        }

        private void doVoteRequest(net_clsv_svcl_vote_request parsedPacket) {
            gameState.voting.inProgress = true;
            gameState.voting.no = 0;
            gameState.voting.yes = 0;
            gameState.voting.playerID = parsedPacket.playerID;
            gameState.voting.what = BaboUtils.bytesToString(parsedPacket.vote);
        }

        private void doMapChunk(net_svcl_map_chunk parsedPacket) {
            if (isDownloadingMap && (gameState.thisPlayer != null)) {
                // Map has been recieved when last chunk is 0
                if (parsedPacket.size == 0) {
                    isDownloadingMap = false;
                    gameState.map.mapBuffer.Position = 0;
                    gameState.map.createMap();
                    if (!gameState.map.mapCreated) {
                        gameState.needToShutDown = true;
                        return;
                    }
                    gameState.minimap.updateControlSize();

                    // Re-query server info
                    net_clsv_gameversion_accepted gameVersionAccepted = new net_clsv_gameversion_accepted(true);
                    gameVersionAccepted.playerID = gameState.thisPlayer.playerID;
                    //strcpy(gameVersionAccepted.password, m_password.s);
                    doSendPacket(new BaboRawPacket(gameVersionAccepted));
                }
                else {
                    gameState.map.mapBuffer.Write(parsedPacket.data, 0, parsedPacket.size);
                }
            }
        }

        private void doMsg(net_svcl_msg parsedPacket) {
            gameState.chatMessages.Add(String.Format(l10n.serverMessage,
                parsedPacket.teamID, parsedPacket.msgDest, parsedPacket.message));
        }

        private void doChat(net_clsv_svcl_chat parsedPacket) {
            if (gameState.thisPlayer == null)
                return;
            if ((parsedPacket.teamID <= (int)BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR)
                || (parsedPacket.teamID == (int)gameState.thisPlayer.teamID)) {
                gameState.chatMessages.Add(BaboUtils.bytesToString(parsedPacket.message));
            }
        }

        private void doGameVersion(net_svcl_gameversion parsedPacket) {
            if ((parsedPacket.gameVersion == GAME_VERSION_CL) && (gameState.thisPlayer != null)) {
                //send my player info
                net_clsv_svcl_player_info playerInfo = new net_clsv_svcl_player_info(true);

                byte[] mac = Encoding.ASCII.GetBytes(myMac);
                Array.Copy(mac, playerInfo.macAddr, playerInfo.macAddr.Length); //use 6 bytes only here
                playerInfo.playerID = gameState.thisPlayer.playerID;
                byte[] name = Encoding.ASCII.GetBytes(gameState.thisPlayer.playerName);
                Array.Copy(name, playerInfo.playerName, name.Length);

                doSendPacket(new BaboRawPacket(playerInfo));

                //send accept version
                net_clsv_gameversion_accepted gameVersionAccepted = new net_clsv_gameversion_accepted(true);
                gameVersionAccepted.playerID = gameState.thisPlayer.playerID;
                doSendPacket(new BaboRawPacket(gameVersionAccepted));
            }
            else {
                Debug.Log("Wrong game version or player not inited: " + parsedPacket.gameVersion.ToString());
            }
        }

        private void doPlayerChangeName(net_clsv_svcl_player_change_name parsedPacket) {
            PlayerState ps;
            if (gameState.players.TryGetValue(parsedPacket.playerID, out ps)) {
                string newName = BaboUtils.bytesToString(parsedPacket.playerName);
                gameState.eventMessages.Add(String.Format(l10n.playerChangedHisNameFor, ps.playerName, newName));
                ps.playerName = newName;
            }
        }

        private void doTeamRequest(net_clsv_svcl_team_request parsedPacket) {
            PlayerState playerState;
            if (gameState.players.TryGetValue(parsedPacket.playerID, out playerState)) {
                playerState.teamID = (BaboPlayerTeamID)parsedPacket.teamRequested;
            }
        }

        private void doPlayerUpdateSkin(net_clsv_svcl_player_update_skin updateSkin) {
            PlayerState ps;
            if (!gameState.players.TryGetValue(updateSkin.playerID, out ps))
                return;
            if (ps == gameState.thisPlayer)
                return;
            updateSkin.skin[6] = 0;
            ps.body.skin = BaboUtils.bytesToString(updateSkin.skin);
            ps.body.blueDecal = BaboUtils.fromBaboColor(updateSkin.blueDecal);
            ps.body.greenDecal = BaboUtils.fromBaboColor(updateSkin.greenDecal);
            ps.body.redDecal = BaboUtils.fromBaboColor(updateSkin.redDecal);
            BaboPlayerTeamID teamColor = BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR;
            if ((gameState.getGameType() != BaboGameType.GAME_TYPE_DM) || (gameState.getGameType() != BaboGameType.GAME_TYPE_SND))
                teamColor = ps.teamID;
            ps.body.updateSkin(teamColor);
        }

        private void doPlayerUpdateStats(net_svcl_player_update_stats playerStats) {
            PlayerState ps;
            if (!gameState.players.TryGetValue(playerStats.playerID, out ps))
                return;
            ps.kills = playerStats.kills;
            ps.deaths = playerStats.deaths;
            ps.score = playerStats.score;
            ps.returns = playerStats.returns;
            ps.flagAttempts = playerStats.flagAttempts;
            ps.timePlayedCurGame = playerStats.timePlayedCurGame;
        }

        private void doNewPlayer(net_svcl_newplayer parsedPacket) {
            if (gameState.players.ContainsKey(parsedPacket.newPlayerID))
                return;
            PlayerState playerState = PlayerState.createSelf(parsedPacket.newPlayerID, gameState.baboModel);
            playerState.netID = parsedPacket.baboNetID;
            gameState.players.Add(parsedPacket.newPlayerID, playerState);

            if (gameState.thisPlayer == null) {
                gameState.thisPlayer = playerState;
            }
        }

        private void doPlayerSpawn(net_svcl_player_spawn playerSpawn) {
            PlayerState ps;
            if (!gameState.players.TryGetValue(playerSpawn.playerID, out ps))
                return;
            playerSpawn.skin[6] = 0; // Au cas qu'un hacker s'amuse
            ps.body.skin = BaboUtils.bytesToString(playerSpawn.skin);
            ps.body.blueDecal = BaboUtils.fromBaboColor(playerSpawn.blueDecal);
            ps.body.greenDecal = BaboUtils.fromBaboColor(playerSpawn.greenDecal);
            ps.body.redDecal = BaboUtils.fromBaboColor(playerSpawn.redDecal);
            ;
            BaboPlayerTeamID teamColor = BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR;
            if ((gameState.getGameType() != BaboGameType.GAME_TYPE_DM) || (gameState.getGameType() != BaboGameType.GAME_TYPE_SND))
                teamColor = ps.teamID;
            ps.body.updateSkin(teamColor);
            ps.setWeaponType((BaboWeapon)playerSpawn.weaponID);
            ps.secondaryWeapon.setWeapon((BaboWeapon)playerSpawn.meleeID);
            ps.prepareToSpawn(new Vector3((float)playerSpawn.position[0] / 10.0f, (float)playerSpawn.position[1] / 10.0f, (float)playerSpawn.position[2] / 10.0f));
        }

        private void doPlayerEnumState(net_svcl_player_enum_state parsedPacket) {
            PlayerState ps;
            if (gameState.players.TryGetValue(parsedPacket.playerID, out ps)) {
                gameState.players.Remove(parsedPacket.playerID);
                ps.destroy();
            }

            ps = PlayerState.createSelf(parsedPacket.playerID, gameState.baboModel);

            ps.netID = parsedPacket.babonetID;
            ps.playerName = BaboUtils.bytesToString(parsedPacket.playerName);
            ps.ip = BaboUtils.bytesToString(parsedPacket.playerIP);
            ps.kills = parsedPacket.kills;
            ps.deaths = (int)parsedPacket.deaths;
            ps.score = (int)parsedPacket.score;
            ps.returns = (int)parsedPacket.returns;
            ps.flagAttempts = (int)parsedPacket.flagAttempts;
            ps.damage = (int)parsedPacket.damage;
            ps.status = (BaboPlayerStatus)parsedPacket.status;
            ps.teamID = (BaboPlayerTeamID)parsedPacket.teamID;
            ps.setWeaponType((BaboWeapon)parsedPacket.weaponID);
            ps.life = parsedPacket.life;
            ps.dmg = parsedPacket.dmg;
            parsedPacket.skin[6] = 0;
            ps.body.skin = BaboUtils.bytesToString(parsedPacket.skin);
            ps.body.blueDecal = BaboUtils.fromBaboColor(parsedPacket.blueDecal);
            ps.body.greenDecal = BaboUtils.fromBaboColor(parsedPacket.greenDecal);
            ps.body.redDecal = BaboUtils.fromBaboColor(parsedPacket.redDecal);
            BaboPlayerTeamID teamColor = BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR;
            if ((gameState.getGameType() != BaboGameType.GAME_TYPE_DM) || (gameState.getGameType() != BaboGameType.GAME_TYPE_SND))
                teamColor = ps.teamID;
            ps.body.updateSkin(teamColor);

            gameState.eventMessages.Add(string.Format(l10n.addingPlayer, ps.playerName));

            gameState.players.Add(parsedPacket.playerID, ps);
        }

        private void doPlayerInfo(net_clsv_svcl_player_info parsedPacket) {
            PlayerState playerState;
            if (gameState.players.TryGetValue(parsedPacket.playerID, out playerState)) {
                playerState.playerName = BaboUtils.bytesToString(parsedPacket.playerName);
                playerState.ip = BaboUtils.bytesToString(parsedPacket.playerIP);

                gameState.eventMessages.Add(String.Format(l10n.playerJoinedGame, playerState.playerName));
            }
        }

        private void doPlayerDisconnect(net_svcl_player_disconnect parsedPacket) {
            PlayerState player;
            if (gameState.players.TryGetValue(parsedPacket.playerID, out player)) {
                gameState.eventMessages.Add(String.Format(l10n.playerDisconnected, player.playerName));
                gameState.players.Remove(parsedPacket.playerID);
                player.destroy();
            }
        }

        private void doServerDisconnect() {
            gameState.eventMessages.Add(l10n.serverDisconnected);
            gameState.needToShutDown = true;
        }

        private void doMapList(net_svcl_map_list parsedPacket) {
            gameState.eventMessages.Add(BaboUtils.bytesToString(parsedPacket.mapName));
        }

        private void requestMap(byte[] name) {
            isDownloadingMap = true;

            net_clsv_map_request request = new net_clsv_map_request(true);
            request.mapName = name;
            request.uniqueClientID = gameState.thisPlayer.playerID;
            doSendPacket(new BaboRawPacket(request));
        }

        private void doMapChange(net_svcl_map_change parsedPacket) {
            gameState.map.mapName = BaboUtils.bytesToString(parsedPacket.mapName);
            gameState.setGameType((BaboGameType)parsedPacket.gameType);
            requestMap(parsedPacket.mapName);
        }

        private void doChangeGameType(net_svcl_change_game_type parsedPacket) {
            gameState.setGameType((BaboGameType)parsedPacket.newGameType);
        }

        private void doFlagEnum(net_svcl_flag_enum parsedPacket) {
            gameState.flagsState[BaboTeamColor.BLUE].state = (FlagStateID)parsedPacket.flagState[0];
            gameState.flagsState[BaboTeamColor.BLUE].position =
                BaboUtils.fromBaboPosition(parsedPacket.positionBlue, gameState.map.wShift, gameState.map.hShift);

            gameState.flagsState[BaboTeamColor.RED].state = (FlagStateID)parsedPacket.flagState[1];
            gameState.flagsState[BaboTeamColor.RED].position =
                BaboUtils.fromBaboPosition(parsedPacket.positionRed, gameState.map.wShift, gameState.map.hShift);

            /*if (Debug.isDebugBuild)
                Debug.LogFormat("Flag enum. red {0} {1} blue {2} {3}",
                    ((FlagStateID)parsedPacket.flagState[1]).ToString(),
                    gameState.flagsState[BaboTeamColor.RED].position.ToString(),
                    ((FlagStateID)parsedPacket.flagState[0]).ToString(),
                    gameState.flagsState[BaboTeamColor.BLUE].position.ToString());*/
        }

        private void doDropFlag(net_svcl_drop_flag dropFlag) {
            gameState.flagsState[(BaboTeamColor)dropFlag.flagID].state = FlagStateID.ON_FLOOR;
            gameState.flagsState[(BaboTeamColor)dropFlag.flagID].position =
                BaboUtils.fromBaboPosition(dropFlag.position, gameState.map.wShift, gameState.map.hShift);

            /*if (Debug.isDebugBuild)
                Debug.LogFormat("Flag drop {0} {1}", ((BaboPlayerTeamID)dropFlag.flagID).ToString(),
                    gameState.flagsState[(BaboTeamColor)dropFlag.flagID].position.ToString());*/
        }

        private void doChangeFlagState(net_svcl_change_flag_state flagState) {
            BaboFlagsState.BaboFlagState fs = gameState.flagsState[(BaboTeamColor)flagState.flagID];

            if (fs.state == (FlagStateID)flagState.newFlagState)
                return;
            fs.state = (FlagStateID)flagState.newFlagState;

            PlayerState ps;
            if (!gameState.players.TryGetValue(flagState.playerID, out ps))
                return;
            string eventMsg = "";
            switch (fs.state) {
                case FlagStateID.ON_FLOOR: //handles by net_svcl_drop_flag message
                    break;
                case FlagStateID.ON_POD:
                    if (flagState.flagID == (byte)ps.teamID) {
                        ps.returns++;
                        if ((BaboPlayerTeamID)flagState.flagID == BaboPlayerTeamID.PLAYER_TEAM_RED)
                            eventMsg = string.Format(l10n.playerReturnedFlag, ps.playerName, l10n.red);
                        else
                            eventMsg = string.Format(l10n.playerReturnedFlag, ps.playerName, l10n.blue);
                    }
                    else {
                        ps.score++;
                        switch (ps.teamID) {
                            case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
                                gameState.blueTeamScore++;
                                eventMsg = string.Format(l10n.playerScoresFlag, ps.playerName, l10n.blue);
                                break;
                            case BaboPlayerTeamID.PLAYER_TEAM_RED:
                                gameState.redTeamScore++;
                                eventMsg = string.Format(l10n.playerScoresFlag, ps.playerName, l10n.red);
                                break;
                            default:
                                Debug.LogFormat("Player {0} from team {1} must not score flag", ps.playerID, ps.teamID.ToString());
                                return;
                        }
                    }
                    break;
                default:
                    ps.flagAttempts++;
                    if ((BaboPlayerTeamID)flagState.flagID == BaboPlayerTeamID.PLAYER_TEAM_RED)
                        eventMsg = string.Format(l10n.playerTookFlag, ps.playerName, l10n.red);
                    else
                        eventMsg = string.Format(l10n.playerTookFlag, ps.playerName, l10n.blue);
                    break;
            }
            gameState.eventMessages.Add(eventMsg);
        }

        private void doServerInfo(net_svcl_server_info parsedPacket) {
            //gameState.gotGameState = true;

            gameState.mapSeed = parsedPacket.mapSeed;
            gameState.map.mapName = BaboUtils.bytesToString(parsedPacket.mapName);
            BaboGameType gameType = (BaboGameType)parsedPacket.gameType;
            switch (gameType) {
                case BaboGameType.GAME_TYPE_CTF:
                    gameState.blueTeamScore = parsedPacket.blueWin;
                    gameState.redTeamScore = parsedPacket.redWin;
                    break;
                case BaboGameType.GAME_TYPE_TDM:
                    gameState.blueTeamScore = parsedPacket.blueScore;
                    gameState.redTeamScore = parsedPacket.redScore;
                    break;
            }
            gameState.setGameType(gameType);
            if (gameState.thisPlayer != null) {
                // On n'est plus en mode loading (y a tu vraiment un mode loader? lol)
                gameState.thisPlayer.status = BaboPlayerStatus.PLAYER_STATUS_DEAD;
            }
            // If no map created, send request
            if (!gameState.map.mapCreated) {
                //gameState.gotGameState = false;
                requestMap(parsedPacket.mapName);
            }
            gameState.updateMenuInfo();
        }

        //round state
        private void doGameState(net_svcl_game_state parsedPacket) {
            // Do not load round state if map still downloading
            if (isDownloadingMap)
                return;

            gameState.roundState = (BaboRoundState)parsedPacket.newState;

            if (parsedPacket.reInit != 0) {
                // Ouch, on fout toute а 0 (score, etc)
            }
        }

        private void doPlayerPing(net_svcl_player_ping parsedPacket) {
            PlayerState player;
            if (gameState.players.TryGetValue(parsedPacket.playerID, out player)) {
                player.ping = parsedPacket.ping;
            }
        }

        private void doPing(net_svcl_ping parsedPacket) {
            if (gameState.thisPlayer == null)
                return;
            net_clsv_pong pong = new net_clsv_pong();
            pong.playerID = gameState.thisPlayer.playerID;
            doSendPacket(new BaboRawPacket(pong));
        }

        private void doSyncTimer(net_svcl_synchronize_timer parsedPacket) {
            //if (parsedPacket.frameID > gameState.serverFrameID)
            {
                gameState.serverFrameID = parsedPacket.frameID;
                gameState.gameTimeLeft = parsedPacket.gameTimeLeft;
                gameState.roundTimeLeft = parsedPacket.roundTimeLeft;
            }
        }

        private void doHashSeed(net_svcl_hash_seed parsedPacket) {
            // we receive an hash seed, we need to send a response back
            byte[] hash = Encoding.ASCII.GetBytes(BaboUtils.GetMac());

            net_svcl_hash_seed_reply hash_seed = new net_svcl_hash_seed_reply();

            hash_seed.s1 = hash[0];
            hash_seed.s2 = hash[1];
            hash_seed.s3 = hash[2];
            hash_seed.s4 = hash[3];

            doSendPacket(new BaboRawPacket(hash_seed));
        }

        private void doAdminAccepted() {
            gameState.eventMessages.Add(l10n.adminAccepted);
            gameState.isAdmin = true;
        }

        private void doPlaySound(net_svcl_play_sound parsedPacket) {
            //TODO: sounds
        }

        private void doBadChecksumEntity(net_svcl_bad_checksum_entity bce) {
            gameState.eventMessages.Add(string.Format(l10n.badCheckSumEntity, bce.id, BaboUtils.bytesToString(bce.name),
                BaboUtils.bytesToString(bce.playerIP)));
        }

        private void doBadChecksumInfo(net_svcl_bad_checksum_info bci) {
            gameState.eventMessages.Add(string.Format(l10n.badCheckSumInfo, bci.number));
        }

        private void doConsole(BaboRawPacket parsedPacket) {
            gameState.eventMessages.Add(BaboUtils.bytesToString(parsedPacket.data));
        }

        private void doAutoBalance() {
            gameState.autoBalanceTimer = (float)gameState.serverVars.sv_autoBalanceTime;
        }
    }
}
