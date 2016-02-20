using System;
using System.Runtime.InteropServices;

namespace BaboNetwork
{
	class BaboKey
	{
		public static string RND_KEY = "RND1";
		public static ushort KEY_SIZE = 9;
		public byte[] value = new byte[KEY_SIZE];

	}

    public enum BaboPacketTypeID 
    {
        NET_CLSV_PONG = 1,
        NET_CLSV_SPAWN_REQUEST = 2,
		// La version du server est acceptй par le client
		NET_CLSV_PLAYER_SHOOT = 3,
		NET_CLSV_GAMEVERSION_ACCEPTED = 4,
		// On demande au server de pickuper un item par terre
		NET_CLSV_PICKUP_REQUEST = 5,
		// On demande au server d'кtre admin!
		NET_CLSV_ADMIN_REQUEST = 6,
		// Vote
		NET_CLSV_VOTE = 7,
		// On map list request
		NET_CLSV_MAP_LIST_REQUEST = 8,

		// Le server accept une new connection, il envoit а tout le monde le ID du joueur
		NET_SVCL_NEWPLAYER = 101,
		// Le server envoit ses info aux nouveaux clients
		// Suivi de зa, il va lui envoyer la liste de tout les joueurs
		// А partir de ce moment lа, le joueur devra recevoir tout les messages du server
		// Pour garder l'йtat du server а jour
		NET_SVCL_SERVER_INFO = 102,
		// Le server fou le camp, il le dit а tout le monde (en moins quil plante lа)
		NET_SVCL_SERVER_DISCONNECT = 103,
		// Le client fou le camp, le server le sait tout suite, et le shoot au autres 
		// (en moins quil plante lа)
		NET_SVCL_PLAYER_DISCONNECT = 104,
		// Le client fou le camp, le server le sait tout suite, et le shoot au autres 
		// (en moins quil plante lа)
		NET_SVCL_PLAYER_ENUM_STATE = 105,
		// Le server envoit un ping а toute les seconde а tout les joueurs
		NET_SVCL_PING = 106,
		// Le server envoit а chaque seconde le ping de toute le monde а tout le monde
		NET_SVCL_PLAYER_PING = 107,
		// Le server reзois la request de spawner du joueur, et renvoit а TOUT le monde
		// sa position de spawn
		NET_SVCL_PLAYER_SPAWN = 108,
		// Le server modifie une variable sv_, il va l'envoyer а tout le monde
		// Les sv ce sont les seuls qui affectent le gameplay et que le server garde l'exclusivitйe
		NET_SVCL_SV_CHANGE = 109,
		// Un client a tirй, le server a effectuй la collision et renvoi le rйsultat aux autres joueurs
		NET_SVCL_PLAYER_SHOOT = 110,
		// Pour dire qu'on supprime un projectile
		NET_SVCL_DELETE_PROJECTILE = 111,
		// La position d'un projectile (uniquement controlй par le server)
		NET_SVCL_PROJECTILE_COORD_FRAME = 112,
		// Pour spawner une explosion
		NET_SVCL_EXPLOSION = 113,
		// Si on a touchй un joueur l'hors d'une explosion par exemple
		NET_SVCL_PLAYER_HIT = 114,
		// Le server emet un son et veut que les clients le jou
		NET_SVCL_PLAY_SOUND = 115,
		// La version du server
		NET_SVCL_GAMEVERSION = 116,
		// Pour synchroniser le temps des horloges du jeu
		NET_SVCL_SYNCHRONIZE_TIMER = 117,
		// Pour Changer l'йtat d'un flag
		NET_SVCL_CHANGE_FLAG_STATE = 118,
		// Un joueur est mort ou disconnectй, il laisse tomber le flag
		// Le server communique alors la position exacte aux autres players
		NET_SVCL_DROP_FLAG = 119,
		// Un joueur join la game, on send juste le enum state
		NET_SVCL_FLAG_ENUM = 120,
		// On change le state du round
		NET_SVCL_GAME_STATE = 121,
		// On change le type de game
		NET_SVCL_CHANGE_GAME_TYPE = 122,
		// Le server change de map, il le dit aux autres !
		NET_SVCL_MAP_CHANGE = 123,
        // Un joueur ramasse un item, on le dit а tout le monde
        NET_SVCL_PICKUP_ITEM = 124,
        // Un joueur passe sur une flame, la flame se colle sur lui
        NET_SVCL_FLAME_STICK_TO_PLAYER = 125,
        // La console envoit les messages console aux admin
        NET_SVCL_CONSOLE = 126,
        // Le server accept le user/pass du admin
        NET_SVCL_ADMIN_ACCEPTED = 127,
        // Le server averti qu'il va y avoir un auto-balance dans = 15secondes.
        NET_SVCL_AUTOBALANCE = 128,
        // Le server met fin au vote
        NET_SVCL_END_VOTE = 129,
        // Le server update le voting status
        NET_SVCL_UPDATE_VOTE = 130,
        NET_SVCL_VOTE_RESULT = 131,
        NET_SVCL_MSG = 132,
        NET_SVCL_PLAYER_UPDATE_STATS = 133,
        NET_SVCL_BAD_CHECKSUM_ENTITY = 134,
        NET_SVCL_BAD_CHECKSUM_INFO = 135,

        NET_CLSV_SVCL_PLAYER_INFO = 201,
        NET_CLSV_SVCL_CHAT = 202,
        NET_CLSV_SVCL_TEAM_REQUEST = 203,
        NET_CLSV_SVCL_PLAYER_COORD_FRAME = 204,
        NET_CLSV_SVCL_PLAYER_CHANGE_NAME = 205,
        NET_CLSV_SVCL_PLAYER_PROJECTILE = 206,
        NET_CLSV_SVCL_PLAYER_SHOOT_MELEE = 207,
        NET_CLSV_SVCL_VOTE_REQUEST = 208,
        NET_CLSV_MAP_REQUEST = 209,
        NET_SVCL_MAP_CHUNK = 210,
        NET_SVCL_MAP_LIST = 211,
        NET_CLSV_SVCL_PLAYER_UPDATE_SKIN = 212,
		//213 sends by pro client as first packet, unknown

        NET_CLSV_BROADCAST_QUERY = 301,
        NET_CLSV_BROADCAST_GAME_INFO = 302,
        NET_SVCL_HASH_SEED = 404,
        NET_SVCL_HASH_SEED_REPLY = 405,
		NET_SVCL_CREATE_MINIBOT = 1001,
		NET_SVCL_MINIBOT_COORD_FRAME = 1002
    };

    // Quand le client recois un ping, il renvoit un pong
    public struct net_clsv_pong
    {
        public byte playerID; // Le ID du joueur concernй
                              //	public byte[] bidon = new byte[31];
    };

    // Le client est pret а spawner (apparaitre)
    // Il va envoyer une request au server, et ce dernier va dйcider OЩ il spawn

    public struct net_clsv_spawn_request
    {
        public byte playerID; // Le ID du joueur concernй
        public byte weaponID; // Le ID du gun avec lequel spawner
        public byte meleeID;

        // Skin info
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] skin;

        //--- Les couleurs custom du babo
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] redDecal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] greenDecal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] blueDecal;

        public net_clsv_spawn_request(bool init)
        {
            playerID = 0;
            weaponID = 0;
            meleeID = 0;
            skin = new byte[7];
            redDecal = new byte[3];
            greenDecal = new byte[3];
            blueDecal = new byte[3];
        }
    };

    // Le client tire du fusil (activitй commune chez les cocassien)
    // Le server et les clients connaissent le fusil utilisй par le joueur, donc pas besoin de l'envoyer

    public struct net_clsv_player_shoot
    {
        public byte playerID; // Le ID du joueur
        public byte weaponID; // Le ID du type de gun
        public byte nuzzleID; // le ID du nuzzle du fusil qui l'a tirй
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] p1; // Le point du dйbut du ray
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] p2; // Le point de la fin du ray
    };


    public struct net_clsv_gameversion_accepted
    {
        public byte playerID;      // Le ID du joueur
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] password;  // Password

        public net_clsv_gameversion_accepted(bool init)
        {
            playerID = 0;
            password = new byte[16];
        }
    };


    public struct net_clsv_pickup_request
    {
        public byte playerID; // Le ID du joueur en question
    };


    public struct net_clsv_admin_request
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
        public byte[] login;     //md5
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
        public byte[] password;  //md5
    };

    public struct net_clsv_vote
    {
        public bool value;
        public byte playerID;
    };


    public struct net_clsv_map_list_request
    {
        public byte playerID;
        public bool all;
    };


    public struct net_svcl_newplayer
    {
        public byte newPlayerID; // Le ID du nouveau Joueur (de 0 а 31)
        public Int32 baboNetID;
    };


    public struct net_svcl_server_info
    {
        public Int32 mapSeed; // Le seed de la map, pour le random
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] mapName; // 15 + '\0'

        // Le type de parti
        public byte gameType;

        // Les score
        public short blueScore;
        public short redScore;
        public short blueWin;
        public short redWin;
    };


    public struct net_svcl_player_disconnect
    {
        public byte playerID; // Le ID du joueur
    };

    public struct net_svcl_player_enum_state
    {
        public byte playerID; // Le ID du joueur
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] playerName; // Le nom du joueur, 31 + \0 caractиres
        public byte teamID; // Son team
        public byte status; // Son status
        public short kills;
        public short deaths;
        public short score; // Son score
        public short returns;
        public short flagAttempts;
        public short damage;
        public float life; // Sa vie
        public float dmg;
        public byte weaponID; // Le gun qu'il a
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] playerIP;
        public Int32 babonetID;

        // Skin info
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] skin;

        //--- Les couleurs custom du babo
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] redDecal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] greenDecal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] blueDecal;
    };


    public struct net_svcl_ping
    {
        public byte playerID; // Le ID du joueur concervй
                              //	public byte bidon[31];
    };


    public struct net_svcl_player_ping
    {
        public byte playerID; // Le ID du joueur
        public short ping; // Son ping avec le server, en miliseconde
    };


    public struct net_svcl_player_spawn
    {
        public byte playerID; // Le ID du joueur
        public byte weaponID; // Le ID du gun avec lequel spawner
        public byte meleeID; // Le ID du melee gun avec lequel spawner
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] position; // La position oщ il spawn

        // Skin info
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] skin;

        //--- Les couleurs custom du babo
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] redDecal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] greenDecal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] blueDecal;
    };


    public struct net_svcl_sv_change
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public byte[] svChange; // 80 c'est assez
    };


    public struct net_svcl_player_shoot
    {
        public byte playerID; // Le ID du joueur qui l'a tirй
        public byte hitPlayerID; // Le ID du joueur qu'on a touchй, si -1 on a touchй un mur
        public byte nuzzleID; // le ID du nuzzle du fusil qui l'a tirй (pour savoir oщ spawner le feu)
        public byte weaponID; // Le ID du type de gun
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] p1; // Le point du dйbut du ray
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] p2; // Le point de la fin du ray (point d'impact)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] normal; // La normal de l'impact
    };


    public struct net_svcl_delete_projectile
    {
        public Int32 projectileID; // Son ID dans le vector
    };


    public struct net_svcl_projectile_coord_frame
    {
        public Int32 uniqueID; // Le ID unique du projectile
        public short projectileID; // Le ID du projectile concernй
        public Int32 frameID; // Le frame auquel зa a йtй envoyй (on en a de besoin pour crйer des belles interpolations)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] position; // Sa position
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] vel; // Sa velocity
                           //	public Int32 uniqueProjectileID;
                           // Sa rotation sur l'axe est calculй cфtй client, vu que c uniquement visuel
    };


    public struct net_svcl_explosion
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] position; // La position de l'explosion dans map
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] normal; // L'orientation de l'explosion
        public float radius; // Sa puissance !! (зa va aussi faire shaker la vue :P)
        public byte playerID;
        // On ne dit pas qui l'a provoquй et tout, c'est le server qui va faire les hits
    };


    public struct net_svcl_player_hit
    {
        public byte playerID; // Le joueur touchй
        public byte fromID; // De qui зa vient
        public byte weaponID; // Le type d'arme utilisй
        public float damage; // ne pas oublier le damage infligй ! ** New, la vie restante **
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] vel; // La velocity qu'on recoit par le coup
    };


    public struct net_svcl_play_sound
    {
        public byte soundID;
        public byte volume;
        public byte range;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] position;
    };


    public struct net_svcl_gameversion
    {
        public UInt32 gameVersion;
    };


    public struct net_svcl_synchronize_timer
    {
        public Int32 frameID;
        public float gameTimeLeft;
        public float roundTimeLeft;
    };


    public struct net_svcl_change_flag_state
    {
        public byte flagID; // 0 ou 1
        public sbyte newFlagState; // Son nouvel йtat
        public byte playerID; // Le ID du player qui a effectuй l'action
    };


    public struct net_svcl_drop_flag
    {
        public byte flagID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] position;
    };


    public struct net_svcl_flag_enum
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public sbyte[] flagState;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] positionBlue;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] positionRed;
    };


    public struct net_svcl_game_state
    {
        public sbyte newState;
        public byte reInit; // Pour restarter le round а neuf ou en parti (trace de sang, vie, etc)
    };


    public struct net_svcl_change_game_type
    {
        public byte newGameType;
    };


    public struct net_svcl_map_change
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] mapName; // 15 + '\0'
        public byte gameType; // Le type dla game
    };


    public struct net_svcl_pickup_item
    {
        public byte playerID;
        public byte itemType;
        public byte itemFlag;
    };


    public struct net_svcl_flame_stick_to_player
    {
        public short projectileID; // Le ID unique du projectile
        public byte playerID; // Le ID du joueur sur qui зa stick
    };


    public struct net_svcl_update_vote
    {
        public byte nbYes;
        public byte nbNo;
    };

    // Le server shoot le rйsultat des votes

    public struct net_svcl_vote_result
    {
        public bool passed;
    };


    public struct net_svcl_msg
    {
        public byte msgDest; // where msg should be displayed, 0 - chat
        public byte teamID; // -2 - all, -1 - spectators, 1 - blue, 2 - red
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 130)]
        public byte[] message; // Null terminated string. De 79 + \0 caractиres
    };


    public struct net_svcl_player_update_stats
    {
        public byte playerID;
        public short kills;
        public short deaths;
        public short score; // Son score
        public short returns;
        public short flagAttempts;
        public float timePlayedCurGame;
    };


    public struct net_svcl_bad_checksum_entity
    {
        public int id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] playerIP;
    };


    public struct net_svcl_bad_checksum_info
    {
        public int number;
    };


    // Le client recois son ID, il envoit ses info (player name, etc), 
    // et le server le renvois aux autres
    [StructLayout(LayoutKind.Sequential)]
    public struct net_clsv_svcl_player_info
    {
        public byte playerID; // Le ID du joueur
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] playerIP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] playerName; // Le nom du joueur, 31 + \0 caractиres
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
        public byte[] username;      // Account username
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] password;      // Account password (MD5)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] macAddr; // player's mac adress, mouhouha

        public net_clsv_svcl_player_info(bool init)
        {
            playerID = 0;
            playerIP = new byte[16];
            playerName = new byte[32];
            username = new byte[21];
            password = new byte[32];
            macAddr = new byte[20];
        }
    };

    // Le client йcris un message, l'envoit au server

    public struct net_clsv_svcl_chat
    {
        public sbyte teamID; // -1 for all, >= 0 for team ID
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 130)]
        public byte[] message; // Null terminated string. De 79 + \0 caractиres
    };

    // Le client veux changer de team, il le demande d'abords au server

    public struct net_clsv_svcl_team_request
    {
        public byte playerID; // Son ID
        public sbyte teamRequested; // L'etat quil demande
    };

    // La position du joueur

    public struct net_clsv_svcl_player_coord_frame
    {
        public byte playerID; // Le ID du joueur concernй
        public Int32 frameID; // Le frame auquel зa a йtй envoyй (on en a de besoin pour crйer des belles interpolations)
                             //	public float angle; // Par oщ il regarde
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] position; // Sa position
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] vel; // Sa velocity
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] mousePos; // La position oщ il vise
        public Int32 babonetID;
        public int camPosZ;
        // Son orientation sera calculй client side, vu que c pas full important c une boule
    };

    //--- mini bot creation

    public struct net_svcl_create_minibot
    {
        public byte playerID; //--- Player ID owning that bot
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] position; //--- Bot position
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] mousePos; //--- Where it aims
    };


    public struct net_svcl_minibot_coord_frame
    {
        public byte playerID; // Le ID du joueur concernй
        public Int32 frameID; // Le frame auquel зa a йtй envoyй (on en a de besoin pour crйer des belles interpolations)
                             //	public float angle; // Par oщ il regarde
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] position; // Sa position
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] vel; // Sa velocity
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] mousePos; // La position oщ il vise
        public Int32 babonetID; // ?? don't need that
    };


    // On change le nom du joueur pendant le round (devra attendre la fin, ou au prochain round)
    // Techniquement зa ne devrait pas кtre allowй, mais зa va кtre hot

    public struct net_clsv_svcl_player_change_name
    {
        public byte playerID; // Le ID du joueur
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] playerName; // Le nom du joueur, 31 + \0 caractиres
    };

    // Le player shoot un projectile, on demande au server de crйer l'instance

    public struct net_clsv_svcl_player_projectile
    {
        public byte playerID; // Le ID du joueur
        public byte weaponID; // Le ID du type de gun qui a shootй le projectile
        public byte nuzzleID; // Le ID du nuzzle du fusil qui a l'a tirй
        public byte projectileType; // Le type du projectile
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] position; // La position initial du projectile
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public sbyte[] vel; // La velocitйe initial du projectile
        public Int32 uniqueID;
        //	public Int32 uniqueProjectileID;
    };

    // Le player shoot avec son melee

    public struct net_clsv_svcl_player_shoot_melee
    {
        public byte playerID;
    };

    // On request un vote

    public struct net_clsv_svcl_vote_request
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public byte[] vote; // La commande
        public byte playerID; // Le player ID
    };

    // On request map

    public struct net_clsv_map_request
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] mapName; // 15 + '\0'
        public UInt32 uniqueClientID;

        public net_clsv_map_request(bool init)
        {
            mapName = new byte[16];
            uniqueClientID = 0;
        }
    };

    // On request map

    public struct net_svcl_map_chunk
    {
        public ushort size;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 250)]
        public byte[] data; //250 bytes chunks
    };

    // On map list request

    public struct net_svcl_map_list
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] mapName; // 15 + '\0'
    };

    // Le player shoot avec son melee

    public struct net_clsv_svcl_player_update_skin
    {
        public byte playerID;

        // Skin info
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] skin;

        //--- Les couleurs custom du babo
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] redDecal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] greenDecal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] blueDecal;
    };

    // BROADCAST MESSAGE

    public struct net_clsv_broadcast_query
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] key;   //unique bv2 key for broadcasting
    };

    public struct net_svcl_broadcast_game_info
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] key;   //unique bv2 key for broadcasting
        public bv2_row gameInfo;
    };

    public struct net_svcl_hash_seed
    {
        public short s1;
        public short s2;
        public short s3;
        public short s4;
    };
    public struct net_svcl_hash_seed_reply //the same as net_svcl_hash_seed. it seems C# can't do it for me in other way
    {
        public short s1;
        public short s2;
        public short s3;
        public short s4;
    };
}
