# 叶瞬光的衣服、头发与饰物动态

本次配置使用免费开源的 UnityChan SpringBone。场景为 `Assets/Scenes/SampleScene.unity`。
角色保留现有 Humanoid、材质和骨骼层级，弹簧组件只添加到审查过的衣摆、飘带、发束、尾饰和挂饰上。
头皮、腰部连接处、身体、脸、眼睛、手指、IK 以及 MMD 辅助骨均不直接参与弹簧模拟。

## 查看效果

1. 打开 SampleScene，点击 Play。
2. 在 Hierarchy 选择最外层的“叶瞬光”。
3. 找到 `Secondary Motion Preview` 组件，勾选 `Preview Motion`。
4. 人物会轻轻移动、转身，从而带动衣摆、长发和饰物。
5. 取消勾选会回到原位；停止 Play 会退出预览。

预览默认关闭，只是方便在尚未配置走路动画时观察物理效果。未来正常播放人物动画时，无需开启它。
在 Play 模式中的临时参数修改，停止后会由 Unity 还原；要保存调参，请记下数值，退出 Play 后再填写。

## 分组与参数

根物体的 `Spring Manager` 管理全部弹簧骨骼。每根骨骼仍保留独立的 `Spring Bone` 组件，
可以用原插件窗口检查与编辑。选中一个骨骼后，勾选 `標準インスペクター表示` 可以显示标准字段，
其中 `Tip Node` 明确指定实际末端，避免 MMD 的“錘”辅助骨干扰计算。

`Assets/Editor/YeshunguangSecondaryMotionSetup.cs` 集中记录各组参数和固定连接处的选择规则。
这是针对当前模型的工具，不适用于任意模型。

菜单 `Tools > Character Physics > Configure Yeshunguang` 会重新应用这些默认参数，
因此手动调参后不要随意重复运行它。菜单 `Validate Yeshunguang` 使用独立副本运行数值检查。

## 文件

- `bone-audit.txt`：骨骼的父子关系、位置、顶点权重与相关材质。
- `setup-report.txt`：实际添加的骨骼、分组、末端、角度限制与碰撞体数量。
- `validation-report.txt`：运动、转头和停止后的稳定性检查结果。
- 场景备份：`Assets/Scenes/Backups/SampleScene_BeforeSecondaryMotion_20261016.unity`。

身体碰撞体随对应骨骼运动，衣摆之间加入了相邻行的软距离约束。
这仍然是骨骼物理：没有发束之间、布料表面之间的完整自碰撞。
当前参数为保守的起始设置；加入实际跑步、攻击和快速转身动画后，需要再针对那些动作调参。
