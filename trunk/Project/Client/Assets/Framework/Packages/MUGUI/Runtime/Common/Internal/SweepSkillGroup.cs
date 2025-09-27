using System;
using Game;
using Game.Core;
using UnityEngine;
using Game.UI;

namespace MUGUI
{
    public class SweepSkillGroup : MonoBehaviour
    {

        public EmptyGraphic border;
        public GameObject rotateRoot1;
        public GameObject rotateRoot2;
        public CanvasGroup[] skillBtn = new CanvasGroup[8];
        public float Rot1 = 180;
        private float Rot2 = 135;
        public float[] showRot = { 0,45,90,135};
        public float rotSpeed = 1f;

        Animator mAnimator;
        Vector2 startPos;
        Vector2 origionPos = new Vector2(0, 0);
        int page = 1;
        bool startRotate = false;
        bool onDrag = false;
        bool isReturn = false;
        System.Action mStartDragCallBack = null;
        System.Action<int> mEndDragCallBack = null;
        System.Action<int> mSweepEndCallBack = null;
        private bool canSweep = false;
        public bool CanSweep
        {
            get { return canSweep; }
            set { canSweep = value; }
        }
        bool forward = true;
        
        void Start()
        {
            mAnimator = this.gameObject.GetComponent<Animator>();
            DragEventTriggerListener lis = DragEventTriggerListener.Get(border.gameObject);
            lis.onDrag += OnDragRotate;
            lis.onDragStart += OnBeginDrag;
            lis.onDragEnd += OnEndDrag;
            origionPos.x = Screen.width - border.transform.localPosition.x;
            origionPos.y = -border.transform.localPosition.y;
            Rot2 = Rot1 + 180f;
        }

        private void Update()
        {
            if (!canSweep)
            {
                return;
            }
            if (!startRotate)
            {
                return;
            }
            //if (isReturn)
            //{
            Quaternion targetRotation1 = new Quaternion(0, 0, 0, 0);
            Quaternion targetRotation2 = new Quaternion(0, 0, 0, 0);
            if (page == 1)
            {
                targetRotation1 = Quaternion.AngleAxis(360, new Vector3(0, 0, 1));
                targetRotation2 = Quaternion.AngleAxis(Rot1, new Vector3(0, 0, 1));
            }
            else
            {
                targetRotation1 = Quaternion.AngleAxis(Rot1, new Vector3(0, 0, 1));
                targetRotation2 = Quaternion.AngleAxis(360, new Vector3(0, 0, 1));
            }

            Quaternion newRotation1 = Quaternion.Slerp(rotateRoot1.transform.localRotation, targetRotation1, rotSpeed);
            Quaternion newRotation2 = Quaternion.Slerp(rotateRoot2.transform.localRotation, targetRotation2, rotSpeed);

            if (Quaternion.Angle(newRotation1, targetRotation1) < 0.2f)
            {
                newRotation1 = targetRotation1;
                newRotation2 = targetRotation2;
                startRotate = false;
                if (mSweepEndCallBack != null)
                {
                    forward = true;
                    mSweepEndCallBack(page);
                }
            }
            rotateRoot1.transform.localRotation = newRotation1;
            rotateRoot2.transform.localRotation = newRotation2;
            CheckButtonVisible(newRotation1.eulerAngles.z, 0);
            //}
            //else
            //{
            /*
                float zAngle1 = rotateRoot1.transform.rotation.eulerAngles.z;
                float zAngle2 = rotateRoot2.transform.rotation.eulerAngles.z;
                if (zAngle1 < 0)
                {
                    zAngle1 += 360;
                }
                if (zAngle2 < 0)
                {
                    zAngle2 += 360;
                }
                float targetAngle1 = 0;
                float targetAngle2 = 0;
                if (page == 1)
                {
                    if (forward)
                    {
                        targetAngle1 = 359.9f;
                        targetAngle2 = Rot1;
                    }
                    else
                    {
                        targetAngle1 = 0.1f;
                        targetAngle2 = Rot1 - 360f;
                    }
                }
                else
                {
                    if (forward)
                    {
                        targetAngle1 = Rot1;
                        targetAngle2 = 359.9f;
                    }
                    else
                    {
                        targetAngle1 = Rot1 - 360f;
                        targetAngle2 = 0.1f;
                    }
                }

                float newAngle1 = zAngle1 + (targetAngle1 - zAngle1) * 0.2f;
                float newAngle2 = zAngle2 + (targetAngle2 - zAngle2) * 0.2f;

                if (targetAngle1 - newAngle1 < 0.5f)
                {
                    newAngle1 = targetAngle1;
                    newAngle2 = targetAngle2;
                    startRotate = false;
                    if (mSweepEndCallBack != null)
                    {
                        forward = true;
                        mSweepEndCallBack(page);
                    }
                }
                */
            //}
        }

        private void OnDestroy()
        {

        }

        private void StartRotate()
        {
            startRotate = true;
            if (page == 1) page = 2;
            else page = 1;
            isReturn = false;
        }

        private void StartReturn()
        {
            startRotate = true;
            isReturn = true;
        }

        void OnBeginDrag(GameObject go, Vector2 pos)
        {
            if (!canSweep)
            {
                return;
            }
            startPos = pos;
            onDrag = true;
            startRotate = false;
            if(mStartDragCallBack != null)
            {
                mStartDragCallBack();
            }
        }

        void OnEndDrag(GameObject go, Vector2 pos)
        {
            if (!canSweep)
            {
                return;
            }
            if (onDrag == false)
            {
                return;
            }
            if (Vector2.Distance(pos, startPos) > 5)
            {
                Vector2 Arrow1 = (startPos - origionPos).normalized;
                Vector2 Arrow2 = (pos - origionPos).normalized;
                float cross = Arrow1.x * Arrow2.y - Arrow1.y * Arrow2.x;
                if (Vector2.Angle(Arrow1, Arrow2) > 20)
                {
                    forward = cross >= 0;
                    StartRotate();
                }
                else
                {
                    StartReturn();
                }
            }
            else
            {
                StartReturn();
            }
            if (mEndDragCallBack != null)
            {
                mEndDragCallBack(page);
            }
            onDrag = false;
        }

        void OnDragRotate(GameObject go, Vector2 delta, Vector2 pos)
        {
            if (!canSweep)
            {
                return;
            }
            if (onDrag == false)
            {
                return;
            }
            Vector2 Arrow1 = (startPos - origionPos).normalized;
            Vector2 Arrow2 = (pos - origionPos).normalized;
            float cross = Arrow1.x * Arrow2.y - Arrow1.y * Arrow2.x;
            Quaternion targetRotation1 = new Quaternion(0, 0, 0, 0);
            Quaternion targetRotation2 = new Quaternion(0, 0, 0, 0);
            float mAngel = Vector2.Angle(Arrow1, Arrow2);
            if (page == 1)
            {
                if (cross >= 0)
                {
                    targetRotation1 = Quaternion.AngleAxis(mAngel, new Vector3(0, 0, 1));
                    targetRotation2 = Quaternion.AngleAxis(mAngel + Rot1, new Vector3(0, 0, 1));
                    if (mAngel > 100)
                    {
                        forward = true;
                        OnEndDrag(go, pos);
                    }
                    CheckButtonVisible(mAngel, 1);
                }
                else
                {
                    targetRotation1 = Quaternion.AngleAxis(mAngel, new Vector3(0, 0, -1));
                    targetRotation2 = Quaternion.AngleAxis(mAngel + Rot1, new Vector3(0, 0, -1));
                    if (mAngel > 100)
                    {
                        forward = false;
                        OnEndDrag(go, pos);
                    }
                    CheckButtonVisible(-mAngel, -1);
                }
            }
            else
            {
                if (cross >= 0)
                {
                    targetRotation1 = Quaternion.AngleAxis(mAngel + Rot1, new Vector3(0, 0, 1));
                    targetRotation2 = Quaternion.AngleAxis(mAngel, new Vector3(0, 0, 1));
                    if (mAngel > 100)
                    {
                        forward = true;
                        OnEndDrag(go, pos);
                    }
                    CheckButtonVisible(mAngel + Rot1, 1);
                }
                else
                {
                    targetRotation1 = Quaternion.AngleAxis(mAngel + Rot1, new Vector3(0, 0, -1));
                    targetRotation2 = Quaternion.AngleAxis(mAngel, new Vector3(0, 0, -1));
                    if (mAngel > 100)
                    {
                        forward = false;
                        OnEndDrag(go, pos);
                    }
                    CheckButtonVisible(-(mAngel + Rot1), -1);
                }
            }
            rotateRoot1.transform.localRotation = targetRotation1;
            rotateRoot2.transform.localRotation = targetRotation2;
            
        }

        public void UpdateDragEnable(bool bEnable)
        {
            if (border)
            {
                border.raycastTarget = bEnable;
                //border.enabled = bEnable;
            }
        }

        void CheckButtonVisible(float zAngle, int arrow)
        {
            if (zAngle < 0)
            {
                zAngle = zAngle + 360;
            }
            //int realArrow = page;
            //if (arrow == 1)
            //{
            //    if (realArrow == 1) realArrow = 2;
            //    else realArrow = 1;
            //}
            //print("zAngel:"+ zAngle);
            bool revert = false;
            if (zAngle > 180)
            {
                zAngle = zAngle - 180;
                revert = true;
            }
            
            float[] alpha = new float[4];
            for (int i = 0; i < 4; i++)
            {
                alpha[i] = (zAngle - showRot[i]) / 25;
                if (alpha[i] < 0) alpha[i] = 0;
                else if (alpha[i] > 1) alpha[i] = 1;
                //if (realArrow == 1)
                //{
                //skillBtn[i].alpha = 1 - alpha[i]; 
                //skillBtn[i + 4].alpha = alpha[i];
                //}
                //else
                //{
                //skillBtn[i].alpha = 1 - alpha[i];
                //skillBtn[i + 4].alpha = alpha[i];
                //}   
                if (revert == false)
                {
                    skillBtn[i].alpha = 1 - alpha[i];
                    skillBtn[i + 4].alpha = alpha[i];
                }
                else
                {
                    skillBtn[i].alpha = alpha[i]; 
                    skillBtn[i + 4].alpha = 1 - alpha[i];
                }
            }
            for (int i = 0; i < 8; i++)
            {
                skillBtn[i].blocksRaycasts = (skillBtn[i].alpha == 1);
            }
        }

        public void ResetAll()
        {
            startRotate = false;
            page = 1;
            rotateRoot1.transform.localRotation = new Quaternion(0, 0, 0, 0);
            rotateRoot2.transform.localRotation = Quaternion.AngleAxis(Rot1, new Vector3(0, 0, 1));
            onDrag = false;
            isReturn = false;
            CheckButtonVisible(-0.1f,0);
        }

        public void ResetToCurrentPage()
        {
            startRotate = false;
            if (page == 1)
            {
                rotateRoot1.transform.localRotation = new Quaternion(0, 0, 0, 0);
                rotateRoot2.transform.localRotation = Quaternion.AngleAxis(Rot1, new Vector3(0, 0, 1));
                CheckButtonVisible(-0.1f, 0);
            }
            else if(page == 2)
            {
                rotateRoot1.transform.localRotation = Quaternion.AngleAxis(360 - Rot1, new Vector3(0, 0, 1));
                rotateRoot2.transform.localRotation = new Quaternion(0, 0, 0, 0);
                CheckButtonVisible(360 - Rot1, 0);
            }
            onDrag = false;
            isReturn = false;
            //CheckButtonVisible(-0.1f, 0);
        }

        public void AddRotateStartDragBack(Action func)
        {
            mStartDragCallBack = func;
        }

        public void AddRotateEndDragBack(Action<int> func)
        {
            mEndDragCallBack = func;
        }

        public void AddSweepEndCallBack(Action<int> func)
        {
            mSweepEndCallBack = func;
        }

        public void ForceSweepPage(int page)
        {
            if (!canSweep)
            {
                return;
            }
            if (this.page == 1)
            {
                rotateRoot1.transform.localRotation = Quaternion.AngleAxis(0.5f, new Vector3(0, 0, 1));
                rotateRoot2.transform.localRotation = Quaternion.AngleAxis(0.5f + Rot1, new Vector3(0, 0, 1));
            }
            else
            {
                rotateRoot1.transform.localRotation = Quaternion.AngleAxis(0.5f + Rot1, new Vector3(0, 0, 1));
                rotateRoot2.transform.localRotation = Quaternion.AngleAxis(0.5f, new Vector3(0, 0, 1));
            }

            startRotate = true;
            if (this.page == page)
            {
                isReturn = true;
            }
            else
            {
                this.page = page;
                isReturn = false;
            }
        }

        public void ForceToPageImmt(int page)
        {
            this.page = page;
            if (this.page == 1)
            {
                rotateRoot1.transform.localRotation = Quaternion.AngleAxis(0f, new Vector3(0, 0, 1));
                rotateRoot2.transform.localRotation = Quaternion.AngleAxis(0f + Rot1, new Vector3(0, 0, 1));
                CheckButtonVisible(0, 0);
            }
            else
            {
                rotateRoot1.transform.localRotation = Quaternion.AngleAxis(0f + Rot1, new Vector3(0, 0, 1));
                rotateRoot2.transform.localRotation = Quaternion.AngleAxis(0f, new Vector3(0, 0, 1));
                CheckButtonVisible(180, 0);
            }
            if (mSweepEndCallBack!=null)
            {
                mSweepEndCallBack(page);
            }
        }
        public void PrepareHideAll()
        {
            onDrag = false;
            startRotate = false;
            if (page == 1)
            {
                skillBtn[4].alpha = 0;
                skillBtn[5].alpha = 0;
                skillBtn[6].alpha = 0;
                skillBtn[7].alpha = 0;
            }
            else
            {
                skillBtn[0].alpha = 0;
                skillBtn[1].alpha = 0;
                skillBtn[2].alpha = 0;
                skillBtn[3].alpha = 0;
            }
        }
        
    }
}
