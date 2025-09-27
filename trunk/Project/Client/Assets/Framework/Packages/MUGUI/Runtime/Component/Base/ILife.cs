namespace Game.UI
{
    public interface ILife
    {
        void OnInit();
        void OnShow(params object[] _data);
        void OnHide();
        void OnDispose();
    }
}