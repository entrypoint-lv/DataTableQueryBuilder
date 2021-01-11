using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace DataTableQueryBuilder
{
    /// <summary>
    /// Represents a generic response for data table.
    /// </summary>
    public interface IDataTableResponse : IActionResult
    {
        public int TotalRecords { get; }

        /// <summary>
        /// Gets filtered record count (total records available after filtering).
        /// </summary>
        public int TotalRecordsFiltered { get; }

        /// <summary>
        /// Gets data object (collection).
        /// </summary>
        public object? Data { get; }

        /// <summary>
        /// Gets aditional parameters for response.
        /// </summary>
        public IDictionary<string, object>? AdditionalParameters { get; }
    }
}
