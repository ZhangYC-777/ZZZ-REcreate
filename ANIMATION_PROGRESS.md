# 人物动画提取与适配进度

记录时间：2026-10-18 22:59（北京时间）。用户暂时没有时间，本轮在此保存进度，后续从剩余问题继续。

## 用户确定的范围

- **状态机由用户自己搭建，不要代写、代建状态机或 Animator Controller。**
- 只提供适配现有模型的动画：叶瞬光、星见雅。
- 优先跑步、闪避、普攻，以及跑步停止、普攻结束等后摇；技能动画暂不处理。
- 当前工程为 `ZERO/`，Unity 2022.3.62f1c1。已存在叶瞬光模型与 SpringBone 配置；星见雅 FBX 在 `ModelExports/Unity/星见雅/`。

## 已完成

1. 定位本机国服《绝区零》3.2.0：
   `D:\miHoYo Launcher\games\ZenlessZoneZero Game`。
   只读取游戏文件，没有修改游戏安装目录。
2. 完成 10,094 个资源文件的定向扫描。角色内部名：星见雅 `Unagi`，叶瞬光 `Zhenzhen`。
3. 成功导出 **48 段候选身体动画**：星见雅 32 段，叶瞬光 16 段。
   包含两人的跑步循环、停跑、前后闪避，以及普攻和各段收招；星见雅还有起跑和方向变体。额外包含待机/AFK，尚可在交付时精简。
4. 在独立临时 Unity 工程修正了目标 MMD 模型的 Humanoid 映射：
   星见雅自动映射错误地选择了左袖骨骼；两人还有拇指/食指、头发/眼睛映射问题。
   已生成两个独立 TargetAvatar 资源，**尚未替换主工程模型或场景中的 Avatar**。
5. 转换成 **96 个候选 `.anim`**：每段 InPlace / RootMotion 两版。
   最后一次数值验证：96 个片段、18,782 次采样、0 个异常报告。
   数值检查包括人形动画标记、曲线数值、关键身体关节运动及中途蒙皮网格尺寸。
6. 已渲染 30 张姿态预览，**尚未人工看图检查**。

## 仍需处理，不能直接视为最终交付

- **叶瞬光 `Run_End` 原地版仍有约 2.55 米的髋部水平位移。**
  其 RootMotion 版在关闭根运动的定时采样中也出现同样位移，需要检查源动画、根曲线/IK 曲线与采样方式。
  其他已抽查的原地片段水平髋部变化已降至约 0.07–0.25 米（正常身体摆动范围），但尚未逐个确认完整世界位移行为。
- 第一版通过 Bake Into Pose 制作的 InPlace 有明显漂移，已用 RootMotion 副本固定 `RootT.x/z`（以及存在时的 `MotionT.x/z`）重做。不要使用早期版本。
- `.anim` 仍是 YAML，总大小约 **1.19 GB**。尝试 `EditorSettings.serializationMode=ForceBinary` + `ForceReserializeAssets` 并未实际转成二进制。
  `binary-verified.txt` **仅代表资源重新加载成功，不能据此宣称已转成二进制或缩小体积**。
  后续可做有误差上限的关键帧精简，或验证真正的原生二进制保存方法，再复测。
- 尚未将成品放入主工程正式 `Assets/Animations`，尚未生成最终动作目录、接入说明或完成正式交付。
- 当前候选只保留 Animator 人形身体/手指曲线；游戏专用武器、刀鞘、特效骨骼轨道未移植到 MMD 模型。角色武器挂点与现有布料物理应单独核实，不要宣称整套游戏表演已经复原。

## 持久保存的位置

工作原本位于 `ZERO/Temp/AnimationExtraction/`，为避免 Unity 清理 Temp，已复制到仓库根目录 **`AnimationWork/`**。
该目录通过 `.gitignore` 排除，保存在本机，不应把大量解包资源和工具直接提交到 Git。

- `AnimationWork/Raw/Miyabi/`：32 个原始骨架动画 FBX。
- `AnimationWork/Raw/Yeshunguang/`：16 个原始骨架动画 FBX。
- `AnimationWork/FastMaps/`：四份 assets/cabs JSONL 索引及扫描日志。
- `AnimationWork/Miyabi-plan.json`、`Yeshunguang-plan.json`：选定的动画、源文件、CAB 偏移和 Animator/Avatar ID。
- `AnimationWork/ValidationProject/Assets/Targets/`：用于验证的两个 MMD 模型副本，已修正 Humanoid。
- `AnimationWork/ValidationProject/Assets/Sources/`：原骨架动画 FBX 及导入设置。
- `AnimationWork/ValidationProject/Assets/Adapted/`：96 个候选动画，以及两个 `*_TargetAvatar.asset`。
- `AnimationWork/ValidationProject/validation.json`：最后一次逐帧数值检查报告。
- `AnimationWork/ValidationProject/motion-metrics.json`：跑步、停跑、闪避、普攻的抽查位移数据。
- `AnimationWork/ValidationProject/Previews/`：30 张灰色材质姿态预览 PNG。
- `AnimationWork/ValidationProject/Assets/Editor/AnimationAdapter.cs`：映射、转换、验证、原地修正辅助代码，仅在验证工程中。
- `AnimationWork/ValidationProject/Assets/Editor/AnimationPreviewRender.cs`：姿态渲染及序列化尝试代码。
- `AnimationWork/Exporter/`：定向导出辅助程序源码与可执行文件。
- `AnimationWork/Scanner/`：扫描辅助程序源码与可执行文件。
- `AnimationWork/Ultimate/`：实际成功解码使用的 AnimeStudio Ultimate 1.7.0 .NET 10 发布包。
- `AnimationWork/Source/`、`AnimeStudio/`：最初普通 AnimeStudio 版本的源码和程序，留作对照。

## 工具兼容与本轮报错

- 普通 AnimeStudio master 能读取 3.2.0 包，但导出普攻时 `ModelConverter.ReadCurveData` 越界，不能用于本次最终解码。
- 使用 [AnimeStudio Ultimate 1.7.0](https://github.com/LegionInstaller/AnimeStudioUltimate/releases/tag/v1.7.0) 后，48/48 动画已成功导出。
- Exporter 项目引用 Ultimate DLL。重建时 `PreserveNewest` 可能留下旧版本原生 DLL；需要用 `Ultimate/bin/*.dll` 显式覆盖 Exporter 输出目录，避免 `DecompressTracksZZZ` 入口点找不到。
- 用户看到过的“应用程序错误”来自扫描器同时初始化 `CNKeys.json` 的文件争用。
  已停止出错进程、预先复制配置、加顶层异常捕获，修复后四个扫描进程全部完成，错误日志均为空。不是游戏资源或主工程损坏。
- 最后一次 Unity 批处理 `finalize.log` 已正常以退出码 0 结束；记录时没有遗留本轮扫描器或批处理 Unity 进程。用户原本打开的 Unity 编辑器未关闭。

## 下次继续的顺序

1. 从 `AnimationWork/ValidationProject` 继续，不依赖可能被清理的旧 Temp。Library 缓存未备份，Unity 首次打开会重新导入。
2. 先检查叶瞬光 `Run_End` 的身体漂移原因，修正并验证原地版；复核 RootMotion 版在真实递进时间播放时的行为。
3. 看姿态预览，必要时以贴图/武器挂点补充检查，确认身体骨骼和手指动作可用。
4. 处理候选动画体积，复测变更后的曲线误差与播放。
5. 将通过检查的 `.anim`、对应 TargetAvatar、`.meta` 放入主工程正式动画目录，附动作名称/时长/循环设置清单和 Avatar 使用说明。
6. 保持用户的状态机、相机、场景和现有物理配置不受无关修改。
