# 人物动画提取与适配进度

更新时间：2026-10-19（北京时间）。动画资源已完成交付，状态机仍由用户搭建。

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
5. 最终交付 **96 个 `.anim`**：48 段动作各有 InPlace / RootMotion 两版，已放入正式工程：
   `ZERO/Assets/Animations/Characters/`。
   附带 `Miyabi_TargetAvatar.asset`、`Yeshunguang_TargetAvatar.asset`、完整动作清单及使用说明。
6. 最终采用三次曲线关键帧精简，动画从约 **1.19 GB 降到 342,840,604 字节（约 343 MB）**，关键帧从 2,484,056 减到 694,862。
   全部 96 段重新载入后，以 60 Hz 比较所有人形骨骼：最大位置差约 **2.215 mm**，最大旋转差约 **0.241°**，0 个错误。循环接缝检查通过。
7. 对全部 96 个片段进行真实递进时间播放检查，并以现有模型原导入设置的副本 + 专用 Avatar 再次验证；重新导入、骨骼和姿势比较均通过。
8. 渲染并检查了 **48 张四帧动作预览（共 192 帧）**，另附 6 页动作速览。
   预览使用逐帧 BakeMesh 刷新蒙皮，修复了初版离线相机缓存同一姿势的问题；早期 `Previews/` 的 30 张图不应作为最终预览。

## 已解决的问题与交付边界

- 叶瞬光 `Run_End` 的约 2.55 m 原地漂移已修复。原因是源动画起始身体水平位置偏约 2 m，固定偏移值仍会在转身时造成漂移；最终把 InPlace 的 `RootT.x/z`（以及存在时的 `MotionT.x/z`）归零，并将位置/高度/旋转 Bake Into Pose。修复后水平髋部变化约 **0.164 m**，根物体不平移。
- 最终 `.anim` 为 **文本格式**。实验性二进制保存虽一度减小文件，但在文本序列化项目重新导入后会自动展开，未作为最终方案。`binary-verified.txt`、`binary-sizes.tsv` 等实验记录不是最终验收依据。
- 最终交付目录不包含状态机、Animator Controller、运行时脚本或测试场景。未修改主场景、相机、模型导入设置或 SpringBone 配置。
- 身体与手指已适配；原游戏武器、刀鞘、特效和附属骨骼轨道没有移植。本轮预览没有贴图和衣物物理，不能视为最终战斗渲染效果。
- 主场景 Animator 尚未代为指定新 Avatar；用户接入时需按 `Characters/README.md` 指定对应 TargetAvatar。星见雅 FBX 仍在 `ModelExports/Unity/星见雅/`，尚未搬入 ZERO。
- 源资源名 `Attack_06`、`_02` 等保留为普攻分支；未确认与游戏逻辑中每一普攻段的严格对应。闪避/后摇常自带数秒回待机尾段，可按用户自己的状态机裁剪或提前切换。
- **用户可从正式资源目录开始接入，动画提取与身体适配工作已完成。**

## 正式交付位置

- `ZERO/Assets/Animations/Characters/README.md`：Avatar 指定、InPlace / RootMotion 用法、完整动作表与适配边界。
- `ZERO/Assets/Animations/Characters/AnimationCatalog.csv`：96 个动画的时长、循环、用途、路径、根运动净位移。
- `ZERO/Assets/Animations/Characters/Yeshunguang/`：16 段动作的两版及专用 Avatar。
- `ZERO/Assets/Animations/Characters/Miyabi/`：32 段动作的两版及专用 Avatar。
- `ZERO/AnimationReview/`：最终数值/姿势/兼容性报告、资源 SHA-256、预览和速览。验证在独立工程完成，没有向主工程添加验证脚本。

## 持久保存的位置

工作原本位于 `ZERO/Temp/AnimationExtraction/`，为避免 Unity 清理 Temp，已复制到仓库根目录 **`AnimationWork/`**。
该目录通过 `.gitignore` 排除，保存在本机，不应把大量解包资源和工具直接提交到 Git。

- `AnimationWork/Raw/Miyabi/`：32 个原始骨架动画 FBX。
- `AnimationWork/Raw/Yeshunguang/`：16 个原始骨架动画 FBX。
- `AnimationWork/FastMaps/`：四份 assets/cabs JSONL 索引及扫描日志。
- `AnimationWork/Miyabi-plan.json`、`Yeshunguang-plan.json`：选定的动画、源文件、CAB 偏移和 Animator/Avatar ID。
- `AnimationWork/ValidationProject/Assets/Targets/`：用于验证的两个 MMD 模型副本，已修正 Humanoid。
- `AnimationWork/ValidationProject/Assets/Sources/`：原骨架动画 FBX 及导入设置。
- `AnimationWork/ValidationProject/Assets/Adapted/`：96 个早期候选动画，以及两个 `*_TargetAvatar.asset`。
- `AnimationWork/ValidationProject/Assets/Final/`：最终精简版动画，已复制到正式交付目录。
- `AnimationWork/ValidationProject/delivery-validation.json`、`final-review.json`、`integration-verified.txt`：最终检查依据。
- `AnimationWork/ValidationProject/FinalPreviews/`、`ReviewSheets/`：最终预览。
- `AnimationWork/ValidationProject/Assets/Editor/AnimationDelivery.cs`：最终曲线精简与原地修正。
- `AnimationWork/ValidationProject/Assets/Editor/AnimationFinalReview.cs`：最终姿势对比、兼容性检查和离线渲染。
- `AnimationWork/publish_animations.py`：已执行的交付复制、校验与目录/说明生成程序。
- `AnimationWork/ValidationProject/validation.json`：早期候选的逐帧数值检查报告，最终请用 delivery-validation / final-review。
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
- 2026-10-19 最终 `rebuild.log`、`preview-final.log` 均以退出码 0 完成。曾有一次自动权限审批超时，按工具提示重试后成功；没有遗留本轮扫描器或批处理 Unity 进程。用户原本打开的 Unity 编辑器未关闭。

## 后续接入

### 2026-10-21 临时演示已按用户要求清理

- 用户查看临时优化后明确要求删除。已切回 `ZERO/Assets/Scenes/CharacterPreview.unity`，移除整个 `ZERO/Assets/Temporary3CDemo/` 及其 `.meta`，包括演示场景、玩家 prefab、Controller、输入/状态逻辑、衣摆运行时优化、测试剑和测试报告。
- 同时移除本轮 `AnimationWork/DemoOptimizationBackup-20261021/`、`AnimationWork/DemoOptimizationReview/`、检查日志与 `mcp_demo.py`。此前对比视频和临时演示不再存在。
- **正式动画与专用 Avatar、原模型和武器 FBX、原物理插件、相机、地形和原场景保留。** 原始提取资料、工具及独立动画验证工程继续保留在 AnimationWork；没有删除整个目录。
- 临时优化没有合入正式状态机或正式动画。后续仍由用户自己搭建状态机；如果需要优化衣摆或移植武器独立轨道，应重新明确范围。
- 清理后复核：210 个受保护文件内容不变，96 个正式 `.anim` 保留；剩余场景/预制体/资源没有对已删除测试资产的引用。Unity 当前为已保存的 CharacterPreview 场景，未播放、未编译，Missing Script 为 0，控制台错误/警告为 0。
- 提取工具记录不变：最初使用 Escartem/AnimeStudio，最终 48 段成功解码使用 AnimeStudio Ultimate 1.7.0。

1. 先阅读正式 `Characters/README.md`，在角色的 Animator 指定对应 TargetAvatar。建议先用 InPlace，人物位移由用户现有控制逻辑负责。
2. 用户自行搭状态机、设置动作混合和取消时机；不要自动代建。
3. 若继续做武器/刀鞘挂点或附属骨骼适配，应作为新范围确认；现有身体动画已交付。
4. 如需重做身体动画，在独立验证工程运行 `AnimationFinalReview.Rebuild`（需图形设备），它会依次精简、逐帧比较、检查模型兼容性和渲染。完成后重新生成速览并检查，再复制交付。不要使用旧的实验性 `Finish` / `SaveBinary` 作为最终处理流程。
