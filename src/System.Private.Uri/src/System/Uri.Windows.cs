namespace System
{
	public partial class Uri
	{
#if MONO
        private const bool Windows_IsWindowsSystem = true;
#else
		private const bool IsWindowsSystem = true;
#endif
    }
}