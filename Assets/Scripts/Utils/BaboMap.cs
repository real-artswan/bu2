using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Utils
{
    public struct BaboMapCell
    {
        public bool passable;
        public float[] splater; //must be 4 elems
        public int height;
    }

    public class BaboMap
    {
        public ulong mapVersion = 0;
        public short width, height = 0;
        public BaboMapCell[] cells = null;
        public Vector3 blueFlagPodPos, redFlagPodPos;
        public Vector3 blueObjective, redObjective;
        public List<Vector3> dm_spawns = new List<Vector3>();
        public List<Vector3> blue_spawns = new List<Vector3>();
        public List<Vector3> red_spawns = new List<Vector3>();
        public short theme = 0;
        public short weather = 0;

        private byte[] author_name_buff = new byte[] { }; //25 bytes max
        public String author_name
        {
            get { return Encoding.ASCII.GetString(author_name_buff); }
        }

        //To add dirt
        private void setTileDirt(int x, int y, float value)
        {
            if (x < 0) return;
            if (y < 0) return;
            if (x >= width) return;
            if (y >= height) return;

            cells[y * width + x].splater = new float[4];

            cells[y * width + x].splater[1] = value;

            // My neighbors who know what vertex
            if (x > 0)
            {
                cells[y * width + x - 1].splater[2] = cells[y * width + x].splater[1];
                if (y > 0)
                {
                    cells[(y - 1) * width + x - 1].splater[3] = cells[y * width + x].splater[1];
                }
            }
            if (y > 0)
            {
                cells[(y - 1) * width + x].splater[0] = cells[y * width + x].splater[1];
            }
        }

        private void readCellsData(BinaryReader reader)
        {
            width = reader.ReadInt16();
            height = reader.ReadInt16();
            cells = new BaboMapCell[width * height];
            for (int j = 0; j < height; ++j)
            {
                for (int i = 0; i < width; ++i)
                {
                    byte data = reader.ReadByte();
                    cells[j * width + i].passable = (data & 128) != 0;
                    cells[j * width + i].height = (data & 127);
                    data = reader.ReadByte();
                    setTileDirt(i, j, ((float)data) / 255.0f);
                }
            }
        }

        private Vector3 readVector3(BinaryReader reader)
        {
            float x, y, z;
            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

		public BaboMap(Stream mapStream)
        {
			BinaryReader reader = new BinaryReader(mapStream);
            mapVersion = reader.ReadUInt32();
            Debug.Log(String.Format("Map version: {0}", mapVersion));
            switch (mapVersion)
            {
                case 10010:
                    {
                        readCellsData(reader);
                        break;
                    }
                case 10011:
                    {
                        readCellsData(reader);

                        // Les flag
                        blueFlagPodPos = readVector3(reader);
                        redFlagPodPos = readVector3(reader);

                        // Les ojectifs
                        blueObjective = readVector3(reader);
                        redObjective = readVector3(reader);

                        // Les spawn point
                        short nbSpawn = reader.ReadInt16();
                        for (int i = 0; i < nbSpawn; ++i)
                        {
                            dm_spawns.Add(readVector3(reader));
                        }
                        nbSpawn = reader.ReadInt16();
                        for (int i = 0; i < nbSpawn; ++i)
                        {
                            blue_spawns.Add(readVector3(reader));
                        }
                        nbSpawn = reader.ReadInt16();
                        for (int i = 0; i < nbSpawn; ++i)
                        {
                            red_spawns.Add(readVector3(reader));
                        }
                        break;
                    }
                case 20201:
                    {
                        theme = reader.ReadInt16();
                        weather = reader.ReadInt16();

                        readCellsData(reader);

                        // Les flag
                        blueFlagPodPos = readVector3(reader);
                        redFlagPodPos = readVector3(reader);

                        // Les ojectifs
                        blueObjective = readVector3(reader);
                        redObjective = readVector3(reader);

                        // Les spawn point
                        short nbSpawn = reader.ReadInt16();
                        for (int i = 0; i < nbSpawn; ++i)
                        {
                            dm_spawns.Add(readVector3(reader));
                        }
                        nbSpawn = reader.ReadInt16();
                        for (int i = 0; i < nbSpawn; ++i)
                        {
                            blue_spawns.Add(readVector3(reader));
                        }
                        nbSpawn = reader.ReadInt16();
                        for (int i = 0; i < nbSpawn; ++i)
                        {
                            red_spawns.Add(readVector3(reader));
                        }
                        break;
                    }
                case 20202:
                    {
                        author_name_buff = reader.ReadBytes(25);

                        // Note: we DO NOT want to overwrite the author field if it's being edited
                        theme = reader.ReadInt16();
                        weather = reader.ReadInt16();

                        readCellsData(reader);
                        // common spawns
                        short nbSpawn = reader.ReadInt16();
                        for (int i = 0; i < nbSpawn; ++i)
                        {
                            dm_spawns.Add(readVector3(reader));
                        }

                        // read game-type specific data
                        // there must always be one game-type specific section per one supported game type
						int enumsCount = Enum.GetValues(typeof(BaboGameType)).Length - 1;
						for (int j = 0; j < enumsCount; j++)
                        {
                            short id = reader.ReadInt16();
                            switch ((BaboGameType)id)
                            {
                                case BaboGameType.GAME_TYPE_DM:
                                case BaboGameType.GAME_TYPE_TDM:
                                    break; // nothing to do for DM and TDM
                                case BaboGameType.GAME_TYPE_CTF:
                                    {
                                        // flags
                                        blueFlagPodPos = readVector3(reader);
                                        redFlagPodPos = readVector3(reader);
                                    }
                                    break;
                                case BaboGameType.GAME_TYPE_SND:
                                    {
                                        // bombs
                                        blueObjective = readVector3(reader);
                                        redObjective = readVector3(reader);
                                        // team-spawns
                                        nbSpawn = reader.ReadInt16();
                                        for (int i = 0; i < nbSpawn; ++i)
                                        {
                                            blue_spawns.Add(readVector3(reader));
                                        }
                                        nbSpawn = reader.ReadInt16();
                                        for (int i = 0; i < nbSpawn; ++i)
                                        {
                                            red_spawns.Add(readVector3(reader));
                                        }
                                    }
                                    break;
                                default:
                                    //console->add(CString("\x3> Error: unknown game-type id found in map-file, the value is %d", id));
                                    break;
                            }
                        }
                        break;
                    }
            }
            Debug.Log(String.Format("Map size: {0}x{1}", width, height));
        }
    }
	public enum BaboMapThemes
	{
		// Classic themes
		THEME_GRASS,
		THEME_SNOW,
		THEME_SAND,
		THEME_CITY,
		THEME_MODERN,
		THEME_LAVA,
		THEME_ANIMAL,
		THEME_ORANGE,
		// Pacifist's themes (WOW!)
		THEME_CORE,
		THEME_FROZEN,
		THEME_GRAIN,
		THEME_MEDIEVAL,
		THEME_METAL,
		THEME_RAINY,
		THEME_REAL,
		THEME_ROAD,
		THEME_ROCK,
		THEME_SAVANA,
		THEME_SOFT,
		THEME_STREET,
		THEME_TROPICAL,
		THEME_WINTER,
		THEME_WOODEN
	}
}