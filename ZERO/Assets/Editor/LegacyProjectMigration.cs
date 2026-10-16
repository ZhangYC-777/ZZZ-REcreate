using System;
using System.IO;
using System.Linq;
using System.Text;
using Unity.Animations.SpringBones;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>Explicit migration and checks for this project's Unity 2022.3 copy.</summary>
public static class LegacyProjectMigration
{
    private const string ScenePath = "Assets/Scenes/CharacterPreview.unity";
    private const string PipelinePath = "Assets/Settings/LegacyURP.asset";
    private const string RendererPath = "Assets/Settings/LegacyRenderer.asset";

    [MenuItem("Tools/Project Migration/Finish Unity 2022 Migration")]
    public static void Finish()
    {
        if (!Application.unityVersion.StartsWith("2022.3."))
            throw new InvalidOperationException("Run this tool in the Unity 2022.3 ZERO project.");
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode first.");

        Directory.CreateDirectory("MigrationReview");
        var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
        if (pipeline == null)
        {
            var renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
            renderer.name = "LegacyRenderer";
            AssetDatabase.CreateAsset(renderer, RendererPath);
            var rendererSettings = new SerializedObject(renderer);
            SetInt(rendererSettings, "m_RenderingMode", 2); // Forward+, matching the source scene.
            rendererSettings.ApplyModifiedPropertiesWithoutUndo();

            var ssaoType = typeof(UniversalRendererData).Assembly.GetType(
                "UnityEngine.Rendering.Universal.ScreenSpaceAmbientOcclusion", true);
            var ssao = (ScriptableRendererFeature)ScriptableObject.CreateInstance(ssaoType);
            ssao.name = "ScreenSpaceAmbientOcclusion";
            AssetDatabase.AddObjectToAsset(ssao, renderer);
            var ao = new SerializedObject(ssao);
            SetFloat(ao, "m_Settings.Intensity", .4f);
            SetFloat(ao, "m_Settings.DirectLightingStrength", .25f);
            SetFloat(ao, "m_Settings.Radius", .3f);
            SetInt(ao, "m_Settings.Source", 1);
            SetInt(ao, "m_Settings.Samples", 1);
            ao.ApplyModifiedPropertiesWithoutUndo();
            ssao.Create();
            renderer.rendererFeatures.Add(ssao);
            EditorUtility.SetDirty(renderer);
            EditorUtility.SetDirty(ssao);

            pipeline = UniversalRenderPipelineAsset.Create(renderer);
            pipeline.name = "LegacyURP";
            pipeline.supportsCameraDepthTexture = true;
            pipeline.supportsCameraOpaqueTexture = true;
            pipeline.supportsHDR = true;
            pipeline.msaaSampleCount = 1;
            AssetDatabase.CreateAsset(pipeline, PipelinePath);
            var settings = new SerializedObject(pipeline);
            SetInt(settings, "m_MainLightShadowmapResolution", 2048);
            SetInt(settings, "m_AdditionalLightShadowsSupported", 1);
            SetInt(settings, "m_AdditionalLightsShadowmapResolution", 2048);
            SetInt(settings, "m_ShadowCascadeCount", 4);
            SetFloat(settings, "m_ShadowDistance", 50f);
            SetFloat(settings, "m_ShadowDepthBias", .1f);
            SetFloat(settings, "m_ShadowNormalBias", .5f);
            settings.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(pipeline);
        }

        GraphicsSettings.defaultRenderPipeline = pipeline;
        int quality = QualitySettings.GetQualityLevel();
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            QualitySettings.SetQualityLevel(i, false);
            QualitySettings.renderPipeline = pipeline;
        }
        QualitySettings.SetQualityLevel(quality, false);
        PlayerSettings.colorSpace = ColorSpace.Linear;
        AssetDatabase.SaveAssets();

        var scene = EditorSceneManager.OpenScene(ScenePath);
        var root = GameObject.Find("叶瞬光");
        if (root == null) throw new InvalidOperationException("The migrated character was not loaded.");
        var preview = root.GetComponent<SecondaryMotionPreview>();
        if (preview != null) preview.previewMotion = false;
        var camera = Camera.main;
        if (camera == null) throw new InvalidOperationException("The migrated scene has no main camera.");
        if (scene.GetRootGameObjects().All(go => go.GetComponent<Light>() == null))
            throw new InvalidOperationException("The migrated scene has no main light.");

        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = root;
        if (SceneView.lastActiveSceneView != null)
            SceneView.lastActiveSceneView.Frame(new Bounds(root.transform.position + Vector3.up * .8f,
                new Vector3(1.5f, 1.9f, 1.5f)), false);
        Debug.Log(Validate());
    }

    [MenuItem("Tools/Project Migration/Validate Unity 2022 Migration")]
    public static void ValidateMenu() => Debug.Log(Validate());

    public static string Validate()
    {
        var root = GameObject.Find("叶瞬光");
        if (root == null) throw new InvalidOperationException("Open CharacterPreview first.");
        var animator = root.GetComponentInChildren<Animator>();
        var bones = root.GetComponentsInChildren<SpringBone>(true);
        var manager = root.GetComponent<SpringManager>();
        var colliders = root.GetComponentsInChildren<SpringCapsuleCollider>(true).Length +
            root.GetComponentsInChildren<SpringSphereCollider>(true).Length;
        var missing = root.GetComponentsInChildren<Transform>(true)
            .Sum(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject));
        var materials = root.GetComponentsInChildren<Renderer>(true)
            .SelectMany(r => r.sharedMaterials).Distinct().ToArray();
        var badMaterials = materials.Where(m => m == null || m.shader == null ||
            m.shader.name == "Hidden/InternalErrorShader" || !m.shader.isSupported ||
            ShaderUtil.ShaderHasError(m.shader)).ToArray();
        bool human = animator != null && animator.avatar != null &&
            animator.avatar.isValid && animator.avatar.isHuman;
        bool references = manager != null && manager.springBones.Length == 339 &&
            manager.springBones.All(b => b != null) && bones.All(b => b.tipNode != null && b.pivotNode != null);
        bool finite = bones.All(b => !float.IsNaN(b.transform.position.sqrMagnitude) &&
            !float.IsInfinity(b.transform.position.sqrMagnitude));
        var simulation = YeshunguangSecondaryMotionSetup.Validate();
        bool pass = human && references && finite && bones.Length == 339 && colliders == 13 &&
            missing == 0 && badMaterials.Length == 0 && simulation.Contains("PASS: True");
        var report = new StringBuilder();
        report.AppendLine("Unity: " + Application.unityVersion);
        report.AppendLine("Scene: " + root.scene.path);
        report.AppendLine("Humanoid avatar valid: " + human);
        report.AppendLine("Spring bones: " + bones.Length);
        report.AppendLine("Body colliders: " + colliders);
        report.AppendLine("Spring endpoint/manager references valid: " + references);
        report.AppendLine("Missing character scripts: " + missing);
        report.AppendLine("Materials: " + materials.Length + "; invalid shaders: " + badMaterials.Length);
        report.AppendLine("Cinemachine: " + UnityEditor.PackageManager.PackageInfo
            .FindForAssembly(typeof(Cinemachine.CinemachineVirtualCamera).Assembly).version);
        report.AppendLine("URP: " + UnityEditor.PackageManager.PackageInfo
            .FindForAssembly(typeof(UniversalRenderPipelineAsset).Assembly).version);
        report.AppendLine(simulation);
        report.AppendLine("MIGRATION PASS: " + pass);
        Directory.CreateDirectory("MigrationReview");
        File.WriteAllText("MigrationReview/validation-report.txt", report.ToString());
        if (!pass) throw new InvalidOperationException(report.ToString());
        return report.ToString();
    }

    private static void SetInt(SerializedObject target, string name, int value)
    {
        var property = target.FindProperty(name);
        if (property == null) throw new InvalidOperationException("Missing URP setting: " + name);
        if (property.propertyType == SerializedPropertyType.Boolean) property.boolValue = value != 0;
        else property.intValue = value;
    }

    private static void SetFloat(SerializedObject target, string name, float value)
    {
        var property = target.FindProperty(name);
        if (property == null) throw new InvalidOperationException("Missing URP setting: " + name);
        property.floatValue = value;
    }
}
