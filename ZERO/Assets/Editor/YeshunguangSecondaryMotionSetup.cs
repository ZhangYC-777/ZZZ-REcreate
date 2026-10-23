using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Unity.Animations.SpringBones;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>Explicit, model-specific setup; never applies physics to an arbitrary skeleton.</summary>
public static class YeshunguangSecondaryMotionSetup
{
    private const string Prefix = "__SM_";
    private sealed class Profile
    {
        public string name;
        public int category;
        public float stiffness, drag, angle, angular, radius;
        public Profile(string n, int c, float s, float d, float a, float angularS, float r)
        { name = n; category = c; stiffness = s; drag = d; angle = a; angular = angularS; radius = r; }
    }

    private static readonly Profile Cloth = new Profile("衣摆", 0, 1400f, .72f, 3.5f, 850f, .004f);
    private static readonly Profile Ribbon = new Profile("衣袖飘带", 0, 1400f, .72f, 3f, 850f, .003f);
    private static readonly Profile Tail = new Profile("腰后尾饰", 0, 1500f, .72f, 2.5f, 850f, .009f);
    private static readonly Profile LongHair = new Profile("长发", 1, 1300f, .70f, 2.8f, 800f, .004f);
    private static readonly Profile ShortHair = new Profile("侧发和刘海", 1, 1600f, .73f, 5f, 950f, .0025f);
    private static readonly Profile HeadRibbon = new Profile("头部飘带", 2, 1400f, .72f, 2.5f, 850f, .002f);
    private static readonly Profile Ornament = new Profile("小挂饰和发饰", 2, 2000f, .78f, 4f, 1000f, .002f);

    private static bool Helper(Transform t) => t.name.EndsWith("錘") ||
        t.name.StartsWith("_dummy_") || t.name.StartsWith("_shadow_") || t.name.StartsWith(Prefix);

    private static Profile Classify(string n)
    {
        Match m = Regex.Match(n, @"^q_(\d+)_[012]$");
        if (m.Success) return int.Parse(m.Groups[1].Value) >= 2 ? Cloth : null;
        m = Regex.Match(n, @"^(?:左|右)?后q_(\d+)$");
        if (m.Success) return int.Parse(m.Groups[1].Value) >= 2 ? Cloth : null;
        m = Regex.Match(n, @"^w_(\d+)_1$");
        if (m.Success) return int.Parse(m.Groups[1].Value) >= 2 ? Tail : null;
        m = Regex.Match(n, @"^[左右]wd_(\d+)$");
        if (m.Success) return int.Parse(m.Groups[1].Value) >= 2 ? Ribbon : null;
        if (Regex.IsMatch(n, @"^[左右]wd2_2$")) return Ornament;
        m = Regex.Match(n, @"^[左右]f([12]?)_(\d+)_1$");
        if (m.Success) return int.Parse(m.Groups[2].Value) >=
            (m.Groups[1].Value.Length == 0 ? 2 : 1) ? LongHair : null;
        m = Regex.Match(n, @"^[左右]耳_(\d+)_1$");
        if (m.Success) return int.Parse(m.Groups[1].Value) >= 2 ? LongHair : null;
        m = Regex.Match(n, @"^[左右]cf_(\d+)$");
        if (m.Success) return int.Parse(m.Groups[1].Value) >= 2 ? ShortHair : null;
        m = Regex.Match(n, @"^[左右]td_(\d+)_1$");
        if (m.Success) return int.Parse(m.Groups[1].Value) >= 1 ? HeadRibbon : null;
        if (n.StartsWith("Ctr_") && n.Contains("Hair") && !n.Contains("HairAc"))
            return !n.EndsWith("_01") && !n.StartsWith("Ctr_BD_") ? ShortHair : null;
        if (Regex.IsMatch(n, @"^Ctr_[LR]_HairAc_02$") ||
            (n.Contains("HeadBand") && (n.EndsWith("_02") || n.Contains("HeadBandB"))) ||
            Regex.IsMatch(n, @"^[左右]ts$") || n == "Ctr_BackOrn_New_02" || n == "Ctr_BackOrn_New_03" ||
            Regex.IsMatch(n, @"^Skn_[LR]_Spn_Pendant_02$") || Regex.IsMatch(n, @"^Skn_[LR]_Jade_0[234]$"))
            return Ornament;
        return null;
    }

    private static Transform NewChild(Transform parent, string name)
    {
        var existing = parent.Find(name);
        if (existing != null) return existing;
        var go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create secondary motion helper");
        go.transform.SetParent(parent, false);
        return go.transform;
    }

    private static Transform Endpoint(Transform bone, HashSet<Transform> weighted)
    {
        var children = bone.Cast<Transform>().Where(t => !Helper(t)).ToArray();
        if (children.Length == 1) return children[0];
        if (children.Length > 1)
        {
            Vector3 direction = bone.parent != null ? (bone.position - bone.parent.position).normalized : Vector3.down;
            return children.OrderByDescending(t => Vector3.Dot((t.position - bone.position).normalized, direction)).First();
        }
        // A zero-weight leaf is only the endpoint of the preceding simulated bone.
        if (!weighted.Contains(bone)) return null;
        var marker = bone.Cast<Transform>().FirstOrDefault(t => t.name.EndsWith("錘") &&
            Vector3.Distance(t.position, bone.position) > .005f);
        if (marker != null) return marker;
        var tip = NewChild(bone, Prefix + "Tip");
        Vector3 offset = bone.parent != null ? bone.position - bone.parent.position : Vector3.down * .02f;
        float length = Mathf.Clamp(offset.magnitude * .5f, .012f, .035f);
        if (offset.sqrMagnitude < 1e-8f) offset = Vector3.down;
        tip.position = bone.position + offset.normalized * length;
        return tip;
    }

    private static SpringCapsuleCollider Capsule(Transform anchor, string name, Vector3 a, Vector3 b, float radius)
    {
        var t = NewChild(anchor, Prefix + name);
        t.position = a;
        t.rotation = Quaternion.FromToRotation(Vector3.up, (b - a).normalized);
        var c = t.GetComponent<SpringCapsuleCollider>() ?? Undo.AddComponent<SpringCapsuleCollider>(t.gameObject);
        c.radius = radius;
        c.height = Vector3.Distance(a, b);
        EditorUtility.SetDirty(c);
        return c;
    }

    private static float CapsuleDistance(SpringCapsuleCollider c, Vector3 point)
    {
        var a = c.transform.position;
        var b = c.transform.TransformPoint(0f, c.height, 0f);
        var ab = b - a;
        var nearest = a + ab * Mathf.Clamp01(Vector3.Dot(point - a, ab) / Mathf.Max(ab.sqrMagnitude, 1e-8f));
        return Vector3.Distance(point, nearest) - c.radius;
    }

    [MenuItem("Tools/Character Physics/Configure Yeshunguang")]
    public static void ConfigureMenu() => Debug.Log(Configure());

    public static string Configure()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode before configuring.");
        var root = GameObject.Find("叶瞬光");
        if (root == null) throw new InvalidOperationException("The scene character 叶瞬光 was not found.");
        var nodes = root.GetComponentsInChildren<Transform>(true).Where(t => !t.name.StartsWith(Prefix)).ToArray();
        var map = nodes.GroupBy(t => t.name).ToDictionary(g => g.Key, g => g.First());
        if (!map.ContainsKey("q_0_0") || !map.ContainsKey("左f_0_1"))
            throw new InvalidOperationException("This setup only supports the audited Yeshunguang skeleton.");
        var weighted = new HashSet<Transform>();
        foreach (var smr in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            foreach (var w in smr.sharedMesh.boneWeights)
            {
                if (w.weight0 > .02f) weighted.Add(smr.bones[w.boneIndex0]);
                if (w.weight1 > .02f) weighted.Add(smr.bones[w.boneIndex1]);
                if (w.weight2 > .02f) weighted.Add(smr.bones[w.boneIndex2]);
                if (w.weight3 > .02f) weighted.Add(smr.bones[w.boneIndex3]);
            }
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Configure Yeshunguang secondary motion");
        Undo.RegisterFullObjectHierarchyUndo(root, "Configure Yeshunguang secondary motion");
        var capsules = new List<SpringCapsuleCollider>();
        Func<Vector3, Vector3> world = p => root.transform.TransformPoint(p);
        capsules.Add(Capsule(map["頭"], "Head", world(new Vector3(0f, 1.445f, .002f)), world(new Vector3(0f, 1.49f, .002f)), .068f));
        capsules.Add(Capsule(map["上半身2"], "Chest", world(new Vector3(0f, 1.13f, .012f)), world(new Vector3(0f, 1.265f, .005f)), .064f));
        capsules.Add(Capsule(map["上半身"], "Abdomen", world(new Vector3(0f, 1.005f, .008f)), world(new Vector3(0f, 1.09f, .008f)), .060f));
        foreach (string side in new[] { "左", "右" })
        {
            float sign = side == "左" ? -1f : 1f;
            capsules.Add(Capsule(map[side + "足D"], side + "Thigh",
                world(new Vector3(sign * .08f, .86f, .006f)), map[side + "ひざD"].position, .053f));
            capsules.Add(Capsule(map[side + "ひざD"], side + "Shin", map[side + "ひざD"].position,
                map[side + "足首D"].position, .036f));
            capsules.Add(Capsule(map[side + "腕"], side + "UpperArm", map[side + "腕"].position,
                map[side + "ひじ"].position, .031f));
            capsules.Add(Capsule(map[side + "ひじ"], side + "Forearm", map[side + "ひじ"].position,
                map[side + "手首"].position, .025f));
        }
        var spheres = new List<SpringSphereCollider>();
        foreach (float sign in new[] { -1f, 1f })
        {
            var t = NewChild(map["下半身"], Prefix + (sign < 0f ? "LeftHip" : "RightHip"));
            t.position = world(new Vector3(sign * .065f, .94f, .003f));
            var c = t.GetComponent<SpringSphereCollider>() ?? Undo.AddComponent<SpringSphereCollider>(t.gameObject);
            c.radius = .057f;
            spheres.Add(c);
        }
        var profiles = new Dictionary<SpringBone, Profile>();
        foreach (var t in nodes)
        {
            if (Helper(t)) continue;
            var p = Classify(t.name);
            if (p == null) continue;
            var tip = Endpoint(t, weighted);
            if (tip == null || Vector3.Distance(t.position, tip.position) < .001f) continue;
            var bone = t.GetComponent<SpringBone>() ?? Undo.AddComponent<SpringBone>(t.gameObject);
            Undo.RecordObject(bone, "Configure spring bone");
            bone.tipNode = tip;
            bone.stiffnessForce = p.stiffness;
            bone.dragForce = p.drag;
            bone.springForce = Vector3.zero;
            bone.windInfluence = p.category == 1 ? .7f : .4f;
            bone.radius = p.radius;
            bone.angularStiffness = p.angular;
            bone.yAngleLimits = new AngleLimits { active = true, min = -p.angle, max = p.angle };
            bone.zAngleLimits = new AngleLimits { active = true, min = -p.angle, max = p.angle };
            var pivot = NewChild(t.parent, Prefix + "Pivot_" + t.name);
            pivot.position = t.position;
            pivot.rotation = Quaternion.FromToRotation(Vector3.left, (tip.position - t.position).normalized);
            if (pivot.GetComponent<SpringBonePivot>() == null) Undo.AddComponent<SpringBonePivot>(pivot.gameObject);
            bone.pivotNode = pivot;
            // Avoid introducing a collision where the authored rest pose is already embedded.
            bone.capsuleColliders = capsules.Where(c => CapsuleDistance(c, tip.position) > p.radius + .001f).ToArray();
            bone.sphereColliders = spheres.Where(c => Vector3.Distance(c.transform.position, tip.position) > c.radius + p.radius + .001f).ToArray();
            bone.panelColliders = new SpringPanelCollider[0];
            bone.lengthLimitTargets = new Transform[0];
            profiles.Add(bone, p);
            EditorUtility.SetDirty(bone);
        }
        // Cross-row ties keep the three strips of each cloth panel together.
        foreach (var bone in profiles.Keys)
        {
            var targets = new List<Transform>();
            var m = Regex.Match(bone.name, @"^q_(\d+)_([012])$");
            if (m.Success)
            {
                int index = int.Parse(m.Groups[1].Value) + 1, strip = int.Parse(m.Groups[2].Value);
                foreach (int other in new[] { strip - 1, strip + 1 })
                    if (map.TryGetValue("q_" + index + "_" + other, out var target)) targets.Add(target);
            }
            m = Regex.Match(bone.name, @"^(左|右)?后q_(\d+)$");
            if (m.Success)
            {
                int index = int.Parse(m.Groups[2].Value) + 1;
                string side = m.Groups[1].Value;
                foreach (string other in side.Length == 0 ? new[] { "左", "右" } : new[] { "" })
                    if (map.TryGetValue(other + "后q_" + index, out var target)) targets.Add(target);
            }
            bone.lengthLimitTargets = targets.ToArray();
        }
        var manager = root.GetComponent<SpringManager>() ?? Undo.AddComponent<SpringManager>(root);
        manager.automaticUpdates = true;
        manager.isPaused = false;
        manager.simulationFrameRate = 60;
        manager.gravity = new Vector3(0f, -.35f, 0f);
        manager.dynamicRatio = 1f;
        manager.enableAngleLimits = true;
        manager.enableCollision = true;
        manager.enableLengthLimits = true;
        manager.bounce = 0f;
        manager.friction = .7f;
        manager.collideWithGround = false;
        manager.FindSpringBones(true);
        EditorUtility.SetDirty(manager);
        PrefabUtility.RecordPrefabInstancePropertyModifications(manager);
        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(root.scene);
        EditorSceneManager.SaveScene(root.scene);
        var report = new StringBuilder();
        report.AppendLine("Unity " + Application.unityVersion);
        report.AppendLine("Spring bones: " + manager.springBones.Length);
        report.AppendLine("Body colliders: " + (capsules.Count + spheres.Count));
        foreach (var g in profiles.GroupBy(p => p.Value.name)) report.AppendLine(g.Key + ": " + g.Count());
        foreach (var item in profiles)
            report.AppendLine(item.Key.name + " | " + item.Value.name + " | tip=" + item.Key.tipNode.name +
                " | limit=" + item.Value.angle + " | colliders=" + (item.Key.capsuleColliders.Length + item.Key.sphereColliders.Length));
        Directory.CreateDirectory("ModelExports/PhysicsReview");
        File.WriteAllText("ModelExports/PhysicsReview/setup-report.txt", report.ToString());
        Selection.activeGameObject = root;
        return string.Join("\n", report.ToString().Split('\n').Take(11));
    }

    [MenuItem("Tools/Character Physics/Validate Yeshunguang")]
    public static void ValidateMenu() => Debug.Log(Validate());

    public static string Validate()
    {
        var source = GameObject.Find("叶瞬光");
        if (source == null || source.GetComponent<SpringManager>() == null) return "Character is not configured.";
        var testScene = EditorSceneManager.NewPreviewScene();
        GameObject clone = null;
        try
        {
            clone = Object.Instantiate(source);
            clone.name = "SecondaryMotionValidationClone";
            SceneManager.MoveGameObjectToScene(clone, testScene);
            var manager = clone.GetComponent<SpringManager>();
            manager.automaticUpdates = false;
            manager.FindSpringBones(true);
            manager.UpdateBoneIsAnimatedStates(new string[0]);
            typeof(SpringManager).GetField("forceProviders", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(manager, new ForceProvider[0]);
            var bones = manager.springBones;
            var rotations = bones.Select(b => b.transform.localRotation).ToArray();
            var lengths = bones.Select(b => Vector3.Distance(b.transform.position, b.ComputeChildPosition())).ToArray();
            foreach (var b in bones) b.Initialize(manager);
            Vector3 origin = clone.transform.position;
            Quaternion rootRotation = clone.transform.rotation;
            var head = clone.GetComponentsInChildren<Transform>().First(t => t.name == "頭");
            Quaternion headRotation = head.localRotation;
            var smr = clone.GetComponentInChildren<SkinnedMeshRenderer>();
            var restMesh = new Mesh();
            smr.BakeMesh(restMesh);
            var restVertices = restMesh.vertices;
            Object.DestroyImmediate(restMesh);
            bool finite = true;
            float maxAngle = 0f, maxLengthError = 0f, maxTipTravel = 0f;
            string worstLengthBone = "", worstMotionBone = "";
            int worstLengthFrame = 0;
            var restTips = bones.Select(b => clone.transform.InverseTransformPoint(b.CurrentTipPosition)).ToArray();
            for (int frame = 0; frame < 1200; frame++)
            {
                float time = frame / 60f;
                bool moving = frame >= 240 && frame < 900;
                clone.transform.position = origin + (moving ? rootRotation * new Vector3(.09f * Mathf.Sin(time * 3f), 0f, .025f * Mathf.Sin(time * 2f)) : Vector3.zero);
                clone.transform.rotation = rootRotation * Quaternion.Euler(0f, moving ? 12f * Mathf.Sin(time * 2f) : 0f, 0f);
                head.localRotation = headRotation * Quaternion.Euler(moving ? 4f * Mathf.Sin(time * 2f) : 0f, moving ? 10f * Mathf.Sin(time * 2.5f) : 0f, 0f);
                manager.UpdateDynamics();
                for (int i = 0; i < bones.Length; i++)
                {
                    var tip = bones[i].CurrentTipPosition;
                    finite &= !float.IsNaN(tip.x) && !float.IsNaN(tip.y) && !float.IsNaN(tip.z) &&
                        !float.IsInfinity(tip.x) && !float.IsInfinity(tip.y) && !float.IsInfinity(tip.z);
                    maxAngle = Mathf.Max(maxAngle, Quaternion.Angle(rotations[i], bones[i].transform.localRotation));
                    float lengthError = Mathf.Abs(Vector3.Distance(bones[i].transform.position, tip) - lengths[i]);
                    if (lengthError > maxLengthError)
                    { maxLengthError = lengthError; worstLengthBone = bones[i].name; worstLengthFrame = frame; }
                    float motion = Vector3.Distance(restTips[i], clone.transform.InverseTransformPoint(tip));
                    if (motion > maxTipTravel) { maxTipTravel = motion; worstMotionBone = bones[i].name; }
                }
            }
            var finalMesh = new Mesh();
            smr.BakeMesh(finalMesh);
            float meshDrift = 0f;
            var finalVertices = finalMesh.vertices;
            for (int i = 0; i < restVertices.Length; i++) meshDrift = Mathf.Max(meshDrift, Vector3.Distance(restVertices[i], finalVertices[i]));
            Object.DestroyImmediate(finalMesh);
            var result = "20-second / 1200-step validation at 60 Hz\n" +
                "Bones: " + bones.Length + "\nFinite particles: " + finite +
                "\nMaximum local rotation from rest: " + maxAngle.ToString("F3") + " degrees" +
                "\nMaximum particle segment length error: " + maxLengthError.ToString("F6") + " m" +
                " (" + worstLengthBone + ", frame " + worstLengthFrame + ")" +
                "\nMaximum endpoint motion: " + maxTipTravel.ToString("F4") + " m" +
                " (" + worstMotionBone + ")" +
                "\nMaximum mesh drift after settling: " + meshDrift.ToString("F4") + " m\n" +
                "PASS: " + (finite && maxAngle < 30f && maxLengthError < .025f && maxTipTravel > .0001f && meshDrift < .15f);
            File.WriteAllText("ModelExports/PhysicsReview/validation-report.txt", result);
            return result;
        }
        finally
        {
            if (clone != null) Object.DestroyImmediate(clone);
            EditorSceneManager.ClosePreviewScene(testScene);
        }
    }
}
