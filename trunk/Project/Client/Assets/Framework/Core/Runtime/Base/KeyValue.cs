namespace Game.Core
{
    public class KeyValue
    {
        public string Key;
        public object Value;

        public KeyValue(string key, object value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}