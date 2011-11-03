#region File Information
//
// File: "Installer.cs"
// Purpose: "Static class used to register and unregister extension"
// Author: "Geoplex"
// 
#endregion

#region (c) Copyright 2011 Geoplex
//
// THE SOFTWARE IS PROVIDED "AS-IS" AND WITHOUT WARRANTY OF ANY KIND,
// EXPRESS, IMPLIED OR OTHERWISE, INCLUDING WITHOUT LIMITATION, ANY
// WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE.
//
// IN NO EVENT SHALL GEOPLEX BE LIABLE FOR ANY SPECIAL, INCIDENTAL,
// INDIRECT OR CONSEQUENTIAL DAMAGES OF ANY KIND, OR ANY DAMAGES WHATSOEVER
// RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER OR NOT ADVISED OF THE
// POSSIBILITY OF DAMAGE, AND ON ANY THEORY OF LIABILITY, ARISING OUT OF OR IN
// CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.Connection.AGS;
using ESRI.ArcGIS.Server;
using System.ServiceProcess;

namespace GPX.Server.Extension
{
    class Installer
    {

        static string classid;
        static string desc;
        static string name;
        static string displayName;
        static string supportsRest;
        static string supportsMSD;
        static string somProcessName;
        static string regFilePath;
        static string unregFilePath;
        static string serverName;

        static void Main(string[] args)
        {
            try
            {

                //read the configuration and present to user
                Console.WriteLine("The following settings are being used:");
                Console.WriteLine(string.Empty);

                foreach (string key in ConfigurationManager.AppSettings)
                {
                    switch (key)
                    {
                        case "SoeClassId":
                            classid = ConfigurationManager.AppSettings[key];
                            break;
                        case "SoeDescription":
                            desc = ConfigurationManager.AppSettings[key];
                            break;
                        case "SoeDisplayName":
                            displayName = ConfigurationManager.AppSettings[key];
                            break;
                        case "SoeName":
                            name = ConfigurationManager.AppSettings[key];
                            break;
                        case "SomProcessName":
                            somProcessName = ConfigurationManager.AppSettings[key];
                            break;
                        case "SupportsMSD":
                            supportsMSD = ConfigurationManager.AppSettings[key];
                            break;
                        case "SupportsREST":
                            supportsRest = ConfigurationManager.AppSettings[key];
                            break;
                        default:
                            break;
                    }

                    Console.WriteLine(key + " : " + ConfigurationManager.AppSettings[key]);
                }



                string usage = "Usage: REGISTER | UNREGISTER | LISTEXTENSIONS";


                //ask user to enter command
                Console.WriteLine(string.Empty);
                Console.WriteLine(usage);

                string command = Console.ReadLine();


                Console.WriteLine("Enter the server name:");
                serverName = Console.ReadLine();


                switch (command.ToUpper())
                {
                    case "REGISTER": Register(); break;
                    case "UNREGISTER": UnRegister(); break;
                    case "LISTEXTENSIONS": ListConfigs(); break;
                    default: WriteLine(usage); break;
                }

            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    WriteLine(ex.Message);
                    WriteLine(ex.StackTrace);
                    ex = ex.InnerException;
                }
            }

            Console.WriteLine("Do you want o restart ArcGIS Server? (y/n)");
            string restart = Console.ReadLine();

            if (restart == "y")
                RestartService(somProcessName, 15000);
        }


        public static void RestartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);

            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                // ...
            }
        }

        private static IServerObjectAdmin2 Connect()
        {
            // Must run as an user in the agsadmin group on the SOM
            var agsConn = new AGSServerConnection();
            agsConn.Host = serverName;
            agsConn.Connect();
            return (IServerObjectAdmin2)agsConn.ServerObjectAdmin;
        }

        private static void ListConfigs()
        {
            var soa = Connect();
            // Get the extensions that extend MapServer server objects
            var list = soa.GetConfigurations();
            list.Reset();

            var item = list.Next();
            while (item != null)
            {
                WriteLine("{0}, {1}, {2}", item.Name, item.TypeName, item.Description);
                item = list.Next();
            }
        }


        private static void Register()
        {
           

            Console.WriteLine("Registering.........");

            Regasm();
            RegisterAGS();
        }

        private static void UnRegister()
        {

            Console.WriteLine("Unregistering.........");

            UnRegisterAGS();
            UnRegasm();
        }

        private static void UnRegisterAGS()
        {
            var soa = Connect();

            IServerObjectExtensionType3 soet = soa.CreateExtensionType() as IServerObjectExtensionType3;

            soet.CLSID = classid;
            soet.Description = desc;
            soet.Name = name;
            soet.DisplayName = displayName;

            if (!ExtensionRegistered(soa, soet.Name))
            {
                Console.WriteLine("During unregistration the extension " + soet.Name + " was not found");

            }
            else
            {

                soa.DeleteExtensionType("MapServer", soet.Name); //todo - externalise the server type
                Console.WriteLine("Unregistering " + soet.Name + " with ArcGIS Server complete");

            }

        }

        private static void RegisterAGS()
        {
            var soa = Connect();
            IServerObjectExtensionType3 soet = soa.CreateExtensionType() as IServerObjectExtensionType3;


            soet.CLSID = classid;
            soet.Description = desc;
            soet.Name = name;
            soet.DisplayName = displayName;

            soet.Info.SetProperty("SupportsREST", supportsRest);
            soet.Info.SetProperty("SupportsMSD", supportsMSD);

            if (ExtensionRegistered(soa, soet.Name))
            {
                Console.WriteLine("During registration the extension " + soet.Name + " was already found");
                Console.WriteLine("If you wish to register the extension with different settings, unregister the existing extension first");
            }
            else
            {
                soa.AddExtensionType("MapServer", soet);
                Console.WriteLine("Registering " + soet.Name + " with ArcGIS Server complete");
            }



        }

        static private bool ExtensionRegistered(ESRI.ArcGIS.Server.IServerObjectAdmin2 serverObjectAdmin, string extensionName)
        {
            // Get the extensions that extend MapServer server objects
            ESRI.ArcGIS.Server.IEnumServerObjectExtensionType extensionTypes = serverObjectAdmin.GetExtensionTypes("MapServer");
            extensionTypes.Reset();

            // If an extension with a name matching that passed-in is found, return true
            ESRI.ArcGIS.Server.IServerObjectExtensionType extensionType = extensionTypes.Next();
            while (extensionType != null)
            {
                if (extensionType.Name == extensionName)
                    return true;

                extensionType = extensionTypes.Next();
            }

            // No matching extension was found, so return false
            return false;
        }

        private static void Regasm()
        {
            var t = Type.GetType(classid);
            if (t == null)
                throw new Exception("cannot get type for " + classid);

            var regSvcs = new RegistrationServices();

            regSvcs.RegisterAssembly(t.Assembly, AssemblyRegistrationFlags.SetCodeBase);
            WriteLine("{0} registered with COM", classid);
        }

        private static void UnRegasm()
        {
            var t = Type.GetType(classid);
            if (t == null)
                throw new Exception("cannot get type for " + classid);

            var regSvcs = new RegistrationServices();
            regSvcs.UnregisterAssembly(t.Assembly);
            WriteLine("{0} unregistered from COM", classid);
        }

        private static void WriteLine(string s)
        {
            Debug.WriteLine(s);
            Console.WriteLine(s);
        }
        private static void WriteLine(string format, params object[] args)
        {
            Debug.WriteLine(String.Format(format, args));
            Console.WriteLine(format, args);
        }
    }
}
