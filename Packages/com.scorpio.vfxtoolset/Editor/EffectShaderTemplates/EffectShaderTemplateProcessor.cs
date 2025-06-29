using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace com.scorpio.vfxtoolset.Editor
{
    public static class EffectShaderTemplateProcessor
    {
        private static string k_TargetMeshEffectShaderTemplatePath =
            "Packages/com.scorpio.vfxtoolset/Editor/EffectShaderTemplates/84-Shader_MeshEffectShader.shader.txt";
        private static string k_TargetParticleEffectShaderTemplatePath =
            "Packages/com.scorpio.vfxtoolset/Editor/EffectShaderTemplates/85-Shader_ParticleEffectShader.shader.txt";
        
        [MenuItem("Assets/Create/Shader/MeshEffectShader", false, 84)]
        static void CreateMeshEffectShaderFromTemplate()
        {
            string locationPath = GetSelectedPathOrFallback();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                ScriptableObject.CreateInstance<MeshEffectShaderCreationAsset>(),
                locationPath + "/Theseus_MeshEffect_Name.shader",
                null, k_TargetMeshEffectShaderTemplatePath);
        }
        
        [MenuItem("Assets/Create/Shader/ParticleEffectShader", false, 85)]
        static void CreateParticleEffectShaderFromTemplate()
        {
            string locationPath = GetSelectedPathOrFallback();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                ScriptableObject.CreateInstance<ParticleEffectShaderCreationAsset>(),
                locationPath + "/Theseus_ParticleEffect_Name.shader",
                null, k_TargetParticleEffectShaderTemplatePath);
        }
        
        private static string GetSelectedPathOrFallback()
        {
            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }
        class MeshEffectShaderCreationAsset : EffectShaderCreationAssetBase
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                _fileNamePrefix = "Theseus_MeshEffect_";
                base.Action(instanceId, pathName, resourceFile);
            }
        }
        
        class ParticleEffectShaderCreationAsset : EffectShaderCreationAssetBase
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                _fileNamePrefix = "Theseus_ParticleEffect_";
                base.Action(instanceId, pathName, resourceFile);
            }
        }

        abstract class EffectShaderCreationAssetBase : EndNameEditAction
        {
            protected string _fileNamePrefix;
            
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Object targetObject = CreateScriptAssetFromTemplate(pathName, resourceFile);
                ProjectWindowUtil.ShowCreatedAsset(targetObject);
            }

            private Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
            {
                string fullPath = Path.GetFullPath(pathName);
                StreamReader streamReader = new StreamReader(resourceFile);
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
                string name = fileNameWithoutExtension.Replace(_fileNamePrefix, string.Empty);
                
                //用户创建脚本后将输入的文件名进行替换
                text = Regex.Replace(text, "#NAME#", name);
                bool encoderShouldEmitUTF8Identifier = true;
                bool throwOnInvalidBytes = false;
                UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
                //替换后将最终的代码文本写入
                bool append = false;
                StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
                streamWriter.Write(text);
                streamWriter.Close();
                AssetDatabase.ImportAsset(pathName);
                return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
            }
        }
    }
}