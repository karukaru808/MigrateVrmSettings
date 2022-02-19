using UnityEditor;
using UnityEngine;
using VRM;

namespace CIFER.Tech.MigrateVrmSettings
{
    public class MigrateVrmSettingsWindow : EditorWindow
    {
        private static VRMMeta _oldVrm, _newVrm;

        private bool _isMeta = true,
            _isBlendShape = true,
            _isFirstPerson = true,
            _isLookAtBoneApplyer = true,
            _isMaterial = true,
            _isSpringBone = true;

        [MenuItem("CIFER.tech/MigrateVrmSettings")]
        private static void Open()
        {
            var window = GetWindow<MigrateVrmSettingsWindow>("MigrateVrmSettings");
            window.minSize = new Vector2(350f, 300f);
        }

        private void OnGUI()
        {
            _oldVrm = EditorGUILayout.ObjectField("古いVRM", _oldVrm, typeof(VRMMeta), true) as VRMMeta;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("↓", new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter
            });

            EditorGUILayout.Space();

            _newVrm = EditorGUILayout.ObjectField("新しいVRM", _newVrm, typeof(VRMMeta), true) as VRMMeta;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("移行設定", new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter
            });

            EditorGUILayout.Space();

            _isMeta = EditorGUILayout.Toggle("メタデータ", _isMeta);
            _isBlendShape = EditorGUILayout.Toggle("ブレンドシェイプ（表情）", _isBlendShape);
            _isFirstPerson = EditorGUILayout.Toggle("FirstPerson", _isFirstPerson);
            _isLookAtBoneApplyer = EditorGUILayout.Toggle("LookAtBoneApplyer", _isLookAtBoneApplyer);
            _isMaterial = EditorGUILayout.Toggle("マテリアル／テクスチャ", _isMaterial);
            _isSpringBone = EditorGUILayout.Toggle("SpringBone", _isSpringBone);

            GUILayout.FlexibleSpace();

            //エラー、警告判定
            if (_oldVrm == null || _newVrm == null)
            {
                EditorGUILayout.HelpBox("VRMを選択してください。", MessageType.Error);
                return;
            }

            //ロード
            if (GUILayout.Button("設定を移行する"))
            {
                var data = new MigrateVrmSettingsData()
                {
                    OldVrm = _oldVrm,
                    NewVrm = _newVrm,
                    IsMeta = _isMeta,
                    IsBlendShape = _isBlendShape,
                    IsFirstPerson = _isFirstPerson,
                    IsLookAtBoneApplyer = _isLookAtBoneApplyer,
                    IsMaterial = _isMaterial,
                    IsSpringBone = _isSpringBone,
                };
                MigrateVrmSettings.Resetting(data);
            }
        }
    }
}