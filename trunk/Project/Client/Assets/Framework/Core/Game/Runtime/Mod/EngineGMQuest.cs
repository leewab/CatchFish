using UnityEngine;

namespace MUGame {

    //引擎组加的客户端GM命令对lua的回调执行的类
    public static class EngineGMQuest {
        
        // 设置UI角色主光
        public static void SetUIMainLightColor(float r, float g, float b) {
            Shader.SetGlobalColor("_mLightColor", new Color(r, g, b, 1));
        }

        // 设置UI角色环境光
        public static void SetUIAmbientColor(float r, float g, float b) {
            Shader.SetGlobalColor("_CustomAmbient", new Color(r, g, b, 1));
        }
    }
}
