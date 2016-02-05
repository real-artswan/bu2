using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Utils
{
	public class MapConverter
	{
		public static void CreateUnityMap(BaboMap baboMap, Map mapObject)
		{
            //scale floor
            mapObject.floor.transform.localScale = new Vector3((float)baboMap.width / 10f, 1, (float)baboMap.height / 10f);
			//load texture
			MeshRenderer rend = mapObject.floor.GetComponent<MeshRenderer>();
			//TODO: load texture from map theme
			Texture tex = Resources.Load("textures/themes/grass/tex_floor") as Texture;
			rend.material.mainTexture = tex;
			rend.material.mainTextureScale = new Vector2(baboMap.height, baboMap.width);

			//create walls
			GameObject wallModel = Resources.Load<GameObject>("models/Wall");
			//TODO: load wall texture from map theme
			Texture tex1 = Resources.Load("textures/themes/grass/tex_wall_center") as Texture;

			Transform wallsParent = mapObject.wallsParent.transform;
			float wShift = baboMap.width / 2 - 0.5f;
			float hShift = baboMap.height / 2 - 0.5f;
			for (int hInd = 0; hInd < baboMap.height; hInd++) {
				for (int wInd = 0; wInd < baboMap.width; wInd++) {
					BaboMapCell cell = baboMap.cells[hInd * baboMap.width + wInd];
					if (!cell.passable) {
						GameObject wall = MonoBehaviour.Instantiate(wallModel);
						wall.tag = "Wall";
						wall.transform.parent = wallsParent;
						wall.transform.position = new Vector3(wInd - wShift, (float)cell.height / 2f, hInd - hShift);
						wall.transform.localScale = new Vector3(1, cell.height, 1);
						//texture
						MeshRenderer rend1 = wall.GetComponent<MeshRenderer>();
						rend1.material.mainTexture = tex1;
						rend1.material.mainTextureScale = new Vector2(1, cell.height);
					}
				}
			}
			//MonoBehaviour.Destroy(wallModel);
			//create spawns
			int i = 0;
			foreach (Vector3 spawn in baboMap.dm_spawns) {
				string spawnTag = "Respawn";
				GameObject spawnObject = new GameObject();
				//GameObject spawnObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
				spawnObject.transform.rotation = Quaternion.Euler(90, 0, 0);
				spawnObject.transform.position = new Vector3(spawn.x - wShift - 0.5f, spawn.z, spawn.y - hShift - 0.5f);
				spawnObject.name = String.Format("{0}_{1}", spawnTag, i++);
				spawnObject.transform.parent = mapObject.gameObject.transform;

				spawnObject.tag = spawnTag;
			}
		}
	}
}