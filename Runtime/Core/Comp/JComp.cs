namespace JECS.Core
{
    public abstract class JComp
    {
#if UNITY_EDITOR
        public int UID { get; private set; }
#else
        public int UID;
#endif

        public abstract int CompId { get; }

        internal void InternalOnLoad(JWorld w, int uid)
        {
            UID = uid;
            OnLoad(w);
        }

        protected abstract void OnLoad(JWorld w);

        internal void InternalOnClear(JWorld w)
        {
            UID = -1;
            OnClear(w);
        }

        protected abstract void OnClear(JWorld w);
    }
}