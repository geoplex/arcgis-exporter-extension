#region File Information
//
// File: "ExportOutputSpatialReference"
// Purpose: "Describes the output spatial reference of geographical data within an export"
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


namespace GPX.Server.Extension.Spatial
{
    /// <summary>
    /// 
    /// </summary>
    public class ExportOutputSpatialReference
    {
        #region Member Variables
        #endregion

        #region Constructors

        public ExportOutputSpatialReference()
        {

        }

        #endregion

        #region Properties

        private int _wkid;
        private string _coordinateSystemType;
        private long? _transformationId;

        public long? TransformationId
        {
            get { return _transformationId; }
            set { _transformationId = value; }
        }


        public string CoordinateSystemType
        {
            get { return _coordinateSystemType; }
            set { _coordinateSystemType = value; }
        }


        public int Wkid
        {
            get { return _wkid; }
            set { _wkid = value; }
        }


        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion

        #region Enums
        #endregion
    }
}
