﻿namespace Essentials
{
    using System;
    using System.Reflection;
    using System.Security.Permissions;

    using EnsoulSharp.SDK;

    using Essentials.Properties;
    // 1. Add your DLL on Resources
    // 2. Resource DLL build Action: Embedded Resource
    // 2. install script on EnsoulSharp.Loader
    // 3. test it work fine <3
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
