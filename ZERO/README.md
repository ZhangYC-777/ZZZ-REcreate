# ZERO：Unity 2022.3 迁移工程

2026-10-16 从同仓库的 `ZZZ/`（Unity 6000.6.3f1）迁入，继续采用之前的模型、渲染和骨骼动态流程。

2026-10-19 已加入叶瞬光、星见雅的跑步、停跑、闪避、普攻及收招动画，共 48 段动作的 InPlace / RootMotion 两版。接入前阅读 [动画与 Avatar 使用说明](Assets/Animations/Characters/README.md)，动作预览和验证报告见 [AnimationReview](AnimationReview/README.md)。状态机由开发者自行搭建。

## 打开与查看

1. 用 **Unity 2022.3.62f1c1** 打开此目录。本机对应 `E:/unity/Editor/Unity.exe`。
2. 打开 **`Assets/Scenes/CharacterPreview.unity`**，这是已迁移的人物场景，也是当前构建列表中的场景。
3. 点击 Play，选中最外层 **叶瞬光**，在 `Secondary Motion Preview` 中勾选 `Preview Motion`，即可观察衣摆、头发、飘带与饰物摆动。预览默认关闭；停止播放后会恢复。
4. 调整骨骼物理仍使用 **Tools → UnityChan SpringBone**，根物体上的 `Spring Manager` 管理整套动态。

`Assets/Scenes/SampleScene.unity` 是 ZERO 建立时的空场景，予以保留。原 ZZZ 工程与仓库根目录 `ModelExports/` 中的 Blender、FBX 导出资源也保留。

## 沿用的处理方式

- 直接复用原工程内已导入的叶瞬光、武器 FBX 及贴图；保留资源 GUID 和材质映射，无需重新运行 Blender 转换。
- 保留原 Humanoid Avatar 与骨骼层级、现有 Toon Shader、24 个角色材质、相机、主光和后处理参数。
- 服装、头发、发饰的动态继续用 **UnityChan SpringBone**：**339 个弹簧骨骼、13 个身体碰撞体**。保留此前对 MMD “錘”辅助骨的明确末端 `tipNode` 修正与分组参数。
- Animation Rigging 用于原来的 `Bone Renderer` 骨骼显示；动态计算仍由 UnityChan 完成。
- 在 URP 14 中重新建立 `LegacyURP.asset` 和 `LegacyRenderer.asset`，使用 Forward+、深度/不透明纹理与原强度的 SSAO；所有质量档均指定兼容管线。

## 已固定的包

| 包 | 版本 |
| --- | --- |
| Cinemachine | **2.10.7** |
| Universal RP | **14.0.12** |
| Animation Rigging | **1.2.1** |
| Input System | **1.11.2** |
| MCP for Unity | **10.0.0**，与原工程相同的 Git 标签 |

Cinemachine 2.x 保留 `CinemachineVirtualCamera`、FreeLook 和 `Cinemachine` 命名空间的工作流。主要差异来自 Cinemachine 的 2.x/3.x 版本，Unity 版本号并不直接代表相机插件的版本。
此场景沿用原来的 Main Camera；尚未额外建立跟随相机或人物移动控制。旧输入方式保持启用，可用于后续 Cinemachine 2.x 教程；同时保留 Input System 包和原动作资源。

参考：[Cinemachine 2.10 官方文档](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.10/manual/index.html)、[Cinemachine 3 的迁移差异](https://docs.unity.cn/Packages/com.unity.cinemachine@3.1/manual/CinemachineUpgradeFrom2.html)。

## MCP

继续使用本机既有的 `http://127.0.0.1:8080/mcp` 服务。多个工程同时连接时，操作此工程前选择 **`ZERO@64c01837ad63e7cf`**。
如需重新连接，可用 **Tools → Project Migration → Connect Local MCP**，或者从 Unity 的 MCP 面板连接会话。

## 验证与备份

已在 Unity 2022.3 实测：

- Humanoid Avatar 有效；339 个弹簧骨骼及管理器、末端、轴心引用完整；13 个碰撞体保留。
- 场景的所有根物体均无 Missing Script；24 个材质的 Shader 可用。
- 原来的 20 秒、1200 步运动与转头测试通过；停止后的最大网格偏移约 **1.34 厘米**。
- 实际 Play 运行超过 30 秒，开启动态预览后 **339/339** 个端点坐标正常；Console 错误与警告均为 **0**，运行画面已检查。
- 本次完成编辑器迁移与播放验证，未执行 Player 构建。后续加入正式跑步、攻击等动画时，仍按之前流程调节动态参数。

`MigrationReview/` 保存验证报告、实际运行截图、迁移前 ZERO 的 Assets/Packages/ProjectSettings 备份，以及原工程当前未保存状态的场景副本。原骨骼配置说明与报告在 `MigrationReview/SourcePhysicsReview/`。

需要复查时，可运行 **Tools → Project Migration → Validate Unity 2022 Migration**。
`Finish Unity 2022 Migration` 用于初次建立迁移场景设置，会重新指定管线、质量档和构建场景；常规开发无需重复运行。
原有 **Configure Yeshunguang** 会重新应用默认物理参数，手动调参后请勿随意运行。
