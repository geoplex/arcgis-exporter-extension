#region File Information
//
// File: "MapServer.cs"
// Purpose: "Helper class used for interfacing with ArcGIS Server
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
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SOESupport;

namespace GPX.Server.Extension.Spatial
{
    public class MapServer
    {
        private IMapServer3  _server;

        public IMapServer3  Server
        {
            set { _server = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapServer"/> class.
        /// </summary>
        /// <param name="serverObject">The server object.</param>
        public MapServer(IServerObject serverObject)
        {
            Server = serverObject as IMapServer3;
            if (_server == null)
                throw new Exception("Unable to access the map server.");
        }

        /// <summary>
        /// Queries the specified layer id.
        /// </summary>
        /// <param name="layerId">The layer id.</param>
        /// <param name="spatialFilter">The spatial filter.</param>
        /// <param name="attributeFilter">The attribute filter.</param>
        /// <param name="transformationId">The transformation id.</param>
        /// <returns></returns>
        public RecordSet Query(int layerId, IGeometry spatialFilter, string attributeFilter, long? transformationId)
        {
            ISpatialFilter filter = new SpatialFilterClass();
            RecordSet recordSet = null;
            IQueryResultOptions resultOptions;
            IMapTableDescription tableDesc;
            IQueryResult result;

            try
            {
                //set the filter properties
                filter.Geometry = spatialFilter;
                filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                if (!String.IsNullOrEmpty(attributeFilter))
                    filter.WhereClause = attributeFilter;

                //set the result options
                resultOptions = new QueryResultOptionsClass();
                resultOptions.Format = esriQueryResultFormat.esriQueryResultRecordSetAsObject;

                //todo - submitted forum question to esri: http://forums.arcgis.com/threads/39248-Setting-GeoTransformation-on-QueryResultOptionsClass-does-not-work?p=132913
                //apply transformation if required
                if (transformationId != null)
                {
                    //ISpatialReferenceFactory2 srFactory;
                    //srFactory = new SpatialReferenceEnvironmentClass();

                    Type factoryType = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment");
                    ISpatialReferenceFactory3 srFactory = (ISpatialReferenceFactory3)Activator.CreateInstance(factoryType);


                    IGeoTransformation gt;
                    gt = srFactory.CreateGeoTransformation((int)transformationId) as IGeoTransformation;

                    resultOptions.GeoTransformation = gt;
                }
                

                tableDesc = GetTableDesc(_server, layerId);
                result = _server.QueryData(_server.DefaultMapName, tableDesc, filter, resultOptions);

                //IlayerDescription3 lyrDesc = GetLayerDesc(_server, layerId);
                //result = _server.QueryFeatureData2(_server.DefaultMapName, lyrDesc, filter, resultOptions);

                recordSet = result.Object as RecordSet;

                return recordSet;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                filter = null;
                resultOptions = null;
                tableDesc = null;
                result = null;
            }

        }

        private ILayerDescription3 GetLayerDesc(IMapServer3 mapServer, int layerId)
        {
            ILayerDescriptions layerDescs = mapServer.GetServerInfo(mapServer.DefaultMapName).DefaultMapDescription.LayerDescriptions;
            long c = layerDescs.Count;

            for (int i = 0; i < c; i++)
            {
                ILayerDescription3 layerDesc = (ILayerDescription3)layerDescs.get_Element(i);

                if (layerDesc.ID == layerId)
                {
                    return layerDesc;
                }
            }

            throw new ArgumentOutOfRangeException("layerId");
        }


        /// <summary>
        /// Gets the table description.
        /// </summary>
        /// <param name="mapServer">The map server.</param>
        /// <param name="layerId">The layer id.</param>
        /// <returns></returns>
        /// <remarks>Sets the result options to return field names and densify geometries</remarks>
        private IMapTableDescription GetTableDesc(IMapServer3 mapServer, int layerId)
        {
            ILayerDescriptions layerDescs = mapServer.GetServerInfo(mapServer.DefaultMapName).DefaultMapDescription.LayerDescriptions;
            long c = layerDescs.Count;

            for (int i = 0; i < c; i++)
            {
                ILayerDescription3 layerDesc = (ILayerDescription3)layerDescs.get_Element(i);

                
                 
                if (layerDesc.ID == layerId)
                {
                    layerDesc.LayerResultOptions = new LayerResultOptionsClass();
                    layerDesc.LayerResultOptions.ReturnFieldNamesInResults = true;

                    //optionally apply densify or generalise operation to geometries
                    //layerDesc.LayerResultOptions.GeometryResultOptions = new GeometryResultOptionsClass();
                    //layerDesc.LayerResultOptions.GeometryResultOptions.DensifyGeometries = false; 
                    

                    return (IMapTableDescription)layerDesc;
                }
            }

            throw new ArgumentOutOfRangeException("layerId");
        }

        /// <summary>
        /// Gets the feature class field alias.
        /// </summary>
        /// <param name="layerId">The layer id.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public string GetFeatureClassFieldAlias(int layerId, string fieldName)
        {
            IMapServerDataAccess dataAccess;
            IFeatureClass fc;

            try
            {
                if (layerId < 0)
                    throw new ArgumentOutOfRangeException("layerID");

                if (_server == null)
                    throw new Exception("Unable to access the map server.");

                // Access the source feature class.
                string mapName = _server.DefaultMapName;
                dataAccess = (IMapServerDataAccess)_server;
                fc = (IFeatureClass)dataAccess.GetDataSource(mapName, layerId);


                if (fc == null)
                    throw new Exception("Unable to access the feature class for layer id:" + layerId);

                int fieldIndex;
                try
                {
                     fieldIndex = fc.Fields.FindField(fieldName);
                }
                catch (Exception)
                {

                    throw new Exception("Unable to access the field: " + fieldName + "on feature class: " + fc.AliasName);
                }
                

                return fc.Fields.get_Field(fieldIndex).AliasName;
            }
            finally
            {
                dataAccess = null;
                fc = null;
            }
        }


        /// <summary>
        /// Gets the name of the feature class shape field.
        /// </summary>
        /// <param name="layerId">The layer id.</param>
        /// <returns></returns>
        public string GetFeatureClassShapeFieldName(int layerId)
        {
            IMapServerDataAccess dataAccess;
            IFeatureClass fc;

            try
            {
                if (layerId < 0)
                    throw new ArgumentOutOfRangeException("layerID");

                if (_server == null)
                    throw new Exception("Unable to access the map server.");

                // Access the source feature class.
                string mapName = _server.DefaultMapName;
                dataAccess = (IMapServerDataAccess)_server;
                fc = (IFeatureClass)dataAccess.GetDataSource(mapName, layerId);

                

                if (fc == null)
                    throw new Exception("Unable to access the feature class for layer id:" + layerId);

                return fc.ShapeFieldName;
            }
            finally
            {
                dataAccess = null;
                fc = null;
            }
        }
        

    }
}
