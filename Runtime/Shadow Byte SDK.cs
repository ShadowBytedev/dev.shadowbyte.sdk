#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;
using UnityEngine.Networking;

public class ShadowByteSDK : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;
    private int selectedTab = 0;
    private int tutorialsselectedTab = 0;
    public Sprite logo;

    private bool includeAssets = true;
    private bool includePackageAssets = true;
    private bool includeScenes = true;
    private bool includeScripts = true;

    [Header("Settings")]
    [Tooltip("The root transform of the building.")]
    public Transform buildingRoot;

    [Tooltip("Adjust the intensity of the lights.")]
    public float lightIntensity = 0.16f;

    [Tooltip("Choose the color of the lights.")]
    public Color lightColor = Color.white;

    [Tooltip("Set the maximum number of point lights.")]
    public int maxLights = 50;

    [Tooltip("Define the radius of the point lights.")]
    public float pointLightRadius = 10f;

    [Tooltip("Specify the vertical offset of the point lights.")]
    public float pointLightVerticalOffset = 5f;

    [Tooltip("Toggle to enable/disable shadows for the lights.")]
    public bool enableShadows = false; // Added setting for shadows

    private int totalItems;
    private int processedItems;
    private GameObject lightsContainer;

    private bool optimizeActiveInScene = false;
    private bool optimizeAllInFolder = false;
    private Object folderObject; // Use the aliased 'Object' here

    // Settings for Texture Optimizer
    private bool compressTextures = true;
    private bool generateMipmaps = true;
    private int maxTextureSize = 2048;

    private string changelog = "Loading...";

    private string changelogURL = "https://info.shadowbyte.dev/ShadowByteSDK/Changelog.txt";

    // Define the URL where the latest version information is hosted
    private string latestVersion = "https://info.shadowbyte.dev/ShadowByteSDK/Version.txt";
    private string currentVersion = "6.0.0";

    private bool foldoutworldRequirements = false;
    private bool foldoutworldRecommendations = false;
    private bool foldoutAvatarRequirements = false;
    private bool foldoutAvatarRecommendations = false;
    private bool foldoutVideo = false;
    private bool foldoutIntroduction = true;
    private bool foldoutDownload = true;
    private bool foldoutExtract = false;
    private bool foldoutLocate = false;
    private bool foldoutVerify = false;
    private bool foldoutConfiguring = false;
    private bool foldoutTesting = false;
    private bool foldoutEnjoying = false;
    private bool foldoutTips = false;

    private bool foldoutVersion = true;
    private bool foldoutSupport = false;
    private bool foldoutError = false;

    public Sprite Step1imageSprite;
    public Sprite Step2imageSprite;
    public Sprite Step3imageSprite;
    public Sprite Step4imageSprite;
    public Sprite Step5imageSprite;
    public Sprite Step6imageSprite;
    public Sprite Step7imageSprite;

    [MenuItem("Shadow Byte SDK/ToolKit")]
    static void Init()
    {
        ShadowByteSDK window = (ShadowByteSDK)EditorWindow.GetWindow(typeof(ShadowByteSDK));
        window.titleContent = new GUIContent("Shadow Byte ToolKit");
        window.Show();

        // Fetch the initial changelog
        window.FetchChangelog();

        // Fetch the latest version information
        window.FetchLatestVersion();
    }

    void OnGUI()
    {
        // Begin ScrollView
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        DrawTabs();

        switch (selectedTab)
        {
            case 0:
                DrawHomeTab();
                break;
            case 1:
                DrawAtlasSizeTab();
                break;
            case 2:
                DrawOptimizerTab();
                break;
            case 3:
                DrawUPETab();
                break;
            case 4:
                DrawALTTab();
                break;
            case 5:
                TutorialsTab();
                break;
            case 6:
                DrawChangelogTab();
                break;
            default:
                break;

                // End ScrollView
                GUILayout.EndScrollView();

        }

    }

    void DrawTabs()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Toggle(selectedTab == 0, "Home", "Button"))
            selectedTab = 0;
        if (GUILayout.Toggle(selectedTab == 1, "AtlasSize", "Button"))
            selectedTab = 1;
        if (GUILayout.Toggle(selectedTab == 2, "Texture Optimizer", "Button"))
            selectedTab = 2;
        if (GUILayout.Toggle(selectedTab == 3, "Package Exporter", "Button"))
            selectedTab = 3;
        if (GUILayout.Toggle(selectedTab == 4, "Auto Lighting", "Button"))
            selectedTab = 4;
        if (GUILayout.Toggle(selectedTab == 5, "Tutorials", "Button"))
            selectedTab = 5;
        if (GUILayout.Toggle(selectedTab == 6, "Changelog", "Button"))
            selectedTab = 6;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    void DrawHomeTab()
    {
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = 20;
        {
            // Display logo
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (logo != null)
                GUILayout.Label(logo.texture, GUILayout.MaxHeight(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Centered text header
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Check us out at", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Website"))
            {
                Application.OpenURL("https://shadowbyte.dev");
            }
            if (GUILayout.Button("Gumroad"))
            {
                Application.OpenURL("https://shadowbytedev.gumroad.com/");
            }
            if (GUILayout.Button("Discord"))
            {
                Application.OpenURL("https://shadowhub.dev/");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }

        foldoutVersion = EditorGUILayout.Foldout(foldoutVersion, "Version", true);
        if (foldoutVersion)
        {
            GUILayout.Space(5);
            if (currentVersion != latestVersion)
            {
                EditorGUILayout.HelpBox("A new version (" + latestVersion + ") of the Shadow Byte ToolKit is available. Please update for the latest features and bug fixes.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("You are using the latest version of the Shadow Byte ToolKit (" + currentVersion + ").", MessageType.Info);
            }
        }

        GUILayout.Space(10);

        foldoutSupport = EditorGUILayout.Foldout(foldoutSupport, "Support & License", true);
        if (foldoutSupport)
        {
            GUILayout.Space(5);
            EditorGUILayout.HelpBox(
                 "If you need further support or have any issues, please join our Discord server and open a ticket. " +
                 "We’d be more than happy to assist you.\n\n" +
                 "This tool is protected by copyright and is the intellectual property of Shadow Byte Development. Sharing, redistributing, or reselling is strictly prohibited. Any violation will result in the loss of your license. Thank you for your compliance.",
                 MessageType.Info
                   );
        }
        GUILayout.Space(10);
        foldoutError = EditorGUILayout.Foldout(foldoutError, "VRChat SDK Error", true);
        if (foldoutError)
        {
            GUILayout.Space(5);
            EditorGUILayout.HelpBox(
              "If you get a compiler error when trying to build and test or build and publish using the VRChat SDK, " +
              "just delete this script and try to build and publish or build and test again. The VRChat SDK for whatever " +
              "reason doesn’t like certain scripts and will refuse to build if it’s presented.",
              MessageType.Warning
                );
        }


        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("© 2024 Shadow Byte Development", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        // End ScrollView
        GUILayout.EndScrollView();
    }
    // Method to fetch the latest version information
    void FetchLatestVersion()
    {
        UnityWebRequest www = UnityWebRequest.Get(latestVersion);
        www.SendWebRequest();

        while (!www.isDone) { }

        if (www.result == UnityWebRequest.Result.Success)
        {
            latestVersion = www.downloadHandler.text.Trim();
        }
        else
        {
            latestVersion = "Unknown";
        }
    }
    void DrawUPETab()
    {

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = 20;
        {

            GUILayout.Label("Package Exporter", headerStyle);

            // Include Assets
            includeAssets = EditorGUILayout.Toggle(new GUIContent("Include Assets", "Include assets from the 'Assets' directory"), includeAssets);

            // Include Package Assets
            includePackageAssets = EditorGUILayout.Toggle(new GUIContent("Include Package Assets", "Include assets from the 'Assets/Package' directory"), includePackageAssets);

            // Include Scenes
            includeScenes = EditorGUILayout.Toggle(new GUIContent("Include Scenes", "Include scene files"), includeScenes);

            // Include Scripts
            includeScripts = EditorGUILayout.Toggle(new GUIContent("Include Scripts", "Include script files"), includeScripts);

            GUILayout.Space(20); // Add some space between options and export button

            if (GUILayout.Button("Export Package"))
            {
                ExportPackage();
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("© 2024 Shadow Byte Development", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            // End ScrollView
            GUILayout.EndScrollView();
        }

        void ExportPackage()
        {
            List<string> assetsToExport = new List<string>();
            HashSet<string> includedAssets = new HashSet<string>();

            if (includeAssets)
                GetAssetsInDirectory("Assets", assetsToExport, includedAssets);

            if (includePackageAssets)
                GetAssetsInDirectory("Packages", assetsToExport, includedAssets);

            if (includeScenes)
                GetAssetsInDirectory("Assets", assetsToExport, includedAssets, "*.unity");

            if (includeScripts)
                GetAssetsInDirectory("Assets", assetsToExport, includedAssets, "*.cs");

            if (assetsToExport.Count == 0)
            {
                Debug.LogError("No assets selected for export.");
                return;
            }

            string outputPath = EditorUtility.SaveFilePanel("Save Unity Package", "", "NewPackage", "unitypackage");
            if (outputPath.Length == 0)
                return;

            AssetDatabase.ExportPackage(assetsToExport.ToArray(), outputPath, ExportPackageOptions.Default);
            Debug.Log("Package exported to: " + outputPath);
        }
    }
    void GetAssetsInDirectory(string directory, List<string> assetsToExport, HashSet<string> includedAssets, string searchPattern = "")
    {
        string[] guids = AssetDatabase.FindAssets(searchPattern, new string[] { directory });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!includedAssets.Contains(assetPath))
            {
                assetsToExport.Add(assetPath);
                includedAssets.Add(assetPath);
            }
        }
    }

    void DrawALTTab()
    {

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = 20;

        GUILayout.Label("Auto Lighting Tool", headerStyle);

        GUILayout.Label("Lighting Build Settings", EditorStyles.boldLabel);
        buildingRoot = (Transform)EditorGUILayout.ObjectField(
            new GUIContent("Building Root", "Specify the root transform of the building."),
            buildingRoot,
            typeof(Transform),
            true
        );

        lightIntensity = EditorGUILayout.FloatField(
            new GUIContent("Light Intensity", "Adjust the intensity of the lights."),
            lightIntensity
        );

        lightColor = EditorGUILayout.ColorField(
            new GUIContent("Light Color", "Choose the color of the lights."),
            lightColor
        );

        maxLights = EditorGUILayout.IntField(
            new GUIContent("Max Point Lights", "Set the maximum number of point lights."),
            maxLights
        );

        pointLightRadius = EditorGUILayout.FloatField(
            new GUIContent("Point Light Radius", "Define the radius of the point lights."),
            pointLightRadius
        );

        pointLightVerticalOffset = EditorGUILayout.FloatField(
            new GUIContent("Point Light Vertical Offset", "Specify the vertical offset of the point lights."),
            pointLightVerticalOffset
        );

        enableShadows = EditorGUILayout.Toggle(
            new GUIContent("Enable Shadows", "Toggle to enable/disable shadows for the lights."),
            enableShadows
        );

        if (GUILayout.Button("Start"))
        {
            StartAutoLighting();
        }


        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("© 2024 Shadow Byte Development", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        // End ScrollView
        GUILayout.EndScrollView();
    }


    void StartAutoLighting()
    {
        if (buildingRoot == null)
        {
            Debug.LogError("Building root not assigned. Please assign it in the Inspector.");
            return;
        }

        // Create a container for lights
        lightsContainer = new GameObject("LightsContainer");
        lightsContainer.transform.SetParent(buildingRoot);

        // Count the number of items
        totalItems = CountItemsRecursive(buildingRoot);
        processedItems = 0;

        // Show progress bar
        EditorUtility.DisplayProgressBar("Auto Lighting", "Adding lights...", 0f);

        // Add directional light
        AddDirectionalLight();

        // Add a limited number of point lights based on child objects
        AddLimitedPointLightsRecursive(buildingRoot, maxLights);

        // Add light probes
        AddLightProbes();

        // Clear progress bar
        EditorUtility.ClearProgressBar();

        // Display confirmation dialog
        EditorUtility.DisplayDialog("Auto Lighting", "Auto Lighting process completed for " + totalItems + " objects.", "OK");
    }

    int CountItemsRecursive(Transform parent)
    {
        int count = 1; // Counting the current object
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            count += CountItemsRecursive(child);
        }
        return count;
    }

    void AddDirectionalLight()
    {
        GameObject directionalLightGO = new GameObject("Directional Light");
        directionalLightGO.transform.SetParent(lightsContainer.transform);
        Light directionalLight = directionalLightGO.AddComponent<Light>();
        directionalLight.type = LightType.Directional;
        directionalLight.intensity = lightIntensity;
        directionalLight.color = lightColor;
        directionalLight.shadows = enableShadows ? LightShadows.Soft : LightShadows.None; // Set shadows based on the enableShadows setting

        processedItems++;
        UpdateProgressBar();
    }

    void AddLimitedPointLightsRecursive(Transform parent, int maxLights)
    {
        int lightsToAdd = Mathf.Min(parent.childCount, maxLights);

        for (int i = 0; i < lightsToAdd; i++)
        {
            Transform child = parent.GetChild(i);

            // Add point light to each child
            GameObject pointLightGO = new GameObject("Point Light " + child.name);
            pointLightGO.transform.SetParent(lightsContainer.transform);
            Light pointLight = pointLightGO.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.intensity = lightIntensity;
            pointLight.color = lightColor;

            // Adjust settings for point lights
            pointLight.range = pointLightRadius;
            pointLight.transform.position = child.position + new Vector3(0f, pointLightVerticalOffset, 0f);

            processedItems++;
            UpdateProgressBar();
        }
    }

    void AddLightProbes()
    {
        LightProbeGroup lightProbeGroup = buildingRoot.gameObject.GetComponent<LightProbeGroup>();
        if (lightProbeGroup == null)
        {
            lightProbeGroup = buildingRoot.gameObject.AddComponent<LightProbeGroup>();
        }
    }

    void UpdateProgressBar()
    {
        float progress = (float)processedItems / totalItems;
        EditorUtility.DisplayProgressBar("Auto Lighting", "Adding lights...", progress);

        if (processedItems == totalItems)
        {
            EditorUtility.ClearProgressBar();
        }
    }


    void DrawOptimizerTab()
    {
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = 20;

        GUILayout.Label("Texture Optimizer", headerStyle);

        // Add options for bulk Texture Optimizer
        GUILayout.Space(5);
        optimizeActiveInScene = EditorGUILayout.Toggle("Optimize Active in Scene", optimizeActiveInScene);
        optimizeAllInFolder = EditorGUILayout.Toggle("Optimize All in Folder", optimizeAllInFolder);
        if (optimizeAllInFolder)
        {
            folderObject = EditorGUILayout.ObjectField("Select Folder", folderObject, typeof(Object), false);
        }

        GUILayout.Space(10);

        // Add settings for Texture Optimizer
        GUILayout.Label("Texture Optimizer Settings", EditorStyles.boldLabel);
        compressTextures = EditorGUILayout.Toggle("Compress Textures", compressTextures);
        generateMipmaps = EditorGUILayout.Toggle("Generate Mipmaps", generateMipmaps);
        maxTextureSize = EditorGUILayout.IntField("Max Texture Size", maxTextureSize);

        GUILayout.Space(10);

        // Add button to trigger bulk optimization
        if (GUILayout.Button("Optimize Textures"))
        {
            if (optimizeActiveInScene)
            {
                OptimizeTexturesInActiveScene();
            }
            else if (optimizeAllInFolder && folderObject != null)
            {
                OptimizeAllTexturesInFolder(AssetDatabase.GetAssetPath(folderObject));
            }
            else
            {
                Debug.LogError("Please select an option for bulk Texture Optimizer.");
            }
        }
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("© 2024 Shadow Byte Development", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        // End ScrollView
        GUILayout.EndScrollView();
    }
    void DrawAtlasSizeTab()
    {
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = 20;

        GUILayout.Label("AtlasSize Optimizer", headerStyle);

        GUILayout.Space(10);

    if (GUILayout.Button("Set 512x512"))
    {
        SetAtlasSize(512);
    }
    if (GUILayout.Button("Set 1024x1024"))
    {
        SetAtlasSize(1024);
    }
    if (GUILayout.Button("Set 2048x2048"))
    {
        SetAtlasSize(2048);
    }
    if (GUILayout.Button("Set 4096x4096"))
    {
        SetAtlasSize(4096);
    }
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("© 2024 Shadow Byte Development", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        // End ScrollView
        GUILayout.EndScrollView();
    }
    void OptimizeTexturesInActiveScene()
    {
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            Component[] renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (Component renderer in renderers)
            {
                if (renderer != null && renderer is Renderer)
                {
                    Material[] materials = ((Renderer)renderer).sharedMaterials;
                    foreach (Material material in materials)
                    {
                        if (material != null)
                        {
                            // Ensure the material's main texture is not null
                            if (material.mainTexture != null)
                            {
                                // Ensure the material's main texture is recognized as an asset
                                Texture2D mainTexture = material.mainTexture as Texture2D;
                                if (mainTexture != null && AssetDatabase.Contains(mainTexture))
                                {
                                    Debug.Log("Processing texture: " + mainTexture.name);
                                    TextureUtility.OptimizeTexture(mainTexture, compressTextures, generateMipmaps, maxTextureSize);
                                }
                                else
                                {
                                    Debug.LogWarning("Texture not recognized as an asset: " + mainTexture.name);
                                }
                            }
                            else
                            {
                                Debug.LogWarning("Main texture of material is NULL.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Material is NULL.");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Renderer is NULL or not of type Renderer.");
                }
            }
        }
        Debug.Log("Bulk Texture Optimizer in active scene complete.");
    }
    void OptimizeAllTexturesInFolder(string folderPath)
    {
        string[] assetPaths = AssetDatabase.FindAssets("t:Texture2D", new string[] { folderPath });
        foreach (string assetPath in assetPaths)
        {
            string fullPath = AssetDatabase.GUIDToAssetPath(assetPath);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath);
            if (texture != null)
            {
                TextureUtility.OptimizeTexture(texture, compressTextures, generateMipmaps, maxTextureSize);
            }
        }
        Debug.Log("Bulk Texture Optimizer in folder complete.");
    }

    void SetAtlasSize(int size)
    {
        LightmapEditorSettings.maxAtlasHeight = size;
        LightmapEditorSettings.maxAtlasSize = size;
        Debug.Log($"Set Lightmap Atlas Size to {size}x{size}");

        // Show notification
        EditorUtility.DisplayDialog("Atlas Size Set", $"Set Lightmap Atlas Size to {size}x{size}", "OK");
    }

    Texture2D[] GetAllTextures(Material material)
    {
        Texture2D[] textures = new Texture2D[material.shader.GetPropertyCount()];
        for (int i = 0; i < material.shader.GetPropertyCount(); i++)
        {
            if (material.shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Texture)
            {
                textures[i] = (Texture2D)material.GetTexture(material.shader.GetPropertyName(i));
            }
        }
        return textures;
    }

    void DrawChangelogTab()
    {
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = 20;

        GUIStyle changelogStyle = new GUIStyle(EditorStyles.boldLabel);
        changelogStyle.alignment = TextAnchor.MiddleCenter;
        changelogStyle.wordWrap = true;
        changelogStyle.fontSize = 18;

        {
            // Display splash screen only if enabled
            GUILayout.Label("Changelog", headerStyle);
            GUILayout.Space(10);
            GUILayout.Label(changelog, changelogStyle);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("© 2024 Shadow Byte Development", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            // End ScrollView
            GUILayout.EndScrollView();
        }
    }

    void TutorialsTab()
    {
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = 20;

        // Display splash screen only if enabled
        GUILayout.Label("Tutorials", headerStyle);
        GUILayout.Space(10);

        DrawTutorialsTabs();

        switch (tutorialsselectedTab)
        {
            case 0:
                DrawInfoTab();
                break;
            case 1:
                DrawWorldsNewTab();
                break;
            case 2:
                DrawWorldsOldTab();
                break;
            default:
                break;
        }
    }

    void DrawTutorialsTabs()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Toggle(tutorialsselectedTab == 0, "Info", "Button"))
            tutorialsselectedTab = 0;
        if (GUILayout.Toggle(tutorialsselectedTab == 1, "World/Avatars", "Button"))
            tutorialsselectedTab = 1;
        if (GUILayout.Toggle(tutorialsselectedTab == 2, "Worlds - RAR", "Button"))
            tutorialsselectedTab = 2;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        // In the original version, there was an EndScrollView without a BeginScrollView
        // You must ensure a corresponding BeginScrollView if you need an EndScrollView here.
    }
    void DrawInfoTab()
    {
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);

        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = 20;

        GUIStyle helpBoxStyle = new GUIStyle(GUI.skin.box);
        helpBoxStyle.fontSize = 15;
        helpBoxStyle.wordWrap = true;
        helpBoxStyle.alignment = TextAnchor.MiddleLeft;

        // Introduction Foldout
        foldoutIntroduction = EditorGUILayout.Foldout(foldoutIntroduction, "Introduction", true);
        if (foldoutIntroduction)
        {
            GUILayout.Label(
                "Welcome to the tutorial on how to extract and open VRChat world or avatar project files from a RAR or (.unitypackage) file using VRChat CreatorCompanion. This guide will assist you in setting up your world projects efficiently with recommended tools and plugins.", helpBoxStyle);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
        }
        GUILayout.Space(10);
        GUILayout.Label("Worlds", headerStyle);
        GUILayout.Space(10);
        // Requirements Foldout
        foldoutAvatarRequirements = EditorGUILayout.Foldout(foldoutAvatarRequirements, "Requirements", true);
        if (foldoutAvatarRequirements)
        {
            EditorGUILayout.HelpBox(
                "If you need further support or have any issues, please join our Discord server and open a ticket. " +
                "We’d be more than happy to assist you.\n\n" +
                "This tool is protected by copyright and is the intellectual property of Shadow Byte. Sharing, redistributing, or reselling is strictly prohibited. Any violation will result in the loss of your license. Thank you for your compliance.",
                MessageType.Info
                  );
            EditorGUILayout.HelpBox(
               "VRChat Creator Companion\n" +
               "Unity: 2022.3.6f1\n" +
               "A text editor (We recommend Visual Studio Code)\n" +
               "VRChat SDK - Worlds\n" +
               "Required for a lot of our worlds (Winrar)\n" +
               "Required for all of our worlds (CyanTrigger)", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
        }
        GUILayout.Space(10);
        // Recommendations Foldout
        foldoutworldRecommendations = EditorGUILayout.Foldout(foldoutworldRecommendations, "Recommendations", true);
        if (foldoutworldRecommendations)
        {
            EditorGUILayout.HelpBox(
               "Recommended for all of our worlds (VRWorld Toolkit)\n" +
               "Recommended for our club worlds (AudioLink) !NOT QUEST FRIENDLY!\n" +
               "Recommended for our club worlds (VR Stage Lighting) !NOT QUEST FRIENDLY!\n" +
               "Recommended for our club worlds (LumaDriver) !NOT QUEST FRIENDLY!\n" +
               "Recommended for our club worlds (LTCGI) !NOT QUEST FRIENDLY!", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
        }

        GUILayout.Space(10);
        GUILayout.Label("Avatars", headerStyle);
        GUILayout.Space(10);
        foldoutworldRequirements = EditorGUILayout.Foldout(foldoutworldRequirements, "Requirements", true);
        if (foldoutworldRequirements)
        {
            EditorGUILayout.HelpBox(
               "VRChat Creator Companion\n" +
               "Unity: 2022.3.6f1\n" +
               "A text editor (We recommend Visual Studio Code)\n" +
               "VRChat SDK - Avatars\n" +
               "Required for a lot of our avatars (Avatars 3.0 Manager)\n" +
               "Required for all of our avatars (VRCFury)", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
        }
        GUILayout.Space(10);
        // Recommendations Foldout
        foldoutAvatarRecommendations = EditorGUILayout.Foldout(foldoutAvatarRecommendations, "Recommendations", true);
        if (foldoutAvatarRecommendations)
        {
            EditorGUILayout.HelpBox(
               "Recommended for all of our avatars (Gesture Manager)\n" +
               "Recommended for our avatars (lilAvatarUtils)", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
        }

        // Centered text header
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Still Need Help?", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Website"))
        {
            Application.OpenURL("https://shadowbyte.dev");
        }
        if (GUILayout.Button("Discord"))
        {
            Application.OpenURL("Shadow Byte");
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("© 2024 Shadow Byte Development", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
        GUILayout.EndScrollView();
    }

    void DrawWorldsOldTab()
    {
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);

        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = 20;

        GUIStyle helpBoxStyle = new GUIStyle(GUI.skin.box);
        helpBoxStyle.fontSize = 15;
        helpBoxStyle.wordWrap = true;
        helpBoxStyle.alignment = TextAnchor.MiddleLeft;

        // Requirements Foldout
        foldoutDownload = EditorGUILayout.Foldout(foldoutDownload, "Download Your RAR File", true);
        if (foldoutDownload)
        {
            EditorGUILayout.HelpBox(
                "Ensure that you have downloaded the RAR file containing the VRChat world project.\n" +
                "This file should have been obtained from the Shadow Development official repositories.", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
        }
        GUILayout.Space(10);
        // Step #1 Foldout

        foldoutExtract = EditorGUILayout.Foldout(foldoutExtract, "Step #1", true);
        if (foldoutExtract)
        {
            EditorGUILayout.HelpBox(
               "- Right-click on the RAR file.\n" +
               "- Select ‘Extract to [file name]/’ to extract its contents into a new folder with the same name as the RAR file.\n" +
               "- Wait for the extraction process to complete.", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space

            // Display the image sprite if it's assigned
            if (Step1imageSprite != null)
            {
                GUILayout.Label(Step1imageSprite.texture, GUILayout.MaxHeight(268), GUILayout.MaxWidth(477)); // Display the texture of the sprite
            }
        }
        GUILayout.Space(10);
        foldoutLocate = EditorGUILayout.Foldout(foldoutLocate, "Step #2", true);
        if (foldoutLocate)
        {
            EditorGUILayout.HelpBox(
               "- In CreatorCompanion, select ‘Open Existing Project’.\n" +
               "- Browse to the directory of your extracted project files.\n" +
               "- Choose the Unity project folder", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
                                         // Display the image sprite if it's assigned
            if (Step2imageSprite != null)
            {
                GUILayout.Label(Step2imageSprite.texture, GUILayout.MaxHeight(696), GUILayout.MaxWidth(1536)); // Display the texture of the sprite
            }
        }
        GUILayout.Space(10);
        foldoutVerify = EditorGUILayout.Foldout(foldoutVerify, "Step #3", true);
        if (foldoutVerify)
        {
            EditorGUILayout.HelpBox(
                "- Once the project is opened, CreatorCompanion will check for the necessary dependencies and plugins.\n" +
                "- Install any missing components as recommended by CreatorCompanion.", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
                                         // Display the image sprite if it's assigned
            if (Step3imageSprite != null)
            {
                GUILayout.Label(Step3imageSprite.texture, GUILayout.MaxHeight(268), GUILayout.MaxWidth(477)); // Display the texture of the sprite
            }
        }
        GUILayout.Space(10);
        // Video Foldout
        foldoutVideo = EditorGUILayout.Foldout(foldoutVideo, "Video", true);
        if (foldoutVideo)
        {
            EditorGUILayout.HelpBox(
                "Watch this setup video to learn how to download and run our\n" +
                "VRChat worlds in Unity. It's essential for getting started smoothly.\n" +
                "If you need further support, please open a ticket in our Discord server or on our website.", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
            if (GUILayout.Button("How to Video"))
            {
                Application.OpenURL("https://www.youtube.com/watch?v=mCoMY2MN1Cg");
            }
        }
        GUILayout.EndScrollView();
    }

    void DrawWorldsNewTab()
    {
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);

        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = 20;

        GUIStyle helpBoxStyle = new GUIStyle(GUI.skin.box);
        helpBoxStyle.fontSize = 15;
        helpBoxStyle.wordWrap = true;
        helpBoxStyle.alignment = TextAnchor.MiddleLeft;

        // Requirements Foldout
        foldoutDownload = EditorGUILayout.Foldout(foldoutDownload, "Download Your File", true);
        if (foldoutDownload)
        {
            EditorGUILayout.HelpBox(
                "Ensure that you have downloaded the Unity package containing the VRChat world or avatar project.\n" +
                "This file should have been obtained from the Shadow Development official repositories.", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
        }
        GUILayout.Space(10);
        // Step #1 Foldout

        foldoutExtract = EditorGUILayout.Foldout(foldoutExtract, "Step #1", true);
        if (foldoutExtract)
        {
            EditorGUILayout.HelpBox(
                "Open CreatorCompanion and select 'New Project.'\n" +
                "Choose a location and name for your project.\n" +
                "Click 'Create Project' to generate the new Unity project.", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space

            // Display the image sprite if it's assigned
            if (Step4imageSprite != null)
            {
                GUILayout.Label(Step4imageSprite.texture, GUILayout.MaxHeight(268), GUILayout.MaxWidth(477)); // Display the texture of the sprite
            }
        }
        GUILayout.Space(10);

        foldoutLocate = EditorGUILayout.Foldout(foldoutLocate, "Step #2", true);
        if (foldoutLocate)
        {
            EditorGUILayout.HelpBox(
                "Open Unity and wait for the new project to load.\n" +
                "In the Unity menu, go to 'Assets' > 'Import Package' > 'Custom Package...'\n" +
                "Navigate to the location where you extracted the RAR file contents.\n" +
                "Select the package file (.unitypackage) and click 'Open.'", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
                                         // Display the image sprite if it's assigned
            if (Step5imageSprite != null)
            {
                GUILayout.Label(Step5imageSprite.texture, GUILayout.MaxHeight(696), GUILayout.MaxWidth(1536)); // Display the texture of the sprite
            }
        }
        GUILayout.Space(10);
        foldoutVerify = EditorGUILayout.Foldout(foldoutVerify, "Step #3", true);
        if (foldoutVerify)
        {
            EditorGUILayout.HelpBox(
    "Unity will display a list of assets included in the package file.\n" +
       "Review the assets and make sure all necessary components are selected.\n" +
       "Click 'Import' to import the selected assets into your Unity project.", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
                                         // Display the image sprite if it's assigned
            if (Step6imageSprite != null)
            {
                GUILayout.Label(Step6imageSprite.texture, GUILayout.MaxHeight(268), GUILayout.MaxWidth(477)); // Display the texture of the sprite
            }
        }
        GUILayout.Space(10);
        foldoutConfiguring = EditorGUILayout.Foldout(foldoutConfiguring, "Step #4", true);
        if (foldoutConfiguring)
        {
            EditorGUILayout.HelpBox(
    "Once the import process is complete, navigate to the imported assets in the Unity Project window.\n" +
       "You may find prefabs, scripts, textures, materials, and other resources related to the VRChat world or avatar.\n" +
       "Follow any additional instructions provided by Shadow Development to set up and configure the world or avatar.", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
                                         // Display the image sprite if it's assigned
        }
        GUILayout.Space(10);
        foldoutTesting = EditorGUILayout.Foldout(foldoutTesting, "Step #5", true);
        if (foldoutTesting)
        {
            EditorGUILayout.HelpBox(
   "To test the VRChat world, ensure that you have the VRChat SDK installed and configured in Unity.\n" +
       "Open the VRChat SDK Control Panel window.\n" +
       "Log in to your VRChat account if prompted.\n" +
       "Follow the SDK's instructions to build and upload the world or avatar to VRChat.", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
                                         // Display the image sprite if it's assigned
            if (Step7imageSprite != null)
            {
                GUILayout.Label(Step7imageSprite.texture, GUILayout.MaxHeight(268), GUILayout.MaxWidth(477)); // Display the texture of the sprite
            }
        }
        GUILayout.Space(10);
        foldoutEnjoying = EditorGUILayout.Foldout(foldoutEnjoying, "Step #6", true);
        if (foldoutEnjoying)
        {
            EditorGUILayout.HelpBox(
   "Once uploaded, you can visit your VRChat world in-game and explore it with friends and other users.", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
                                         // Display the image sprite if it's assigned
        }
        GUILayout.Space(10);
        foldoutTips = EditorGUILayout.Foldout(foldoutTips, "Additional Tips", true);
        if (foldoutTips)
        {
            EditorGUILayout.HelpBox(
    "Make sure to regularly save your Unity project as you work on it.\n" +
       "Back up your project files to prevent data loss.\n" +
       "Refer to Unity's documentation and VRChat's resources for troubleshooting and additional assistance.", MessageType.Info);
            GUILayout.ExpandWidth(true); // Ensure the label expands horizontally to fill the available space
                                         // Display the image sprite if it's assigned
        }

        GUILayout.EndScrollView();
    }


    void FetchChangelog()
    {
        UnityWebRequest www = UnityWebRequest.Get(changelogURL);
        www.SendWebRequest();

        while (!www.isDone) { }

        if (www.result == UnityWebRequest.Result.Success)
        {
            changelog = www.downloadHandler.text;
        }
        else
        {
            changelog = "Failed to fetch changelog.";
        }
    }


    public static class TextureUtility
    {
        public static void OptimizeTexture(Texture2D texture, bool compressTextures, bool generateMipmaps, int maxTextureSize)
        {
            // Get the asset path of the texture
            string path = AssetDatabase.GetAssetPath(texture);

            // Check if the texture has a valid asset path
            if (!string.IsNullOrEmpty(path))
            {
                // Retrieve the TextureImporter
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                // Check if the TextureImporter is valid
                if (importer != null)
                {
                    // Set texture import settings
                    importer.textureCompression = compressTextures ? TextureImporterCompression.Compressed : TextureImporterCompression.Uncompressed;
                    importer.mipmapEnabled = generateMipmaps;
                    importer.maxTextureSize = maxTextureSize;

                    // Apply and save the import settings
                    importer.SaveAndReimport();
                }
                else
                {
                    // Log an error if TextureImporter is not found
                    Debug.LogError("Failed to optimize texture. Texture importer not found. Path: " + path);
                }
            }
            else
            {
                // Log an error if the texture path is invalid or empty
                Debug.LogError("Failed to optimize texture. Invalid texture path.");
            }
        }
    }
}
#endif