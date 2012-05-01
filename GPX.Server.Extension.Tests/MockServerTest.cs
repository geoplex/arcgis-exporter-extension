using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using MbUnit.Framework;
using Newtonsoft.Json.Linq;

namespace GPX.Server.Extension.Tests
{
    [TestFixture]
    public class MockServerTest
    {
        Exporter exporter;


        /// <summary>
        /// Set up - initialises license and creates mock server object helper
        /// </summary>
        [SetUp]
        public void TestSetUp()
        {
            //bind to a license - this is required so that we can mock a server object helper
            RuntimeManager.BindLicense(ProductCode.Server);
            if (!RuntimeManager.Bind(ProductCode.Server))
            {
                throw new ApplicationException("Cannot bind to ArcGIS Server License");

            }

            //create an instance of the extension & initialise it using the mock server object helper
            exporter = new Exporter();
            exporter.Init(MockServerObjectCreator.CreateMockServerObjectHelper(@"C:\Git\arcgis-exporter-extension\GPX.Server.Extension.Tests\testdata\California.mxd"));
            IPropertySet propSet = new PropertySet();
            exporter.Construct(propSet);
        }


        /// <summary>
        /// Exports the layer resource request.
        /// </summary>
        /// <param name="capabilities">The capabilities.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="operationName">Name of the operation.</param>
        /// <param name="operationInput">The operation input.</param>
        /// <param name="outputFormat">The output format.</param>
        /// <param name="requestProperties">The request properties.</param>
        /// <param name="expectedResult">The expected result.</param>
        [Test]
        [Row("", "ExportLayers/2", "", "", "json", "{}", 1, Description = "Valid Export Layer Resource")]
        [Row("", "ExportLayers/12", "", "", "json", "{}", 0, Description = "Invalid Export Layer Resource")]
        public void ExportLayerResourceRequest(string capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties, int expectedResult)
        {

            string outProps;
            string res = SendRequest(capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out outProps);

            //parse the respinse and validate count of entity
            JObject o = JObject.Parse(res);
            string p = (string)o["name"];

            int result;
            if (string.IsNullOrEmpty(p))
                result = 0;
            else
                result = 1;

            Assert.AreEqual(expectedResult, result);
        }

        /// <summary>
        /// Exports the Geo RSS layer.
        /// </summary>
        /// <param name="capabilities">The capabilities.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="operationName">Name of the operation.</param>
        /// <param name="operationInput">The operation input.</param>
        /// <param name="outputFormat">The output format.</param>
        /// <param name="requestProperties">The request properties.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="itemElementName">Name of the item element.</param>
        [Test]
        [Row(
            "",
            "ExportLayers/2",
            "ExportLayer",
            "{\"filterGeometry\":null,\"geometryType\":null,\"where\":null,\"exportProperties\":{\"Title\":\"Californian EarthQuakes\",\"ID\":\"Feed Id\",\"Language\":\"en-us\",\"CopyRight\":\"Copyright 2011\",\"Generator\":\"http://www.mapbutcher.com\",\"Description\":\"A GeoRSS format of Californian EarthQuakes\",\"Link\":{\"BaseUri\":null,\"Length\":0,\"MediaType\":null,\"RelationshipType\":null,\"Title\":\"Mapbutcher\",\"Uri\":\"http://www.mapbutcher.com\"},\"Author\":{\"Email\":\"mapbutcher@mapbutcher.com\",\"Name\":\"Simon\",\"Uri\":\"http://www.mapbutcher.com\"},\"GeometryFormat\":\"simple\",\"GeometryField\":\"Shape\",\"FeedFormat\":\"Atom\",\"Items\":[{\"Key\":\"Author\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Simon Hope\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Content\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Contributors\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"mapbutcher@mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Copyright\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Copyright 2011\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Id\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":\"UFI\",\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"LastUpdatedTime\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Links\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"http://www.mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"PublishDate\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"Summary\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Title\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}}]}}",
            "georss",
            "{}",
            221,
            "http://www.w3.org/2005/Atom",
            "entry",
            Description = "Export Points Simple Atom")]
        [Row(
            "",
            "ExportLayers/2",
            "ExportLayer",
            "{\"filterGeometry\":null,\"geometryType\":null,\"where\":null,\"exportProperties\":{\"Title\":\"Californian EarthQuakes\",\"ID\":\"Feed Id\",\"Language\":\"en-us\",\"CopyRight\":\"Copyright 2011\",\"Generator\":\"http://www.mapbutcher.com\",\"Description\":\"A GeoRSS format of Californian EarthQuakes\",\"Link\":{\"BaseUri\":null,\"Length\":0,\"MediaType\":null,\"RelationshipType\":null,\"Title\":\"Mapbutcher\",\"Uri\":\"http://www.mapbutcher.com\"},\"Author\":{\"Email\":\"mapbutcher@mapbutcher.com\",\"Name\":\"Simon\",\"Uri\":\"http://www.mapbutcher.com\"},\"GeometryFormat\":\"GML\",\"GeometryField\":\"Shape\",\"FeedFormat\":\"Atom\",\"Items\":[{\"Key\":\"Author\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Simon Hope\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Content\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Contributors\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"mapbutcher@mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Copyright\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Copyright 2011\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Id\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":\"UFI\",\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"LastUpdatedTime\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Links\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"http://www.mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"PublishDate\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"Summary\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Title\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}}]}}",
            "georss",
            "{}",
            221,
            "http://www.w3.org/2005/Atom",
            "entry",
            Description = "Export Points GML Atom")]
        [Row(
            "",
            "ExportLayers/7",
            "ExportLayer",
            "{\"filterGeometry\":null,\"geometryType\":null,\"where\":null,\"exportProperties\":{\"Title\":\"Californian Rivers\",\"ID\":\"Feed Id\",\"Language\":\"en-us\",\"CopyRight\":\"Copyright 2011\",\"Generator\":\"http://www.mapbutcher.com\",\"Description\":\"A GeoRSS format of Californian Rivers\",\"Link\":{\"BaseUri\":null,\"Length\":0,\"MediaType\":null,\"RelationshipType\":null,\"Title\":\"Mapbutcher\",\"Uri\":\"http://www.mapbutcher.com\"},\"Author\":{\"Email\":\"mapbutcher@mapbutcher.com\",\"Name\":\"Simon\",\"Uri\":\"http://www.mapbutcher.com\"},\"GeometryFormat\":\"simple\",\"GeometryField\":\"Shape\",\"FeedFormat\":\"Atom\",\"Items\":[{\"Key\":\"Author\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Simon Hope\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Content\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Contributors\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"mapbutcher@mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Copyright\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Copyright 2011\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Id\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":\"UFI\",\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"LastUpdatedTime\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Links\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"http://www.mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"PublishDate\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"Summary\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Title\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}}]}}",
            "georss",
            "{}",
            500,
            "http://www.w3.org/2005/Atom",
            "entry",
            Description = "Export Lines Simple Atom - test assumes 500 feature limit threshhold set by mock server object")]
        [Row(
            "",
            "ExportLayers/7",
            "ExportLayer",
            "{\"filterGeometry\":null,\"geometryType\":null,\"where\":null,\"exportProperties\":{\"Title\":\"Californian Rivers\",\"ID\":\"Feed Id\",\"Language\":\"en-us\",\"CopyRight\":\"Copyright 2011\",\"Generator\":\"http://www.mapbutcher.com\",\"Description\":\"A GeoRSS format of Californian Rivers\",\"Link\":{\"BaseUri\":null,\"Length\":0,\"MediaType\":null,\"RelationshipType\":null,\"Title\":\"Mapbutcher\",\"Uri\":\"http://www.mapbutcher.com\"},\"Author\":{\"Email\":\"mapbutcher@mapbutcher.com\",\"Name\":\"Simon\",\"Uri\":\"http://www.mapbutcher.com\"},\"GeometryFormat\":\"GML\",\"GeometryField\":\"Shape\",\"FeedFormat\":\"Atom\",\"Items\":[{\"Key\":\"Author\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Simon Hope\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Content\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Contributors\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"mapbutcher@mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Copyright\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Copyright 2011\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Id\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":\"UFI\",\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"LastUpdatedTime\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Links\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"http://www.mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"PublishDate\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"Summary\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Title\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}}]}}",
            "georss",
            "{}",
            500,
            "http://www.w3.org/2005/Atom",
            "entry",
            Description = "Export Lines GML Atom - test assumes 500 feature limit threshhold set by mock server object")]
        [Row(
            "",
            "ExportLayers/11",
            "ExportLayer",
            "{\"filterGeometry\":null,\"geometryType\":null,\"where\":null,\"exportProperties\":{\"Title\":\"Californian Urban Areas\",\"ID\":\"Feed Id\",\"Language\":\"en-us\",\"CopyRight\":\"Copyright 2011\",\"Generator\":\"http://www.mapbutcher.com\",\"Description\":\"A GeoRSS format of Californian Urban Areas\",\"Link\":{\"BaseUri\":null,\"Length\":0,\"MediaType\":null,\"RelationshipType\":null,\"Title\":\"Mapbutcher\",\"Uri\":\"http://www.mapbutcher.com\"},\"Author\":{\"Email\":\"mapbutcher@mapbutcher.com\",\"Name\":\"Simon\",\"Uri\":\"http://www.mapbutcher.com\"},\"GeometryFormat\":\"Simple\",\"GeometryField\":\"Shape\",\"FeedFormat\":\"Atom\",\"Items\":[{\"Key\":\"Author\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Simon Hope\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Content\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Contributors\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"mapbutcher@mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Copyright\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Copyright 2011\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Id\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":\"UFI\",\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"LastUpdatedTime\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Links\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"http://www.mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"PublishDate\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"Summary\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Title\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}}]}}",
            "georss",
            "{}",
            38,
            "http://www.w3.org/2005/Atom",
            "entry",
            Description = "Export Polygon Simple Atom")]
        [Row(
            "",
            "ExportLayers/11",
            "ExportLayer",
            "{\"filterGeometry\":null,\"geometryType\":null,\"where\":null,\"exportProperties\":{\"Title\":\"Californian Urban Areas\",\"ID\":\"Feed Id\",\"Language\":\"en-us\",\"CopyRight\":\"Copyright 2011\",\"Generator\":\"http://www.mapbutcher.com\",\"Description\":\"A GeoRSS format of Californian Urban Areas\",\"Link\":{\"BaseUri\":null,\"Length\":0,\"MediaType\":null,\"RelationshipType\":null,\"Title\":\"Mapbutcher\",\"Uri\":\"http://www.mapbutcher.com\"},\"Author\":{\"Email\":\"mapbutcher@mapbutcher.com\",\"Name\":\"Simon\",\"Uri\":\"http://www.mapbutcher.com\"},\"GeometryFormat\":\"GML\",\"GeometryField\":\"Shape\",\"FeedFormat\":\"Atom\",\"Items\":[{\"Key\":\"Author\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Simon Hope\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Content\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Contributors\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"mapbutcher@mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Copyright\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Copyright 2011\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Id\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":\"UFI\",\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"LastUpdatedTime\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Links\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"http://www.mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"PublishDate\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"Summary\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Title\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}}]}}",
            "georss",
            "{}",
            38,
            "http://www.w3.org/2005/Atom",
            "entry",
            Description = "Export Polygon GML Atom")]
        public void ExportGeoRSS_AtomFormat_Layer(string capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties, int expectedResult, string nameSpace, string itemElementName)
        {
            string outProps;
            int result = 0;

            string res = SendRequest(capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out outProps);

            //parse the respose
            XDocument xDoc = XDocument.Load(new StringReader(res));
            System.Xml.Linq.XNamespace ns = nameSpace;
            //var items = xDoc.Descendants();
            foreach (var el in xDoc.Root.Elements(ns + itemElementName)) { result++; }


            Assert.AreEqual(expectedResult, result);

            TextWriter tw = new StreamWriter(@"C:\Git\arcgis-exporter-extension\GPX.Server.Extension.Tests\testresults\" + Gallio.Framework.TestContext.CurrentContext.Test.Name + "_" + DateTime.Now.ToFileTime() + ".xml");

            // write a line of text to the file
            tw.WriteLine(res);

            // close the stream
            tw.Close();


        }

        [Test]
        [Row(
            "",
            "ExportLayers/2",
            "ExportLayer",
            "{\"filterGeometry\":null,\"geometryType\":null,\"where\":null,\"exportProperties\":{\"Title\":\"Californian EarthQuakes\",\"ID\":\"Feed Id\",\"Language\":\"en-us\",\"CopyRight\":\"Copyright 2011\",\"Generator\":\"http://www.mapbutcher.com\",\"Description\":\"A GeoRSS format of Californian EarthQuakes\",\"Link\":{\"BaseUri\":null,\"Length\":0,\"MediaType\":null,\"RelationshipType\":null,\"Title\":\"Mapbutcher\",\"Uri\":\"http://www.mapbutcher.com\"},\"Author\":{\"Email\":\"mapbutcher@mapbutcher.com\",\"Name\":\"Simon\",\"Uri\":\"http://www.mapbutcher.com\"},\"GeometryFormat\":\"simple\",\"GeometryField\":\"Shape\",\"FeedFormat\":\"Rss\",\"Items\":[{\"Key\":\"Author\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Simon Hope\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Content\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Contributors\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"mapbutcher@mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Copyright\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Copyright 2011\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Id\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":\"UFI\",\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"LastUpdatedTime\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Links\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"http://www.mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"PublishDate\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"Summary\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Title\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}}]}}",
            "georss",
            "{}",
            221,
            "item",
            Description = "Export Points Simple RSS")]
        [Row(
            "",
            "ExportLayers/2",
            "ExportLayer",
            "{\"filterGeometry\":null,\"geometryType\":null,\"where\":null,\"exportProperties\":{\"Title\":\"Californian EarthQuakes\",\"ID\":\"Feed Id\",\"Language\":\"en-us\",\"CopyRight\":\"Copyright 2011\",\"Generator\":\"http://www.mapbutcher.com\",\"Description\":\"A GeoRSS format of Californian EarthQuakes\",\"Link\":{\"BaseUri\":null,\"Length\":0,\"MediaType\":null,\"RelationshipType\":null,\"Title\":\"Mapbutcher\",\"Uri\":\"http://www.mapbutcher.com\"},\"Author\":{\"Email\":\"mapbutcher@mapbutcher.com\",\"Name\":\"Simon\",\"Uri\":\"http://www.mapbutcher.com\"},\"GeometryFormat\":\"simple\",\"GeometryField\":\"Shape\",\"OutputSpatialReference\": {\"Wkid\": 102100,\"CoordinateSystemType\": \"projected\",\"TransformationId\": null},\"FeedFormat\":\"Rss\",\"Items\":[{\"Key\":\"Author\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Simon Hope\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Content\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Contributors\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"mapbutcher@mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Copyright\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Copyright 2011\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Id\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":\"UFI\",\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"LastUpdatedTime\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Links\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"http://www.mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"PublishDate\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"Summary\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Title\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}}]}}",
            "georss",
            "{}",
            221,
            "item",
            Description = "Export Points Simple RSS - Include Spatial Reference")]
        [Row(
            "",
            "ExportLayers/2",
            "ExportLayer",
            "{\"filterGeometry\":null,\"geometryType\":null,\"where\":null,\"exportProperties\":{\"Title\":\"Californian EarthQuakes\",\"ID\":\"Feed Id\",\"Language\":\"en-us\",\"CopyRight\":\"Copyright 2011\",\"Generator\":\"http://www.mapbutcher.com\",\"Description\":\"A GeoRSS format of Californian EarthQuakes\",\"Link\":{\"BaseUri\":null,\"Length\":0,\"MediaType\":null,\"RelationshipType\":null,\"Title\":\"Mapbutcher\",\"Uri\":\"http://www.mapbutcher.com\"},\"Author\":{\"Email\":\"mapbutcher@mapbutcher.com\",\"Name\":\"Simon\",\"Uri\":\"http://www.mapbutcher.com\"},\"GeometryFormat\":\"GML\",\"GeometryField\":\"Shape\",\"FeedFormat\":\"Rss\",\"Items\":[{\"Key\":\"Author\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Simon Hope\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Content\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Contributors\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"mapbutcher@mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Copyright\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Copyright 2011\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Id\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":\"UFI\",\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"LastUpdatedTime\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Links\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"http://www.mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"PublishDate\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"Summary\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Title\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}}]}}",
            "georss",
            "{}",
            221,
            "item",
            Description = "Export Points GML RSS")]
        [Row(
            "",
            "ExportLayers/7",
            "ExportLayer",
            "{\"filterGeometry\":null,\"geometryType\":null,\"where\":null,\"exportProperties\":{\"Title\":\"Californian Rivers\",\"ID\":\"Feed Id\",\"Language\":\"en-us\",\"CopyRight\":\"Copyright 2011\",\"Generator\":\"http://www.mapbutcher.com\",\"Description\":\"A GeoRSS format of Californian Rivers\",\"Link\":{\"BaseUri\":null,\"Length\":0,\"MediaType\":null,\"RelationshipType\":null,\"Title\":\"Mapbutcher\",\"Uri\":\"http://www.mapbutcher.com\"},\"Author\":{\"Email\":\"mapbutcher@mapbutcher.com\",\"Name\":\"Simon\",\"Uri\":\"http://www.mapbutcher.com\"},\"GeometryFormat\":\"simple\",\"GeometryField\":\"Shape\",\"FeedFormat\":\"Rss\",\"Items\":[{\"Key\":\"Author\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Simon Hope\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Content\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Contributors\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"mapbutcher@mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Copyright\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Copyright 2011\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Id\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":\"UFI\",\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"LastUpdatedTime\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Links\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"http://www.mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"PublishDate\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"Summary\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Title\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}}]}}",
            "georss",
            "{}",
            500,
            "item",
            Description = "Export Lines Simple Rss - test assumes 500 feature limit threshhold set by mock server object")]
        [Row(
            "",
            "ExportLayers/7",
            "ExportLayer",
            "{\"filterGeometry\":null,\"geometryType\":null,\"where\":null,\"exportProperties\":{\"Title\":\"Californian Rivers\",\"ID\":\"Feed Id\",\"Language\":\"en-us\",\"CopyRight\":\"Copyright 2011\",\"Generator\":\"http://www.mapbutcher.com\",\"Description\":\"A GeoRSS format of Californian Rivers\",\"Link\":{\"BaseUri\":null,\"Length\":0,\"MediaType\":null,\"RelationshipType\":null,\"Title\":\"Mapbutcher\",\"Uri\":\"http://www.mapbutcher.com\"},\"Author\":{\"Email\":\"mapbutcher@mapbutcher.com\",\"Name\":\"Simon\",\"Uri\":\"http://www.mapbutcher.com\"},\"GeometryFormat\":\"GML\",\"GeometryField\":\"Shape\",\"FeedFormat\":\"Rss\",\"Items\":[{\"Key\":\"Author\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Simon Hope\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Content\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Contributors\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"mapbutcher@mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Copyright\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"Copyright 2011\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Id\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":\"UFI\",\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"LastUpdatedTime\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Links\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":\"http://www.mapbutcher.com\",\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"PublishDate\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":true}},{\"Key\":\"Summary\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}},{\"Key\":\"Title\",\"Value\":{\"PostCondition\":null,\"PreConditon\":null,\"MappedContent\":null,\"MappedContentAlias\":null,\"FixedContent\":null,\"MappedContentDelimeter\":null,\"AutoGenerate\":false}}]}}",
            "georss",
            "{}",
            500,
            "item",
            Description = "Export Lines GML Rss - test assumes 500 feature limit threshhold set by mock server object")]
        public void ExportGeoRSS_RSSFormat_Layer(string capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties, int expectedResult, string itemElementName)
        {
            string outProps;
            int result = 0;

            string res = SendRequest(capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out outProps);
            Gallio.Framework.TestLog.WriteLine(res);

            //parse the respose
            XDocument xDoc = XDocument.Load(new StringReader(res));
            foreach (var el in xDoc.Root.Elements("channel").Elements(itemElementName)) { result++; }


            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [Row(
            "",
            "ExportLayers/2",
            "ExportLayer",
            "{\"filterGeometry\":null,\"geometryType\":null,\"where\":null,\"exportProperties\":{\"GeometryField\": \"Shape\"}}",
            "geojson",
            "{}",
            Description = "Export Points as GeoJson")]
        public void ExportGeoJson(string capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties)
        {
            string outProps;

            string res = SendRequest(capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out outProps);
            Gallio.Framework.TestLog.WriteLine(res);
        }


        private string SendRequest(string capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties, out string outputProperties)
        {
            byte[] response = exporter.HandleRESTRequest(
                capabilities,
                resourceName,
                operationName,
                operationInput,
                outputFormat,
                requestProperties,
                out outputProperties);


            return Encoding.Default.GetString(response);
        }
    }
}
