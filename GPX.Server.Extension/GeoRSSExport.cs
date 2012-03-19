#region File Information
//
// File: "GeoRSSExport.cs"
// Purpose: "Extends SyndicationFeed and implements IExport - reprsents a GeoRSS export"
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
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GPX.Server.Extension
{
    public class GeoRSSExport : SyndicationFeed, IExport
    {
        private const string XML_NS = "http://www.w3.org/2000/xmlns/";
        private const string GEORSS_NS = "http://www.georss.org/georss";
        private const string GEORSS_NS_PREFIX = "georss";
        private const string GML_NS = "http://www.opengis.net/gml";
        private const string GML_NS_PREFIX = "gml";


        /// <summary>
        /// Populates this feed instance with the contents of the record set, based upon the passed in property set
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="exportProperties">The export properties.</param>
        public void CreateExport(RecordSet results, IExportProperties exportProperties)
        {

            GeoRSSExportProperties feedProperties = (GeoRSSExportProperties)exportProperties;



            //add the georss namespace
            this.AttributeExtensions.Add(new XmlQualifiedName(GEORSS_NS_PREFIX, XML_NS), GEORSS_NS);

            //if the geometry type is gml then also add the GML namespace
            if (feedProperties.GeometryFormat == "GML")
                this.AttributeExtensions.Add(new XmlQualifiedName(GML_NS_PREFIX, XML_NS), GML_NS);

            //set up feed level properties
            this.Authors.Add(feedProperties.Author);
            this.Copyright = new TextSyndicationContent(feedProperties.CopyRight);
            this.Description = new TextSyndicationContent(feedProperties.Description);
            this.Generator = feedProperties.Generator;
            this.Id = Guid.NewGuid().ToString();
            this.Language = feedProperties.Language;
            this.Links.Add(feedProperties.Link);
            this.Title = new TextSyndicationContent(feedProperties.Title);

            //set up the item collection for the feed
            List<SyndicationItem> items = CreateItemCollection(results, feedProperties);
            this.Items = items;


        }

        private List<SyndicationItem> CreateItemCollection(RecordSet results, GeoRSSExportProperties feedProperties)
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            ICursor cursor;
            SimpleGeometryFactory simpleFactory = new SimpleGeometryFactory();
            GMLGeometryFactory gmlFactory = new GMLGeometryFactory();

            try
            {
                //create a feed item for each record
                cursor = results.get_Cursor(false);

                Exporter.logger.LogMessage(ESRI.ArcGIS.SOESupport.ServerLogger.msgType.infoStandard, "CreateItemCollection", 999999, "Cursor Set");
                IRow row = cursor.NextRow();

                int rowCount = 0;

                if (row != null)
                {
                    while (row != null)
                    {
                        //on the first row log out the field name and field alias
                        if (rowCount == 0)
                        {
                            for (int r = 0; r < row.Fields.FieldCount; r++)
                            {
                                IField field = row.Fields.get_Field(r);

                                Exporter.logger.LogMessage(ESRI.ArcGIS.SOESupport.ServerLogger.msgType.infoDetailed,
                                "CreateItemCollection",
                                999999,
                                "field Name: " + field.Name +
                                " field alias: " + field.AliasName
                                );
                            }
                        }


                        SyndicationItem item = new SyndicationItem();

                        //set the item properties
                        foreach (KeyValuePair<string, GeoRSSExportItem> itemConfig in feedProperties.Items)
                        {
                            GeoRSSExportItem itemExportProperty = itemConfig.Value;

                            if (itemExportProperty != null)
                            {
                                SetItemValues(row, ref item, itemConfig, itemExportProperty);

                            }

                        }

                        //process the geometry

                        int fieldIndex = row.Fields.FindField(feedProperties.GeometryField);

                        if (fieldIndex == -1)
                            throw new Exception("Could not locate geometry field:" + feedProperties.GeometryField);

                        if (row.get_Value(fieldIndex) != null)
                        {
                            IGeometry geom = row.get_Value(fieldIndex) as IGeometry;
                            GeoRSSGeometry geometry = null;
                            switch (feedProperties.GetGeoRSSGeomtryType())
                            {
                                case GeoRSSGeometryFormat.Simple:
                                    geometry = simpleFactory.GetGeometry(geom);
                                    break;
                                case GeoRSSGeometryFormat.GML:
                                    geometry = gmlFactory.GetGeometry(geom);
                                    break;
                                default:
                                    break;
                            }

                            if (geom != null)
                                item.ElementExtensions.Add(geometry.GeometryAsXML);
                        }


                        items.Add(item);

                        row = cursor.NextRow();

                        rowCount++;

                    }
                }
                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cursor = null;
            }
        }

        private static void SetItemValues(IRow row, ref SyndicationItem item, KeyValuePair<string, GeoRSSExportItem> itemConfig, GeoRSSExportItem itemExportProperty)
        {


            try
            {
                //Title
                if (itemConfig.Key == "Title")
                    item.Title = new TextSyndicationContent(GetItemContent(row, itemExportProperty));

                //Content
                if (itemConfig.Key == "Content")
                    item.Content = new TextSyndicationContent(GetItemContent(row, itemExportProperty));

                //Author
                //todo  - we only currently support author as a single string, assume to be name - better to support all author properties i.e.
                //authorName
                //authorEmail
                //authorUri
                if (itemConfig.Key == "Author")
                {

                    SyndicationPerson author = new SyndicationPerson();
                    author.Name = GetItemContent(row, itemExportProperty);
                    item.Authors.Add(author);
                }

                //Contributors
                //todo  - we only currently support author as a single string, assume to be name - better to support all author properties i.e.
                //controbutorName
                //controbutorEmail
                //controbutorNameUri
                if (itemConfig.Key == "Contributors")
                {
                    SyndicationPerson author = new SyndicationPerson();
                    author.Name = GetItemContent(row, itemExportProperty);
                    item.Contributors.Add(author);

                }

                //Copyright
                if (itemConfig.Key == "Copyright")
                    item.Copyright = new TextSyndicationContent(GetItemContent(row, itemExportProperty));

                //Id 
                if (itemConfig.Key == "Id")
                {

                    if (itemExportProperty.AutoGenerate)
                        item.Id = Guid.NewGuid().ToString();
                    else
                        item.Id = GetItemContent(row, itemExportProperty);

                }

                //PublishDate
                if (itemConfig.Key == "PublishDate")
                {

                    if (itemExportProperty.AutoGenerate)
                        item.PublishDate = DateTime.Now;
                    else
                        item.PublishDate = Convert.ToDateTime(GetItemContent(row, itemExportProperty));

                }


                //LastUpdatedTime
                if (itemConfig.Key == "LastUpdatedTime")
                {

                    if (itemExportProperty.AutoGenerate)
                        item.LastUpdatedTime = DateTime.Now;
                    else
                        item.LastUpdatedTime = Convert.ToDateTime(GetItemContent(row, itemExportProperty));

                }



                //Links
                //todo  - we only currently support link as a single string, assume to be uri - better to support all link properties
                if (itemConfig.Key == "Links")
                {
                    SyndicationLink link = new SyndicationLink();
                    link.Uri = new Uri(GetItemContent(row, itemExportProperty));
                    item.Links.Add(link);
                }

                //Summary
                if (itemConfig.Key == "Summary")
                    //item.Summary = new TextSyndicationContent(GetItemContent(row, itemExportProperty));
                    item.Summary = SyndicationContent.CreateHtmlContent(GetItemContent(row, itemExportProperty));
            }
            catch (Exception ex)
            {
                Exporter.logger.LogMessage(ESRI.ArcGIS.SOESupport.ServerLogger.msgType.error,
                    "SetItenValues",
                    999999,
                    "Item Config Key:" + itemConfig.Key +
                    "Error Message: " + ex.Message.ToString());
            }
            finally
            {


            }



        }

        /// <summary>
        /// Gets the content of the item.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="itemExportProperty">The item export property.</param>
        /// <returns></returns>
        private static string GetItemContent(IRow row, GeoRSSExportItem itemExportProperty)
        {
            string itemContent = null;


            try
            {
                if (!string.IsNullOrEmpty(itemExportProperty.FixedContent))
                {
                    itemContent = itemExportProperty.FixedContent;
                }

                if (!string.IsNullOrEmpty(itemExportProperty.MappedContent))
                {


                    StringBuilder content = new StringBuilder();
                    bool multiContent = false;

                    //check for pre and post conditions
                    if (!string.IsNullOrEmpty(itemExportProperty.PreConditon))
                        content.Append(itemExportProperty.PreConditon);

                    //mapped content supports multiple fields - if there is more than one field supplied, display as html, using provided delimeter
                    List<string> inputFields = itemExportProperty.MappedContent.Split(',').ToList();



                    //check if multiple fields
                    if (inputFields.Count > 1) multiContent = true;

                    //vaidate
                    List<string> inputFieldAlias = null;
                    if (multiContent)
                    {
                        inputFieldAlias = itemExportProperty.MappedContentAlias.Split(',').ToList();

                        if (inputFields.Count != inputFieldAlias.Count)
                            throw new Exception("Mapped field count does not equal alias field count");

                    }



                    for (int i = 0; i < inputFields.Count; i++)
                    {

                        int fieldIndex = row.Fields.FindField(inputFields[i]);

                        if (row.get_Value(fieldIndex) != null)
                        {

                            var value = row.get_Value(fieldIndex);

                            if (multiContent)
                            {
                                //include field alias and mapped delimeter
                                content.Append(inputFieldAlias[i] + ": ");
                                content.Append(Convert.ToString(value));
                                content.Append(itemExportProperty.MappedContentDelimeter);

                            }
                            else
                                content.Append(Convert.ToString(value));
                        }
                    }

                    //foreach (string inputField in inputFields)
                    //{
                    //    int fieldIndex = row.Fields.FindField(inputField);

                    //    if (row.get_Value(fieldIndex) != null)
                    //    {

                    //        var value = row.get_Value(fieldIndex);

                    //        if (multiContent)
                    //        {
                    //            //include field alias and mapped delimeter
                    //            content.Append(row.Fields.Field[fieldIndex].AliasName + ": ");
                    //            content.Append(Convert.ToString(value));
                    //            content.Append(itemExportProperty.MappedContentDelimeter);

                    //        }
                    //        else
                    //            content.Append(Convert.ToString(value));
                    //    }
                    //}


                    //apply post condition if required
                    if (!string.IsNullOrEmpty(itemExportProperty.PostCondition))
                        content.Append(itemExportProperty.PostCondition);

                    itemContent = content.ToString();
                }

                return itemContent;
            }
            catch (Exception ex)
            {
                Exporter.logger.LogMessage(ESRI.ArcGIS.SOESupport.ServerLogger.msgType.error,
                    "GetItemContent",
                    999999,
                    "Item Export Property Mapped Content:" + itemExportProperty.MappedContent +
                    "Item Export Property Mapped Content Alias:" + itemExportProperty.MappedContentAlias +
                    "Item Export Property Fixed Content:" + itemExportProperty.FixedContent +
                    "Error Message: " + ex.Message.ToString());

                for (int r = 0; r < row.Fields.FieldCount; r++)
                {
                    IField field = row.Fields.get_Field(r);
                    Exporter.logger.LogMessage(ESRI.ArcGIS.SOESupport.ServerLogger.msgType.infoDetailed,
                    "GetItemContent",
                    999999,
                    "field Name: " + field.Name +
                    "field alias: " + field.AliasName
                    );
                }


                throw ex;
            }
            finally
            {

            }
        }
    }
}
