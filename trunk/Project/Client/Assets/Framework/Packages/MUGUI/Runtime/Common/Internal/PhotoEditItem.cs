using Game;
using Game.Core;
using Game.UI;
using UnityEngine;
using MUGame;
using UnityEngine.UI;

namespace MUGUI
{
    public class PhotoEditItem : MonoBehaviour
    {

        public Button close;
        public Button rotate;
        public Button scale;
        public Image image;
        public Image border;
        public InputField input;

        public float borderWidth = 20f;

        Vector2 mScaleRange = new Vector2(0.2f, 2);
        Vector2 mSizeDelta;
        Vector2 mInputSizeDelta;
        RectTransform mBorderRect;
        Color borderColor = new Color(0,0,0,1);

        Vector2 mDragOffset = new Vector2(0,0);

        internal System.Action<PhotoEditItem> onClose = null;
        internal System.Action<PhotoEditItem> onShow = null;
       
        void Start()
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get( rotate.gameObject );
            lis.onDrag += OnDragRotate;
            lis.onDragStart += OnBeginDrag;
            lis.onDragEnd += OnEndDrag;

            lis = DragEventTriggerListener.Get( scale.gameObject );
            lis.onDrag += OnDragScale;
            lis.onDragStart += OnBeginDrag;
            lis.onDragEnd += OnEndDrag;

            borderColor = border.color;

            mBorderRect = border.GetComponent<RectTransform>();
            mSizeDelta = image.GetComponent<RectTransform>().sizeDelta;

            EventTriggerListener elis = EventTriggerListener.Get( image.gameObject );
            elis.onDown = OnPointDown;
            elis.onUp = OnPointUp;

            elis = EventTriggerListener.Get( close.gameObject );
            elis.onClick += OnClickClose;
           

            lis = DragEventTriggerListener.Get( image.gameObject );
            lis.onDrag += OnDragImage;
            lis.onDragStart += OnDragImageStart;
            if ( input != null )
            {
                mInputSizeDelta = input.GetComponent<RectTransform>().sizeDelta;
                lis = DragEventTriggerListener.Get( input.textComponent.gameObject );
                lis.onDrag += OnDragImage;
                lis.onDragStart += OnDragImageStart;

                //elis = EventTriggerListener.Get( input.textComponent.gameObject );
                //elis.onDown = OnPointDown;
                //elis.onUp = OnPointUp;
            }

            Image img = gameObject.transform.parent.GetComponent<Image>();
            if ( img != null )
            {
                elis = EventTriggerListener.Get( img.gameObject );
                elis.onClick += OnClickBackground;
            }

            SetSizeDelta( mSizeDelta );
            AdjustPos();

            ShowEdit( true );
            ShowBackground( true );
        }

        public void ShowEdit( bool show )
        {
            close.gameObject.SetActive( show );
            rotate.gameObject.SetActive( show );
            scale.gameObject.SetActive( show );
            border.gameObject.SetActive( show );
            if ( show )
            {
                if ( onShow != null )
                {
                    onShow( this );
                }
            }
        }

        private void OnDestroy()
        {
            Image img = gameObject.transform.parent.GetComponent<Image>();
            if ( img != null )
            {
                EventTriggerListener elis = EventTriggerListener.Get( img.gameObject );
                elis.onClick -= OnClickBackground;
            }
        }

        public bool IsShowEdit()
        {
            return close.gameObject.activeSelf;
        }

        public void SetImageRes( string name )
        {
            ImageLoader imgLoader = ImageLoader.GetOrAddImageLoader(image);
            imgLoader.ImageName = name;
            imgLoader.Reload();
        }

        void ShowBackground( bool show )
        {
            Image img = gameObject.transform.parent.GetComponent<Image>();
            if ( img != null )
            {
                img.enabled = show;
            }
        }

        void OnClickClose( GameObject go )
        {
            ShowBackground( false );

            GameObject.Destroy( gameObject );

            if ( onClose != null )
            {
                onClose( this );
            }
        }

        void OnClickBackground( GameObject go )
        {
            ShowEdit( false );
            ShowBackground( false );
        }

        void OnPointDown( GameObject go)
        {
            SetBtnAlpha( 0 );
        }

        void OnPointUp( GameObject go )
        {
            ShowEdit( true );
            SetBtnAlpha( 1 );
            ShowBackground( true );
        }

        void OnBeginDrag( GameObject go, Vector2 delta )
        {          
            SetBtnAlpha( 0 );
        }

        void OnEndDrag( GameObject go, Vector2 delta )
        {
            SetBtnAlpha( 1 );
            AdjustPos();
        }

        void OnDragImageStart( GameObject go, Vector2 pos )
        {
            Camera cm = GetUICamera();
            if ( input && go == input.textComponent.gameObject )
            {
                Vector2 scrpos = cm.WorldToScreenPoint( input.transform.position );
                mDragOffset = scrpos - pos;
            }
            else
            {
                Vector2 scrpos = cm.WorldToScreenPoint( image.transform.position );
                mDragOffset = scrpos - pos;
            }
        }

        void OnDragImage( GameObject go, Vector2 delta, Vector2 pos )
        {
            pos += mDragOffset;
            Camera cm = GetUICamera();
            Vector3 v3 = cm.ScreenToWorldPoint( pos );
            v3.z = 0;
            image.gameObject.transform.position = v3;
            v3 = image.gameObject.transform.localPosition;
            v3.z = 0;
            image.gameObject.transform.localPosition = v3;
        }

        static Camera sUICamera;
        Camera GetUICamera()
        {
            if ( sUICamera == null )
            {
                GameObject obj = GameObject.Find( "UICamera" );
                sUICamera = obj.GetComponent<Camera>();
            }
            return sUICamera;           
        }

        void OnDragRotate( GameObject go, Vector2 delta, Vector2 pos )
        {
            int i = 0;
            ++i;
            Vector3 angle = image.gameObject.transform.localEulerAngles;
            angle.z -= delta.y;

            image.gameObject.transform.localEulerAngles = angle;
        }

        void SetBtnAlpha( float alpha )
        {
            Color c = new Color( 1, 1, 1, alpha );
            scale.gameObject.GetComponent<Image>().color = c;
            close.gameObject.GetComponent<Image>().color = c;
            rotate.gameObject.GetComponent<Image>().color = c;
            if (alpha > 0)
            {
                border.gameObject.GetComponent<Image>().color = borderColor;
            }
            else
            {
                border.gameObject.GetComponent<Image>().color = c;
            }
            
        }

        void AdjustPos()
        {
            scale.gameObject.transform.localPosition = new Vector3( mBorderRect.sizeDelta.x / 2, -mBorderRect.sizeDelta.y / 2, 0 );
            close.gameObject.transform.localPosition = new Vector3( mBorderRect.sizeDelta.x / 2, mBorderRect.sizeDelta.y / 2, 0 );
            rotate.gameObject.transform.localPosition = new Vector3( -mBorderRect.sizeDelta.x / 2, -mBorderRect.sizeDelta.y / 2, 0 );
        }

        void OnDragScale( GameObject go, Vector2 delta, Vector2 pos )
        {
            int i = 0;
            ++i;

            RectTransform rt = image.gameObject.GetComponent<RectTransform>();
            RectTransform inputRt = input == null ? null : input.gameObject.GetComponent<RectTransform>(); 

            //Debug.Log( delta.ToString() );

            if ( Mathf.Abs(delta.x) > Mathf.Abs(delta.y) )
            {               
                 float s = delta.magnitude * Mathf.Sign(delta.x) / rt.sizeDelta.x;
                 SetSizeDelta( rt.sizeDelta * ( 1 + s ) );
                 if ( inputRt != null )
                {
                    SetInputSizeDelta( inputRt.sizeDelta * ( 1 + s ) );
                }

            }
            else
            {
                float s = delta.magnitude * Mathf.Sign( delta.y ) / rt.sizeDelta.y;
                SetSizeDelta ( rt.sizeDelta * ( 1 - s ) );
                if ( inputRt != null )
                {
                    SetInputSizeDelta( inputRt.sizeDelta * ( 1 - s ) );
                }
            }

            //Debug.Log( scale.gameObject.transform.position );
            //Debug.Log( pos );

        }     

        void SetSizeDelta( Vector2 sizeDelta )
        {
            if ( sizeDelta.x < mSizeDelta.x * mScaleRange.x )
            {
                sizeDelta = mSizeDelta * mScaleRange.x;
            }
            if ( sizeDelta.x > mSizeDelta.x * mScaleRange.y )
            {
                sizeDelta = mSizeDelta * mScaleRange.y;
            }

            RectTransform rt = image.gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = sizeDelta;

            rt = border.gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = sizeDelta + new Vector2( borderWidth, borderWidth );
        }

        void SetInputSizeDelta( Vector2 sizeDelta)
        {
            if ( sizeDelta.x < mInputSizeDelta.x * mScaleRange.x )
            {
                sizeDelta = mInputSizeDelta * mScaleRange.x;
            }
            if ( sizeDelta.x > mInputSizeDelta.x * mScaleRange.y )
            {
                sizeDelta = mInputSizeDelta * mScaleRange.y;
            }

            RectTransform rt = input.gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = sizeDelta;
        }

    }
}
