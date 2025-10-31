using System.Security.Principal;

namespace SystemMonitoring
{
    public static class AdminHelper
    {
        public static bool IsRunningAsAdmin()
        {
            using (WindowsIdentity id = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal p = new WindowsPrincipal(id);
                return p.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}
