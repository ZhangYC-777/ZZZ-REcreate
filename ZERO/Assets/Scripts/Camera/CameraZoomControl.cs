using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraZoomControl : MonoBehaviour
{
    //获取虚拟相机和FramingTransposer组件
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer framingTransposer;
    //获取鼠标滚动值
    private float scrollValue;
    //声明目标距离
    private float targetDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;
    //滚动灵敏度
    [SerializeField] private float senstivity;
    [SerializeField] private float smoothness;
    [SerializeField] private float defaultDistance;
    private PlayerInputMap playerInputMap;
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        targetDistance = defaultDistance;
        framingTransposer.m_CameraDistance = defaultDistance;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerInputMap = new PlayerInputMap();
        playerInputMap.Enable();
    }
    private void LateUpdate()
    {
        targetDistance -= scrollValue * senstivity;
        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        framingTransposer.m_CameraDistance = Mathf.Lerp(framingTransposer.m_CameraDistance, targetDistance, Time.deltaTime * smoothness);
    }

    // Update is called once per frame
    void Update()
    {
        scrollValue = GetScrollValue();
    }
    //声明一个函数获取鼠标滚动值
    private float GetScrollValue()
    {
        return playerInputMap.Player.MouseScroll.ReadValue<Vector2>().y;
    }
}
