using System;
using System.Linq;
using System.Collections.Generic;

using AutoMapper;

namespace DataTableQueryBuilder
{
    public abstract class BuildResult<TDataTableFields, TEntity>
    {
        /// <summary>
        /// Gets the total number of records without filtering applied.
        /// </summary>
        protected internal int TotalRecords { get; private set; }

        /// <summary>
        /// Gets the total number of records with filtering applied.
        /// </summary>
        protected internal int TotalRecordsFiltered { get; private set; }

        /// <summary>
        /// Gets the builded query with specified searching and sorting configuration applied.
        /// </summary>
        protected internal IQueryable<TEntity> BuildedQuery { get; private set; }

        public BuildResult(int totalRecords, int totalRecordsFiltered, IQueryable<TEntity> buildedQuery)
        {
            TotalRecords = totalRecords;
            TotalRecordsFiltered = totalRecordsFiltered;
            BuildedQuery = buildedQuery;
        }

        /// <summary>
        /// Maps builded query to data table response.
        /// </summary>
        /// <param name="mapper">An AutoMapper instance.</param>
        /// <returns>Data table response.</returns>
        public IDataTableResponse MapToResponse(IMapper mapper)
        {
            return MapToResponse(mapper, null, null);
        }

        /// <summary>
        /// Maps builded query to data table response.
        /// </summary>
        /// <param name="mapper">An AutoMapper instance.</param>
        /// <param name="mappingOptions">Options for a map operation.</param>
        /// <returns>Data table response.</returns>
        public IDataTableResponse MapToResponse(IMapper mapper, Action<IMappingOperationOptions>? mappingOptions)
        {
            return MapToResponse(mapper, mappingOptions, null);
        }

        /// <summary>
        /// Maps builded query to data table response.
        /// </summary>
        /// <param name="mapper">An AutoMapper instance.</param>
        /// <param name="responseAdditionalParams">Additional parameters to be added to data table response.</param>
        /// <returns>Data table response.</returns>
        public IDataTableResponse MapToResponse(IMapper mapper, Dictionary<string, object>? responseAdditionalParams)
        {
            return MapToResponse(mapper, null, responseAdditionalParams);
        }

        /// <summary>
        /// Maps builded query to data table response.
        /// </summary>
        /// <param name="mapper">An AutoMapper instance.</param>
        /// <param name="mappingOptions">Options for a map operation.</param>
        /// <param name="responseAdditionalParams">Additional parameters to be added to data table response.</param>
        /// <returns>Data table response.</returns>
        public IDataTableResponse MapToResponse(IMapper mapper, Action<IMappingOperationOptions>? mappingOptions, Dictionary<string, object>? responseAdditionalParams)
        {
            var model = mappingOptions == null ? mapper.Map<IEnumerable<TDataTableFields>>(BuildedQuery) : mapper.Map<IEnumerable<TDataTableFields>>(BuildedQuery, mappingOptions);

            return CreateResponse(model, responseAdditionalParams);
        }

        protected abstract IDataTableResponse CreateResponse(IEnumerable<TDataTableFields> model,  Dictionary<string, object>? responseAdditionalParams);
    }
}