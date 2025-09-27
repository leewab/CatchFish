using Framework.Core;
using ProtoBuf;
using protocol;
using UnityEngine;

namespace Framework.Case.Net
{
    public enum TokenState
    {
        HANDSHAKE_WAIT,                //握手等待 token校验等待
        CLIENT_CALL_ASERVER,           //客户端请求验证
        SERVER_REPLAY_CLIENT,          //服务器回复验证
        CLIENT_REPLAY_SERVER,          //客户端回复验证
        HANDSHAKE_SUCCESS,             //握手成功 token校验成功
        HANDSHAKE_FAIL,                //握手失败 token校验失败
    }
    
    public class TokenHandler : Singleton<TokenHandler>
    {
        public static readonly string HANDLESHAKE_1 = "This is the client. Can you hear me"; 
        public static readonly string HANDLESHAKE_2 = "This is the server. I can hear you. Can you hear me"; 
        public static readonly string HANDLESHAKE_3 = "I'm the client. I can hear you";
        public static readonly string HANDLESHAKE_4 = "I'm the server. you can login now";
        
        /// <summary>
        /// 是否开启token校验
        /// </summary>
        public bool IsOpenVerifyToken = true;

        /// <summary>
        /// 当前的token校验
        /// </summary>
        public TokenState CurTokenState = TokenState.HANDSHAKE_WAIT;

        /// <summary>
        /// 请求token校验
        /// </summary>
        public void ReqVerifyToken()
        {
            if (!IsOpenVerifyToken) return;
            protocol.REQ_Connect handshake = new protocol.REQ_Connect
            {
                token = HANDLESHAKE_1
            };
            NetMsgHandler.SendMsg(ProtoDefine.REQ_Connect, handshake);
            CurTokenState = TokenState.CLIENT_CALL_ASERVER;
        }

        /// <summary>
        /// 响应token校验
        /// </summary>
        public void ResVerifyToken(IExtensible msg)
        {
            if (!IsOpenVerifyToken) return;
            RES_Connect handshake = (RES_Connect) msg;
            string token = handshake.token;
            Debug.Log("Handshake state -------" + token + "--------");
            CurTokenState = TokenState.SERVER_REPLAY_CLIENT;
            if (token == HANDLESHAKE_2)
            {
                RES_Connect handshake2 = new RES_Connect
                {
                    token = HANDLESHAKE_3
                };
                NetMsgHandler.SendMsg(ProtoDefine.RES_Connect, handshake2);
                CurTokenState = TokenState.CLIENT_REPLAY_SERVER;
            }
            else if (token == HANDLESHAKE_4)
            {
                CurTokenState = TokenState.HANDSHAKE_SUCCESS;
                Debug.Log("握手成功");
            }
            else
            {
                CurTokenState = TokenState.HANDSHAKE_FAIL;
                Debug.LogError("握手失败，token校验失败");
            }
        }

        /// <summary>
        /// 是否通过token校验
        /// </summary>
        /// <returns></returns>
        public bool IsVerifyToken()
        {
            return CurTokenState == TokenState.HANDSHAKE_SUCCESS;
        }

        /// <summary>
        /// 是否跳过token校验
        /// </summary>
        /// <returns></returns>
        public bool IsAdjustVerifyToken()
        {
            return !IsOpenVerifyToken || IsVerifyToken();
        }
    }
}