#region File Information
//
// File: ""
// Purpose: ""
// Author: "Geoplex"
// 
#endregion

#region (c) Copyright "2011" Geoplex
//
// This is UNPUBLISHED PROPRIETARY SOURCE CODE of GeoPlex.
// The contents of this file may not be disclosed to third parties, copied or
// duplicated in any form, in whole or in part, without the prior written
// permission of GeoPlex.
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
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;


namespace GPX.Server.Extension.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public class MockServerObjectCreator
    {
        #region Member Variables
        #endregion

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Public Methods

        public static MockServerObjectHelper CreateMockServerObjectHelper(string mxdFile)
        {
            MockServerObjectHelper msoh = new MockServerObjectHelper();
            msoh.ServerObject = CreateMockMapServerObject(mxdFile);
            return msoh;

        }

        public static IServerObject CreateMockMapServerObject(string mxdFile)
        {

            IMapServer mapServer = new MapServer();
            IMapServerInit mapinit = mapServer as IMapServerInit;
            mapinit.PhysicalOutputDirectory = @"C:\Data";
            //mapinit.VirtualOutputDirectory = @"C:\Data";
            mapinit.Connect(mxdFile);
            string foo = mapServer.MapName[0];
            IServerObject so = mapServer as IServerObject;
            return so;

        }

        #endregion

        #region Private Methods
        #endregion

        #region Enums
        #endregion
    }
}
