
namespace Game.UI
{
    /// <summary>
    /// UI组件工厂类
    /// 提供了创建UI组件的相关接口
    /// </summary>
    public static class UIComponentFactory
    {
	    public static T Make<T>(GameComponent maker, string path) where T : GameComponent
	    {
		    if (maker == null)
		    {
			    return null;
		    }
		    else
		    {
			    return maker.LMake<T>(path) as T;
		    }
	    }
	    
	    /// <summary>
	    /// 构建Component对象 已知具体对象和父对象
	    /// </summary>
	    /// <param name="obj">Component绑定的GameObject</param>
	    /// <param name="parent">GameObject所在的父对象</param>
	    /// <returns></returns>
	    public static GameComponent MakeComponent(UnityEngine.GameObject obj, UnityEngine.Transform parent = null)
	    {
		    if (obj == null) return null;
		    return new GameComponent(obj, parent);
	    }

	    /// <summary>
	    /// 构建GameComponent对象 已知GameObject的可加载资源名称
	    /// </summary>
	    /// <param name="prefabName"></param>
	    /// <param name="parent"></param>
	    /// <returns></returns>
	    public static GameComponent MakeComponent(string prefabName, UnityEngine.Transform parent = null)
	    {
		    if (string.IsNullOrEmpty(prefabName)) return null;
		    return new GameComponent(prefabName, parent);
	    }
	    
	    /// <summary>
	    /// 构建GameComponent对象 已知父节点的IComponent对象和子对象路径
	    /// </summary>
	    /// <param name="maker"></param>
	    /// <param name="path"></param>
	    /// <returns></returns>
	    public static GameComponent MakeComponent(GameComponent maker, string path)
	    {
		    return Make<GameComponent>(maker, path);
	    }

	    /// <summary>
	    /// 构建EntityComponent派生类组件
	    /// </summary>
	    /// <param name="maker"></param>
	    /// <param name="path"></param>
	    /// <typeparam name="T"></typeparam>
	    /// <returns></returns>
	    public static T MakePlayerComponent<T>(GameComponent maker, string path) where T : BaseEntityComponent
	    {
		    return Make<T>(maker, path);
	    }

	    /// <summary>
	    /// 构建GamePanel对象 已知具体对象和父对象
	    /// </summary>
	    /// <param name="obj">Panel绑定的GameObject</param>
	    /// <param name="parent">GameObject所在的父对象</param>
	    /// <returns></returns>
	    public static T MakePanel<T>(UnityEngine.GameObject obj, UnityEngine.Transform parent = null) where T : GamePanel, new()
	    {
		    if (obj == null) return null;
		    T panel = new T();
		    panel.Create(obj, parent);
		    return panel;
	    }
	    
	    /// <summary>
	    /// 构建GamePanel对象 已知Panel的可加载资源名称
	    /// </summary>
	    /// <param name="panelName"></param>
	    /// <param name="parent"></param>
	    /// <returns></returns>
	    public static T MakePanel<T>(string panelName, UnityEngine.Transform parent = null) where T : GamePanel, new()
	    {
		    if (string.IsNullOrEmpty(panelName)) return null;
		    T panel = new T();
		    panel.Create(panelName, parent);
		    return panel;
	    }
	    
	    public static GamePartUIComponent MakePartUIComponent(GameUIComponent view, string path)
	    {
		    GamePartUIComponent c = new GamePartUIComponent(view, path);
	        view.AddChildComponent(c, view);
	        return c;
	    }
	    
	    public static GameUIComponent MakeUIComponent(GameUIComponent maker, string path)
	    {
		    return Make<GameUIComponent>(maker, path);
	    }

	    public static GameLabel MakeLabel(GameUIComponent maker, string path)
	    {
		    return Make<GameLabel>(maker, path);
	    }

	    public static GameInputField MakeInputField(GameUIComponent maker, string path)
	    {
		    return Make<GameInputField>(maker, path);
	    }

	    public static GameButton MakeButton(GameUIComponent maker, string path)
	    {
		    return Make<GameButton>(maker, path);
	    }

	    public static GameImage MakeImage(GameUIComponent maker, string path)
	    {
#if UNITY_EDITOR
		    var gameImage = Make<GameImage>(maker, path);
		    if (GameConfig.LoadFromPrefab && gameImage != null)
		    {
			    var imageLoader = gameImage.gameObject.GetComponent<ImageLoader>();
			    if (imageLoader != null && string.IsNullOrEmpty(imageLoader.ImageName))
			    {
				    //先获取到原来的sprite
				    var sprite = imageLoader.UIImage.sprite;
				    if (sprite != null)
				    {
					    //猜测图片名称
					    var imageName = sprite.name + ".png";
					    //设置ImageName会Release原来的sprite ？？！！
					    imageLoader.ImageName = imageName;
					    //恢复被Release的Sprite
					    imageLoader.UIImage.sprite = sprite;
				    }
			    }
		    }

		    return gameImage;
#endif
		    return Make<GameImage>(maker, path);
	    }

	    public static GameImage MakeImageTween(GameUIComponent maker, string path)
	    {
		    return Make<GameImageTween>(maker, path);
	    }

	    public static GameImageBar MakeImageBar(GameUIComponent maker, string path)
	    {
		    return Make<GameImageBar>(maker, path);
	    }

	    public static GameImageRoll MakeImageRoll(GameUIComponent maker, string path)
	    {
		    return Make<GameImageRoll>(maker, path);
	    }

	    public static GameAnimatorImage MakeAnimatorImage(GameUIComponent maker, string path)
	    {
		    return Make<GameAnimatorImage>(maker, path);
	    }

	    public static GameGridV2 MakeGrid(GameUIComponent maker, string path)
	    {
		    return Make<GameGridV2>(maker, path);
	    }

	    public static UIListView MakeUIListView(GameUIComponent maker, string path)
	    {
		    return Make<UIListView>(maker, path);
	    }

	    public static GameChatList MakeGameChatList(GameUIComponent maker, string path)
	    {
		    return Make<GameChatList>(maker, path);
	    }

	    public static GameCommonList MakeGameCommonList(GameUIComponent maker, string path)
	    {
		    return Make<GameCommonList>(maker, path);
	    }

	    public static GameTree MakeTree(GameUIComponent maker, string path)
	    {
		    return Make<GameTree>(maker, path);
	    }

	    public static UISampleList MakeUISampleList(GameUIComponent maker, string path)
	    {
		    return Make<UISampleList>(maker, path);
	    }

	    public static UISampleFocusList MakeUISampleFocusList(GameUIComponent maker, string path)
	    {
		    return Make<UISampleFocusList>(maker, path);
	    }

	    public static GameRichText MakeRichText(GameUIComponent maker, string path)
	    {
		    return Make<GameRichText>(maker, path);
	    }

	    public static GameNumberText MakeNumberText(GameUIComponent maker, string path)
	    {
		    return Make<GameNumberText>(maker, path);
	    }

	    public static GameCheckBox MakeCheckBox(GameUIComponent maker, string path)
	    {
		    return Make<GameCheckBox>(maker, path);
	    }

	    public static GameUIEffect MakeUIEffect(GameUIComponent maker, string path)
	    {
		    return Make<GameUIEffect>(maker, path);
	    }
	    
	    public static GameProgressBar MakeProgressBar(GameUIComponent maker, string path)
	    {
		    return Make<GameProgressBar>(maker, path);
	    }
	    
	    public static GameScrollView MakeScrollView(GameUIComponent maker, string path)
	    {
		    return Make<GameScrollView>(maker, path);
	    }
	    
	    public static GameUIModel3D MakeUIModel3D(GameUIComponent maker, string path)
	    {
		    return Make<GameUIModel3D>(maker, path);
	    }
	    
	    public static GameUIModel2D MakeUIModel2D(GameUIComponent maker, string path)
	    {
	     return Make<GameUIModel2D>(maker, path);
	    }
	    
	    public static GameUIScene2D MakeUIScene2D(GameUIComponent maker, string path)
	    {
	     return Make<GameUIScene2D>(maker, path);
	    }
	    
	    
	    //
	    // public static GameActorAvatar MakeActorAvatar(GameUIComponent maker, string path)
	    // {
		   //  return Make<GameActorAvatar>(maker, path);
	    // }
	    //


	    

	    //
	    // public static GameIndicateButton MakeIndicateButton(GameUIComponent maker, string path)
	    // {
		   //  return Make<GameIndicateButton>(maker, path);
	    // }
	    //
	    // public static GameIndicateCheckBox MakeIndicateCheckBox(GameUIComponent maker, string path)
	    // {
		   //  return Make<GameIndicateCheckBox>(maker, path);
	    // }
	    //
	    
	    public static GameTexture MakeTexture(GameUIComponent maker, string path)
	    {
		    return Make<GameTexture>(maker, path);
	    }
  
		// public static GameDropDown MakeDropDown(GameUIComponent maker, string path)
		// {
		// 	return Make<GameDropDown>(maker, path);
		// }
  //
  //       public static GameLabelWithEffects MakeLabelWithEffects(GameUIComponent maker, string path)
		// {
		// 	return Make<GameLabelWithEffects>(maker, path);
		// }
  //
  //       public static GamePhotoEdit MakePhotoEdit( GameUIComponent maker, string path )
  //       {
  //           return Make<GamePhotoEdit>( maker, path );
  //       }
  //
  //       public static GameSweepSkillGroup MakeSweepSkillGroup(GameUIComponent maker, string path)
  //       {
  //           return Make<GameSweepSkillGroup>(maker, path);
  //       }
  //
  //       public static GameUITweener MakeGameUITweener(GameUIComponent maker, string path)
  //       {
  //           return Make<GameUITweener>(maker, path);
  //       }
  //
  //       public static GameAnimatorLabel MakeAnimatorLabel(GameUIComponent maker, string path)
		// {
		// 	return Make<GameAnimatorLabel>(maker, path);
		// }
  //
  //       public static GameWWWImage MakeGameWWWImage(GameUIComponent maker, string path)
  //       {
  //           return Make<GameWWWImage>(maker,path);
  //       }
  //
  //       public static GameVideoUI MakeGameVideoUI(GameUIComponent maker, string path)
  //       {
  //           return Make<GameVideoUI>(maker, path);
  //       }
  //       public static GameMixImage MakeGameMixImage(GameUIComponent maker, string path)
  //       {
  //           return Make<GameMixImage>(maker, path);
  //       }
  //       public static GamePartComponent MakePartComponent(GameUIComponent view, string path)
  //       {
  //           GamePartComponent c = new GamePartComponent();
  //           c.InitComponent(view, path);
  //           view.AddChildComponent(c, view);
  //           return c;
  //       }
  //
  //       public static GameUICircle MakeGameUICircle(GameUIComponent maker, string path)
  //       {
  //           return Make<GameUICircle>(maker, path);
  //       }
    }
}
