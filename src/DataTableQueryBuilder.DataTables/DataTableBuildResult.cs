using System;
using System.Linq;
using System.Collections.Generic;

using AutoMapper;

namespace DataTableQueryBuilder.DataTables
{
    /// <summary>
    /// Represents a DataTables build result.
    /// </summary>
    public class DataTableBuildResult<TDataTableFields, TEntity> : QueryBuildResult<TDataTableFields, TEntity>
    {
        private readonly DataTableRequest request;

        /// <summary>
        /// Creates a new DataTables build result.
        /// </summary>
        /// <param name="totalRecords">Total records in the data source.</param>
        /// <param name="totalRecordsFiltered">Total records filtered from the data source.</param>
        /// <param name="buildedQuery">Builded LINQ query.</param>
        /// <param name="request">DataTables request.</param>
        public DataTableBuildResult(int totalRecords, int totalRecordsFiltered, IQueryable<TEntity> buildedQuery, DataTableRequest request) : base(totalRecords, totalRecordsFiltered, buildedQuery)
        {
            this.request = request;
        }

        /// <summary>
        /// Creates data table response.
        /// </summary>
        /// <param name="mapper">An AutoMapper instance.</param>
        /// <returns>Data table response.</returns>
        public IDataTableResponse CreateResponse(IMapper mapper)
        {
            return CreateResponse(mapper, null, null);
        }

        /// <summary>
        /// Creates data table response.
        /// </summary>
        /// <param name="mapper">An AutoMapper instance.</param>
        /// <param name="mappingOptions">Options for a map operation.</param>
        /// <returns>Data table response.</returns>
        public IDataTableResponse CreateResponse(IMapper mapper, Action<IMappingOperationOptions>? mappingOptions)
        {
            return CreateResponse(mapper, mappingOptions, null);
        }

        /// <summary>
        /// Creates data table response.
        /// </summary>
        /// <param name="mapper">An AutoMapper instance.</param>
        /// <param name="responseAdditionalParams">Additional parameters to be added to data table response.</param>
        /// <returns>Data table response.</returns>
        public IDataTableResponse CreateResponse(IMapper mapper, Dictionary<string, object>? responseAdditionalParams)
        {
            return CreateResponse(mapper, null, responseAdditionalParams);
        }

        /// <summary>
        /// Creates data table response.
        /// </summary>
        /// <param name="mapper">An AutoMapper instance.</param>
        /// <param name="mappingOptions">Options for a map operation.</param>
        /// <param name="responseAdditionalParams">Additional parameters to be added to data table response.</param>
        /// <returns>Data table response.</returns>
        public IDataTableResponse CreateResponse(IMapper mapper, Action<IMappingOperationOptions>? mappingOptions, Dictionary<string, object>? responseAdditionalParams)
        {
            var model = mappingOptions == null ? mapper.Map<IEnumerable<TDataTableFields>>(BuildedQuery) : mapper.Map<IEnumerable<TDataTableFields>>(BuildedQuery, mappingOptions);

            return new DataTableResponse(request, TotalRecords, TotalRecordsFiltered, model, responseAdditionalParams);
        }
    }

    /// <summary>
    /// Represents a DataTables build result.
    /// </summary>
    public class DataTablesBuildResult<TEntity> : QueryBuildResult<TEntity, TEntity>
    {
        private readonly DataTableRequest request;

        /// <summary>
        /// Creates a new DataTables build result.
        /// </summary>
        /// <param name="totalRecords">Total records in the data source.</param>
        /// <param name="totalRecordsFiltered">Total records filtered from the data source.</param>
        /// <param name="buildedQuery">Builded LINQ query.</param>
        /// <param name="request">DataTables request.</param>
        public DataTablesBuildResult(int totalRecords, int totalRecordsFiltered, IQueryable<TEntity> buildedQuery, DataTableRequest request) : base(totalRecords, totalRecordsFiltered, buildedQuery)
        {
            this.request = request;
        }

        /// <summary>
        /// Creates data table response.
        /// </summary>
        /// <returns>Data table response.</returns>
        public IDataTableResponse CreateResponse()
        {
            return CreateResponse(null);
        }

        /// <summary>
        /// Creates data table response.
        /// </summary>
        /// <param name="responseAdditionalParams">Additional parameters to be added to data table response.</param>
        /// <returns>Data table response.</returns>
        public IDataTableResponse CreateResponse(Dictionary<string, object>? responseAdditionalParams)
        {
            return new DataTableResponse(request, TotalRecords, TotalRecordsFiltered, BuildedQuery, responseAdditionalParams);
        }
    }
}