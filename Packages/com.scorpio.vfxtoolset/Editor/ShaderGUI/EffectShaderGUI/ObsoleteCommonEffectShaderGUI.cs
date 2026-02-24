using UnityEditor;
using UnityEngine;

namespace HeroShowRenderingGUI.VFX
{

    public class ObsoleteCommonEffectShaderGUI : EffectShaderGUIBase
    {
        #region Shader Change Detection
        private Shader _lastShader;
        #endregion
        
        private static readonly PersistentModuleView KPsMainModuleView =
            new PersistentModuleView("【主贴图设置】",PropertyInfoData.k_PsMainModulePropInfo);
        private static readonly PersistentModuleView KMeshMainModuleView =
            new PersistentModuleView("【主贴图设置】", PropertyInfoData.k_MeshMainModulePropInfo);
        
        private static readonly PersistentModuleView k_FlowMapModuleData =
            new PersistentModuleView("【FlowMap设置】", PropertyInfoData.k_FlowMapModulePropInfo);
        private static readonly PersistentModuleView k_DistortionModuleData =
            new PersistentModuleView("【扰动设置】", PropertyInfoData.k_NoiseModulePropInfo);
        
        
        private static readonly string  k_WarningContent = "本Shader为Theseus展台特效，目前暂未开放使用";
        private Texture2D _warningBgTexture;
        private GUIStyle _alterStyle;
        
        protected override void AppendPropertyDrawerFunc(MaterialEditor materialEditor)
        {
            _customPropertyFuncMap = PropertyDrawerFuncMap.k_CommonEffectShaderPropertyFuncMap;
        }

        protected override void ChangeModuleByEffectType(MaterialEditor materialEditor)
        {
            Material mat = materialEditor.target as Material;
            if (mat != null && mat.shader != null)
            {
                // 根据shader类型切换主贴图模块
                CommonEffectModuleConfigs.obsoleteShaderModulesConfigData.Remove(KPsMainModuleView);
                CommonEffectModuleConfigs.obsoleteShaderModulesConfigData.Remove(KMeshMainModuleView);
                bool isPsShader = mat.shader.name.Contains("ParticleEffect");
                CommonEffectModuleConfigs.obsoleteShaderModulesConfigData.Insert(3, isPsShader ? KPsMainModuleView : KMeshMainModuleView);
                
                // 根据shader类型切换FlowMap模块和扰动模块
                bool isFlowMapType = mat.shader.name.Contains("FlowMap");
                bool isDistortionType = mat.shader.name.Contains("Distortion");
                CommonEffectModuleConfigs.obsoleteShaderModulesConfigData.Remove(k_DistortionModuleData);
                CommonEffectModuleConfigs.obsoleteShaderModulesConfigData.Remove(k_FlowMapModuleData);
                
                if (isFlowMapType)
                {
                    CommonEffectModuleConfigs.obsoleteShaderModulesConfigData.Insert(5, k_FlowMapModuleData);
                }
                if(isDistortionType)
                {
                    CommonEffectModuleConfigs.obsoleteShaderModulesConfigData.Insert(5, k_DistortionModuleData);
                }
            }
        }
        
        /// <summary>
        /// 检测shader是否发生变化，如果变化则重新执行模块配置
        /// </summary>
        private void CheckShaderChange(MaterialEditor materialEditor)
        {
            Material mat = materialEditor.target as Material;
            if (mat != null && mat.shader != null && _lastShader != mat.shader)
            {
                _lastShader = mat.shader;
                ChangeModuleByEffectType(materialEditor);
                EditorUtility.SetDirty(materialEditor.target);
                materialEditor.Repaint();
            }
        }

        protected override void DrawWarningGUI()
        {
            if (_warningBgTexture == null)
            {
                _warningBgTexture = MakeTex(2, 2, new Color(0.7f, 0.3f, 0.3f, 0.5f));
            }

            if (_alterStyle == null)
            {
                _alterStyle = new GUIStyle
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 15,
                    normal =
                    {
                        textColor = new Color(0.8f, 0.8f, 0.8f, 1f),
                        background = _warningBgTexture
                    },
                    wordWrap = true
                };
            }
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(k_WarningContent, _alterStyle);
            EditorGUILayout.EndHorizontal();
            // EditorGUILayout.HelpBox(k_WarningContent, MessageType.Error, true);
        }

        protected override void DrawModules(MaterialEditor materialEditor, Material material,
            MaterialProperty[] properties)
        {
            // 检测shader变化并自动重新配置模块
            CheckShaderChange(materialEditor);
            
            foreach (var moduleDataRecord in CommonEffectModuleConfigs.obsoleteShaderModulesConfigData)
            {
                DrawModuleWithData(materialEditor, moduleDataRecord, material, properties);
            }
        }
        
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}