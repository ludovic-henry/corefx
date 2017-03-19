namespace System
{
	public partial class Uri
	{
		private static readonly bool IsWindowsSystem = PlatformHelper.IsWindows ? Windows_IsWindowsSystem : Unix_IsWindowsSystem;
	}
}