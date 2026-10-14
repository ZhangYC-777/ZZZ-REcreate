# ZZZ-REcreate
类绝区零战斗demo

## Unity MCP

- Unity 工程目录：`ZZZ/`，编辑器版本：`6000.5.10f1`。
- 已在 `ZZZ/Packages/manifest.json` 安装 [MCP for Unity](https://github.com/CoplayDev/unity-mcp)，固定为 `v10.0.0`，与本机其他 Unity 项目保持一致。
- 本机 Codex 已有全局配置 `unityMCP`，地址为 `http://127.0.0.1:8080/mcp`，无需重复添加项目配置。
- 在 Unity 中通过 `Window → MCP for Unity → Toggle MCP Window` 查看连接；如果服务或会话未启动，在连接面板中启动本地服务并连接会话。本机已安装所需的 `uv` / `uvx`。
- 若 Codex 聊天尚未加载 Unity 工具，在服务启动后重新打开聊天；多个 Unity 项目同时连接时，先通过 `set_active_instance` 选择 `ZZZ@5e3f86368d46f1f6`。

2026-10-14 已通过实际 MCP 协议握手、项目实例识别、场景与层级读取验证：当前场景为 `SampleScene`，含 `Main Camera`、`Directional Light`、`Global Volume`，控制台错误为 0。验证未修改场景。
