using System;
using UnityEngine;

namespace Utils
{
    public class MapConverter
    {
        public static void createMinimap(BaboMap baboMap, int scaleFactor, ref Material minimap) {
            //prepare minimap
            Texture2D minimapTex = new Texture2D(baboMap.width * scaleFactor, baboMap.height * scaleFactor, TextureFormat.ARGB32, false);
            minimap.mainTexture = minimapTex;
            //transparent background
            Color[] clear = new Color[minimapTex.width * minimapTex.height];
            Color cleanColor = new Color(0, 0, 0, 0);
            int i;
            for (i = 0; i < clear.Length; i++)
                clear[i] = cleanColor;
            minimapTex.SetPixels(clear);

            //prepare wall pixels block
            Color[] pixelsBlock = new Color[scaleFactor * scaleFactor];
            for (i = 0; i < pixelsBlock.Length; i++)
                pixelsBlock[i] = minimap.color;

            //draw walls
            for (int hInd = 0; hInd < baboMap.height; hInd++) {
                for (int wInd = 0; wInd < baboMap.width; wInd++) {
                    if (!baboMap.cells[hInd * baboMap.width + wInd].passable) {
                        minimapTex.SetPixels(wInd * scaleFactor, hInd * scaleFactor, scaleFactor, scaleFactor, pixelsBlock);
                    }
                }
            }

            /*//prepare blue pod pixels block
            pixelsBlock = new Color[scaleFactor * scaleFactor];
            for (i = 0; i < pixelsBlock.Length; i++)
                pixelsBlock[i] = Color.blue;

            minimapTex.SetPixels((int)baboMap.blueFlagPodPos.x * scaleFactor, (int)baboMap.blueFlagPodPos.y * scaleFactor,
                scaleFactor, scaleFactor, pixelsBlock);

            //prepare red pod pixels block
            pixelsBlock = new Color[scaleFactor * scaleFactor];
            for (i = 0; i < pixelsBlock.Length; i++)
                pixelsBlock[i] = Color.red;

            minimapTex.SetPixels((int)baboMap.redFlagPodPos.x * scaleFactor, (int)baboMap.redFlagPodPos.y * scaleFactor,
                scaleFactor, scaleFactor, pixelsBlock);*/

            minimapTex.Apply(false);
        }

        public static void CreateUnityMap(BaboMap baboMap, Map mapObject) {
            //scale floor
            mapObject.floor.transform.localScale = new Vector3((float)baboMap.width / 10f, 1, (float)baboMap.height / 10f);
            //load texture
            Renderer rend = mapObject.floor.GetComponent<Renderer>();
            Texture2D tex = Resources.Load<Texture2D>(String.Format("textures/themes/{0}/tex_floor", baboMap.theme.ToString().ToLower()));
            if (tex == null)
                tex = Resources.Load<Texture2D>("textures/grass");
            Material tmpMat = new Material(rend.material);
            tmpMat.name = "floor material";
            tmpMat.mainTexture = tex;
            tmpMat.mainTextureScale = new Vector2(baboMap.width, baboMap.height);
            rend.material = tmpMat;

            //create walls
            GameObject wallModel = Resources.Load<GameObject>("models/Wall");
            //TODO: load wall texture from map theme
            tex = Resources.Load<Texture2D>(String.Format("textures/themes/{0}/tex_wall_center", baboMap.theme.ToString().ToLower()));
            if (tex == null)
                tex = Resources.Load<Texture2D>("textures/dirt2");

            mapObject.wShift = (float)baboMap.width / 2f;
            mapObject.hShift = (float)baboMap.height / 2f;
            float wShift = baboMap.width / 2f - 0.5f;
            float hShift = baboMap.height / 2f - 0.5f;
            for (int hInd = 0; hInd < baboMap.height; hInd++) {
                for (int wInd = 0; wInd < baboMap.width; wInd++) {
                    BaboMapCell cell = baboMap.cells[hInd * baboMap.width + wInd];
                    if (!cell.passable) {
                        GameObject wall = MonoBehaviour.Instantiate(wallModel);
                        wall.tag = "Wall";
                        wall.transform.parent = mapObject.wallsParent;
                        wall.transform.position = new Vector3(wInd - wShift, (float)cell.height / 2f, hInd - hShift);
                        wall.transform.localScale = new Vector3(1, cell.height, 1);
                        //texture
                        rend = wall.GetComponent<Renderer>();
                        tmpMat = new Material(rend.material);
                        tmpMat.mainTexture = tex;
                        tmpMat.mainTextureScale = new Vector2(1, cell.height);
                        rend.material = tmpMat;
                    }
                }
            }
            //create spawns
            int t = 0;
            foreach (Vector3 spawn in baboMap.dm_spawns) {
                string spawnTag = "Respawn";
                GameObject spawnObject = new GameObject();

                spawnObject.transform.rotation = Quaternion.Euler(90, 0, 0);
                spawnObject.transform.position = new Vector3(spawn.x - wShift - 0.5f, spawn.z, spawn.y - hShift - 0.5f);
                spawnObject.name = String.Format("{0}_{1}", spawnTag, t++);
                spawnObject.transform.parent = mapObject.spawnsParent;

                spawnObject.tag = spawnTag;
            }
            //flags
            Material blueMat = Resources.Load<Material>("Materials/BlueTeam");
            Material redMat = Resources.Load<Material>("Materials/RedTeam");
            GameObject flagPodModel = Resources.Load<GameObject>("models/FlagPod");

            Vector3 p = new Vector3(baboMap.blueFlagPodPos.x - wShift - 0.5f, baboMap.blueFlagPodPos.z, baboMap.blueFlagPodPos.y - hShift - 0.5f);
            mapObject.blueFlagPod = GameObject.Instantiate(flagPodModel, p, Quaternion.identity) as GameObject;
            mapObject.blueFlagPod.transform.parent = mapObject.gameObject.transform;
            mapObject.blueFlagPod.name = "BlueFlagPod";
            GameObject colorMark = mapObject.blueFlagPod.transform.FindChild("ColorMark").gameObject;
            Renderer r = colorMark.GetComponent<Renderer>();
            r.material = blueMat;

            p = new Vector3(baboMap.redFlagPodPos.x - wShift - 0.5f, baboMap.redFlagPodPos.z, baboMap.redFlagPodPos.y - hShift - 0.5f);
            mapObject.redFlagPod = GameObject.Instantiate(flagPodModel, p, Quaternion.identity) as GameObject;
            mapObject.redFlagPod.transform.parent = mapObject.gameObject.transform;
            mapObject.redFlagPod.name = "RedFlagPod";
            colorMark = mapObject.redFlagPod.transform.FindChild("ColorMark").gameObject;
            r = colorMark.GetComponent<Renderer>();
            r.material = redMat;

            //objectives
            p = new Vector3(baboMap.blueObjective.x - wShift - 0.5f, baboMap.blueObjective.z, baboMap.blueObjective.y - hShift - 0.5f);
            GameObject obj = new GameObject("BlueObjective");
            obj.transform.parent = mapObject.gameObject.transform;
            obj.transform.position = p;

            p = new Vector3(baboMap.redObjective.x - wShift - 0.5f, baboMap.redObjective.z, baboMap.redObjective.y - hShift - 0.5f);
            obj = new GameObject("RedObjective");
            obj.transform.parent = mapObject.gameObject.transform;
            obj.transform.position = p;
        }
    }
}