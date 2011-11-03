#region File Information
//
// File: "GeoRSSGeometry.cs"
// Purpose: "Contains all classes which represent support GeoRSS geometry types and associated factories"
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
using System.Xml;
using ESRI.ArcGIS.Geometry;
using System.Xml.Linq;

namespace GPX.Server.Extension
{

    public abstract class GeoRSSGeometry 
    {

        public abstract XElement GeometryAsXML{ get; }

        public const string POINT = "point";
        public const string POLYGON = "polygon";
        public const string LINE = "line";
        public const string LINESTRING = "LineString";
        public const string GML_SUFFIX = "where";
        public const string GML_POSITION = "pos";
        public const string GML_POSITION_LIST = "posList";
        public const string GML_POLYGON = "Polygon";
        public const string GML_EXTERIOR = "exterior";
        public const string GML_LINEARRING = "LinearRing";
        public  XNamespace GEORSS_NAMESPACE = "http://www.georss.org/georss";
        public  XNamespace GML_NAMESPACE = "http://www.opengis.net/gml";
        
        public List<string> ParseGeometryCollection(IGeometryCollection geometryCollection)
        {
            var coords = new List<string>();

            //for each geometry in the collection
            for (int i = 0; i < geometryCollection.GeometryCount; i++)
            {
                var pGeom = geometryCollection.Geometry[i];
                var pntCollection = (IPointCollection)pGeom;

                for (var a = 0; a < pntCollection.PointCount; a++)
                {
                    var roundedPoint = Math.Round(pntCollection.Point[a].Y, 5) + " " + Math.Round(pntCollection.Point[a].X, 5);
                    //compare the point to the last one entered to make sure only unique points are entered
                    string lastPoint = "";
                    if (coords.Count > 1) lastPoint = coords[coords.Count - 1];
                    if (!lastPoint.Equals(roundedPoint)) coords.Add(roundedPoint);
                }
            }
            return coords;
        }
    
    }

    public abstract class GeoRSSGeometryFactory
    {

        public abstract GeoRSSGeometry GetGeometry(IGeometry esriGeometry);

    }

    public class GMLPointGeometry : GeoRSSGeometry 
    {

        XElement _geometryAsXML;

        public override XElement GeometryAsXML
        {

            get { return _geometryAsXML; }

        }

        public GMLPointGeometry(IPoint point)
        {
            XElement georsswrappper = new XElement(GEORSS_NAMESPACE + GML_SUFFIX);
            XElement gmlPoint = new XElement(GML_NAMESPACE + POINT);
            XElement gmlPosition = new XElement(GML_NAMESPACE + GML_POSITION);
            gmlPosition.Value = point.Y + " " + point.X;

            gmlPoint.Add(gmlPosition);
            georsswrappper.Add(gmlPoint);

            _geometryAsXML = georsswrappper;

        }
    
    }

    public class GMLLineGeometry : GeoRSSGeometry 
    {

        XElement _geometryAsXML;

        public override XElement GeometryAsXML
        {

            get { return _geometryAsXML; }

        }

        public GMLLineGeometry (IGeometryCollection geometryCollection)
	    {

            XElement georsswrappper = new XElement(GEORSS_NAMESPACE + GML_SUFFIX);
            XElement gmlLineString = new XElement(GML_NAMESPACE + LINESTRING);
            XElement gmlPositionList = new XElement(GML_NAMESPACE + GML_POSITION_LIST);
            
            var coords = base.ParseGeometryCollection(geometryCollection);

            gmlPositionList.Value  = string.Join(" ", coords.ToArray());

            gmlLineString.Add(gmlPositionList);
            georsswrappper.Add(gmlLineString);

             _geometryAsXML = georsswrappper;
	    }

    }

    public class GMLPolygonGeometry : GeoRSSGeometry 
    {
        XElement _geometryAsXML;

        public override XElement GeometryAsXML
        {

            get { return _geometryAsXML; }

        }

        public GMLPolygonGeometry(IGeometryCollection geometryCollection)
        {
            XElement georsswrappper = new XElement(GEORSS_NAMESPACE + GML_SUFFIX);
            XElement gmlPolygon = new XElement(GML_NAMESPACE + GML_POLYGON);
            XElement gmlExterior = new XElement(GML_NAMESPACE + GML_EXTERIOR);
            XElement gmlLinearRing = new XElement(GML_NAMESPACE + GML_LINEARRING);
            XElement gmlPositionList = new XElement(GML_NAMESPACE + GML_POSITION_LIST);

            var coords = base.ParseGeometryCollection(geometryCollection);

            gmlPositionList.Value = string.Join(" ", coords.ToArray());

            gmlLinearRing.Add(gmlPositionList);
            gmlExterior.Add(gmlLinearRing);
            gmlPolygon.Add(gmlExterior);
            georsswrappper.Add(gmlPolygon);

            _geometryAsXML = georsswrappper;

        }
    
    }

    public class SimplePointGeometry : GeoRSSGeometry 
    {
        XElement _geometryAsXML;

        public override XElement GeometryAsXML
        {

            get { return _geometryAsXML; }

        }

        public SimplePointGeometry(IPoint point)
        {
            _geometryAsXML = new XElement(GEORSS_NAMESPACE + POINT);
            _geometryAsXML.Value = point.Y + " " + point.X;
        }
    }

    public class SimpleLineGeometry : GeoRSSGeometry 
    {
        XElement _geometryAsXML;

        public override XElement GeometryAsXML
        {

            get { return _geometryAsXML; }

        }

        public SimpleLineGeometry(IGeometryCollection geometryCollection)
        {
            _geometryAsXML = new XElement(GEORSS_NAMESPACE + LINE);

            var coords = base.ParseGeometryCollection(geometryCollection);

            _geometryAsXML.Value = string.Join(" ", coords.ToArray());


        }
    }

    public class SimplePolygonGeometry : GeoRSSGeometry 
    {
        XElement _geometryAsXML;

        public override XElement GeometryAsXML
        {

            get { return _geometryAsXML; }

        }

        public SimplePolygonGeometry(IGeometryCollection geometryCollection)
        {
            _geometryAsXML = new XElement(GEORSS_NAMESPACE + POLYGON);

            var coords = base.ParseGeometryCollection(geometryCollection);

            _geometryAsXML.Value = string.Join(" ", coords.ToArray());
        }

    }

    public class GMLGeometryFactory : GeoRSSGeometryFactory
    {

        public override GeoRSSGeometry GetGeometry(IGeometry esriGeometry)
        {
            GeoRSSGeometry geometry = null;

            string error = string.Format("A GML GeoRSS Geometry of type {0} cannot be created", Enum.GetName(typeof(esriGeometryType), esriGeometry.GeometryType));

            switch (esriGeometry.GeometryType)
            {
                case esriGeometryType.esriGeometryAny:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryBag:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryBezier3Curve:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryCircularArc:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryEllipticArc:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryEnvelope:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryLine:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryMultiPatch:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryMultipoint:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryNull:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryPath:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryPoint:
                    IPoint point = esriGeometry as Point;
                    geometry = new GMLPointGeometry(point);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    IGeometryCollection polygons = esriGeometry as IGeometryCollection;
                    geometry = new GMLPolygonGeometry(polygons);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    IGeometryCollection polylines = esriGeometry as IGeometryCollection;
                    geometry = new GMLLineGeometry(polylines);
                    break;
                case esriGeometryType.esriGeometryRay:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryRing:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometrySphere:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryTriangleFan:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryTriangleStrip:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryTriangles:
                    throw new ArgumentOutOfRangeException(error);
                default:
                    throw new ArgumentOutOfRangeException(error);
            }

            return geometry;


        }

    }

    public class SimpleGeometryFactory : GeoRSSGeometryFactory
    {
        public override GeoRSSGeometry GetGeometry(IGeometry esriGeometry)
        {
            GeoRSSGeometry geometry = null;

            string error = string.Format("A Simple GeoRSS Geometry of type {0} cannot be created", Enum.GetName(typeof(esriGeometryType), esriGeometry.GeometryType));

            switch (esriGeometry.GeometryType)
            {
                case esriGeometryType.esriGeometryAny:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryBag:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryBezier3Curve:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryCircularArc:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryEllipticArc:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryEnvelope:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryLine:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryMultiPatch:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryMultipoint:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryNull:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryPath:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryPoint:
                    IPoint point = esriGeometry as IPoint;
                    geometry = new SimplePointGeometry(point);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    IGeometryCollection polygons = esriGeometry as IGeometryCollection;
                    geometry = new SimplePolygonGeometry(polygons);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    IGeometryCollection polylines = esriGeometry as IGeometryCollection;
                    geometry = new SimpleLineGeometry(polylines);
                    break;
                case esriGeometryType.esriGeometryRay:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryRing:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometrySphere:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryTriangleFan:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryTriangleStrip:
                    throw new ArgumentOutOfRangeException(error);
                case esriGeometryType.esriGeometryTriangles:
                    throw new ArgumentOutOfRangeException(error);
                default:
                    throw new ArgumentOutOfRangeException(error);
            }

            return geometry;


        }
    }

}
