using UnityEditor;
using UnityEngine;

namespace ScorpioEditor.ShaderMemoryTest
{
    public static class ShaderMemoryTestPreloadedShaders
    {
        private const string GraphicsSettingsPath = "ProjectSettings/GraphicsSettings.asset";

        /// <summary>
        /// 将 Shader Variant Collection 加入 Graphics Settings 的 Preloaded Shaders，避免构建时 strip 变体。
        /// </summary>
        public static bool AddSvcToPreloadedShaders(ShaderVariantCollection svc, out string error)
        {
            error = null;
            if (svc == null)
            {
                error = "SVC 为空。";
                return false;
            }

            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(GraphicsSettingsPath);
            if (assets == null || assets.Length == 0)
            {
                error = "无法加载 GraphicsSettings 资产。";
                return false;
            }

            SerializedObject so = new SerializedObject(assets[0]);
            SerializedProperty prop = so.FindProperty("m_PreloadedShaders");
            if (prop == null)
            {
                error = "GraphicsSettings 中未找到 m_PreloadedShaders 属性。";
                return false;
            }

            Object svcAsset = svc;
            if (svcAsset == null)
            {
                error = "SVC 未保存为资产，请先保存。";
                return false;
            }

            for (int i = 0; i < prop.arraySize; i++)
            {
                if (prop.GetArrayElementAtIndex(i).objectReferenceValue == svcAsset)
                {
                    error = null;
                    return true;
                }
            }

            prop.arraySize++;
            prop.GetArrayElementAtIndex(prop.arraySize - 1).objectReferenceValue = svcAsset;
            so.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            return true;
        }

        /// <summary>
        /// 从 SVC 资产路径加载并加入 Preloaded Shaders。
        /// </summary>
        public static bool AddSvcToPreloadedShadersByPath(string svcAssetPath, out string error)
        {
            error = null;
            var svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(svcAssetPath);
            if (svc == null)
            {
                error = $"无法加载 SVC：{svcAssetPath}";
                return false;
            }
            return AddSvcToPreloadedShaders(svc, out error);
        }
    }
}
