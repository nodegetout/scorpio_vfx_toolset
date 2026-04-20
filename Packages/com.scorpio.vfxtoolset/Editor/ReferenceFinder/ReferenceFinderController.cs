using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using com.scorpio.vfxtoolset.Editor.Data;
using UnityEditor;
using UnityEngine;

namespace com.scorpio.vfxtoolset.Editor
{
    public class ReferenceFinderController
    {
        //缓存路径
        private static readonly string k_CachePath = "Library/ReferenceFinderCache.json";
        private static readonly string k_CacheVersion = "V2";

        //资源引用信息字典
        public Dictionary<string, AssetDescription> assetDict = new Dictionary<string, AssetDescription>();

        //收集资源引用信息并更新缓存
        public void CollectDependenciesInfo()
        {
            try
            {
                // For now, we'll keep the same approach but with optimizations
                ReadFromCache();
                var allAssets = AssetDatabase.GetAllAssetPaths();
                int totalCount = allAssets.Length;

                // Process assets in batches to avoid UI blocking
                const int batchSize = 100;
                for (int i = 0; i < allAssets.Length; i += batchSize)
                {
                    //每遍历100个Asset，更新一下进度条，同时对进度条的取消操作进行处理
                    if ((i % 100 == 0) && EditorUtility.DisplayCancelableProgressBar("Refresh",
                            string.Format("Collecting {0} assets", i), (float)i / totalCount))
                    {
                        EditorUtility.ClearProgressBar();
                        return;
                    }

                    // Process batch of assets
                    int endIndex = Math.Min(i + batchSize, allAssets.Length);
                    for (int j = i; j < endIndex; j++)
                    {
                        if (File.Exists(allAssets[j]))
                            ImportAsset(allAssets[j]);
                    }

                    // Periodic GC collection to prevent memory buildup
                    if (i % (batchSize * 20) == 0)
                        GC.Collect();
                }

                //将信息写入缓存
                EditorUtility.DisplayCancelableProgressBar("Refresh", "Write to cache", 1f);
                WriteToCache();
                //生成引用数据
                EditorUtility.DisplayCancelableProgressBar("Refresh", "Generating asset reference info", 1f);
                UpdateReferenceInfo();
                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                EditorUtility.ClearProgressBar();
            }
        }

        //通过依赖信息更新引用信息
        private void UpdateReferenceInfo()
        {
            foreach (var asset in assetDict)
            {
                foreach (var assetGuid in asset.Value.dependencies)
                {
                    if (assetDict.ContainsKey(assetGuid) && !assetDict[assetGuid].references.Contains(asset.Key))
                    {
                        assetDict[assetGuid].references.Add(asset.Key);
                    }
                }
            }
        }

        //生成并加入引用信息
        private void ImportAsset(string path)
        {
            if (!path.StartsWith("Assets/") && !path.StartsWith("Packages/"))
                return;

            //通过path获取guid进行储存
            string guid = AssetDatabase.AssetPathToGUID(path);
            //获取该资源的最后修改时间，用于之后的修改判断
            Hash128 assetDependencyHash = AssetDatabase.GetAssetDependencyHash(path);
            //如果assetDict没包含该guid或包含了修改时间不一样则需要更新
            if (!assetDict.ContainsKey(guid) || assetDict[guid].assetDependencyHash != assetDependencyHash.ToString())
            {
                //将每个资源的直接依赖资源转化为guid进行储存
                var guids = AssetDatabase.GetDependencies(path, false).Select(AssetDatabase.AssetPathToGUID).ToList();

                //生成asset依赖信息，被引用需要在所有的asset依赖信息生成完后才能生成
                AssetDescription ad = new AssetDescription();
                ad.name = Path.GetFileNameWithoutExtension(path);
                ad.path = path;
                ad.assetDependencyHash = assetDependencyHash.ToString();
                ad.dependencies = guids;
                ad.guid = guid; // Set the guid for cache compatibility

                assetDict[guid] = ad;
            }
        }

        //读取缓存信息
        public bool ReadFromCache()
        {
            assetDict.Clear();
            if (!File.Exists(k_CachePath))
            {
                return false;
            }

            try
            {
                string json = File.ReadAllText(k_CachePath);
                // Use simple JSON parsing - Unity's environment may not support System.Text.Json
                var cacheData = JsonUtility.FromJson<CacheData>(json);

                if (cacheData.Version != k_CacheVersion)
                {
                    return false;
                }

                // Load assets
                foreach (var assetData in cacheData.Assets)
                {
                    assetDict[assetData.guid] = assetData;
                }

                // Update reference information
                UpdateReferenceInfo();
                return true;
            }
            catch (Exception)
            {
                // If deserialization fails, return false to trigger full rebuild
                return false;
            }
        }

        //写入缓存
        private void WriteToCache()
        {
            if (File.Exists(k_CachePath))
                File.Delete(k_CachePath);

            var cacheData = new CacheData
            {
                Version = k_CacheVersion,
                Assets = assetDict.Values.ToList()
            };

            // Use JsonUtility for serialization (more compatible with Unity)
            string json = JsonUtility.ToJson(cacheData, true);
            File.WriteAllText(k_CachePath, json);
        }

        // Cache data structure for JSON serialization
        [System.Serializable]
        private class CacheData
        {
            public string Version = "";
            public List<AssetDescription> Assets = new List<AssetDescription>();
        }

        //更新引用信息状态
        public void UpdateAssetState(string guid)
        {
            AssetDescription ad;
            bool hasFound = assetDict.TryGetValue(guid, out ad);
            if (!hasFound)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ad = new AssetDescription
                {
                    name = Path.GetFileNameWithoutExtension(path),
                    path = path,
                    state = AssetState.NODATA
                };
                assetDict.Add(guid, ad);
                return;
            }
            
            bool hasData = ad.state != AssetState.NODATA;
            if (hasData)
            {
                bool fileExists = File.Exists(ad.path);
                if (!fileExists)
                {
                    ad.state = AssetState.MISSING;
                }
                else
                {
                    //修改时间与记录的不同为修改过的资源
                    var curAssetDependencyHash = AssetDatabase.GetAssetDependencyHash(ad.path).ToString();
                    bool hasChanged = ad.assetDependencyHash !=curAssetDependencyHash;
                    ad.state = hasChanged ? AssetState.CHANGED : AssetState.NORMAL;
                }
            }
        }

        //根据引用信息状态获取状态描述
        private static readonly Dictionary<AssetState, string> k_StateInfoMap = new()
        {
            { AssetState.NORMAL, "Normal" },
            { AssetState.MISSING, "<color=#FF0000FF>Missing</color>" },
            { AssetState.NODATA, "<color=#FFE300FF>No Data</color>" },
            { AssetState.CHANGED, "<color=#F0672AFF>Changed</color>" },
        };
        public static string GetInfoByState(AssetState state)
        {
            return k_StateInfoMap[state];
        }

        public enum AssetState
        {
            NORMAL,
            CHANGED,
            MISSING,
            NODATA,
        }
    }
}
