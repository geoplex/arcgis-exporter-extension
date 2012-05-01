#region File Information
//
// File: "GeoJsonExportProperties.cs"
// Purpose: "Implements IExportProperties  - Represents the definition of an GeoJson export."
// Author: "Geoplex"
// 
#endregion

#region (c) Copyright 2012 Geoplex
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


using GPX.Server.Extension.Spatial;
using Newtonsoft.Json;

namespace GPX.Server.Extension
{
    /// <summary>
    /// 
    /// </summary>
    public class GeoJsonExportProperties : IExportProperties
    {
        #region Member Variables

        private string _geometryField;
        private ExportOutputSpatialReference _outputSr;

        #endregion

        #region Constructors

        public GeoJsonExportProperties()
        {

        }

        public GeoJsonExportProperties(string propertiesAsJson)
        {

            GeoJsonExportProperties properties = JsonConvert.DeserializeObject<GeoJsonExportProperties>(propertiesAsJson);
            this.GeometryField = properties.GeometryField;
            this.OutputSpatialReference = properties.OutputSpatialReference;
        }

        #endregion

        #region Properties


        public string GeometryField
        {
            get { return _geometryField; }
            set { _geometryField = value; }
        }

        public ExportOutputSpatialReference OutputSpatialReference
        {
            get { return _outputSr; }
            set { _outputSr = value; }
        }
        #endregion

        #region Public Methods


        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        #endregion

        #region Private Methods
        #endregion

        #region Enums
        #endregion
    }
}
