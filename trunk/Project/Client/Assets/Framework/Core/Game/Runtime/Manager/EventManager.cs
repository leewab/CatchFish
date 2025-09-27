using System.Collections.Generic;

namespace Game
{
    public static class EventManager
    {
        public delegate void EventFunction(params object[] args);

        private static Dictionary<int, EventFunction> mEventDictionary = new Dictionary<int, EventFunction>();

        public static void AddEvent(int eventId, EventFunction mEventFunc)
        {
            if (null == mEventDictionary) mEventDictionary = new Dictionary<int, EventFunction>();
            if (!mEventDictionary.ContainsKey(eventId))
            {
                mEventDictionary[eventId] = mEventFunc;
            }
        }

        public static void RemoveEvent(int eventId)
        {
            if (null == mEventDictionary) return;
            if (mEventDictionary.ContainsKey(eventId))
            {
                mEventDictionary[eventId] = null;
            }
        }

        public static void Broadcast(int eventId, params object[] args)
        {
            if (null == mEventDictionary) return;
            if (mEventDictionary.ContainsKey(eventId))
            {
                mEventDictionary[eventId]?.Invoke(args);
            }
        }
    }
}