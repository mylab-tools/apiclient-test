using System;
using System.Text;

namespace MyLab.ApiClient.Test
{
    /// <summary>
    /// Provides extension methods for <see cref="CallDetails{T}"/> and <see cref="TestCallDetails{T}"/>
    /// </summary>
    public static class CallDetailsExtensions
    {
        /// <summary>
        /// Gets web-call dump for test output 
        /// </summary>
        /// <typeparam name="TRes">call result</typeparam>
        /// <param name="call">web call</param>
        /// <param name="contractType">API contract</param>
        /// <returns>string dump</returns>
        public static string ToTestDump<TRes>(this CallDetails<TRes> call, Type contractType = null)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));

            var sb = new StringBuilder();

            WriteToString(call, sb, contractType);
            
            return sb.ToString();
        }

        /// <summary>
        /// Gets web-call dump for test output 
        /// </summary>
        /// <typeparam name="TRes">call result</typeparam>
        /// <typeparam name="TContract">API contract</typeparam>
        /// <param name="call">web call</param>
        /// <returns>string dump</returns>
        public static string ToTestDump<TRes, TContract>(this CallDetails<TRes> call)
        {
            return ToTestDump(call, typeof(TContract));
        }

        /// <summary>
        /// Gets web-call dump for test output 
        /// </summary>
        /// <typeparam name="TRes">call result</typeparam>
        /// <typeparam name="TContract">API contract</typeparam>
        /// <param name="call">web call</param>
        /// <returns>string dump</returns>
        public static string ToTestDump<TRes, TContract>(this TestCallDetails<TRes> call)
        {
            return ToTestDump(call, typeof(TContract));
        }

        /// <summary>
        /// Gets web-call dump for test output 
        /// </summary>
        /// <typeparam name="TRes">call result</typeparam>
        /// <param name="contractType">API contract</param>
        /// <param name="call">web call</param>
        /// <returns>string dump</returns>
        public static string ToTestDump<TRes>(this TestCallDetails<TRes> call, Type contractType = null)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));

            var sb = new StringBuilder();

            WriteToString<TRes>(call, sb, contractType);

            if (call.ResponseProcessingError)
            {
                var contractName = GetContractName(contractType);

                sb.AppendLine("");
                sb.AppendLine($"===== RESPONSE PROC ERROR ({contractName}) =====");
                sb.AppendLine("");
                sb.AppendLine(call.ProcessingError.ToString());
                sb.AppendLine("===== RESPONSE PROC ERROR END =====");
            }

            return sb.ToString();
        }

        static void WriteToString<TRes>(CallDetails<TRes> call, StringBuilder sb, Type contractType)
        {
            var contractName = GetContractName(contractType);

            sb.AppendLine("");
            sb.AppendLine($"===== REQUEST BEGIN ({contractName}) =====");
            sb.AppendLine("");
            sb.AppendLine(call.RequestDump);
            sb.AppendLine("===== REQUEST END =====");
            sb.AppendLine("");
            sb.AppendLine($"===== RESPONSE BEGIN ({contractName}) =====");
            sb.AppendLine("");
            sb.AppendLine(call.ResponseDump);
            sb.AppendLine("===== RESPONSE END =====");
        }

        static string GetContractName(Type contractType) => contractType != null
            ? contractType.Name
            : "[undefined contract]";
    }
}