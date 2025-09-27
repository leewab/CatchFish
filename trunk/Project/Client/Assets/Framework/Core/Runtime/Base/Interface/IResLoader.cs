using System;
using Game.Core;
using UnityEngine;

public interface IResLoader
{
    public string Name { get; }
    public RES_LOAD_STATE State { get; }
    public void UnLoad();
    public bool Load(string name, Action<string, GameObject> onLoadCallBack);
}