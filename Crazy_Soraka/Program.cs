namespace LoadEncrypt
{
    using System;
    using System.Reflection;
    using System.Security.Permissions;

    using EnsoulSharp.SDK;

    using LoadEncrypt.Properties;

    internal class Program
    {
        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void OnGameLoad()
        {
            try
            {
                var a = Assembly.Load(Resources.Script);
                var myType = a.GetType("Script.Example");// namespace + class name
                var methon = myType.GetMethod("Init", BindingFlags.Public | BindingFlags.Static); // methon

                if (methon != null)
                {
                    methon.Invoke(null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
