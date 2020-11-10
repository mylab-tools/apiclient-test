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
        /// <typeparam name="TContract">API contract</typeparam>
        /// <param name="call">web call</param>
        /// <returns>string dump</returns>
        public static string ToTestDump<TRes, TContract>(this CallDetails<TRes> call)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));

            var sb = new StringBuilder();

            WriteToString<TRes, TContract>(call, sb);
            
            return sb.ToString();
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
            if (call == null) throw new ArgumentNullException(nameof(call));

            var sb = new StringBuilder();

            WriteToString<TRes, TContract>(call, sb);

            if (call.ResponseProcessingError)
            {
                sb.AppendLine("");
                sb.AppendLine($"===== RESPONSE PROC ERROR ({typeof(TContract).Name}) =====");
                sb.AppendLine("");
                sb.AppendLine(call.ProcessingError.ToString());
                sb.AppendLine("===== RESPONSE PROC ERROR END =====");
            }

            return sb.ToString();
        }

        static void WriteToString<TRes, TContract>(CallDetails<TRes> call, StringBuilder sb)
        {
            sb.AppendLine("");
            sb.AppendLine($"===== REQUEST BEGIN ({typeof(TContract).Name}) =====");
            sb.AppendLine("");
            sb.AppendLine(call.RequestDump);
            sb.AppendLine("===== REQUEST END =====");
            sb.AppendLine("");
            sb.AppendLine($"===== RESPONSE BEGIN ({typeof(TContract).Name}) =====");
            sb.AppendLine("");
            sb.AppendLine(call.ResponseDump);
            sb.AppendLine("===== RESPONSE END =====");
        }
    }
}