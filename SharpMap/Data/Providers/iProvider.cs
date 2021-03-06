// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Threading;
using GeoAPI.Features;
using GeoAPI.Geometries;
using IGeometry = GeoAPI.Geometries.IGeometry;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// Interface for data providers
    /// </summary>
    public interface IProvider : IDisposable
    {
        /// <summary>
        /// Gets the connection ID of the datasource
        /// </summary>
        /// <remarks>
        /// <para>The ConnectionID should be unique to the datasource (for instance the filename or the
        /// connectionstring), and is meant to be used for connection pooling.</para>
        /// <para>If connection pooling doesn't apply to this datasource, the ConnectionID should return String.Empty</para>
        /// </remarks>
        string ConnectionID { get; }

        /// <summary>
        /// Returns true if the datasource is currently open
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// The spatial reference ID (CRS)
        /// </summary>
        int SRID { get; set; }

        /// <summary>
        /// Gets the features within the specified <see cref="GeoAPI.Geometries.Envelope"/>
        /// </summary>
        /// <param name="extent">The extent to be queried</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A stream of geometries of features within the specified <see cref="GeoAPI.Geometries.Envelope"/></returns>
        IEnumerable<IGeometry> GetGeometriesInView(Envelope extent, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns all objects whose <see cref="GeoAPI.Geometries.Envelope"/> intersects 'bbox'.
        /// </summary>
        /// <remarks>
        /// This method is usually much faster than the QueryFeatures method, because intersection tests
        /// are performed on objects simplified by their <see cref="GeoAPI.Geometries.Envelope"/>, and using the Spatial Index
        /// </remarks>
        /// <param name="extent">The extent to be queried</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>Object ids of features</returns>
        IEnumerable<object> GetOidsInView(Envelope extent, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns the geometry corresponding to the Object ID
        /// </summary>
        /// <param name="oid">The object Id</param>
        /// <returns>geometry</returns>
        IGeometry GetGeometryByOid(object oid);

        /// <summary>
        /// Returns the data associated with all the geometries that are intersected by 'geom'
        /// </summary>
        /// <param name="geom">Geometry to intersect with</param>
        /// <param name="ds">FeatureDataSet to fill data into</param>
        /// <param name="cancellationToken">A cancellation token</param>
        void ExecuteIntersectionQuery(IGeometry geom, IFeatureCollectionSet ds, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns the data associated with all the geometries that are intersected by 'geom'
        /// </summary>
        /// <param name="box">Envelope to intersect with</param>
        /// <param name="ds">FeatureDataSet to fill data into</param>
        /// <param name="cancellationToken">A cancellation token</param>
        void ExecuteIntersectionQuery(Envelope box, IFeatureCollectionSet ds, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns the number of features in the dataset
        /// </summary>
        /// <returns>number of features</returns>
        int GetFeatureCount();

        /// <summary>
        /// Returns a <see cref="GeoAPI.Features.IFeature"/> based on its unique <paramref name="oid">object id</paramref>
        /// </summary>
        /// <param name="oid">The object id of the feature to get.</param>
        /// <returns>A feature</returns>
        IFeature GetFeatureByOid(object oid);

        /// <summary>
        /// <see cref="Envelope"/> of dataset
        /// </summary>
        /// <returns>The 2-dimensional extent of the layer</returns>
        Envelope GetExtents();

        /// <summary>
        /// Opens the datasource
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the datasource
        /// </summary>
        void Close();
    }

    /// <summary>
    /// Interface for specific provider implementations
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity</typeparam>
    public interface IProvider<TEntity> : IProvider 
        where TEntity : IComparable<TEntity>, IEquatable<TEntity>
    {
        /// <summary>
        /// Returns all feature identifieres of features whose <see cref="GeoAPI.Geometries.Envelope"/> intersects <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The envelope that objects should intersect</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A stream of identifiers</returns>
        IEnumerable<TEntity> GetFidsInView(Envelope view, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns the geometry of a feature corresponding to the feature's identifier <paramref name="fid"/>
        /// </summary>
        /// <param name="fid">The identifier of the feature to get the geometry from</param>
        /// <returns>A geometry, if the feature exists, otherwise implementiation should throw <see cref="ArgumentOutOfRangeException"/>.</returns>
        IGeometry GetGeometryByFid(TEntity fid);

        /// <summary>
        /// Returns the geometry corresponding to the Object ID
        /// </summary>
        /// <param name="view">The view</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A stream of features</returns>
        IFeature<TEntity> GetFeaturesInView(Envelope view, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns the feature corresponding to the feature's identifier <paramref name="fid"/>
        /// </summary>
        /// <param name="fid">The identifier of the feature to get the geometry from</param>
        /// <returns>A feature, if one exists, otherwise implementiation should throw <see cref="ArgumentOutOfRangeException"/>.</returns>
        IFeature<TEntity> GetFeatureByFid(TEntity fid);
    }
}