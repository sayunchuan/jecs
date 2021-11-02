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

        internal void OnLoad(JWorld w, int uid)
        {
            UID = uid;
            _BasicOnLoad(w);
        }

        protected abstract void _BasicOnLoad(JWorld w);

        internal void OnRelease(JWorld w)
        {
            UID = -1;
            _BasicOnRelease(w);
        }

        protected abstract void _BasicOnRelease(JWorld w);
    }
}