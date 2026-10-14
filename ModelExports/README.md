# PMX → Blender → Unity：Mac 与 Windows 操作方法

2026-10-14：本机 Blender 5.2.2 LTS 的旧 CATS 0.19.0 在启用时使用了已经移除的 `TESTING` 分类，导致安装报错。已安装并启用 NeoNeko 社区分支 CATS 5.2.4.1，旧插件保持停用。Blender 插件信息里显示 5.2.4 是安装包清单的版本号；其代码版本为 5.2.4.1。

## 本次转换结果

`Unity/` 内的两个角色文件夹可整体复制到 Unity 项目的 `Assets/Art/Characters/` 下。FBX 与 `Textures/` 应一起复制，保持目录结构。

| 文件 | 三角面 | 骨骼 | 顶点表情 |
| --- | ---: | ---: | ---: |
| 星见雅/星见雅.fbx | 33528 | 484 | 52 |
| 星见雅/武器.fbx | 4353 | 17 | 0 |
| 星见雅/幽灵.fbx | 1306 | 1 | 0 |
| 叶瞬光/叶瞬光.fbx | 64619 | 672 | 57 |
| 叶瞬光/叶瞬光_白.fbx | 69951 | 776 | 57 |
| 叶瞬光/武器.fbx | 4196 | 12 | 0 |

已重新导入每个 FBX，核对三角面、骨骼、顶点表情数量均一致，并核对原 PMX 引用的贴图均存在。详见 `conversion_report.json`。这些检查在 Blender 完成，尚未在 Unity 检验 Humanoid Avatar 或运行时动画。

`Blender/` 保存导入后的工作文件与原 MMD 材质节点，供继续编辑。跨电脑搬运时保留整个 `ModelExports/` 的目录结构，贴图路径使用相对路径。原始 PMX 仍在下载文件夹。

本次导出保留原骨骼与网格，没有运行 CATS 自动修骨、减面、合并材质或材质图集操作。FBX 材质已转成基础标准材质，携带基础颜色、透明度和主贴图引用。原 MMD toon/sphere 材质、材质与骨骼类型的 morph、刚体、关节和 MMD IK 行为不会由该 FBX 流程完整迁移到 Unity；Unity 中应另设渲染材质、动画骨骼映射和头发衣物物理。

## 当前合适的方法

| 系统 | 推荐流程 | 适用情况 |
| --- | --- | --- |
| macOS | Blender 5.2 LTS + MMD Tools 4.x → FBX → Unity | 只需要 PMX 导入与格式转换 |
| Windows | Blender 5.2 LTS + MMD Tools 4.x → FBX → Unity | 与 Mac 相同，可共用 FBX 和贴图 |
| 两个系统 | Blender 5.2 + NeoNeko CATS 5.2.4.1 → FBX → Unity | 还需要辅助整理骨骼、模型或表情 |

MMD Tools 官方支持 Blender 4.2–5.2。CATS 上述发布版本支持 Blender 5.1 与 5.2。以后升级 Blender 时，应先核对插件对应版本。

### 方法一：MMD Tools 做基础转换

1. 解压完整模型包，保留 `.pmx` 和 `tex/`、`spa/` 等贴图目录。模型通常从作者或发布平台下载，再导入 Blender。
2. Blender：`编辑 → 偏好设置 → 获取扩展`，搜索 `MMD Tools` 并安装。离线安装时从官方扩展页下载 ZIP，使用右上角菜单的 `从磁盘安装`，直接选择 ZIP。
3. `文件 → 导入 → MikuMikuDance Model (.pmd, .pmx)`，选择角色 PMX。导入缩放可以从 `0.08` 开始，再检查人物实际身高。本次星见雅网格高度约 1.715 米。
4. 保留网格、骨骼和 morph。只做 Unity 转换时可取消 MMD 的刚体/关节与显示框导入。先保存 `.blend` 工作文件。
5. 选中模型网格和 Armature，使用 `文件 → 导出 → FBX`。开启 `Selected Objects`，对象类型选 `Mesh` 和 `Armature`；`Forward = -Z`、`Up = Y`；关闭 `Add Leaf Bones`；没有动画时关闭 `Bake Animation`。有顶点表情时关闭 `Apply Modifiers`。为了保留本次原始骨架，未启用 `Only Deform Bones`。
6. FBX 的材质导出需要基础标准节点；MMD 专用节点不能保证直接还原。把主贴图接到 Principled BSDF 的 Base Color，或者在 Unity 中按 `conversion_report.json` 中的材质贴图对应关系重新指定。随 FBX 复制贴图，不依赖下载文件夹的绝对路径。

### 方法二：使用本机已安装的新版 CATS

1. 在 Blender 3D 视图按 `N`，打开侧栏的 `CATS` 页签。
2. 使用 `Import Model / 导入模型` 导入 PMX。
3. 检查贴图与人物尺寸，保存 `.blend`。如果需要 `Fix Model / 修复模型` 等整理工具，在另存的工作副本上操作，再检查骨骼、权重和表情。
4. 使用 `Export Model / 导出模型` 输出 FBX，随 FBX 保留贴图。

Windows 安装同一版本 CATS 时，先停用旧版，再从发布页下载 `cats_blender_plugin-5.2.4.1.zip`，不要选择 Source code ZIP，也不要预先解压；在偏好设置右上角选择 `从磁盘安装`，安装并启用。

## Unity 导入后的设置

1. 把整个角色目录放进 `Assets`。直接使用导出的 FBX，团队成员无需安装 Blender 来导入这些 FBX。
2. 在 FBX Inspector 的 `Rig` 页先用 `Generic` 检查骨架和模型是否可用。要复用人形动画时改为 `Humanoid → Create From This Model → Apply → Configure`，检查必需的人体骨骼映射与 T Pose；原骨骼名保留，不能保证自动映射成功。
3. 武器通常作为手部骨骼的子物体；无需强行将武器和幽灵设置为 Humanoid。
4. 本项目安装了 URP。材质使用 `Universal Render Pipeline/Lit` 或相容的 Toon Shader，将主贴图放到 Base Map；透明、Alpha Clipping 与双面选项按实际材质设置。粉色通常应先检查 Shader 与渲染管线是否匹配。
5. 要达到游戏中的二次元效果，后续还需 Toon Shader、光照与脸部阴影设置。头发和衣物物理需要在 Unity 另行配置。

## 官方来源

- [NeoNeko CATS 5.2.4.1 发布与安装包](https://git.neoneko.org/NeoNeko/Cats-Blender-Plugin/releases/tag/5.2.4.1)
- [NeoNeko CATS 安装说明](https://git.neoneko.org/NeoNeko/Cats-Blender-Plugin)
- [MMD Tools 官方仓库、版本兼容和安装说明](https://github.com/MMD-Blender/blender_mmd_tools)
- [MMD Tools 官方 Blender 扩展页](https://extensions.blender.org/add-ons/mmd-tools/)
- [Unity 模型文件格式说明](https://docs.unity3d.com/Manual/3D-formats.html)
- [Unity Humanoid Avatar 配置](https://docs.unity3d.com/Manual/ConfiguringtheAvatar.html)
