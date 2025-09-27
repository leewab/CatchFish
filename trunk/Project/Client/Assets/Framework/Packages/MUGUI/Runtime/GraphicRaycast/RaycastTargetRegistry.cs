using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastTargetRegistry
{
    private static RaycastTargetRegistry s_Instance;

    private readonly Dictionary<Canvas, CustomIndexedSet<Graphic>> m_Graphics =
        new Dictionary<Canvas, CustomIndexedSet<Graphic>>();

    protected RaycastTargetRegistry()
    {
        // This is needed for AOT on IOS. Without it the compile doesn't get the definition of the Dictionarys
#pragma warning disable 168
        Dictionary<Graphic, int> emptyGraphicDic;
        Dictionary<ICanvasElement, int> emptyElementDic;
#pragma warning restore 168
    }

    public static RaycastTargetRegistry instance
    {
        get
        {
            if (s_Instance == null)
                s_Instance = new RaycastTargetRegistry();
            return s_Instance;
        }
    }

    public static void RegisterGraphicForCanvas(Canvas c, Graphic graphic)
    {
        if (c == null)
        {
            c = graphic.gameObject.GetComponentInParent<Canvas>();
        }

        if (c == null)
        {
            return;
        }

        CustomIndexedSet<Graphic> graphics;
        instance.m_Graphics.TryGetValue(c, out graphics);

        if (graphics != null)
        {
            graphics.AddUnique(graphic);
            return;
        }

        // Dont need to AddUnique as we know its the only item in the list
        graphics = new CustomIndexedSet<Graphic>();
        graphics.Add(graphic);
        instance.m_Graphics.Add(c, graphics);
    }

    public static void UnregisterGraphicForCanvas(Canvas c, Graphic graphic)
    {
        if (c == null)
        {
            c = graphic.gameObject.GetComponentInParent<Canvas>();
        }

        if (c == null)
        {
            return;
        }

        CustomIndexedSet<Graphic> graphics;
        if (instance.m_Graphics.TryGetValue(c, out graphics))
        {
            graphics.Remove(graphic);

            if (graphics.Count == 0)
                instance.m_Graphics.Remove(c);
        }
    }

    private static readonly List<Graphic> s_EmptyList = new List<Graphic>();

    public static IList<Graphic> GetGraphicsForCanvas(Canvas canvas)
    {
        CustomIndexedSet<Graphic> graphics;
        if (instance.m_Graphics.TryGetValue(canvas, out graphics))
            return graphics;

        return s_EmptyList;
    }
}