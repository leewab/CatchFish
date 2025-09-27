namespace Game.UI
{
    public interface IEditorView
    {
        void OnGUI();
        void OnGUI(BaseEditorModel model);
        void OnGUI(BaseEditorModel model, params System.Action[] actions);
        void Update();
        void Clear();
    }
}