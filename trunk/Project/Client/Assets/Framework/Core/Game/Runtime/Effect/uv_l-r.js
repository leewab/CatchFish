var scrollSpeed:float ;
//var uvtexture:Texture;//放2d圖
function Update () 
{
    var offset = Time.time * scrollSpeed;
    if (GetComponent.<Renderer>() == null) return;
    if (GetComponent.<Renderer>().sharedMaterial == null) return;
    if (!GetComponent.<Renderer>().sharedMaterial.HasProperty("_MainTex")) return;
    GetComponent.<Renderer>().sharedMaterial.SetTextureOffset ("_MainTex", Vector2(-offset,0));
}