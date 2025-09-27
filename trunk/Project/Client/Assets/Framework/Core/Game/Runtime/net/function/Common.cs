using System;
using Game;
using Game.Core;
using UnityEngine;

namespace MUGame.Functions
{
    static class Common
    {
        public static SafeFunc<Vector3> GetStickDirection;
        public static SafeAction<long, string> StartPick;
        public static SafeAction<bool> EndCollect;
        public static SafeAction<float, string, Action> StartArea;

        public static SafeAction<float> OnStickMove;
        public static SafeAction<float> OnStickMoveEnd;
    }
}
