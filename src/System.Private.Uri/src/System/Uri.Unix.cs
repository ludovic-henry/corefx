namespace System
{
	public partial class Uri
	{
#if MONO
        private const bool Unix_IsWindowsSystem = false;
#else
		private const bool IsWindowsSystem = false;
#endif
    }
}