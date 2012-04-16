#region File Information
//
// File: "GeoRSSExportProperties.cs"
// Purpose: "Implements IExportProperties  - Represents the definition of an GeoRSS export."
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


using System.Collections.Generic;
using GPX.Server.Extension.Spatial;
using Newtonsoft.Json;


namespace GPX.Server.Extension
{
    public class GeoRSSExportProperties : IExportProperties
    {

        private string _title;
        private string _desription;
        private string _generator;
        private string _id;
        private string _copyRight;
        private string _language;
        private GeoRSSAuthor _author;
        private GeoRSSLink _link;
        private string _feedFormat;
        private string _geometryFormat;
        private List<KeyValuePair<string, GeoRSSExportItem>> _itens;
        private string _geometryField;
        private ExportOutputSpatialReference _outputSr;


        #region Properties

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        public string CopyRight
        {
            get { return _copyRight; }
            set { _copyRight = value; }
        }

        public string Generator
        {
            get { return _generator; }
            set { _generator = value; }
        }

        public string Description
        {
            get { return _desription; }
            set { _desription = value; }
        }

        public GeoRSSLink Link
        {
            get { return _link; }
            set { _link = value; }
        }

        public GeoRSSAuthor Author
        {
            get { return _author; }
            set { _author = value; }
        }

        public string GeometryFormat
        {
            get { return _geometryFormat; }
            set { _geometryFormat = value; }
        }

        public string FeedFormat
        {
            get { return _feedFormat; }
            set { _feedFormat = value; }
        }

        public List<KeyValuePair<string, GeoRSSExportItem>> Items
        {
            get { return _itens; }
            set { _itens = value; }
        }

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

        #region Constructors

        public GeoRSSExportProperties()
        {
            //this.Items = new List<KeyValuePair<string, GeoRSSExportItem>>();
            //Items.Add(new KeyValuePair<string, GeoRSSExportItem>("Author", new GeoRSSExportItem()));
            //Items.Add(new KeyValuePair<string, GeoRSSExportItem>("Content", new GeoRSSExportItem()));
            //Items.Add(new KeyValuePair<string, GeoRSSExportItem>("Contributors", new GeoRSSExportItem()));
            //Items.Add(new KeyValuePair<string, GeoRSSExportItem>("Copyright", new GeoRSSExportItem()));
            //Items.Add(new KeyValuePair<string, GeoRSSExportItem>("Id", new GeoRSSExportItem()));
            //Items.Add(new KeyValuePair<string, GeoRSSExportItem>("LastUpdatedTime", new GeoRSSExportItem()));
            //Items.Add(new KeyValuePair<string, GeoRSSExportItem>("Links", new GeoRSSExportItem()));
            //Items.Add(new KeyValuePair<string, GeoRSSExportItem>("PublishDate", new GeoRSSExportItem()));
            //Items.Add(new KeyValuePair<string, GeoRSSExportItem>("Summary", new GeoRSSExportItem()));
            //Items.Add(new KeyValuePair<string, GeoRSSExportItem>("Title", new GeoRSSExportItem()));

        }

        public GeoRSSExportProperties(string propertiesAsJson)
        {

            GeoRSSExportProperties properties = JsonConvert.DeserializeObject<GeoRSSExportProperties>(propertiesAsJson);
            this.Author = properties.Author;
            this.Link = properties.Link;
            this.Title = properties.Title;
            this.Description = properties.Description;
            this.Generator = properties.Generator;
            this.ID = properties.ID;
            this.CopyRight = properties.CopyRight;
            this.Language = properties.Language;
            this.GeometryFormat = properties.GeometryFormat;
            this.FeedFormat = properties.FeedFormat;
            this.Items = properties.Items;
            this.GeometryField = properties.GeometryField;
            this.OutputSpatialReference = properties.OutputSpatialReference;
        }

        #endregion

        public GeoRSSGeometryFormat GetGeoRSSGeomtryType()
        {
            GeoRSSGeometryFormat geomType = GeoRSSGeometryFormat.Simple;

            if (!string.IsNullOrEmpty(this.GeometryFormat))
            {
                if (this.GeometryFormat == "GML")
                    geomType = GeoRSSGeometryFormat.GML;
                if (this.GeometryFormat == "Simple")
                    geomType = GeoRSSGeometryFormat.Simple;
            }

            return geomType;

        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
