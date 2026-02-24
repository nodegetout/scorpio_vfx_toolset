using UnityEditor;
using UnityEngine;

namespace HeroShowRenderingGUI.VFX
{
    public class ASEffectShaderGUI : ShaderGUI
    {
        private static readonly string  k_WarningContent = "本Shader为Theseus 带有颜色问题的展台特效Shader，仅开放给以下6套皮肤使用:" +
                                                           "\n 1. 肃游 国服 \n 2. 阿鲁卡多S+  \n 3. 心狱镇凶二期_贝拉尼塔 克林特 赛西里奥" +
                                                           "\n 4. M7格兰杰SP \n 5. 厄伊妲";
        private Texture2D _warningBgTexture;
        private GUIStyle _alterStyle;
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            DrawWarningGUI();
            base.OnGUI(materialEditor, properties);
        }
        
        protected virtual void DrawWarningGUI()
        {
            // var warningContent = "本Shader为Theseus 带有颜色问题的展台特效Shader，仅开放给以下皮肤使用使用:\n 1. 肃游 skin01";
            // EditorGUILayout.HelpBox(warningContent, MessageType.Error, true);
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