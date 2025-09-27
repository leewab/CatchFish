/***********************************************
				EasyTouch V
	Copyright © 2014-2015 The Hedgehog Team
    http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HedgehogTeam.EasyTouch
{
    // This is the class that simulate touches with the mouse.
    // Internal use only, DO NOT USE IT
    public class EasyTouchInput
    {
        private enum PointerType
        {
            LeftMouse = 0,
            Simulate = 1,
            RightMouse = 2,
        }
        private enum PointerState
        {
            None,
            Begin,
            End,
            Touching,
        }
        private List<PointerType> pointers = new List<PointerType>();

        #region private members
        private Vector2[] oldMousePosition = new Vector2[3];
        private int[] tapCount = new int[3];
        private float[] startActionTime = new float[3];
        private float[] deltaTime = new float[3];
        private float[] tapeTime = new float[3];

        // Complexe 2 fingers simulation
        private bool bComplex = false;
        private Vector2 deltaFingerPosition;
        private Vector2 oldFinger2Position = Vector2.zero;
        private Vector2 complexCenter;
        #endregion

        #region Public methods
        // Return the number of touch
        public int TouchCount()
        {

#if ((UNITY_ANDROID || UNITY_IOS || UNITY_WINRT || UNITY_BLACKBERRY || UNITY_TVOS || CLOUD_GAME) && !UNITY_EDITOR)
			return getTouchCount(true);
#else
            return getTouchCount(false);
#endif

        }

        private int getTouchCount(bool realTouch)
        {
            int count = 0;

            if (realTouch || EasyTouch.instance.enableRemote)
            {
                count = Input.touchCount;
            }
            else
            {
                pointers.Clear();
                if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(0))
                {
                    //count = 1;
                    pointers.Add(PointerType.LeftMouse);
                    if (EasyTouch.GetSecondeFingerSimulation())
                    {
                        if (IsAnyKeyDown() || IsAnyKeyTouching() || IsAnyKeyUp())
                        {
                            pointers.Add(PointerType.Simulate);
                        }
                    }
                }
                if (Input.GetMouseButton(1) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonDown(1))
                {
                    pointers.Add(PointerType.RightMouse);
                }
                count = pointers.Count;
                if (count == 0)
                {
                    complexCenter = Vector2.zero;
                    oldMousePosition[0] = new Vector2(-1, -1);
                    oldMousePosition[1] = new Vector2(-1, -1);
                }
            }
            return count;
        }

        // return in Finger structure all informations on an touch
        public Finger GetMouseTouch(int fingerIndex, Finger myFinger)
        {

            Finger finger;

            if (myFinger != null)
            {
                finger = myFinger;
            }
            else
            {
                finger = new Finger();
                finger.gesture = EasyTouch.GestureType.None;
            }

            if(fingerIndex < 0 || fingerIndex >= pointers.Count)
            {
                return null;
            }
            var pointerType = pointers[fingerIndex];
            PointerState state = GetPointerState(pointerType);
            Vector2 position = GetPointerPosition(pointerType);

            //fingerIndex在这里有两种意思，第一种是：在目前所有按下的finger列表中的序号
            //另外一种是：某个finger的唯一ID （可能会复用，但是不会发生冲突）
            //当finger数量发生变化的时候，某个finger在当前finger列表中的序号很可能发生变化，但是唯一ID不应该发生变化
            //该函数传入的fingerIndex是第一种意义，但是之后都是用第二种意义
            //为避免误解，之后使用fingerId来代表finger
            var fingerId = (int)pointerType;
            finger.fingerIndex = fingerId;
            finger.position = position;

            if(pointerType == PointerType.Simulate && oldFinger2Position == Vector2.zero)
            {
                oldFinger2Position = position;
            }

            if (state == PointerState.Begin)
            {
                finger.deltaPosition = Vector2.zero;
                tapCount[fingerId] = tapCount[fingerId] + 1;
                finger.tapCount = tapCount[fingerId];
                startActionTime[fingerId] = Time.realtimeSinceStartup;
                deltaTime[fingerId] = startActionTime[fingerId];
                finger.deltaTime = 0;
                finger.phase = TouchPhase.Began;
                oldMousePosition[fingerId] = finger.position;

                if (tapCount[fingerId] == 1)
                {
                    tapeTime[fingerId] = Time.realtimeSinceStartup;
                }
            }
            else if(state == PointerState.End)
            {
                finger.deltaPosition = finger.position - oldMousePosition[fingerId];
                finger.tapCount = tapCount[fingerId];
                finger.deltaTime = Time.realtimeSinceStartup - deltaTime[fingerId];
                finger.phase = TouchPhase.Ended;
                oldMousePosition[fingerId] = finger.position;
            }
            else if(state == PointerState.Touching)
            {
                if (Time.realtimeSinceStartup - tapeTime[fingerId] > 0.5)
                {
                    tapCount[fingerId] = 0;
                }
                finger.deltaPosition = finger.position - oldMousePosition[fingerId];

                finger.tapCount = tapCount[fingerId];
                finger.deltaTime = Time.realtimeSinceStartup - deltaTime[fingerId];
                if (finger.deltaPosition.sqrMagnitude < 1)
                {
                    finger.phase = TouchPhase.Stationary;
                }
                else
                {
                    finger.phase = TouchPhase.Moved;
                }

                oldMousePosition[fingerId] = finger.position;
                deltaTime[fingerId] = Time.realtimeSinceStartup;
            }
            else
            {
                finger = null;
            }
            return finger;
        }

        // Get the position of the simulate second finger
        public Vector2 GetSecondFingerPosition()
        {

            Vector2 pos = new Vector2(-1, -1);

            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(EasyTouch.instance.twistKey)) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(EasyTouch.instance.swipeKey)))
            {
                if (!bComplex)
                {
                    bComplex = true;
                    deltaFingerPosition = (Vector2)Input.mousePosition - oldFinger2Position;
                }
                pos = GetComplex2finger();
                return pos;
            }
            else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(EasyTouch.instance.twistKey))
            {
                pos = GetPinchTwist2Finger();
                bComplex = false;
                return pos;
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(EasyTouch.instance.swipeKey))
            {

                pos = GetComplex2finger();
                bComplex = false;
                return pos;
            }

            return pos;
        }
        #endregion

        #region Private methods
        // Get the postion of simulate finger
        private Vector2 GetPointerPosition(PointerType pointerType)
        {
            if (pointerType == PointerType.LeftMouse || pointerType == PointerType.RightMouse)
            {
                return Input.mousePosition;
            }
            else
            {
                return GetSecondFingerPosition();
            }
        }
        private PointerState GetPointerState(PointerType pointerType)
        {
            if(pointerType == PointerType.LeftMouse || pointerType == PointerType.RightMouse)
            {
                int touchIndex = pointerType == PointerType.LeftMouse ? 0 : 1;
                if (Input.GetMouseButtonDown(touchIndex))
                {
                    return PointerState.Begin;
                }else if (Input.GetMouseButtonUp(touchIndex))
                {
                    return PointerState.End;
                }else if (Input.GetMouseButton(touchIndex))
                {
                    return PointerState.Touching;
                }
                else
                {
                    return PointerState.None;
                }
            }
            else
            {
                if((Input.GetMouseButton(0) && IsAnyKeyDown()) || (IsAnyKeyTouching() && Input.GetMouseButtonDown(0)))
                {
                    return PointerState.Begin;
                }else if((Input.GetMouseButton(0) && IsAnyKeyUp()) || (IsAnyKeyTouching() && Input.GetMouseButtonUp(0)) || (Input.GetMouseButtonUp(0) && IsAnyKeyUp()))
                {
                    return PointerState.End;
                }else if(Input.GetMouseButton(0) && IsAnyKeyTouching())
                {
                    return PointerState.Touching;
                }
                else
                {
                    return PointerState.None;
                }
            }
        }
        private bool IsAnyKeyDown()
        {
            return Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(EasyTouch.instance.twistKey) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(EasyTouch.instance.swipeKey);
        }
        private bool IsAnyKeyUp()
        {
            return Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(EasyTouch.instance.twistKey) || Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(EasyTouch.instance.swipeKey);
        }
        private bool IsAnyKeyTouching()
        {
            return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(EasyTouch.instance.twistKey) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(EasyTouch.instance.swipeKey);
        }

        // Simulate for a twist or pinc
        private Vector2 GetPinchTwist2Finger(bool newSim = false)
        {

            Vector2 position;

            if (complexCenter == Vector2.zero)
            {
                position.x = (Screen.width / 2.0f) - (Input.mousePosition.x - (Screen.width / 2.0f));
                position.y = (Screen.height / 2.0f) - (Input.mousePosition.y - (Screen.height / 2.0f));
            }
            else
            {
                position.x = (complexCenter.x) - (Input.mousePosition.x - (complexCenter.x));
                position.y = (complexCenter.y) - (Input.mousePosition.y - (complexCenter.y));
            }
            oldFinger2Position = position;

            return position;
        }

        // complexe Alt + Ctr
        private Vector2 GetComplex2finger()
        {

            Vector2 position;

            position.x = Input.mousePosition.x - deltaFingerPosition.x;
            position.y = Input.mousePosition.y - deltaFingerPosition.y;

            complexCenter = new Vector2((Input.mousePosition.x + position.x) / 2f, (Input.mousePosition.y + position.y) / 2f);
            oldFinger2Position = position;

            return position;
        }
        #endregion
    }
}

