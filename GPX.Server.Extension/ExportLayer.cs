﻿#region File Information
//
// File: "ExporterLayer.cs"
// Purpose: "Describes a layer which is capable of being exported"
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

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SOESupport;

namespace GPX.Server.Extension
{
    public class ExportLayerInfo
    {
        public string Name { get; set; }
        public int ID { get; set; }

        public ExportLayerInfo(IMapLayerInfo mapLayerInfo)
        {
            this.Name = mapLayerInfo.Name;
            this.ID = mapLayerInfo.ID;
        }

        public JsonObject ToJsonObject()
        {

            JsonObject jo = new JsonObject();
            jo.AddString("name", Name);
            jo.AddLong("id", ID);

            return jo;
        }
    }
}
