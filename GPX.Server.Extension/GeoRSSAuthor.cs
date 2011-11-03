#region File Information
//
// File: "GeoRSSAuthor.cs"
// Purpose: "Extends SyndicationPerson to apply JSON formatting"
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
    public class GeoRSSAuthor : SyndicationPerson
    {
        [JsonIgnore]
        public new Dictionary<System.Xml.XmlQualifiedName, string> AttributeExtensions
        {
            get { return base.AttributeExtensions; }
        }

        [JsonIgnore]
        public new SyndicationElementExtensionCollection ElementExtensions
        {
            get { return base.ElementExtensions; }
        }
    }

}
