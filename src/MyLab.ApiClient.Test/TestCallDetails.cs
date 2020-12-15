using System;

namespace MyLab.ApiClient.Test
{
    /// <summary>
    /// Contains details about request and response
    /// </summary>
    /// <typeparam name="TRes">result object type</typeparam>
    public class TestCallDetails<TRes> : CallDetails<TRes>
    {
        /// <summary>
        /// Determines that there was processing result error
        /// </summary>
        public bool ResponseProcessingError { get; set; }

        /// <summary>
        /// Gets response processing exception
        /// </summary>
        public Exception ProcessingError { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TestCallDetails{TRes}"/>
        /// </summary>
        public TestCallDetails(CallDetails<TRes> baseCallDetails)
        {
            base.IsUnexpectedStatusCode = baseCallDetails.IsUnexpectedStatusCode;
            base.RequestDump = baseCallDetails.RequestDump;
            base.RequestMessage = baseCallDetails.RequestMessage;
            base.ResponseContent = baseCallDetails.ResponseContent;
            base.ResponseDump = baseCallDetails.ResponseDump;
            base.ResponseMessage = baseCallDetails.ResponseMessage;
            base.StatusCode = baseCallDetails.StatusCode;
        }
    }

    /// <summary>
    /// Contains details about request and response
    /// </summary>
    public class TestCallDetails : CallDetails
    {
        /// <summary>
        /// Determines that there was processing result error
        /// </summary>
        public bool ResponseProcessingError { get; set; }

        /// <summary>
        /// Gets response processing exception
        /// </summary>
        public Exception ProcessingError { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TestCallDetails{TRes}"/>
        /// </summary>
        public TestCallDetails(CallDetails baseCallDetails)
        {
            base.IsUnexpectedStatusCode = baseCallDetails.IsUnexpectedStatusCode;
            base.RequestDump = baseCallDetails.RequestDump;
            base.RequestMessage = baseCallDetails.RequestMessage;
            base.ResponseDump = baseCallDetails.ResponseDump;
            base.ResponseMessage = baseCallDetails.ResponseMessage;
            base.StatusCode = baseCallDetails.StatusCode;
        }
    }
}