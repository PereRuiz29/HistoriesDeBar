using System.IO;
using UnityEditor;
using UnityEngine;

namespace SpriteImporter
{
    public class TexturePostprocessor : AssetPostprocessor
    {
        private const string EXTENSION_PNG = ".png";
        private const string PIXEL_MAP_SUFFIX = ".map";        //overlay map
        private const string PIXEL_SOURCE_PREFIX = "source."; //overlay animation frame
        private const string NORMAL_PREFIX = "normal."; //normal animation frame
        private const string FINAL_PREFIX = "final."; //normal animation frame


        //With the overlay animation and the overlay map create the source animation

        private const string SECONDARY_TEXTURE_NAME_NORMAL = "_NormalMap";


        private const string PixelMapFolder = "OverlayMap";
        private const string PixelAnimationFolder = "OverlayAnimation";

        private void OnPostprocessTexture(Texture2D texture)
        {
            var fileName = Path.GetFileNameWithoutExtension(assetPath);
            var extension = Path.GetExtension(assetPath);

            if (extension != EXTENSION_PNG)
                return;

            if (fileName.EndsWith(PIXEL_MAP_SUFFIX))
                ProcessPixelMap(texture, fileName);
            else if (fileName.StartsWith(PIXEL_SOURCE_PREFIX))
                ProcessPixelSource(texture, fileName.Replace(PIXEL_SOURCE_PREFIX, ""));
            else if (fileName.StartsWith(FINAL_PREFIX))
                SetNormalMap(texture, fileName.Replace(FINAL_PREFIX, ""));
        }

        private void ProcessPixelSource(Texture2D texture, string fileName)
        {
            if (!fileName.Contains('_'))
                return;

            string mapName = fileName.Split('_')[0] + PIXEL_MAP_SUFFIX;
            PixelMap map = FindPixelMap(mapName);
            if (map == null)
                return;

            Color32[] skinData = texture.GetPixels32();
            for (var i = 0; i < skinData.Length; i++)
            {
                if (map.lookup.TryGetValue(skinData[i], out var position))
                {
                    skinData[i].r = (byte)position.x;
                    skinData[i].g = (byte)position.y;
                    skinData[i].b = 0;
                    skinData[i].a = 255;
                }
                else
                {
                    skinData[i] = Color.clear;
                }
            }



            //SearchFolder
            string folderPath = FindFolderPathByName(PixelAnimationFolder);
            if (folderPath == null)
                Debug.LogError("Create the folder: Code must be implemented");

            fileName = FINAL_PREFIX + fileName;
            string path = Path.Combine(folderPath, $"{fileName}.asset");
            path = Path.ChangeExtension(path, EXTENSION_PNG);

            Texture2D skinTexture = new Texture2D(texture.width, texture.height);
            skinTexture.SetPixels32(skinData);
            //Create a new file, if it's already existing, overwrite it.
            File.WriteAllBytes(path, skinTexture.EncodeToPNG()); //encode a texture into a png
            AssetDatabase.ImportAsset(path);


            // Reimport the asset to apply the changes immediately. Apply from "outside" the asset to avoid entering on a loop
            EditorApplication.delayCall += () =>
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            };
        }

        //Find the normal map for the texture and apply it as a secondary texture.
        private void SetNormalMap(Texture2D texture, string fileName)
        {
            //Create TextureImporter and set values
            TextureImporter importer = assetImporter as TextureImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 16;
            importer.filterMode = FilterMode.Point;
            //importer.textureType = TextureImporterType.Default;
            importer.GetDefaultPlatformTextureSettings().format = TextureImporterFormat.RGBA32;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            //find Normal Map
            string normalMapName = NORMAL_PREFIX + fileName;
            Texture2D normalMapTexture = FindTexture2DByName(normalMapName);
            if (normalMapTexture != null)  //If Normal map not found
            {
                //Create secundary texture
                SecondarySpriteTexture[] secondarySpriteTextures = new SecondarySpriteTexture[] {
                    new SecondarySpriteTexture {
                        name = SECONDARY_TEXTURE_NAME_NORMAL,
                        texture = normalMapTexture
                    }
                };

                importer.secondarySpriteTextures = secondarySpriteTextures;
            }
            else
                Debug.LogError("Normal Map NOT FOUND for the Texture: " + normalMapName);

        }

        //Process overlay map and save his values in a scriptable object
        private void ProcessPixelMap(Texture2D texture, string fileName)
        {
            var map = FindPixelMap(fileName);
            if (map == null)
            {
                //Create intance of the scriptableObject PixelMap(overlay map)
                map = ScriptableObject.CreateInstance<PixelMap>();
                map.name = fileName;

                string folderPath = FindFolderPathByName(PixelMapFolder);
                if (folderPath == null)
                    Debug.LogError("Create the folder: Code must be implemented");

                AssetDatabase.CreateAsset(
                    map,
                    //Path.Combine(folderPath, $"{fileName}.asset")

                    //asset path is the path name of the asset being imported.
                    Path.Combine(Path.GetDirectoryName(assetPath), $"{fileName}.asset")
                );
            }

            map.data = texture.GetPixels32();
            map.lookup.Clear();
            for (var i = 0; i < map.data.Length; i++)
                if (map.data[i].a > 0)
                {
                    map.lookup[map.data[i]] = new Vector2Int(
                        i % texture.width,
                        i / texture.width
                    );
                }

            EditorUtility.SetDirty(map);
            AssetDatabase.SaveAssets();
        }

        //Find pixel map 
        private static PixelMap FindPixelMap(string fileName)
        {
            var guids = AssetDatabase.FindAssets("t:" + nameof(PixelMap));

            foreach (var guid in guids)
            {
                var asset = AssetDatabase.LoadMainAssetAtPath(
                    AssetDatabase.GUIDToAssetPath(guid)
                ) as PixelMap;

                if (asset != null && asset.name == fileName)
                    return asset;
            }

            return null;
        }

        //Find folder path, return null if fail
        private string FindFolderPathByName(string folderName)
        {
            //array of global unike identifiers      filter to just folders
            string[] guids = AssetDatabase.FindAssets("t:Folder " + folderName);

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return path;
            }

            return null;
        }

        //Find Texture2D, return null if fail
        private Texture2D FindTexture2DByName(string textureName)
        {
            //array of global unike identifiers      filter to just folders
            string[] guids = AssetDatabase.FindAssets("t:Texture2D " + textureName);

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Debug.Log("path: " + path);
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }

            Debug.LogError("guids.Length: " + guids.Length);
            Debug.LogError("More than 1 Texture2D found with the name: " + textureName);
            return null;
        }
    }
}
