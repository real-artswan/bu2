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

		private void doSendPacket(BaboRawPacket rawPacket)
		{
			if (sendPacketCallback != null)
				sendPacketCallback(rawPacket);
		}

		public BaboNetPacketProcessor(GameState gameState, AddPacketCallback doSendPacket)
		{
			this.gameState = gameState;
			this.sendPacketCallback = doSendPacket;
			
			myMac = BaboUtils.GetMac();
		}
		public bool processPacket(BaboRawPacket rawPacket)
		{
			object parsedPacket = rawPacket.packetToStruct();
			switch ((BaboPacketTypeID)rawPacket.typeID)
			{
				/*case BaboPacketTypeID.NET_CLSV_PONG:
					doPong((net_clsv_pong)parsedPacket);
					break;*/
				case BaboPacketTypeID.NET_CLSV_SPAWN_REQUEST:
					doSpawnRequest((net_clsv_spawn_request)parsedPacket);
					break;
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
				case BaboPacketTypeID.NET_CLSV_BROADCAST_QUERY:
					break;
				case BaboPacketTypeID.NET_CLSV_BROADCAST_GAME_INFO:
					break;
				case BaboPacketTypeID.NET_SVCL_HASH_SEED:
					doHashSeed((net_svcl_hash_seed)parsedPacket);
					break;
				case BaboPacketTypeID.NET_SVCL_HASH_SEED_REPLY:
					break;
				case BaboPacketTypeID.NET_SVCL_PROJECTILE_COORD_FRAME:
					doProjectileCoordFrame((net_svcl_projectile_coord_frame)parsedPacket);
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
				case BaboPacketTypeID.NET_CLSV_PLAYER_SHOOT:
					doClsvPlayerShoot((net_clsv_player_shoot)parsedPacket);
					break;
				case BaboPacketTypeID.NET_CLSV_GAMEVERSION_ACCEPTED:
					doGameVersionAccepted((net_clsv_gameversion_accepted)parsedPacket);
					break;
				case BaboPacketTypeID.NET_CLSV_PICKUP_REQUEST:
					doPickupRequest((net_clsv_pickup_request)parsedPacket);
					break;
				case BaboPacketTypeID.NET_SVCL_NEWPLAYER:
					doNewPlayer((net_svcl_newplayer)parsedPacket);
					break;
				default:
					return false;
			}
			return true;
		}

		private void doBadChecksumInfo(net_svcl_bad_checksum_info parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doBadChecksumEntity(net_svcl_bad_checksum_entity parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doPlayerUpdateSkin(net_clsv_svcl_player_update_skin parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doMapList(net_svcl_map_list parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void requestMap(byte[] name)
		{
			isDownloadingMap = true;

			net_clsv_map_request request = new net_clsv_map_request(true);
			request.mapName = name;
			request.uniqueClientID = gameState.thisPlayer.playerState.playerID;
			doSendPacket(new BaboRawPacket(request));
		}

		private void doMapChange(net_svcl_map_change parsedPacket)
		{
			gameState.map.mapName = Encoding.ASCII.GetString(parsedPacket.mapName);
			gameState.setGameType((BaboGameType)parsedPacket.gameType);
			requestMap(parsedPacket.mapName);
		}

		private void doChangeGameType(net_svcl_change_game_type parsedPacket)
		{
			gameState.setGameType((BaboGameType)parsedPacket.newGameType);
		}

		private void doFlagEnum(net_svcl_flag_enum parsedPacket)
		{
			if (gameState.map.isActiveAndEnabled)
			{
				gameState.map.flagsState.blueState = (BaboFlagsState.FlagState)parsedPacket.flagState[0];
				gameState.map.flagsState.redState = (BaboFlagsState.FlagState)parsedPacket.flagState[1];
				gameState.map.flagsState.bluePos = new Vector3(
					parsedPacket.positionBlue[0],
					parsedPacket.positionBlue[1],
					parsedPacket.positionBlue[2]);
				gameState.map.flagsState.redPos = new Vector3(
					parsedPacket.positionRed[0],
					parsedPacket.positionRed[1],
					parsedPacket.positionRed[2]);
			}
		}

		private void doDropFlag(net_svcl_drop_flag parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doChangeFlagState(net_svcl_change_flag_state parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doConsole(BaboRawPacket parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doPlaySound(net_svcl_play_sound parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doAutoBalance()
		{
			throw new NotImplementedException();
		}

		private void doDeleteProjectile(net_svcl_delete_projectile parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doPlayerChangeName(net_clsv_svcl_player_change_name parsedPacket)
		{
			PlayerState ps;
			if (gameState.players.TryGetValue(parsedPacket.playerID, out ps))
			{
				string newName = Encoding.ASCII.GetString(parsedPacket.playerName);
				gameState.eventMessages.Add(String.Format(l10n.playerChangedHisNameFor, ps.playerName, newName));
				ps.playerName = newName;
			}
		}

		private void doSvChange(net_svcl_sv_change parsedPacket)
		{
			gameState.serverVars.setRawVar(Encoding.ASCII.GetString(parsedPacket.svChange));
		}

		private void doCreateMinibot(net_svcl_create_minibot parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doMinibotCoordFrame(net_svcl_minibot_coord_frame parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doTeamRequest(net_clsv_svcl_team_request parsedPacket)
		{
			PlayerState playerState;
			// Est-ce que ce player existe
			if (gameState.players.TryGetValue(parsedPacket.playerID, out playerState))
			{
				playerState.teamID = (BaboPlayerTeamID)parsedPacket.teamRequested;
			}
		}

		private void doAdminAccepted()
		{
			throw new NotImplementedException();
		}

		private void doVoteRequest(net_clsv_svcl_vote_request parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doPlayerUpdateStats(net_svcl_player_update_stats parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doMsg(net_svcl_msg parsedPacket)
		{
			gameState.chatMessages.Add(String.Format(l10n.serverMessage, 
				parsedPacket.teamID, parsedPacket.msgDest, parsedPacket.message));
		}

		private void doUpdateVote(net_svcl_update_vote parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doVoteResult(net_svcl_vote_result parsedPacket)
		{
			throw new NotImplementedException();
		}

		private void doMapChunk(net_svcl_map_chunk parsedPacket)
		{
			if (isDownloadingMap && (gameState.thisPlayer.playerState != null))
			{
				// Map has been recieved when last chunk is 0
				if (parsedPacket.size == 0)
				{
					isDownloadingMap = false;
					gameState.map.mapBuffer.Position = 0;
					gameState.map.createMap();
					if (!gameState.map.isActiveAndEnabled)
					{
						gameState.needToShutDown = true;
						return;
					}

					// Re-query server in case state has changed
					net_clsv_gameversion_accepted gameVersionAccepted = new net_clsv_gameversion_accepted(true);
					gameVersionAccepted.playerID = gameState.thisPlayer.playerState.playerID;
					//strcpy(gameVersionAccepted.password, m_password.s);
					doSendPacket(new BaboRawPacket(gameVersionAccepted));
				}
				else
				{
					gameState.map.mapBuffer.Write(parsedPacket.data, 0, parsedPacket.size);
				}
			}
		}

		private void doChat(net_clsv_svcl_chat parsedPacket)
		{
			// On print dans console
			if (gameState.thisPlayer.playerState != null)
			{
				if ((parsedPacket.teamID <= (int)BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR) 
					|| (parsedPacket.teamID == (int)gameState.thisPlayer.playerState.teamID))
				{
					gameState.chatMessages.Add(Encoding.ASCII.GetString(parsedPacket.message));
				}
			}
		}

		private void doGameVersion(net_svcl_gameversion parsedPacket)
		{
			if ((parsedPacket.gameVersion == GAME_VERSION_CL) && (gameState.thisPlayer.playerState != null))
			{
				//send my player info
				net_clsv_svcl_player_info playerInfo = new net_clsv_svcl_player_info(true);

				byte[] mac = Encoding.ASCII.GetBytes(myMac);
				Array.Copy(mac, playerInfo.macAddr, playerInfo.macAddr.Length); //use 6 bytes only here
				playerInfo.playerID = gameState.thisPlayer.playerState.playerID;
				byte[] name = Encoding.ASCII.GetBytes(gameState.gameVars.playerName.text);
				Array.Copy(name, playerInfo.playerName, name.Length);

				doSendPacket(new BaboRawPacket(playerInfo));

				//send accept version
				net_clsv_gameversion_accepted gameVersionAccepted = new net_clsv_gameversion_accepted(true);
				gameVersionAccepted.playerID = gameState.thisPlayer.playerState.playerID;
				doSendPacket(new BaboRawPacket(gameVersionAccepted));
			}
			else
			{
				Debug.Log("Wrong game version or player not inited: " + parsedPacket.gameVersion.ToString());
			}
		}

		private void doNewPlayer(net_svcl_newplayer parsedPacket)
		{
			if (gameState.players.ContainsKey(parsedPacket.newPlayerID))
				return;
			PlayerState playerState = new PlayerState(parsedPacket.newPlayerID);
			playerState.netID = parsedPacket.baboNetID;
			gameState.players.Add(parsedPacket.newPlayerID, playerState);
			
			if (gameState.thisPlayer.playerState == null) {
				gameState.thisPlayer.playerState = playerState;
			}
		}

		private void doPickupRequest(net_clsv_pickup_request parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doGameVersionAccepted(net_clsv_gameversion_accepted parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doClsvPlayerShoot(net_clsv_player_shoot parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doSvclPlayerShoot(net_svcl_player_shoot parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doPlayerSpawn(net_svcl_player_spawn parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doPlayerEnumState(net_svcl_player_enum_state parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doPlayerDisconnect(net_svcl_player_disconnect parsedPacket)
		{
			PlayerState player;
			if (gameState.players.TryGetValue(parsedPacket.playerID, out player))
			{
				gameState.eventMessages.Add(String.Format(l10n.playerDisconnected, player.playerName));
				gameState.players.Remove(parsedPacket.playerID); //remove body too
			}
		}

		private void doServerDisconnect()
		{
			gameState.needToShutDown = true;
			gameState.eventMessages.Add(l10n.serverDisconnected);
		}

		private void doServerInfo(net_svcl_server_info parsedPacket)
		{   
			gameState.isConnected = true;
			gameState.gotGameState = true;

			gameState.mapSeed = parsedPacket.mapSeed;
			gameState.map.mapName = Encoding.ASCII.GetString(parsedPacket.mapName);
			gameState.hud.blueTeamScore.text = parsedPacket.blueScore.ToString();
			gameState.hud.redTeamScore.text = parsedPacket.redScore.ToString();
			gameState.blueWin = parsedPacket.blueWin;
			gameState.redWin = parsedPacket.redWin;
			gameState.setGameType((BaboGameType)parsedPacket.gameType);
			// On a fini de loader, on change notre status
			if (gameState.thisPlayer.playerState != null)
			{
				// On n'est plus en mode loading (y a tu vraiment un mode loader? lol)
				gameState.thisPlayer.playerState.status = BaboPlayerStatus.PLAYER_STATUS_DEAD;
			}
			// If no map created, send request
			if (!gameState.map.mapCreated) {
				gameState.isConnected = false;
				gameState.gotGameState = false;
				requestMap(parsedPacket.mapName);
			}
			gameState.updateMenuInfo();
		}

		//round state
		private void doGameState(net_svcl_game_state parsedPacket)
		{
			// Do not load round state if map still downloading
			if(isDownloadingMap)
				return;

			gameState.roundState = (BaboRoundState)parsedPacket.newState;

			if (parsedPacket.reInit != 0)
			{
				// Ouch, on fout toute а 0 (score, etc)
			}
		}

		private void doPlayerHit(net_svcl_player_hit parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doExplosion(net_svcl_explosion parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doProjectileCoordFrame(net_svcl_projectile_coord_frame parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doHashSeed(net_svcl_hash_seed parsedPacket)
		{
			// we receive an hash seed, we need to send a response back
			byte[] hash = Encoding.ASCII.GetBytes(BaboUtils.GetMac());

			net_svcl_hash_seed_reply hash_seed = new net_svcl_hash_seed_reply();

			hash_seed.s1 = hash[0];
			hash_seed.s2 = hash[1];
			hash_seed.s3 = hash[2];
			hash_seed.s4 = hash[3];

			doSendPacket(new BaboRawPacket(hash_seed));
		}

		private void doPlayerShootMelee(net_clsv_svcl_player_shoot_melee parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doPlayerProjectile(net_clsv_svcl_player_projectile parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doPlayerCoordFrame(net_clsv_svcl_player_coord_frame parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doPlayerInfo(net_clsv_svcl_player_info parsedPacket)
		{
			PlayerState playerState;
			if (gameState.players.TryGetValue(parsedPacket.playerID, out playerState)) {
				playerState.playerName = Encoding.ASCII.GetString(parsedPacket.playerName);
				playerState.ip = Encoding.ASCII.GetString(parsedPacket.playerIP);

				gameState.eventMessages.Add(String.Format(l10n.playerJoinedGame, playerState.playerName));
			}
		}

		private void doFlameStickToPlayer(net_svcl_flame_stick_to_player parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doPickupItem(net_svcl_pickup_item parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doSpawnRequest(net_clsv_spawn_request parsedPacket)
		{
			//throw new NotImplementedException();
		}

		private void doPlayerPing(net_svcl_player_ping parsedPacket)
		{
			PlayerState player;
			if (gameState.players.TryGetValue(parsedPacket.playerID, out player))
			{
				player.ping = parsedPacket.ping;
			}
		}

		private void doPing(net_svcl_ping parsedPacket)
		{
			if (gameState.thisPlayer.playerState != null)
			{
				net_clsv_pong pong = new net_clsv_pong();
				pong.playerID = gameState.thisPlayer.playerState.playerID;
				doSendPacket(new BaboRawPacket(pong));
			}
		}

		private void doSyncTimer(net_svcl_synchronize_timer parsedPacket)
		{
			//if (parsedPacket.frameID > gameState.serverFrameID)
			{
				gameState.serverFrameID = parsedPacket.frameID;
				gameState.gameTimeLeft = parsedPacket.gameTimeLeft;
				int gtl = (int)parsedPacket.gameTimeLeft + 1;
                int rtl = (int)parsedPacket.roundTimeLeft + 1;
                gameState.hud.gameTimer.text = String.Format("{0:d2}:{1:d2}", gtl / 60, gtl % 60);
                gameState.hud.roundTimer.text = String.Format("{0:d2}:{1:d2}", rtl / 60, rtl % 60);
            }
		}
	}
}
