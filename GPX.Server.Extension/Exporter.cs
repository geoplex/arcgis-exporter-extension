
#region File Information
//
// File: "Exporter.cs"
// Purpose: "Provides the implementation of an ArcGIS Server Object Extension"
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
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.EnterpriseServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.SOESupport;
using System.IO;
using ESRI.ArcGIS.Carto;
using System.Xml;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using System.ServiceModel.Syndication;
using Newtonsoft.Json;


namespace GPX.Server.Extension
{
    [ComVisible(true)]
    [Guid("67CBD04A-99AB-4686-A514-541B74E2DC8D")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Exporter : ServicedComponent, IServerObjectExtension, IObjectConstruct, IRESTRequestHandler
    {
        private string soe_name;

        private IPropertySet configProps;
        private IServerObjectHelper serverObjectHelper;
        public static ServerLogger logger;
        private IRESTRequestHandler reqHandler;

        private const int SOE_ERROR_CODE = 99999;
        private const string GEORSS_FORMAT = "georss";
        private const string GEOJSON_FORMAT = "geojson";


        public Exporter()
        {
            soe_name = this.GetType().Name;
            logger = new ServerLogger();
            reqHandler = new SoeRestImpl(soe_name, CreateRestSchema()) as IRESTRequestHandler;
        }

        #region IServerObjectExtension Members

        public void Init(IServerObjectHelper pSOH)
        {
            serverObjectHelper = pSOH;
            
        }

        public void Shutdown()
        {
        }

        #endregion

        #region IObjectConstruct Members

        public void Construct(IPropertySet props)
        {
            configProps = props;

        }

        #endregion

        #region IRESTRequestHandler Members

        public string GetSchema()
        {
            return reqHandler.GetSchema();
        }

        public byte[] HandleRESTRequest(string Capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties, out string responseProperties)
        {
            return reqHandler.HandleRESTRequest(Capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out responseProperties);
        }

        #endregion

        private RestResource CreateRestSchema()
        {
            //main extension resource
            //provides a layer list of all layers in the service on which the extension is attached to.
            RestResource rootRes = new RestResource(soe_name, false, RootResHandler);

            //provides a single layer view - basically name and layer id
            RestResource exportLayerResource = new RestResource("ExportLayers", true, ExportLayerResourceHandler);



            RestOperation export = new RestOperation("ExportLayer",
                                          new string[] { "filterGeometry","geometryType","where","exportProperties" },
                                          new string[] { "georss" },
                                          ExportLayerHandler,true);


            exportLayerResource.operations.Add(export);

            rootRes.resources.Add(exportLayerResource);


            return rootRes;
        }


        #region Resource Handlers

        private byte[] RootResHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        {
            responseProperties = null;

            ExportLayerInfo[] layerInfos = GetLayerInfos();

            JsonObject[] jos = new JsonObject[layerInfos.Length];

            for (int i = 0; i < layerInfos.Length; i++)
                jos[i] = layerInfos[i].ToJsonObject();

            JsonObject result = new JsonObject();
            result.AddArray("ExportLayers", jos);

            string json = result.ToJson();

            return Encoding.UTF8.GetBytes(json);
        }

        private byte[] ExportLayerResourceHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        {
            responseProperties = null;

            //layerID
            int layerID = Convert.ToInt32(boundVariables["ExportLayersId"]);

            //execute
            ExportLayerInfo layerInfo = GetLayerInfo(layerID);

            string json = layerInfo.ToJsonObject().ToJson();

            return Encoding.UTF8.GetBytes(json);
        }

        #endregion

        #region Operation Handlers

        private byte[] ExportLayerHandler(NameValueCollection boundVariables, JsonObject operationInput, string outputFormat, string requestProperties, out string responseProperties)
        {

            responseProperties = null;


            try
            {

                //hydrate the input
                JsonObject feedPropertiesObject = null;
                JsonObject jsonLocation;
                IGeometry location = null;
                GeoRSSExportProperties feedProperties = null;
                string whereClause;
                string geometryType;
                long? transformationId = null;
                GPX.Server.Extension.Spatial.MapServer mapserver = new Spatial.MapServer(serverObjectHelper.ServerObject);



                //layerID
                int layerID = Convert.ToInt32(boundVariables["ExportLayersId"]);

                if (!operationInput.TryGetJsonObject("exportProperties", out feedPropertiesObject))
                    throw new ArgumentException("Error: Could not parse exportProperties" + feedPropertiesObject.ToJson());
                

                //initialise the correct export properties
                if (outputFormat == GEORSS_FORMAT)
                {
                    if (feedPropertiesObject != null)
                    {

                        feedProperties = new GeoRSSExportProperties(feedPropertiesObject.ToJson());
                    }

                }
                else
                {
                    throw new NotImplementedException();
                }
                

                //todo - implement transformation support
                //if (!operationInput.TryGetAsLong("transformationId", out transformationId))
                //    throw new ArgumentNullException("transformationId");

                //todo - test remove
                //transformationId = 1278;


                if (!operationInput.TryGetJsonObject("filterGeometry", out jsonLocation))
                    throw new ArgumentNullException("filterGeometry");

                if (jsonLocation != null)
                {
                    if (!operationInput.TryGetString("geometryType", out geometryType))
                        throw new ArgumentNullException("Can supply a geometry without a geometryType");

                    switch (geometryType)
                    {
                        case "Polygon":
                            location = Conversion.ToGeometry(jsonLocation, esriGeometryType.esriGeometryPolygon);
                            if (location == null)
                                throw new ArgumentException("ExportLayerHandler: invalid polygon", "filterGeometry");
                            break;
                        case "Point":
                            location = Conversion.ToGeometry(jsonLocation, esriGeometryType.esriGeometryPoint);
                            if (location == null)
                                throw new ArgumentException("ExportLayerHandler: invalid point", "filterGeometry");
                            break;
                        case "Line":
                            location = Conversion.ToGeometry(jsonLocation, esriGeometryType.esriGeometryPolyline);
                            if (location == null)
                                throw new ArgumentException("ExportLayerHandler: invalid polyline", "filterGeometry");
                            break;
                        default:
                            break;
                    }

                }


                if (!operationInput.TryGetString("where", out whereClause))
                    throw new ArgumentNullException("where");

                //todo - dependent upon requested format query the mapserver and set the correct return type
                //i.e. recordset for GeoRSS and JSON for GeoJson

                //run the query on the map server
                RecordSet results = mapserver.Query(layerID, location, whereClause, transformationId);

                //generate the export
                string updateWithEncoding = string.Empty;
                string xmlString = string.Empty;

                if (outputFormat == GEORSS_FORMAT)
                {
                    if (feedProperties != null)
                    {

                       
                        GeoRSSExport export = new GeoRSSExport();
                        export.CreateExport(results, feedProperties);

                        StringBuilder sb = new StringBuilder();
                        XmlWriter xmlWriter = XmlWriter.Create(sb);

                        if (feedProperties.FeedFormat == "Atom")
                        {
                            export.SaveAsAtom10(xmlWriter);
                            responseProperties = "{\"Content-Type\" : \"application/atom+xml\"}";
                        }
                        else
                        {
                            export.SaveAsRss20(xmlWriter);
                            responseProperties = "{\"Content-Type\" : \"application/rss+xml\"}";
                        }

                        xmlWriter.Close();
                        xmlString = sb.ToString();

                        //todo  - is it the xmlwriter that applies the encoding - setting the xmlwritersettings.encoding does not help!
                        updateWithEncoding = xmlString.Replace("utf-16", "utf-8");

                    }
                }
                else
                {
                    //set response properties
                    responseProperties = null;

                    throw new NotImplementedException();
                }
                
                return Encoding.UTF8.GetBytes(updateWithEncoding);
            }
            catch (Exception ex)
            {
                logger.LogMessage(ServerLogger.msgType.error, "ExportLayerHandler", 999999, ex.ToString());
                responseProperties = null;
                string error =  JsonConvert.SerializeObject(ex, Newtonsoft.Json.Formatting.Indented);
                return Encoding.UTF8.GetBytes(error);

            }
            finally
            {

            }


        }

        #endregion


        private ExportLayerInfo GetLayerInfo(int layerID)
        {
            IMapServer3 mapServer;
            IMapLayerInfo layerInfo;
            IMapLayerInfos layerInfos;

            try
            {
                if (layerID < 0)
                    throw new ArgumentOutOfRangeException("layerID");

                mapServer = serverObjectHelper.ServerObject as IMapServer3;
                if (mapServer == null)
                    throw new Exception("Unable to access the map server.");


                layerInfos = mapServer.GetServerInfo(mapServer.DefaultMapName).MapLayerInfos;
                long c = layerInfos.Count;

                for (int i = 0; i < c; i++)
                {
                    layerInfo = layerInfos.get_Element(i);
                    if (layerInfo.ID == layerID)
                        return new ExportLayerInfo(layerInfo);
                }

                throw new ArgumentOutOfRangeException("layerID");
            }
            catch (Exception ex)
            {
                logger.LogMessage(ServerLogger.msgType.error, "GetLayerInfo", SOE_ERROR_CODE, "An error occurred getting the export layer info: " + ex.ToString());
                throw ex;
            }
            finally
            {
                mapServer = null;
                layerInfos = null;
                layerInfo = null;

            }
        }


        private ExportLayerInfo[] GetLayerInfos()
        {
            IMapServer3 mapServer;
            IMapServerInfo msInfo;
            IMapLayerInfos layerInfos;

            try
            {
                mapServer = serverObjectHelper.ServerObject as IMapServer3;
                if (mapServer == null)
                    throw new Exception("Unable to access the map server.");

                msInfo = mapServer.GetServerInfo(mapServer.DefaultMapName);
                layerInfos = msInfo.MapLayerInfos;
                int c = layerInfos.Count;

                ExportLayerInfo[] nearestLayerInfos = new ExportLayerInfo[c];

                for (int i = 0; i < c; i++)
                {
                    IMapLayerInfo layerInfo = layerInfos.get_Element(i);
                    nearestLayerInfos[i] = new ExportLayerInfo(layerInfo);
                }

                return nearestLayerInfos;
            }
            catch (Exception ex)
            {
                logger.LogMessage(ServerLogger.msgType.error, "GetLayerInfos", SOE_ERROR_CODE, "An error occurred getting the export layer collection: " + ex.ToString());
                throw ex;
            }
            finally
            {
                mapServer = null;
                msInfo = null;
                layerInfos = null;
            }
        }


    }
}

