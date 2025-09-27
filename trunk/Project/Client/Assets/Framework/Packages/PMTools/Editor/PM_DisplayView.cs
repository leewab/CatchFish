using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.Core;
using UnityEditor;
using UnityEngine;

namespace Framework.PM
{
    internal class PM_DisplayView : BaseEditorView
    {
        //后台管理
        private Vector2 posUp;
        private Vector2 posRight;
        private Rect splitterRect;
        private Vector2 dragStartPos;
        private bool dragging;
        private float splitterWidth = 5;

        //当前索引值
        private int mCurIndex = 0;
        private float mCurTime = 0;
        private string mCurPackageDir = null;

        //package导入
        private bool mIsStartImport = false;
        private string mImportFile = null;

        //package初始化
        private bool mIsStartInitPackage = false;

        private PM_DisplayModule mDisplayModule = null;
        private PM_AdminModule mAdminModule = new PM_AdminModule();

        public override void OnGUI(BaseEditorModule module)
        {
            if (module == null) return;
            base.OnGUI(module);
            mDisplayModule = (PM_DisplayModule) module;
            if (mDisplayModule == null)
            {
                DrawEmptyPart();
                return;
            }

            if (!mDisplayModule.IsInited) return;

            GUILayout.BeginHorizontal(pBoxGUIStyle, GUILayout.Height(80));
            DrawTopPart();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DrawLeftPart();
            DrawSplitterPart();
            DrawRightPart();
            GUILayout.EndHorizontal();
            UpdateDragEvent();
        }

        public void Update()
        {
            //检测Package导入
            CheckImportPackage();
            //检测Package初始化
            CheckInitPackage();
        }

        public override void Clear()
        {
            base.Clear();
            mDisplayModule = null;
        }

        /// <summary>
        /// 服务器空数据
        /// </summary>
        private void DrawEmptyPart()
        {
            GUILayout.BeginHorizontal(pBoxGUIStyle, GUILayout.Width(600));
            EditorHelper.DrawTitle("服务器端暂无数据", pBlueLableStyle);
            EditorHelper.DrawButton("刷新", new Vector2(120, 80), () => { mDisplayModule.RequestData(); });
            GUILayout.EndHorizontal();
        }

        private void DrawTopPart()
        {
            GUILayout.BeginHorizontal(pBoxGUIStyle);

            GUILayout.BeginVertical(GUILayout.Width(100), GUILayout.Height(60));
            GUILayout.Space(30);
            EditorHelper.DrawLable("Username");
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(60));
            EditorHelper.DrawTitle("PluginManager");
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(100), GUILayout.Height(60));
            GUILayout.Space(10);
            EditorHelper.DrawButton("切换管理端", () => { mAdminModule.IsAdmin = !mAdminModule.IsAdmin; }, pLableGUIStyle,
                GUILayout.Height(20), GUILayout.ExpandWidth(true));
            EditorHelper.DrawButton("联系方式", () => { ProtocolManager.C2SProtocolHander.Req_PM_Service(); },
                pLableGUIStyle, GUILayout.Height(20), GUILayout.ExpandWidth(true));
            EditorHelper.DrawButton("后台管理", () =>
            {
                var adminWin = EditorWindow.GetWindow<PM_AdminWindow>(true, "后台管理");
                adminWin.Init(mAdminModule);
            }, pLableGUIStyle, GUILayout.Height(20), GUILayout.ExpandWidth(true));
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw Left Part
        /// </summary>
        private void DrawLeftPart()
        {
            // Left view
            if (mDisplayModule == null || mDisplayModule.PMAllPackagesDic == null) return;
            GUILayout.BeginVertical(GUILayout.Width(500));
            List<C2S_PB.C2S_PackageInfo> itemDatas = null;
            if (mDisplayModule.PMAllPackagesDic.TryGetValue(mDisplayModule.Titles[mCurIndex], out itemDatas) &&
                null != itemDatas)
            {
                DrawContentTable(itemDatas);
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draw Splitter Part
        /// </summary>
        private void DrawSplitterPart()
        {
            // Splitter
            GUILayout.Box("",
                GUILayout.Width(splitterWidth),
                GUILayout.MaxWidth(splitterWidth),
                GUILayout.MinWidth(splitterWidth),
                GUILayout.ExpandHeight(true));
            splitterRect = GUILayoutUtility.GetLastRect();
        }

        /// <summary>
        /// Draw Right Part
        /// </summary>
        private void DrawRightPart()
        {
            // Right view
            if (mDisplayModule == null || mDisplayModule.Titles == null) return;

            GUILayout.BeginVertical();

            posRight = GUILayout.BeginScrollView(posRight);
            mCurIndex = EditorHelper.DrawToolbar(mDisplayModule.Titles.ToArray(), mCurIndex, new Vector2(100, 24), 0);
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Update Splitter DragEvent
        /// </summary>
        private void UpdateDragEvent()
        {
            if (Event.current != null)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        if (splitterRect.Contains(Event.current.mousePosition))
                        {
                            Debug.Log("Start dragging");
                            dragging = true;
                        }

                        break;
                    case EventType.MouseDrag:
                        if (dragging)
                        {
                            Debug.Log("moving splitter");
                            //                            leftWidth += Event.current.delta.x;
                            //Repaint();
                        }

                        break;
                    case EventType.MouseUp:
                        if (dragging)
                        {
                            Debug.Log("Done dragging");
                            dragging = false;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// 绘制表格数据
        /// </summary>
        /// <param name="_itemDatas"></param>
        private void DrawContentTable(List<C2S_PB.C2S_PackageInfo> _itemDatas)
        {
            if (_itemDatas == null) return;

            //字段标题
            GUILayout.BeginHorizontal(pBoxGUIStyle);
            EditorHelper.DrawLables(
                GetTitleArray(),
                GetTitleSizeArray(),
                GetTitleStyleArray(),
                pGridGUIStyle);

            GUILayout.EndHorizontal();

            //列表内容
            GUILayout.Space(5);
            posUp = GUILayout.BeginScrollView(posUp);

            foreach (var itemData in _itemDatas)
            {
                if (mAdminModule.IsAdmin)
                {
                    DrawContentItem_Admin(itemData, new Vector2(400, 22));
                }
                else
                {
                    DrawContentItem(itemData, new Vector2(400, 22));
                }

                GUILayout.Space(1);
            }

            GUILayout.EndScrollView();
        }

        /// <summary>
        /// 绘制表格中的Item
        /// </summary>
        /// <param name="_info"></param>
        /// <param name="_size"></param>
        private void DrawContentItem(C2S_PB.C2S_PackageInfo _info, Vector2 _size)
        {
            if (_info == null) return;
            GUILayout.BeginHorizontal(pGridGUIStyle);

            int state = 0;
            GUIStyle pCurLableStyle = new GUIStyle();
            pCurLableStyle.alignment = TextAnchor.MiddleCenter;
            if (mDisplayModule.CompareVersion(_info.ClientVersion, _info.ServerVersion) == -1) //本地低版本
            {
                pCurLableStyle.normal.textColor = Color.yellow;
                state = 0;
            }
            else if (mDisplayModule.CompareVersion(_info.ClientVersion, _info.ServerVersion) == 0) //同步
            {
                pCurLableStyle.normal.textColor = Color.green;
                state = 1;
            }
            else if (mDisplayModule.CompareVersion(_info.ClientVersion, _info.ServerVersion) == 1
            ) //高出远端版本 远端版本被他人的低版本覆盖
            {
                pCurLableStyle.normal.textColor = Color.red;
                state = 2;
            }
            else if (mDisplayModule.CompareVersion(_info.ClientVersion, _info.ServerVersion) == -2) //本地无版本
            {
                pCurLableStyle.normal.textColor = Color.green;
                state = 3;
            }
            else if (mDisplayModule.CompareVersion(_info.ClientVersion, _info.ServerVersion) == 2) //远端无版本  远端版本被删除 
            {
                pCurLableStyle.normal.textColor = Color.red;
                state = 4;
            }

            EditorHelper.DrawLables(
                new[]
                {
                    _info.Name,
                    _info.ClientVersion,
                    _info.ServerVersion,
                },
                new Vector2[]
                {
                    new Vector2(160, _size.y),
                    new Vector2(120, _size.y),
                    new Vector2(120, _size.y),
                },
                new GUIStyle[]
                {
                    pLableGUIStyle,
                    pCurLableStyle,
                    pLableGUIStyle,
                },
                pGridGUIStyle);

            ///Operation栏
            GUILayout.BeginHorizontal(GUILayout.Width(100));
            if (state == 0) //本地低版本
            {
                EditorHelper.DrawButton("详情", new Vector2(0, _size.y), () => { OnClickDetailEvent(_info); });
                EditorHelper.DrawButton("更新", new Vector2(0, _size.y), () =>
                {
                    mCurPackageDir = Path.GetFullPath(_info.ClientResPath);
                    OnDownloadEvent(_info.ClientResPath);
                });
            }
            else if (state == 1) //同步
            {
                EditorHelper.DrawButton("详情", new Vector2(0, _size.y), () => { OnClickDetailEvent(_info); });
                EditorHelper.DrawButton("删除", new Vector2(0, _size.y), () => { OnClickDeleteEvent(_info); });
            }
            else if (state == 2) //高出远端版本 远端版本被他人的低版本覆盖
            {
                EditorHelper.DrawButton("详情", new Vector2(0, _size.y), () => { OnClickDetailEvent(_info); });
                EditorHelper.DrawButton("删除", new Vector2(0, _size.y), () => { OnClickDeleteEvent(_info); });
            }
            else if (state == 3) //本地无版本
            {
                EditorHelper.DrawButton("详情", new Vector2(0, _size.y), () => { OnClickDetailEvent(_info); });
                EditorHelper.DrawButton("下载", new Vector2(0, _size.y), () => { OnDownloadEvent(_info.ClientResPath); });
            }
            else if (state == 4) //远端无版本  远端版本被删除 
            {
                EditorHelper.DrawButton("详情", new Vector2(0, _size.y), () => { OnClickDetailEvent(_info); });
                EditorHelper.DrawButton("删除", new Vector2(0, _size.y), () => { OnClickDeleteEvent(_info); });
            }

            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制表格中的Item
        /// </summary>
        /// <param name="_info"></param>
        /// <param name="_size"></param>
        private void DrawContentItem_Admin(C2S_PB.C2S_PackageInfo _info, Vector2 _size)
        {
            if (_info == null) return;
            GUILayout.BeginHorizontal(pGridGUIStyle);

            // "ID",
            // "名    称",
            // "作    者",
            // "时    间",
            // "大    小",
            // "操    作",
            EditorHelper.DrawLables(
                new[]
                {
                    _info.Id.ToString(),
                    _info.Name,
                    _info.Author,
                    _info.UpdateTime,
                    $"{_info.Size / 1000}M",
                },
                new Vector2[]
                {
                    new Vector2(60, _size.y),
                    new Vector2(85, _size.y),
                    new Vector2(85, _size.y),
                    new Vector2(85, _size.y),
                    new Vector2(85, _size.y),
                },
                new GUIStyle[]
                {
                    pLableGUIStyle,
                    pLableGUIStyle,
                    pLableGUIStyle,
                    pLableGUIStyle,
                    pLableGUIStyle,
                },
                pGridGUIStyle);

            GUILayout.BeginHorizontal(GUILayout.Width(100));
            EditorHelper.DrawButton("详情", new Vector2(0, _size.y), () => { OnClickDetailEvent(_info); });
            EditorHelper.DrawButton("删除", new Vector2(0, _size.y), () =>
            {
                mCurIndex = 0;
                ProtocolManager.C2SProtocolHander.Req_PM_DeleteRemote(_info);
            });
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
        }

        private void OnClickDetailEvent(C2S_PB.C2S_PackageInfo _info)
        {
            var pDetailWin = EditorWindow.GetWindow<PM_PackageDetailWindow>(false, _info.Name);
            pDetailWin.Init(_info, mAdminModule.IsAdmin);
        }

        /// <summary>
        /// Download Event
        /// </summary>
        private void OnDownloadEvent(string _clientGUID)
        {
            if (_clientGUID == null) return;
            var pmItem = mDisplayModule.PMAllPackageInfos.Find(info => info.ClientResPath.Equals(_clientGUID)).Datas
                ?.Last();
            var url = string.Format(PM_Define.DownloadURL, pmItem?.ResServerPath);
            var filePath = Directory.GetParent(Application.dataPath).FullName + "/PM_Temp";
            Debug.Log(filePath);
            mIsStartImport = false;
            mCurTime = 0;
            HttpHelper.HttpDownloadFile(url, filePath, true, (_fileName, _fileType, _fileBytes, _fileSize, _curSize) =>
            {
                EditorHelper.ShowProgressBar(_curSize, _fileSize, "下载文件", $"{_fileName}\n   下载中...", () =>
                {
                    mImportFile = $"{filePath}/{_fileName}";
                    mIsStartImport = true;
                });
            });
        }

        /// <summary>
        /// 检测导入Package
        /// </summary>
        private void CheckImportPackage()
        {
            if (!mIsStartImport) return;
            mCurTime += Time.deltaTime * 1f;
            if (mCurTime > 2)
            {
                mIsStartImport = false;
                mCurTime = 0;
                StartImportPackage();
            }
        }

        /// <summary>
        /// 导入Panckage
        /// </summary>
        private void StartImportPackage()
        {
            if (IOHepler.FileExists(mImportFile))
            {
                EditorWindow.GetWindow<PM_DisplayWindow>().Close();
                AssetDatabase.importPackageStarted += packageName =>
                {
                    Debug.Log("///importPackageStarted///" + packageName);
                };
                AssetDatabase.importPackageCompleted += packageName =>
                {
                    Debug.LogError("///Completed////" + packageName);
                    IOHepler.DeleteFile(mImportFile);
                    mIsStartInitPackage = true;
                };
                AssetDatabase.importPackageCancelled += packageName =>
                {
                    Debug.Log("///Cancelled////" + packageName);
                    IOHepler.DeleteFile(mImportFile);
                };
                AssetDatabase.importPackageFailed += (packageName, errorCode) =>
                {
                    Debug.LogError("导入失败：" + errorCode);
                };
                AssetDatabase.ImportPackage(mImportFile, true);
            }
            else
            {
                Debug.LogError("没有找到该文件！");
            }
        }

        /// <summary>
        /// 删除本地版本模块
        /// </summary>
        private void OnClickDeleteEvent(C2S_PB.C2S_PackageInfo _info)
        {
            if (EditorUtility.DisplayDialog("PM Operation", $"该操作即将删除本地 %{_info.Name}% 模块, 是否确认？", "确认", "取消"))
            {
                mCurPackageDir = Path.GetFullPath(_info.ClientResPath);
                if (Directory.Exists(mCurPackageDir))
                {
                    Directory.Delete(mCurPackageDir, true);
                    AssetDatabase.Refresh();
                }
            }
        }


        /// <summary>
        /// 检测初始化Package 对于Package进行项目整理
        /// </summary>
        private void CheckInitPackage()
        {
            if (!mIsStartInitPackage) return;
            mIsStartInitPackage = false;
            var packageInitWin = EditorWindow.GetWindow<PM_PackageInitWindow>(false, "Package初始化");
            packageInitWin.StartInit(mCurPackageDir);
        }

        private string[] GetTitleArray()
        {
            if (mAdminModule.IsAdmin)
            {
                return new[]
                {
                    "ID",
                    "名    称",
                    "作    者",
                    "时    间",
                    "大    小",
                    "操    作",
                };
            }
            else
            {
                return new[]
                {
                    "名称",
                    "本地版本号",
                    "远端版本号",
                    "操作"
                };
            }
        }

        private Vector2[] GetTitleSizeArray()
        {
            if (mAdminModule.IsAdmin)
            {
                return new Vector2[]
                {
                    new Vector2(60, 25),
                    new Vector2(85, 25),
                    new Vector2(85, 25),
                    new Vector2(85, 25),
                    new Vector2(85, 25),
                    new Vector2(100, 25),
                };
            }
            else
            {
                return new Vector2[]
                {
                    new Vector2(160, 26),
                    new Vector2(120, 26),
                    new Vector2(120, 26),
                    new Vector2(100, 26),
                };
            }
        }

        private GUIStyle[] GetTitleStyleArray()
        {
            if (mAdminModule.IsAdmin)
            {
                return new GUIStyle[]
                {
                    pLableGUIStyle,
                    pLableGUIStyle,
                    pLableGUIStyle,
                    pLableGUIStyle,
                    pLableGUIStyle,
                    pLableGUIStyle,
                };
            }
            else
            {
                return new GUIStyle[]
                {
                    pLableGUIStyle,
                    pLableGUIStyle,
                    pLableGUIStyle,
                    pLableGUIStyle,
                };
            }
        }
    }
}