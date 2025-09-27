namespace Game
{
    public enum UIProperty
    {
        //默认都是Set Get 如果是只是其中一个，会用(s)(g)标识
        //每个段预留20个

        //基础属性(1-30)
        Component               = 0,
        Name                    = 1,    //(g)
        Visible                 = 2,
        Layer                   = 3,
        Tag                     = 4,
        Scale                   = 5,
        ScaleX                  = 6,
        ScaleY                  = 7,
        ScaleZ                  = 8,
        Position                = 9,
        PositionX               = 10,
        PositionY               = 11,
        PositionZ               = 12,
        LocalPosition           = 13,
        LocalPositionX          = 14,
        LocalPositionY          = 15,
        LocalPositionZ          = 16,
        Rotation                = 17,
        RotationQuaternion      = 18,
        LocalRotation           = 19,
        LocalRotationQuaternion = 20,
        AnchoredPosition        = 21,

        
        //UI基础属性(31-35)
        Width                   = 31,
        Height                  = 32,
        AnchorsMin              = 33,
        AnchorsMax              = 34,
        
        //UI其他公共属性(某几个组件共用的)(36-40)
        Enable                  = 36,   // 1 Button.interactable 2 Canvas.enabled
        Gray                    = 37,   //(s) Button Image
        Material                = 38,   //(s) Image Text
        Touch                   = 39,   // 触摸出现摇杆
        Color                   = 52,


        //UI-Text文本属性(51-70)
        Text                    = 51,
        TxtLength               = 53,   //(g) 文本长度
        FontSize                = 54,
        PreferredWidth          = 55,
        Alignment               = 56,
        PreferredHeight         = 57,

        //UI-Image图片属性(71-90)
        Image                   = 71,
        FillAmmount             = 72,  //(g)
        Alpha                   = 73,  //(g)
        NativeSize              = 74,  //(g)设置原始大小

        //UI-Button按钮属性(91-100)
        ShowEnable              = 91,  //显示禁用态，逻辑上按钮依然可以点击

        //UI-Checkbox复选框属性(101-120)
        Checked                 = 101, //设置选中
        Activated               = 102, //设置 Toggle的enable
        ChangeCallback          = 103, //(s)设置valuechange事件 
        AllTogglesOff           = 104, //(s)group设置所有的复选框为非选中
        AnyTogglesOn            = 105, //(g)group中是否有选中的复选框

        //UI-Progress进度条属性(121-140)
        ProgressValue           = 121, //进度
        ValueHandler            = 122, //(s)设置OnValueChanged回调函数

        //UI-Grid属性(141-160)
        DocPosition             = 141,
        DocSize                 = 142, //(g)
        ScrollNotifyFunc        = 143,
        ScrollPosChangeFunc     = 144,
        ScrollNotifyVal         = 145,
        AddScrollCallBack       = 146, //(s)
        RemoveScrollCallBack    = 147, //(s)
        ItemSize                = 148, //(g)

        //UI-Effect属性(161-180)
        EffectActor             = 161, //(g) 获得gameobject
        PlayEffect              = 162, //(s)
        DisposeActor            = 163, //(s)

        //UI-Dropdown下拉列表属性(181-200)
        UpdateCaptionText       = 181, //(g)设置下拉列表选中显示的文本

        //GameImageTween属性(201-220)
        TweenFillAmmount        = 201, //(s)
        Clear = 202,//(s)

        //GameNumberText(221-240)
        IsRandom                = 221, //(s)
        CheckTime               = 222, //(s)
        Begin                   = 223, //(s)
        Stop                    = 224, //(s)
        Order                   = 225, //(s)
        FadeOut                 = 226, //(s)

        //GameRichText(241-260)
        AdvanceTextEnable       = 241, //(s)
        SetTextureId            = 242, //(s)
        SetAutoAdapt            = 243, //(s)
        SetSpaceReplace         = 244, //(s)

        //GameWWWImage属性(261-280)
        urlKey                  = 261, //(s)

        //GameTexture属性(281-300)
        TextureName             = 281, //(s)
        Texture                 = 282, //(Texture2D)
        Callback = 1002,
    }
}