using System;
using UnityEditor;

namespace HeroShowRenderingGUI.VFX
{
    public interface IComponentDrawer
    {
        void DrawComponent(float x);
    }

    public class VectorComponent : IComponentDrawer
    {
        public string label;
        public float value;

        public VectorComponent(string label)
        {
            this.label = label;
        }

        public virtual void DrawComponent(float inputValue)
        {
             value = EditorGUILayout.FloatField(label, inputValue);
        }
    }

    public class HiddenComponent : VectorComponent
    {
        public HiddenComponent(string label) : base(label)
        {
        }

        public override void DrawComponent(float inputValue)
        {
            // draw nothing!!
        }
    }

    public class FloatSliderComponent : VectorComponent
    {
        private float minValue;
        private float maxValue;

        public FloatSliderComponent(string label, float minValue, float maxValue) : base(label)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public override void DrawComponent(float inputValue)
        {
            value = EditorGUILayout.Slider(label, inputValue, minValue, maxValue);
        }
    }

    public class IntSliderComponent : VectorComponent
    {
        private int minValue;
        private int maxValue;

        public IntSliderComponent(string label, int minValue, int maxValue) : base(label)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public override void DrawComponent(float inputValue)
        {
            value = EditorGUILayout.IntSlider(label, (int) inputValue, minValue, maxValue);
        }
    }

    public class ToggleComponent : VectorComponent
    {
        public ToggleComponent(string label) : base(label) { }

        public override void DrawComponent(float inputValue)
        {
            bool inputFlag = inputValue > 0;
            bool flag = EditorGUILayout.Toggle(label, inputFlag);
            value = flag? 1f : 0f;
        }
    }

    public class EnumComponent : VectorComponent
    {
        Type _enumType;
        string[] _options;
        public EnumComponent(string label) : base(label) { }

        public EnumComponent(string label, string[] options) : base(label)
        {
            this._options = options;
        }

        public override void DrawComponent(float inputValue)
        {
            int selected = (int) inputValue;
            int newSelected = EditorGUILayout.Popup(label, selected, _options);
            value = newSelected;
        }
    }

    public class IntegerComponent : VectorComponent
    {
        public IntegerComponent(string label) : base(label) { }

        public override void DrawComponent(float inputValue)
        {
            value = EditorGUILayout.IntField(label, (int)inputValue);
        }
    }
}