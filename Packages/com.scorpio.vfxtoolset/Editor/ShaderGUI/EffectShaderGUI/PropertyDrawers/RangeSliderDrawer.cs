using UnityEditor;
using UnityEngine;

public class RangeSliderDrawer : MaterialPropertyDrawer
    {
        //该控件会在以下选项中显示
        private string[] _showList = new string[0];

        //是否总是显示
        private bool _isAlwaysShow = true;

        public RangeSliderDrawer()
        {
            _isAlwaysShow = true;
        }
        public RangeSliderDrawer(params string[] showList)
        {
            _showList = showList;
            _isAlwaysShow = showList == null || showList.Length == 0;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {

            EditorGUI.showMixedValue = prop.hasMixedValue;

            Vector4 originalValue = prop.vectorValue;
            Vector4 value = originalValue;
            
            // 从 Shader 默认值读取范围限制 (z, w)，这样修改 Shader 后会立即生效
            Material mat = editor.target as Material;
            if (mat != null && mat.shader != null)
            {
                int propIndex = mat.shader.FindPropertyIndex(prop.name);
                if (propIndex >= 0)
                {
                    Vector4 shaderDefault = mat.shader.GetPropertyDefaultVectorValue(propIndex);
                    value.z = shaderDefault.z;
                    value.w = shaderDefault.w;
                }
            }
            
            //修正范围
            if (Mathf.Approximately(value.z, value.w))
                value += new Vector4(0, 0, 0, 0.1f);
            if (value.z > value.w)
            {
                (value.z, value.w) = (value.w, value.z);
            }
            
            // 将 x 和 y 限制在新的范围 [z, w] 内
            value.x = Mathf.Clamp(value.x, value.z, value.w);
            value.y = Mathf.Clamp(value.y, value.z, value.w);
            
            // 确保 x <= y
            if (value.x > value.y)
            {
                (value.x, value.y) = (value.y, value.x);
            }

            //滑动条
            Rect rangeRect = new Rect(position)
            {
                width = position.width - 110f
            };

            //绘制最小属性
            Rect minRect = new Rect(position)
            {
                x = position.xMax - 105f,
                width = 50,
            };
            //绘制最大属性
            Rect maxRect = new Rect(position)
            {
                x = position.xMax - 50f,
                width = 50,
            };

            EditorGUI.BeginChangeCheck();
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUILayout.BeginHorizontal();
            value.x = EditorGUI.FloatField(minRect, value.x);
            value.y = EditorGUI.FloatField(maxRect,value.y);
            EditorGUIUtility.labelWidth = 0.0f;
            EditorGUI.MinMaxSlider(rangeRect, label, ref value.x, ref value.y, value.z, value.w);
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUILayout.EndHorizontal();
            EditorGUI.showMixedValue = false;
            
            // 如果 UI 被用户改变，或者修正逻辑改变了值，都需要保存
            if (EditorGUI.EndChangeCheck() || value != originalValue)
            {
                prop.vectorValue = value;
            }
        }
    }