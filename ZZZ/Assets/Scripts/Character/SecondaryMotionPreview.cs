using UnityEngine;

/// <summary>Optional movement for inspecting secondary motion without an animation clip.</summary>
[DisallowMultipleComponent]
public sealed class SecondaryMotionPreview : MonoBehaviour
{
    [Header("运行时预览：勾选后人物会轻轻移动、转身；关闭后回到原位")]
    public bool previewMotion;
    [Range(0f, 0.15f)] public float movementAmplitude = 0.06f;
    [Range(0f, 20f)] public float turnDegrees = 8f;
    [Range(0.1f, 2f)] public float frequency = 0.6f;
    [Header("已配置的弹簧骨骼数量")]
    public int clothingBones;
    public int hairBones;
    public int accessoryBones;

    private bool moving;
    private float elapsed;
    private Vector3 origin;
    private Quaternion rotation;

    private void Update()
    {
        if (!previewMotion)
        {
            Restore();
            return;
        }
        if (!moving)
        {
            origin = transform.localPosition;
            rotation = transform.localRotation;
            elapsed = 0f;
            moving = true;
        }
        elapsed += Time.deltaTime;
        float phase = elapsed * frequency * Mathf.PI * 2f;
        transform.localPosition = origin + rotation * new Vector3(
            movementAmplitude * Mathf.Sin(phase), 0f,
            movementAmplitude * 0.3f * Mathf.Sin(phase * 0.7f));
        transform.localRotation = rotation * Quaternion.Euler(
            0f, turnDegrees * Mathf.Sin(phase * 0.8f), 0f);
    }

    private void Restore()
    {
        if (!moving) return;
        transform.localPosition = origin;
        transform.localRotation = rotation;
        moving = false;
    }

    private void OnDisable() => Restore();
}
