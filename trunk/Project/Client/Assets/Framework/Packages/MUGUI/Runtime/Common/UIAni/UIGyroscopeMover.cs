#if DEBUG
//#define GYRO_TEST
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class UIGyroscopeMover : MonoBehaviour
    {
        [SerializeField]
        private float xMoveSpeed = 5;
        [SerializeField]
        private float xMaxMoveDistance = 250;
        [SerializeField]
        private float yMoveSpeed = 3;
        [SerializeField]
        private float yMaxMoveDistance = 150;
        [SerializeField]
        private float smoothFactor = 1;

        private Quaternion initialRotation = Quaternion.identity;
        private Vector3 initialLocalPosition = Vector3.zero;
        private bool hasInital = false;

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Input.gyro.enabled = false;
            enabled = false;
#else
            Input.gyro.enabled = true;
            enabled = true;
#endif
        }

        // Update is called once per frame
        void Update()
        {

#if GYRO_TEST
            msg = "";
            bool isGyroEnable = Input.gyro.enabled;
            Quaternion currentRotation = Input.gyro.attitude;
            msg = msg + string.Format("is gyro enable ? : {0} , current attitude : {1} , initial attitude : {2}\n", isGyroEnable, currentRotation, initialRotation);
#endif

            if (!Input.gyro.enabled || !IsValidRotation(Input.gyro.attitude))
            {
                //Debug.LogError("not enabled");
                return;
            }


            if (!hasInital)
            {
                var currentAttitude = Input.gyro.attitude;
                if (Mathf.Approximately(currentAttitude.x, 0) && Mathf.Approximately(currentAttitude.y, 0)
                    && Mathf.Approximately(currentAttitude.z, 0) && Mathf.Approximately(currentAttitude.w, 0))
                {
                    return;
                }
                initialRotation = currentAttitude;
                initialLocalPosition = transform.localPosition;
                hasInital = true;
                return;
            }
            //Input.gyro.attitude好像是右手坐标系下的旋转，不过这里不管这一点好像也没事，修改对应的Speed的正负号就好了
            Quaternion relativeRotation = GetRelativeRotation(initialRotation, Input.gyro.attitude);
            //Quaternion relativeRotation = GetRelativeRotation(Quaternion.identity, new Quaternion(-0.5f, -0.2f, -0.6f, -0.6f));
            Vector3 angles = relativeRotation.eulerAngles;
            if (angles.x > 180) angles.x = angles.x - 360;
            if (angles.y > 180) angles.y = angles.y - 360;
            float xOffset = Mathf.Clamp(angles.x * xMoveSpeed, -xMaxMoveDistance, xMaxMoveDistance);
            float yOffset = Mathf.Clamp(angles.y * yMoveSpeed, -yMaxMoveDistance, yMaxMoveDistance);

#if GYRO_TEST
            msg = msg + string.Format("current angles is : {0},current xOffset is : {1} , current yOffset is : {2}\n", angles, xOffset, yOffset);
#endif

            //做一下平滑处理
            Vector3 targetPosition = initialLocalPosition + new Vector3(xOffset, yOffset, 0);

#if GYRO_TEST
            msg = msg + string.Format("current target position : {0}\n", targetPosition);
#endif

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, smoothFactor * Time.deltaTime);

#if GYRO_TEST
            msg = msg + string.Format("local position : {0} local rotation: {1} \n", transform.localPosition, transform.rotation.eulerAngles);
#endif
        }

        private Quaternion GetRelativeRotation(Quaternion current, Quaternion other)
        {
            return Quaternion.Inverse(current) * other;
        }

        private bool IsValidRotation(Quaternion quaternion)
        {
            bool isBetween01 = IsBetween01(quaternion.x) && IsBetween01(quaternion.y) && IsBetween01(quaternion.z) && IsBetween01(quaternion.w);
            float sqrtValue = quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w;
            bool isNormalize = Mathf.Abs(1 - sqrtValue) < 0.1f;
            return isBetween01 && isNormalize;
        }
        private bool IsBetween01(float num)
        {
            return num > -1.001f && num < 1.001f;
        }

#if GYRO_TEST
        string msg = "";
        GUIStyle style;
        private void OnGUI()
        {
            if (style == null)
            {
                style = new GUIStyle();
#if !UNITY_EDITOR
                style.fontSize = 35;
#endif
            }
            GUI.Label(new Rect(300, 200, 1000, 1000), msg, style);
        }
#endif

    }
}

