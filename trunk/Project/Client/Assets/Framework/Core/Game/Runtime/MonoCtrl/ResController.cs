// using Game.UI;
//
// namespace Game
// {
//     public class ResController : BaseMono
//     {
//         private bool _isSkip = true;
//         
//         // private HotfixController _hotfixCtrl;
//         
//         private AppLoadingPanel _appLoadingPanel;
//         
//         private void Start()
//         {
//             LogUtil.Log("ResController Start");
//             if (_isSkip) return;
//             _appLoadingPanel = UIManager.Instance.OpenLoading<AppLoadingPanel>();
//             _appLoadingPanel.ShowLoadingProgress(0);
//             _appLoadingPanel.ShowLoadingTips("检查更新...");
//             if (AppManager.LocalAppFile.IsOpenHotfix)
//             {
//                 StartNormalHotfix();
//             }
//             else
//             {
//                 EndNormalHotfix();
//             }
//         }
//
//         /// <summary>
//         /// 开启正常热更 （热更流程：下载远端AppFile-大包检测更新-启动首次更新-进入游戏二次更新）
//         /// </summary>
//         private void StartNormalHotfix()
//         {
//             // _hotfixCtrl = gameObject.AddOneComponent<HotfixController>();
//             // //开始热更
//             // _hotfixCtrl.StartHotfix();
//         }
//
//         /// <summary>
//         /// 结束正常热更
//         /// </summary>
//         private void EndNormalHotfix()
//         {
//             //开始延迟加载
//             StartDelayLoading();
//         }
//
//         /// <summary>
//         /// 开启定向热更 （更新流程：点击触发某一模块更新，包括场景地图、语言切换、精度切换、或者游戏自动设置到达某一等级或者触发某一条件，开启后台定向热更）
//         /// </summary>
//         private void StartDirectionalHotfix(string dire)
//         {
//             
//         }
//
//         /// <summary>
//         /// 结束定向热更
//         /// </summary>
//         private void EndDirectionalHotfix()
//         {
//             
//         }
//         
//         /// <summary>
//         /// 开启预加载
//         /// </summary>
//         private void StartPreload()
//         {
//             
//         }
//
//         /// <summary>
//         /// 结束预加载
//         /// </summary>
//         private void EndPreload()
//         {
//             
//         }
//
//         public void StartDelayLoading(string[] resArray)
//         {
//             for (int i = 0; i < resArray.Length; i++)
//             {
//                 _appLoadingPanel.ShowLoadingProgress((float)i/resArray.Length);
//                 _appLoadingPanel.ShowLoadingTips($"正在加载资源 {resArray[i]}");
//             }
//         }
//         
//         private void StartDelayLoading()
//         {
//             _appLoadingPanel.ShowLoadingProgress(0);
//             _appLoadingPanel.ShowLoadingTips("正在加载资源...");
//             UIManager.Instance.Close(_appLoadingPanel, true);
//         }
//
//     }
// }