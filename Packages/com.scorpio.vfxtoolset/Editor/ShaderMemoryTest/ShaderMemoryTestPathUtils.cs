using UnityEditor;

namespace ScorpioEditor.ShaderMemoryTest
{
    internal static class ShaderMemoryTestPathUtils
    {
        /// <summary>
        /// 按层级递归创建 Assets 下的目录结构。
        /// </summary>
        public static void EnsureDirectoryExists(string dir)
        {
            if (string.IsNullOrEmpty(dir) || !dir.StartsWith("Assets/")) return;
            string[] parts = dir.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
