using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace DataTableQueryBuilder.Generic
{
    /// <summary>
    /// Represents a Generic response.
    /// </summary>
    public class DataTableResponse : IDataTableResponse
    {
        /// <summary>
        /// Defines the result content type.
        /// </summary>
        private static readonly string ContentType = "application/json; charset={0}";

        /// <summary>
        /// Defines the result enconding.
        /// </summary>
        private static readonly Encoding ContentEncoding = System.Text.Encoding.UTF8;

        /// <summary>
        /// Gets request for validation.
        /// </summary>
        public DataTableRequest Request { get; protected set; }

        /// <summary>
        /// Gets error message, if not successful.
        /// Should only be available for Generic 1.10 and above.
        /// </summary>
        public string? Error { get; protected set; }

        /// <summary>
        /// Gets total record count (total records available on database).
        /// </summary>
        public int TotalRecords { get; protected set; }
        
        /// <summary>
        /// Gets filtered record count (total records available after filtering).
        /// </summary>
        public int TotalRecordsFiltered { get; protected set; }
        
        /// <summary>
        /// Gets data object (collection).
        /// </summary>
        public object? Data { get; protected set; }
        
        /// <summary>
        /// Gets aditional parameters for response.
        /// </summary>
        public IDictionary<string, object>? AdditionalParameters { get; protected set; }

        /// <summary>
        /// Creates a new response instance.
        /// </summary>
        /// <param name="request">Generic request object.</param>
        /// <param name="errorMessage">Error message.</param>
        protected DataTableResponse(DataTableRequest request, string errorMessage, IDictionary<string, object>? additionalParameters)
        {
            Request = request;
            Error = errorMessage;
            AdditionalParameters = additionalParameters;
        }

        /// <summary>
        /// Creates a new response instance.
        /// </summary>
        /// <param name="request">Generic request object.</param>
        /// <param name="totalRecords">Total record count (total records available on database).</param>
        /// <param name="totalRecordsFiltered">Filtered record count (total records available after filtering).</param>
        /// <param name="additionalParameters">Aditional parameters for response.</param>
        /// <param name="data">Data object (collection).</param>
        public DataTableResponse(DataTableRequest request, int totalRecords, int totalRecordsFiltered, object data, IDictionary<string, object>? additionalParameters)
        {
            Request = request;
            TotalRecords = totalRecords;
            TotalRecordsFiltered = totalRecordsFiltered;
            Data = data;

            AdditionalParameters = additionalParameters;
        }

        /// <summary>
        /// Converts this object to a Json compatible response using global naming convention for parameters.
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            // When request is null, there should be no response.
            if (Request == null)
                return "";

            using var stringWriter = new System.IO.StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);

            jsonWriter.WriteStartObject();

            if (IsSuccessResponse())
            {
                // TotalRecords
                jsonWriter.WritePropertyName(ResponseNameConvention.TotalRecords, true);
                jsonWriter.WriteValue(TotalRecords);

                // TotalRecordsFiltered
                jsonWriter.WritePropertyName(ResponseNameConvention.TotalRecordsFiltered, true);
                jsonWriter.WriteValue(TotalRecordsFiltered);

                // Data
                jsonWriter.WritePropertyName(ResponseNameConvention.Data, true);
                jsonWriter.WriteRawValue(DataToJson());
            }
            else
            {
                jsonWriter.WritePropertyName(ResponseNameConvention.Error, true);
                jsonWriter.WriteValue(Error);
            }

            if (AdditionalParameters != null)
            {
                foreach (var keypair in AdditionalParameters)
                {
                    jsonWriter.WritePropertyName(keypair.Key, true);
                    jsonWriter.WriteValue(keypair.Value);
                }
            }

            jsonWriter.WriteEndObject();

            jsonWriter.Flush();

            return stringWriter.ToString();
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var httpResponse = context.HttpContext.Response;
            httpResponse.ContentType = ContentType;

            var contentBytes = ContentEncoding.GetBytes(ToJson() ?? "");

            await httpResponse.Body.WriteAsync(contentBytes, 0, contentBytes.Length);
        }

        private bool IsSuccessResponse()
        {
            return Data != null && String.IsNullOrWhiteSpace(Error);
        }

        private string DataToJson()
        {
            var settings = new JsonSerializerSettings() { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() };
            return JsonConvert.SerializeObject(Data, settings);
        }
    }
}
