using System;
using MUEngine;
using MUGame;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameUIModel3D : GameUIModelBase
    {
        private static Vector2 TargetPivot = new Vector2(0.5f, 0);

        private Vector3 mBaseScale = Vector3.zero;
        private Canvas mUICanvas = null;
        private RectTransform mTrans;

        public override void Load(string res)
        {
            base.Load(res);
            UIModelMgr.SetUIModel3D(UICanvas, this);
        }
        public override void DisposeModel()
        {
            try
            {
                base.DisposeModel();
                UIModelMgr.RemoveUIModel3D(UICanvas, this);
                if (mTimerID != 0)
                {
                    TimerHandler.RemoveTimeactionByID(mTimerID);
                    mTimerID = 0;
                }
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }

        
        protected override void OnInit()
        {
            base.OnInit();
            if (mUITex != null)
            {
                if (mUITex is RawImage)
                {
                    mUITex.color = Color.clear;
                }
                RectTransform target = mUITex.rectTransform;
                //修改中心点，让模型直接站到挂点底部
                Vector2 deltaPivot = target.pivot - TargetPivot;
                if (deltaPivot != Vector2.zero)
                {
                    Vector2 size = target.rect.size;
                    target.pivot = TargetPivot;
                    target.anchoredPosition = target.anchoredPosition - new Vector2(deltaPivot.x * size.x, deltaPivot.y * size.y);
                }
            }
            mTrans = mUITex.rectTransform;
        }

        private int mTimerID = 0;
        private float mInitArmAngle = 0;
        public float InitArmAngle { set { mInitArmAngle = value; } }
        private float mDeltaArmAngle = 0;
        public float DeltaArmAngle { set { mDeltaArmAngle = value; } }

        private Vector3 mRotateAxis = Vector3.right;
        public Vector3 RotateAxis { set { mRotateAxis = value; } }
        private float lasttime;
        private void _RotateArm()
        {
            if (mTimerID == 0 || LogicEntity == null || LogicEntity.GameObject == null) return;
            Transform maintrans = LogicEntity.GameObject.transform;
            Vector3 point = maintrans.TransformPoint(Vector3.zero);
            Vector3 axis = maintrans.TransformDirection(mRotateAxis);
            float curtime = Time.time;
            maintrans.RotateAround(point, axis, mDeltaArmAngle*25*(curtime-lasttime));
            lasttime = curtime;
        }
        public override void AddMount(string res, int type, string bd_name)
        {
            mBaseScale = Vector2.zero;
            base.AddMount(res, type, bd_name);
        }

        public override void RemoveMount()
        {
            mBaseScale = Vector2.zero;
            base.RemoveMount();
            AutoSet();
        }

        protected override int RenderLayer
        {
            get
            {
                return LayerMask.NameToLayer("UI");
            }
        }

        public override void AutoSet()
        {
            base.AutoSet();

            if (LogicEntity == null || LogicEntity.GameObject == null)
            {
                return;
            }

            float _x_offset = this.PosXFactor * mTrans.rect.width;
            float _y_offset = this.PosYFactor * mTrans.rect.height;

            if (mBaseScale == Vector3.zero)
            {
                mBaseScale = Vector3.one * (1 / UICanvas.transform.localScale.x) *
                    (mTrans.rect.height - FootHeight) / (UICanvas.transform as RectTransform).rect.height * UIRoot.UICamera.orthographicSize * 2;
            }

            MUActorEntity p = MountEntity;
            if (p != null)
            {
                Vector3 scale = mBaseScale / (MountHeight + RidingHeight);
                LogicEntity.Scale = scale;
                SetModelRotation(MountInitialAngle);

                if(BackSceneEntity != null)
                {
                    BackSceneEntity.Scale = scale; 
                }
            }
            else
            {
                Vector3 scale = mBaseScale / BodySize;
                LogicEntity.Scale = scale;
                SetModelRotation(InitialAngle);

                if (BackSceneEntity != null)
                {
                    BackSceneEntity.Scale = scale;
                }
            }

            Transform maintrans = LogicEntity.GameObject.transform;
            maintrans.localPosition = new Vector3(_x_offset, FootHeight + _y_offset, -500);
            //LogicEntity.SetEffectOrderInLayer( this.UICanvas.sortingOrder + 1);
            //先渲染模型，再渲染UI，由于UI开启了深度测试，所以遮挡关系将完全靠距离（深度值）确定
            //这个函数使用了一个黑科技，依赖于模型材质与特效材质的RenderQueue的差异（目前，似乎前者是3000，后者是3020），所以可以将它们区分开来，做适合的操作
            //之后这个函数可能会出错，此时请检查所依赖的假定是否成立。或者更换其他的实现方式

            //LogicEntity.SetEffectOrderInLayer(NearestCanvasSortingOrder);

            //走统一的自动设置方法
            //如果有坐骑，那么设置坐骑的SortingOrder不会影响挂在上面的Player，所以两个都得设置
            if(mPlayerEntity != null)
            {
                UIUtil.AutoSetUIEntitySortingOrder(mPlayerEntity);
            }
            if(mMountEntity != null)
            {
                UIUtil.AutoSetUIEntitySortingOrder(mMountEntity);
            }
            //LuaUtil.AutoSetUIEntitySortingOrder(LogicEntity);

            if (BackSceneEntity != null)
            {
                BackSceneEntity.GameObject.transform.localPosition = maintrans.localPosition + mBackSceneOffset;
            }
            if (mInitArmAngle != 0)
            {
                Vector3 center;
                Vector3 axis;
                if (mInitArmAngle > -80)
                {
                    center = mTrans.TransformPoint(new Vector3(0, mTrans.rect.height / 2));
                    axis = new Vector3(0, 0, 1);
                }
                else
                {
                    center = maintrans.TransformPoint(Vector3.zero);
                    axis = new Vector3(1, 0, 0);
                }
                maintrans.RotateAround(center, axis, mInitArmAngle);
            }
            lasttime = Time.time;
            if (mDeltaArmAngle != 0)
                   mTimerID = TimerHandler.SetTimeout(_RotateArm, 0.01f, true, true);
        }

        public override void SetModelRotation(float y)
        {
            y = GetDefaultAngle(y);
            if (LogicEntity != null)
            {
                LogicEntity.Rotation = Quaternion.Euler(0, y, 0);
            }
        }

        private Canvas UICanvas
        {
            get
            {
                if (mUICanvas == null)
                {
                    mUICanvas = UIRoot.Instance.GetNearestCanvas(gameObject);
                }
                
                return mUICanvas;
            }
        }

        public GameUIModel3D(string name, Transform father = null) : base(name, father)
        {
            
        }

        public GameUIModel3D(GameObject obj) : base(obj)
        {
            
        }
        
        public GameUIModel3D() : base()
        {
            
        }
    }
}