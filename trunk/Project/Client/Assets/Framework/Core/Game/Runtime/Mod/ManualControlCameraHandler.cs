using Game;
using Game.Core;
using UnityEngine;

namespace MUGame
{
    public class ManualControlCameraHandler : BaseHandler
    {
        /// <summary>
        /// 单例注册
        /// </summary>
        public static ManualControlCameraHandler Instance => HandlerModule.ManualControlCameraHandler;

        /// <summary>
        /// 控制是否开始移动
        /// </summary>
        public bool AutoMove
        {
            get
            {
                return mCanAutoMove;
            }

            set
            {
                mCanAutoMove = value;
            }
        }

        /// <summary>
        /// 设置场景左右移动边界
        /// </summary>
        public Vector2 Border
        {
            get
            {
                return border;
            }

            set
            {
                border = value;
            }
        }

        public int ScreenWidth
        {
            get
            {
                Debug.Log("Screen.width"+ Screen.width);
                return Screen.width;
            }
        }
        public int ScreenHeight => Screen.height;

        private float CameraAutoMoveSpeed = 1f;
        private Vector3 mCameraRVector = Vector3.zero;
        private Vector3 mCameraLookPoint = Vector3.zero;
        private float mCameraTempRoY = 0;
        private int mMoveDir = 0;
        private bool mCanAutoMove = false;
        private Vector2 border = new Vector2(9, -9);

        public override void Update()
        {
            base.Update();
            OnCameraRotateAround();
        }

        private void OnCameraRotateAround()
        {
            if (!mCanAutoMove)
                return;
            if (Camera.main == null)
                return;

            float r = Time.deltaTime * CameraAutoMoveSpeed;
            Transform camTrans = Camera.main.transform;

            if (mCameraRVector == Vector3.zero)
            {
                Vector3 p2xz = Vector3.ProjectOnPlane(camTrans.forward.normalized, Vector3.up);
                Vector3 c1 = Vector3.Cross(camTrans.forward.normalized, Vector3.up);
                float d1 = Vector3.Dot(c1, Vector3.right);
                float ang = (d1 < 0 ? -1 : 1) * Vector3.Angle(camTrans.forward, p2xz);
                float lookY = 0f;
                if (ang == 0)
                {
                    lookY = camTrans.position.y;
                }
                else
                {
                    float pDis = camTrans.position.y / Mathf.Tan(Mathf.Deg2Rad * ang);
                    lookY = ((pDis - camTrans.position.z) / pDis) * camTrans.position.y;
                }
                mCameraLookPoint = Vector3.up * lookY;
                mCameraRVector = camTrans.position - mCameraLookPoint;
            }

            if (mMoveDir == 0)
            {
                if (mCameraTempRoY - r <= border.y)
                {
                    mCameraTempRoY = border.y;
                    mMoveDir = 1;
                }
                else
                {
                    mCameraTempRoY -= r;
                }
            }
            else
            {
                if (mCameraTempRoY + r >= border.x)
                {
                    mCameraTempRoY = border.x;
                    mMoveDir = 0;
                }
                else
                {
                    mCameraTempRoY += r;
                }
            }

            camTrans.position = mCameraLookPoint + Quaternion.Euler(Vector3.up * mCameraTempRoY) * mCameraRVector;
            camTrans.LookAt(mCameraLookPoint);
        }

        public void MoveByMouse(float offsetX)
        {
            if (mCameraRVector == Vector3.zero)
                return;
            if (Camera.main == null)
                return;
            
            Transform camTrans = Camera.main.transform;
            float dis = (offsetX / Screen.dpi) * CameraAutoMoveSpeed;
            float nextR = mCameraTempRoY + dis;
            if (nextR >= border.x)
                mCameraTempRoY = border.x;
            else if (nextR <= border.y)
                mCameraTempRoY = border.y;
            else
                mCameraTempRoY = nextR;

            camTrans.position = mCameraLookPoint + Quaternion.Euler(Vector3.up * mCameraTempRoY) * mCameraRVector;
            camTrans.LookAt(mCameraLookPoint);
        }

        public void ResetMove()
        {
            mCanAutoMove = false;
            mCameraRVector = Vector3.zero;
            mCameraLookPoint = Vector3.zero;
            mCameraTempRoY = 0;
            mMoveDir = 0;
        }
    }
}
