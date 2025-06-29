using com.scorpio.vfxtoolset.Editor.Data;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace com.scorpio.vfxtoolset.Editor
{
    //资源引用树
    public class AssetTreeView : TreeView
    {
        //图标宽度
        const float kIconWidth = 18f;
        //列表高度
        const float kRowHeights = 20f;

        public AssetViewItem assetRoot;
        private GUIStyle stateGUIStyle = new GUIStyle { richText = true, alignment = TextAnchor.MiddleCenter };

        //列信息
        enum ColumnType
        {
            Name,
            Path,
            State,
        }

        public AssetTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = false;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = kIconWidth;
        }

        //响应右击事件
        protected override void ContextClickedItem(int id)
        {
            SetExpanded(id, !IsExpanded(id));
        }

        //响应双击事件
        protected override void DoubleClickedItem(int id)
        {
            var item = (AssetViewItem)FindItem(id, rootItem);
            //在ProjectWindow中高亮双击资源
            if (item != null)
            {
                var assetObject = AssetDatabase.LoadAssetAtPath(item.data.path, typeof(UnityEngine.Object));
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = assetObject;
                EditorGUIUtility.PingObject(assetObject);
            }
        }

        //生成ColumnHeader
        private static readonly MultiColumnHeaderState.Column[] k_ColumnDisplayInfoArray = new[]
        {
        //图标+名称
        new MultiColumnHeaderState.Column
        {
            headerContent = new GUIContent("Name"),
            headerTextAlignment = TextAlignment.Center,
            sortedAscending = false,
            width = 200,
            minWidth = 60,
            autoResize = false,
            allowToggleVisibility = false,
            canSort = false
        },
        //路径
        new MultiColumnHeaderState.Column
        {
            headerContent = new GUIContent("Path"),
            headerTextAlignment = TextAlignment.Center,
            sortedAscending = false,
            width = 360,
            minWidth = 60,
            autoResize = false,
            allowToggleVisibility = false,
            canSort = false
        },
        //状态
        new MultiColumnHeaderState.Column
        {
            headerContent = new GUIContent("State"),
            headerTextAlignment = TextAlignment.Center,
            sortedAscending = false,
            width = 60,
            minWidth = 60,
            autoResize = false,
            allowToggleVisibility = true,
            canSort = false
        },
    };
        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            var state = new MultiColumnHeaderState(k_ColumnDisplayInfoArray);
            return state;
        }

        protected override TreeViewItem BuildRoot()
        {
            return assetRoot;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (AssetViewItem)args.item;
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (ColumnType)args.GetColumn(i), ref args);
            }
        }

        //绘制列表中的每项内容
        void CellGUI(Rect cellRect, AssetViewItem item, ColumnType column, ref RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);
            switch (column)
            {
                case ColumnType.Name:
                    {
                        var iconRect = cellRect;
                        iconRect.x += GetContentIndent(item);
                        iconRect.width = kIconWidth;
                        if (iconRect.x < cellRect.xMax)
                        {
                            var icon = GetIcon(item.data.path);
                            if (icon != null)
                                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
                        }
                        args.rowRect = cellRect;
                        base.RowGUI(args);
                    }
                    break;
                case ColumnType.Path:
                    {
                        GUI.Label(cellRect, item.data.path);
                    }
                    break;
                case ColumnType.State:
                    {
                        GUI.Label(cellRect, ReferenceFinderController.GetInfoByState(item.data.state), stateGUIStyle);
                    }
                    break;
            }
        }

        //根据资源信息获取资源图标
        private Texture2D GetIcon(string path)
        {
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (obj != null)
            {
                Texture2D icon = AssetPreview.GetMiniThumbnail(obj);
                if (icon == null)
                    icon = AssetPreview.GetMiniTypeThumbnail(obj.GetType());
                return icon;
            }
            return null;
        }
    }
}

