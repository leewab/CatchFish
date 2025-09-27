using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MUEngine
{
    /// <summary>
    /// 用于替代已废弃的JS脚本 “SM_rotateThis” 的C#版脚本
    /// </summary>
    public class SelfRotator : MonoBehaviour
    {
        public float rotationSpeedX = 90;
        public float rotationSpeedY = 0;
        public float rotationSpeedZ = 0;

        private void Update()
        {
            if (rotationSpeedX == 0 && rotationSpeedY == 0 && rotationSpeedZ == 0)
            {
                return;
            }

            Vector3 rotationVec = new Vector3(rotationSpeedX, rotationSpeedY, rotationSpeedZ);
            this.transform.Rotate(rotationVec * Time.deltaTime);
        }

#if UNITY_EDITOR

        /// <summary>
        /// 升级已废弃的JS脚本
        /// </summary>
        /// <param name="node">待处理节点</param>
        /// <returns>是否实际对节点进行了修改</returns>
        public static bool UpgradeJavaScript(GameObject node)
        {
            if (node == null)
            {
                return false;
            }

            Component jsCom = node.GetComponent("SM_rotateThis");
            if (jsCom == null)
            {
                return false;
            }

            SelfRotator rotator = node.GetOrAddComponent<SelfRotator>();
            rotator.rotationSpeedX = (float)(jsCom.GetType().GetField("rotationSpeedX").GetValue(jsCom));
            rotator.rotationSpeedY = (float)(jsCom.GetType().GetField("rotationSpeedY").GetValue(jsCom));
            rotator.rotationSpeedZ = (float)(jsCom.GetType().GetField("rotationSpeedZ").GetValue(jsCom));

            GameObject.DestroyImmediate(jsCom);

            return true;
        }

#endif

    }

}
