using System;
using UnityEngine;

namespace Utils
{
    public class MapConverter
    {
        public static void createMinimap(BaboMap baboMap, int scaleFactor, Material material) {
            //prepare minimap
            Texture2D minimapTex = new Texture2D(baboMap.width * scaleFactor,
                baboMap.height * scaleFactor, TextureFormat.ARGB32, false);

            //transparent background
            Color32[] clear = new Color32[minimapTex.width * minimapTex.height];
            Color32 cleanColor = new Color(0, 0, 0, 0);
            int i;
            for (i = 0; i < clear.Length; i++)
                clear[i] = cleanColor;
            minimapTex.SetPixels32(clear);

            //prepare wall pixels block
            Color32[] pixelsBlock = new Color32[scaleFactor * scaleFactor];
            for (i = 0; i < pixelsBlock.Length; i++)
                pixelsBlock[i] = material.color;

            //draw walls
            for (int hInd = 0; hInd < baboMap.height; hInd++) {
                for (int wInd = 0; wInd < baboMap.width; wInd++) {
                    if (!baboMap.cells[hInd * baboMap.width + wInd].passable) {
                        minimapTex.SetPixels32(wInd * scaleFactor, hInd * scaleFactor, scaleFactor, scaleFactor, pixelsBlock);
                    }
                }
            }

            minimapTex.Apply(false);
            material.mainTexture = minimapTex;
        }

        public static void CreateUnityMap(BaboMap baboMap, Map mapObject) {
            string themePath = mapObject.themesPath + baboMap.theme.ToString().ToLower() + '/';

            mapObject.wShift = (float)baboMap.width / 2f;
            mapObject.hShift = (float)baboMap.height / 2f;

            //mapObject.floor.transform.localScale = new Vector3((float)baboMap.width / 10f, 1, (float)baboMap.height / 10f);

            //generate floor
            Texture2D floorTex = Resources.Load<Texture2D>(themePath + "tex_floor");
            if (floorTex == null)
                floorTex = mapObject.defaultFloorTexture;
            Texture2D dirtTex = Resources.Load<Texture2D>(themePath + "tex_floor_dirt");
            if (dirtTex == null)
                dirtTex = mapObject.defaultDirtTexture;

            GameObject floor = new GameObject();
            floor.isStatic = true;
            floor.name = "Floor";
            BoxCollider floorCollider = floor.AddComponent<BoxCollider>();
            floorCollider.size = new Vector3(baboMap.width, 0.5f, baboMap.height);
            floorCollider.center = new Vector3(mapObject.wShift, -0.25f, mapObject.hShift);
            floor.transform.position = new Vector3(-mapObject.wShift, 0, -mapObject.hShift); //align center to zero point
            floor.transform.parent = mapObject.gameObject.transform;

            if ((mapObject.gameVars == null) || (mapObject.gameVars.splatterGround)) {
                Material foreMat = new Material(mapObject.floorForegroundMaterial);
                foreMat.name = "Floor foreground material";
                foreMat.mainTextureScale = new Vector2(0.5f, 0.5f);
                foreMat.mainTexture = floorTex;
                Material backMat = new Material(mapObject.floorBackgroundMaterial);
                backMat.name = "Floor background material";
                backMat.mainTextureScale = new Vector2(0.5f, 0.5f);
                backMat.mainTexture = dirtTex;

                GenerateGroundMesh(floor, new Material[] { backMat, foreMat }, baboMap);
            }
            else {
                Material backMat = new Material(mapObject.floorBackgroundMaterial);
                backMat.name = "Floor background material";
                backMat.mainTexture = floorTex;

                GenerateGroundMesh(floor, new Material[] { backMat }, baboMap);
            }
            //create walls
            Texture2D wallTex = Resources.Load<Texture2D>(themePath + "tex_wall_center");
            if (wallTex == null)
                wallTex = mapObject.defaultWallTexture;

            float wShiftWall = mapObject.wShift - 0.5f; //cache value
            float hShiftWall = mapObject.hShift - 0.5f; //cache value
            for (int hInd = 0; hInd < baboMap.height; hInd++) {
                for (int wInd = 0; wInd < baboMap.width; wInd++) {
                    BaboMapCell cell = baboMap.cells[hInd * baboMap.width + wInd];
                    if (!cell.passable) {
                        GameObject wall = UnityEngine.Object.Instantiate(mapObject.wallModel);
                        wall.tag = "Wall";
                        wall.transform.parent = mapObject.wallsParent;
                        wall.transform.position = new Vector3(wInd - wShiftWall, cell.height / 2f, hInd - hShiftWall);
                        wall.transform.localScale = new Vector3(1, cell.height, 1);
                        //texture (need new Material for each wall to be able to scale texture separately)
                        Renderer wallRender = wall.GetComponent<Renderer>();
                        //Material wallMat = new Material(wallRender.material);
                        Material wallMat = wallRender.material;
                        wallMat.mainTexture = wallTex;
                        wallMat.mainTextureScale = new Vector2(1, cell.height);
                        //wallRender.material = wallMat;
                    }
                }
            }
            //create spawns
            int t = 0;
            foreach (Vector3 spawn in baboMap.dm_spawns) {
                string spawnTag = "Respawn";
                GameObject spawnObject = new GameObject();

                spawnObject.transform.rotation = Quaternion.Euler(90, 0, 0);
                spawnObject.transform.position = new Vector3(spawn.x - wShiftWall - 0.5f, spawn.z, spawn.y - hShiftWall - 0.5f);
                spawnObject.name = String.Format("{0}_{1}", spawnTag, t++);
                spawnObject.transform.parent = mapObject.spawnsParent;

                spawnObject.tag = spawnTag;
            }
            //flags
            GameObject flagPodModel = mapObject.flagPodModel;

            Vector3 p = new Vector3(baboMap.blueFlagPodPos.x - wShiftWall - 0.5f, baboMap.blueFlagPodPos.z, baboMap.blueFlagPodPos.y - hShiftWall - 0.5f);
            mapObject.blueFlagPod = GameObject.Instantiate(flagPodModel, p, Quaternion.identity) as GameObject;
            mapObject.blueFlagPod.transform.parent = mapObject.gameObject.transform;
            mapObject.blueFlagPod.name = "BlueFlagPod";
            GameObject colorMark = mapObject.blueFlagPod.transform.FindChild("ColorMark").gameObject;
            Renderer r = colorMark.GetComponent<Renderer>();
            r.material = new Material(r.material);
            r.material.color = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_BLUE);

            p = new Vector3(baboMap.redFlagPodPos.x - wShiftWall - 0.5f, baboMap.redFlagPodPos.z, baboMap.redFlagPodPos.y - hShiftWall - 0.5f);
            mapObject.redFlagPod = GameObject.Instantiate(flagPodModel, p, Quaternion.identity) as GameObject;
            mapObject.redFlagPod.transform.parent = mapObject.gameObject.transform;
            mapObject.redFlagPod.name = "RedFlagPod";
            colorMark = mapObject.redFlagPod.transform.FindChild("ColorMark").gameObject;
            r = colorMark.GetComponent<Renderer>();
            r.material = new Material(r.material);
            r.material.color = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_RED);

            //objectives (??)
            p = new Vector3(baboMap.blueObjective.x - wShiftWall - 0.5f, baboMap.blueObjective.z, baboMap.blueObjective.y - hShiftWall - 0.5f);
            GameObject obj = new GameObject("BlueObjective");
            obj.transform.parent = mapObject.gameObject.transform;
            obj.transform.position = p;

            p = new Vector3(baboMap.redObjective.x - wShiftWall - 0.5f, baboMap.redObjective.z, baboMap.redObjective.y - hShiftWall - 0.5f);
            obj = new GameObject("RedObjective");
            obj.transform.parent = mapObject.gameObject.transform;
            obj.transform.position = p;
        }

        private static void GenerateGroundMesh(GameObject floor, Material[] materials, BaboMap baboMap) {
            MeshFilter mf = floor.AddComponent<MeshFilter>();
            Renderer r = floor.AddComponent<MeshRenderer>();
            mf.mesh.subMeshCount = materials.Length;
            r.materials = materials;

            var height = baboMap.height;
            var width = baboMap.width;
            // Build vertices and UVs
            int count = height * width;
            var vertices = new Vector3[count];
            var uv = new Vector2[count];
            Color32[] colors = new Color32[count];

            var tangents = new Vector4[count];

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    int ind = y * width + x;
                    var vertex = new Vector3(x, 0, y);
                    vertices[ind] = vertex;
                    uv[ind] = new Vector2(x, y);
                    colors[ind] = new Color32(255, 255, 255, (byte)(255 - baboMap.cells[ind].splater[1] * 255));
                    // Calculate tangent vector: a vector that goes from previous vertex
                    // to next along X direction. We need tangents if we intend to
                    // use bumpmap shaders on the mesh.
                    var vertexL = new Vector3(x - 1, 0, y);
                    var vertexR = new Vector3(x + 1, 0, y);
                    var tan = (vertexR - vertexL).normalized;
                    tangents[y * width + x] = new Vector4(tan.x, tan.y, tan.z, -1.0f);

                }
            }
            mf.mesh.vertices = vertices;
            mf.mesh.uv = uv;
            mf.mesh.colors32 = colors;

            var triangles = new int[(height - 1) * (width - 1) * 6];
            var index = 0;
            for (int y = 0; y < height - 1; y++) {
                for (int x = 0; x < width - 1; x++) {
                    // For each grid cell output two triangles
                    triangles[index++] = (y * width) + x;
                    triangles[index++] = ((y + 1) * width) + x;
                    triangles[index++] = (y * width) + x + 1;

                    triangles[index++] = ((y + 1) * width) + x;
                    triangles[index++] = ((y + 1) * width) + x + 1;
                    triangles[index++] = (y * width) + x + 1;
                }
            }
            // And assign them to the mesh
            for (int i = 0; i < materials.Length; i++) {
                mf.mesh.SetTriangles(triangles, i);
            }
            // Auto-calculate vertex normals from the mesh
            mf.mesh.RecalculateNormals();
        }
    }
}