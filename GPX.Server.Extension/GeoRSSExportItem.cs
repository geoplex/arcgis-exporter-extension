#region File Information
//
// File: "GeoRSSExportItem.cs"
// Purpose: "Implements IExport Item  - Represents the configuration item controlling how data is mapped to a standard RSS feed item"
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
using System.ServiceModel.Syndication;
using Newtonsoft.Json;

namespace GPX.Server.Extension
{
    public class GeoRSSExportItem : IExportItem
    {
        
        private string _fixedContent;
        private string _mappedContent;
        private string _preCondition;
        private string _postCondition;
        private string _mappedContentDelimeter;
        private bool _autoGenerate;
        private string _mappedContentAlias;
        
        public GeoRSSExportItem()
        {

        }

        public string PostCondition
        {
            get { return _postCondition; }
            set { _postCondition = value; }
        }

        public string PreConditon
        {
            get { return _preCondition; }
            set { _preCondition = value; }
        }

        public string MappedContent
        {
            get { return _mappedContent; }
            set { _mappedContent = value; }
        }

        public string FixedContent
        {
            get { return _fixedContent; }
            set { _fixedContent = value; }
        }

        public string MappedContentDelimeter
        {
            get { return _mappedContentDelimeter; }
            set { _mappedContentDelimeter = value; }
        }

        public bool AutoGenerate
        {
            get { return _autoGenerate; }
            set { _autoGenerate = value; }
        }

        public string MappedContentAlias
        {
            get { return _mappedContentAlias; }
            set { _mappedContentAlias = value; }
        }

    }
}
