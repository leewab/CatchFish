// using System;
// using UnityEngine.SceneManagement;
//
// namespace Game
// {
//     public class SceneController : BaseMono
//     {
//         public void LoadScene(string sceneName)
//         {
//             
//         }
//                 
//         /// <summary>
//         /// 过渡切换场景
//         /// </summary>
//         /// <param name="nextScene"></param>
//         public void SceneTransferSwitch(string nextScene, Action<float> callBack)
//         {
//             var operationLoad = SceneManager.LoadSceneAsync(nextScene);
//             callBack?.Invoke(operationLoad.progress);
//             if (operationLoad.isDone)
//             {
//                 var operationUnload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
//                 if (operationUnload.isDone)
//                 {
//                     SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextScene));
//                 }
//             }
//         }
//     }
// }