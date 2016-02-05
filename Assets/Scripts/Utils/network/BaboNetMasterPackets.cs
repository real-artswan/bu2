using System.Runtime.InteropServices;

namespace BaboNetwork 
{
    public enum BaboMasterPacketTypeID
    {
        BV2_ROW = 997,      //ce que le master retourne pour BV2
        BV2_LIST = 999,		//un client nous demande la liste des game de BV2
        MASTER_INFO = 1002 //packet qui va contenir le nombre denregistrement retourner par la derniere requete
    }

    //this is the struct used to query the current game list
    //only client version is needed
    //struct typeID 999
    public struct bv2_list
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        byte[] Version ; //exemple :  2.02  + '\0'
    };

    //struct received by master server after a query
    //if NbGames == -1 : Wrong version  ... else : Number of games
    //struct typeID 1002
    public struct master_info
    {
        short NbGames;
    };

    //Current Game Infos
    //struct typeID 997		//total bytes : 132
    public struct bv2_row
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        byte[] map ; // 16 + '\0'				//TEXT
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        byte[] serverName ; // 63 + '\0'			//TEXT
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        byte[] password ; // 15 + '\0'			//TEXT
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        byte[] ip ;                        //TEXT		lors d'un update vers le master, IP peut etre vide, le master va catcher le IP anyway en pognant le packet
        ushort port;                        //NUMERIC	
        byte nbPlayer;                  //NUMERIC
        byte maxPlayer;                 //NUMERIC
        ushort flags;                       //NUMERIC
        byte gameType;                  //NUMERIC
        ushort ServerID;                    //if server is REGISTERED, put ID here, else, leave to 0
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        byte[] Version ;                    //version of server
        byte Priority;                  //master server usage only, leave at 0
        ushort DBVersion;
        ushort Padding;
    };
}
