using System;

namespace XtraUpload.Domain
{
    /// <summary>
    /// The OperationResult class contains the success or failure result returned by a service class.
    /// </summary>
    public class OperationResult
    {
        ErrorContent _ErrorContent = new ErrorContent(null, ErrorOrigin.None);
        /// <summary>
        /// Gets a value indicating whether the service call was successfull or not.
        /// </summary>
        /// <value>
        public OperationState State { get; private set; } = OperationState.Success;

        /// <summary>
        /// Gets the error content information.
        /// </summary>
        public ErrorContent ErrorContent 
        {
            get { return _ErrorContent; }
            set 
            {
                State = value.ErrorType != ErrorOrigin.None ? OperationState.Failed : OperationState.Success;
                _ErrorContent = value;
            } 
        }

        /// <summary>
        /// Creates an operation result based on an existing operation result. 
        /// </summary>
        public static T CopyResult<T>(OperationResult result) where T: OperationResult, new()
        {
            return new T() { State = result.State, ErrorContent = new ErrorContent(result.ErrorContent) };
        }
    }
}
