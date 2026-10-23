# ZZZ-REcreate
类绝区零战斗demo

## 当前开发工程（2026-10-16 迁移）

- **旧版工程：`ZERO/`，使用 Unity `2022.3.62f1c1`**，本机编辑器为 `E:/unity/Editor/Unity.exe`。
- 打开 `Assets/Scenes/CharacterPreview.unity`。叶瞬光的现有 FBX、Humanoid、Toon 材质、UnityChan SpringBone 配置已迁入。
- 已安装 Cinemachine **2.10.7**、URP **14.0.12**、Animation Rigging **1.2.1** 和 MCP for Unity **10.0.0**。
- 原 `ZZZ/` Unity 6 工程保留；无需重新经过 Blender 导出模型。
- [迁移、验证与继续操作说明](ZERO/README.md)。

## Unity MCP

- 原 Unity 6 工程目录：`ZZZ/`，当前编辑器版本：`6000.6.3f1`。
- 已在 `ZZZ/Packages/manifest.json` 安装 [MCP for Unity](https://github.com/CoplayDev/unity-mcp)，固定为 `v10.0.0`，与本机其他 Unity 项目保持一致。
- 本机 Codex 已有全局配置 `unityMCP`，地址为 `http://127.0.0.1:8080/mcp`，无需重复添加项目配置。
- 在 Unity 中通过 `Window → MCP for Unity → Toggle MCP Window` 查看连接；如果服务或会话未启动，在连接面板中启动本地服务并连接会话。本机已安装所需的 `uv` / `uvx`。
- 多工程同时连接时，旧版工程选择 `ZERO@64c01837ad63e7cf`；原工程当前实例为 `ZZZ@4d2a7a3b93c205a8`。
- 若 Codex 聊天尚未加载 Unity 工具，在服务启动后重新打开聊天。

2026-10-14 已通过实际 MCP 协议握手、项目实例识别、场景与层级读取验证：当前场景为 `SampleScene`，含 `Main Camera`、`Directional Light`、`Global Volume`，控制台错误为 0。验证未修改场景。
