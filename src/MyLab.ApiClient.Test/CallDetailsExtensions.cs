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
        /// <param name="call">web call</param>
        /// <param name="contractType">API contract</param>
        /// <returns>string dump</returns>
        public static string ToTestDump(this CallDetails call, Type contractType = null)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));

            var sb = new StringBuilder();

            WriteToString(call, sb, contractType);
            
            return sb.ToString();
        }

        static void WriteToString(CallDetails call, StringBuilder sb, Type contractType)
        {
            var contractName = GetContractName(contractType);

            Append(sb, "");
            Append(sb, $"===== REQUEST BEGIN ({contractName}) =====");
            Append(sb, "");
            Append(sb, call.RequestDump);
            Append(sb, "===== REQUEST END =====");
            Append(sb, "");
            Append(sb, $"===== RESPONSE BEGIN ({contractName}) =====");
            Append(sb, "");
            Append(sb, call.ResponseDump);
            Append(sb, "===== RESPONSE END =====");
        }

        static void Append(StringBuilder sb, string str)
        {
            sb.Append(str + "\r\n");
        }

        static string GetContractName(Type contractType) => contractType != null
            ? contractType.Name
            : "[undefined contract]";
    }
}