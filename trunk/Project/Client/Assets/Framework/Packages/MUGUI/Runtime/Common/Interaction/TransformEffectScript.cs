using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MUGUI
{
    [AddComponentMenu("MUGUI/Interaction/Transform Effect")]
    public class TransformEffectScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public GameObject scaleTarget;
        public Vector3 initScale = Vector3.one;
        public Vector3 scaleTo = Vector3.one;

        public Graphic colorTarget;
        public Color initColor = Color.white;
        public Color colorTo = Color.white;

        public void OnPointerDown(PointerEventData eventData)
        {
            if(scaleTarget != null)
            {
                scaleTarget.transform.localScale = scaleTo;
            }
            if(colorTarget != null)
            {
                colorTarget.color = colorTo;
            }
            
        }
       
        public void OnPointerUp(PointerEventData eventData)
        {
            if (scaleTarget != null)
            {
                scaleTarget.transform.localScale = initScale;
            }
            if (colorTarget != null)
            {
                colorTarget.color = initColor;
            }
        }
       
    }
}