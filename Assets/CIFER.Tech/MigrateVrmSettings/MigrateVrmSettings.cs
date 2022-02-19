using System.Collections.Generic;
using System.Linq;
using CIFER.Tech.Utils;
using UnityEngine;
using VRM;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CIFER.Tech.MigrateVrmSettings
{
    public static class MigrateVrmSettings
    {
        public static void Resetting(MigrateVrmSettingsData data)
        {
            #region Meta

            if (data.IsMeta)
                data.NewVrm.Meta = data.OldVrm.Meta;

            #endregion

            #region BlendShape

            if (data.IsBlendShape)
            {
                var newVrmBlendShapeProxy = data.NewVrm.GetComponent<VRMBlendShapeProxy>();
                var oldVrmBlendShapeProxy = data.OldVrm.GetComponent<VRMBlendShapeProxy>();

                var oldClips = oldVrmBlendShapeProxy.BlendShapeAvatar.Clips;
                var newClips = newVrmBlendShapeProxy.BlendShapeAvatar.Clips;

                var oldBlendShapeSmr = oldVrmBlendShapeProxy.GetComponentsInChildren<SkinnedMeshRenderer>()
                    .Where(renderer => renderer.sharedMesh.blendShapeCount > 0);
                var newBlendShapeSmr = oldVrmBlendShapeProxy.GetComponentsInChildren<SkinnedMeshRenderer>()
                    .Where(renderer => renderer.sharedMesh.blendShapeCount > 0);

                for (var i = 0; i < oldClips.Count; i++)
                {
                    //BlendShapeBinding
                    if (newClips.Count <= i)
                    {
                        newClips.Add(oldClips[i]);
                    }

                    newClips[i].BlendShapeName = oldClips[i].BlendShapeName;
                    newClips[i].Values = (from binding in oldClips[i].Values
                        let blendShapeName = oldBlendShapeSmr.FirstOrDefault(smr => smr.name == binding.RelativePath)
                            ?.sharedMesh.GetBlendShapeName(binding.Index)
                        where !string.IsNullOrEmpty(blendShapeName)
                        let blendShapeIndex = newBlendShapeSmr.FirstOrDefault(smr => smr.name == binding.RelativePath)
                            ?.sharedMesh.GetBlendShapeIndex(blendShapeName) ?? -1
                        select new BlendShapeBinding()
                        {
                            RelativePath = binding.RelativePath,
                            Index = blendShapeIndex < 0 ? binding.Index : blendShapeIndex, Weight = binding.Weight,
                        }).ToArray();
                    newClips[i].MaterialValues = oldClips[i].MaterialValues;

#if UNITY_EDITOR
                    EditorUtility.SetDirty(newClips[i]);
#endif
                }

#if UNITY_EDITOR
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
            }

            #endregion

            #region FirstPerson

            if (data.IsFirstPerson)
            {
                var oldFirstPerson = data.OldVrm.GetComponent<VRMFirstPerson>();
                var newFirstPerson = data.NewVrm.GetComponent<VRMFirstPerson>();

                newFirstPerson.FirstPersonBone =
                    CiferTechUtils.FindSameNameTransformInChildren(oldFirstPerson.FirstPersonBone.name,
                        newFirstPerson.transform);
                newFirstPerson.FirstPersonOffset = oldFirstPerson.FirstPersonOffset;

                foreach (var oldRenderer in oldFirstPerson.Renderers.Where(oldRenderer => oldRenderer.Renderer != null))
                {
                    for (var i = 0; i < newFirstPerson.Renderers.Count; i++)
                    {
                        if (newFirstPerson.Renderers[i].Renderer == null)
                            continue;

                        if (oldRenderer.Renderer.name != newFirstPerson.Renderers[i].Renderer.name)
                            continue;

                        newFirstPerson.Renderers[i] = new VRMFirstPerson.RendererFirstPersonFlags()
                        {
                            Renderer = newFirstPerson.Renderers[i].Renderer,
                            FirstPersonFlag = oldRenderer.FirstPersonFlag
                        };
                    }
                }
            }

            #endregion

            #region LookAtBoneApplyer

            if (data.IsLookAtBoneApplyer)
            {
                var oldVrmLookAtBoneApplyer = data.OldVrm.GetComponent<VRMLookAtBoneApplyer>();
                var newVrmLookAtBoneApplyer = data.NewVrm.GetComponent<VRMLookAtBoneApplyer>();

                newVrmLookAtBoneApplyer.HorizontalOuter = oldVrmLookAtBoneApplyer.HorizontalOuter;
                newVrmLookAtBoneApplyer.HorizontalInner = oldVrmLookAtBoneApplyer.HorizontalInner;
                newVrmLookAtBoneApplyer.VerticalDown = oldVrmLookAtBoneApplyer.VerticalDown;
                newVrmLookAtBoneApplyer.VerticalUp = oldVrmLookAtBoneApplyer.VerticalUp;
            }

            #endregion

            #region Material

            if (data.IsMaterial)
            {
                var oldSmrArray = data.OldVrm.GetComponentsInChildren<SkinnedMeshRenderer>();
                var newSmrArray = data.NewVrm.GetComponentsInChildren<SkinnedMeshRenderer>();

                foreach (var oldSmr in oldSmrArray)
                {
                    foreach (var newSmr in newSmrArray)
                    {
                        if (oldSmr.name != newSmr.name)
                            continue;

                        for (var i = 0; i < newSmr.sharedMaterials.Length; i++)
                        {
                            newSmr.sharedMaterials = oldSmr.sharedMaterials;
                        }
                    }
                }
            }

            #endregion

            #region SpringBone

            if (data.IsSpringBone)
            {
                var newSecondaryTransform = data.NewVrm.transform.Find("secondary");

                var oldSpringBones = data.OldVrm.GetComponentsInChildren<VRMSpringBone>();
                var oldColliders = data.OldVrm.GetComponentsInChildren<VRMSpringBoneColliderGroup>();

                CiferTechUtils.DeleteExistSetting<VRMSpringBone>(newSecondaryTransform, false);
                CiferTechUtils.DeleteExistSetting<VRMSpringBoneColliderGroup>(data.NewVrm.transform, true);

                foreach (var oldCollider in oldColliders)
                {
                    var targetTransform =
                        CiferTechUtils.FindSameNameTransformInChildren(oldCollider.name, data.NewVrm.transform);

                    if (targetTransform == null)
                        continue;

                    var newCollider = targetTransform.gameObject.AddComponent<VRMSpringBoneColliderGroup>();
                    newCollider.Colliders = oldCollider.Colliders;
                }


                foreach (var oldSpringBone in oldSpringBones)
                {
                    var newSpringBone = newSecondaryTransform.gameObject.AddComponent<VRMSpringBone>();
                    newSpringBone.m_comment = oldSpringBone.m_comment;
                    newSpringBone.m_stiffnessForce = oldSpringBone.m_stiffnessForce;
                    newSpringBone.m_gravityPower = oldSpringBone.m_gravityPower;
                    newSpringBone.m_gravityDir = oldSpringBone.m_gravityDir;
                    newSpringBone.m_dragForce = oldSpringBone.m_dragForce;

                    if (oldSpringBone.m_center != null)
                        newSpringBone.m_center =
                            CiferTechUtils.FindSameNameTransformInChildren(oldSpringBone.m_center.name,
                                data.NewVrm.transform);

                    newSpringBone.RootBones = new List<Transform>();
                    foreach (var oldRootBone in oldSpringBone.RootBones)
                    {
                        newSpringBone.RootBones.Add(
                            CiferTechUtils.FindSameNameTransformInChildren(oldRootBone.name, data.NewVrm.transform));
                    }

                    newSpringBone.m_hitRadius = oldSpringBone.m_hitRadius;

                    newSpringBone.ColliderGroups = oldSpringBone.ColliderGroups
                        .Select(oldCollider =>
                            CiferTechUtils.FindSameNameTransformInChildren(oldCollider.name, data.NewVrm.transform)
                                ?.GetComponent<VRMSpringBoneColliderGroup>())
                        .Where(targetCollider => targetCollider != null).ToArray();
                }
            }

            #endregion

            Debug.Log($"{typeof(MigrateVrmSettings)}: 設定の移行が完了しました！");
        }
    }

    public struct MigrateVrmSettingsData
    {
        public VRMMeta OldVrm, NewVrm;
        public bool IsMeta, IsBlendShape, IsFirstPerson, IsLookAtBoneApplyer, IsMaterial, IsSpringBone;
    }
}