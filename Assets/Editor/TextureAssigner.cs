using UnityEngine;
using UnityEditor;
using System.IO;

// This script adds a new menu item to automatically assign textures to materials
// based on a common naming convention (e.g., from Sketchfab).
public class TextureAssigner : EditorWindow
{
    // Creates a new menu item in the Unity Editor under "Tools/Assign Textures"
    [MenuItem("Tools/Assign Textures to Selected Materials")]
    public static void AssignTextures()
    {
        // Get all the material files that are currently selected in the Project window.
        Object[] selectedMaterials = Selection.GetFiltered(typeof(Material), SelectionMode.Assets);

        if (selectedMaterials.Length == 0)
        {
            Debug.LogWarning("No materials selected! Please select one or more materials in the Project window.");
            return;
        }

        int materialsUpdated = 0;

        // Go through each selected material.
        foreach (Material mat in selectedMaterials)
        {
            // --- COMMON TEXTURE SUFFIXES ---
            // The script will look for textures with these names, based on the material's name.
            // For example, if the material is "Building_Facade", it will look for "Building_Facade_BaseColor.png", etc.
            string baseColorName = mat.name + "_BaseColor";
            string normalMapName = mat.name + "_Normal_OpenGL"; // Sketchfab often uses OpenGL normals
            string metallicName = mat.name + "_Metallic";
            string roughnessName = mat.name + "_Roughness";
            string occlusionName = mat.name + "_AO";

            // Find and assign each texture.
            bool updated = AssignTextureToSlot(mat, "_MainTex", baseColorName, true); // "_MainTex" is the internal name for Albedo
            updated |= AssignTextureToSlot(mat, "_BumpMap", normalMapName, true, true); // Normals need a special import setting
            updated |= AssignTextureToSlot(mat, "_MetallicGlossMap", metallicName, true);
            updated |= AssignTextureToSlot(mat, "_OcclusionMap", occlusionName, true);
            
            // Handle Roughness (since Unity uses Smoothness)
            Texture2D roughnessTex = FindTexture(roughnessName, true);
            if (roughnessTex != null)
            {
                mat.SetFloat("_Glossiness", 0.5f); // Set a default smoothness since we can't auto-convert
                Debug.Log($"Assigned Roughness for {mat.name}. Please adjust the 'Smoothness' slider manually.", mat);
                updated = true;
            }

            if (updated)
            {
                materialsUpdated++;
            }
        }

        Debug.Log($"Texture assignment complete! {materialsUpdated} out of {selectedMaterials.Length} materials were updated.");
    }

    // Helper function to find and assign a texture
    private static bool AssignTextureToSlot(Material mat, string shaderSlotName, string textureName, bool searchProjectWide, bool isNormalMap = false)
    {
        Texture2D texture = FindTexture(textureName, searchProjectWide);
        if (texture != null)
        {
            // If it's a normal map, make sure its texture type is set correctly.
            if (isNormalMap)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer.textureType != TextureImporterType.NormalMap)
                {
                    importer.textureType = TextureImporterType.NormalMap;
                    importer.SaveAndReimport();
                    Debug.Log($"Corrected texture type for normal map: {texture.name}", texture);
                }
            }
            
            mat.SetTexture(shaderSlotName, texture);
            Debug.Log($"Assigned {texture.name} to {mat.name}'s {shaderSlotName} slot.", mat);
            return true;
        }
        return false;
    }

    // Helper function to find a texture by name in a folder or the entire project.
    private static Texture2D FindTexture(string name, bool searchProjectWide)
    {
        // UPDATED: Search the entire project ('Assets' folder) if searchProjectWide is true.
        string[] searchFolders = searchProjectWide ? null : new[] { "Assets" };
        string[] guids = AssetDatabase.FindAssets($"{name} t:texture2D", searchFolders);

        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        return null;
    }
}

