# UnityChan SpringBone installation

Installed from the public Unity Japan repository on 2026-10-16:
https://github.com/unity3d-jp/UnityChanSpringBone/tree/release/1.1

Upstream version: 1.1.0-preview. The upstream MIT license files are preserved.

Source archive: ModelExports/Downloads/UnityChanSpringBone-release-1.1.zip
SHA-256: 8F2A5E7946A4BA3013C0675BFF0EBCA0566E4454147285F4940A45C870A51AE7

This is an Assets-based source installation. Find its files under
Assets/ThirdParty/UnityChanSpringBone; it is not a registry package in Package Manager.

Open the setup window from **Tools > UnityChan SpringBone**.
The original **UTJ > スプリングボーン窓** menu also remains available.
Components include SpringManager, SpringBone, SpringBonePivot,
SpringSphereCollider, SpringCapsuleCollider, and SpringPanelCollider.

Local compatibility changes:
- Restrict the editor assembly to the Editor platform, so it is excluded from player builds.
- Correct the setup window's icon search to match the repository's Editor/GUI/Icons folder.
- Add an English menu alias without changing the simulation code.
- Subsequent character setup adds an optional `tipNode` endpoint override to SpringBone.
  When assigned, it avoids averaging MMD mass helpers with the actual next joint.
  Unassigned components retain upstream endpoint selection behavior.

The original installation only verified editor compilation and availability of component types.
Subsequent user-authorized character setup is documented in ModelExports/PhysicsReview/README.md.
Player builds still require separate testing.
