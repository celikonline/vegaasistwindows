using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// 2Captcha.com API v2 (createTask / getTaskResult) ile captcha çözümü. API key AppSettings (CaptchaApiKey) veya constructor ile verilir.
    /// </summary>
    public class TwoCaptchaResolver : ICaptchaResolver
    {
        private const string CreateTaskUrl = "https://api.2captcha.com/createTask";
        private const string GetResultUrl = "https://api.2captcha.com/getTaskResult";
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public TwoCaptchaResolver(string apiKey)
        {
            _apiKey = apiKey ?? string.Empty;
            _httpClient = new HttpClient();
        }

        public Task<string> SolveAsync(byte[] captchaImageBytes)
        {
            if (captchaImageBytes == null || captchaImageBytes.Length == 0)
                return Task.FromResult<string>(null);
            var base64 = Convert.ToBase64String(captchaImageBytes);
            return SolveAsync(base64);
        }

        public async Task<string> SolveAsync(string imageBase64)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                return null;
            if (string.IsNullOrWhiteSpace(imageBase64))
                return null;

            var createJson = "{\"clientKey\":\"" + EscapeJson(_apiKey) + "\",\"task\":{\"type\":\"ImageToTextTask\",\"body\":\"" + EscapeJson(imageBase64) + "\"}}";
            HttpResponseMessage createResp;
            try
            {
                createResp = await _httpClient.PostAsync(CreateTaskUrl, new StringContent(createJson, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }

            var createText = await createResp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var taskId = ParseTaskId(createText);
            if (string.IsNullOrEmpty(taskId))
                return null;

            for (int i = 0; i < 24; i++)
            {
                await Task.Delay(5000).ConfigureAwait(false);
                var resultJson = "{\"clientKey\":\"" + EscapeJson(_apiKey) + "\",\"taskId\":" + taskId + "}";
                var resultResp = await _httpClient.PostAsync(GetResultUrl, new StringContent(resultJson, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                var resultText = await resultResp.Content.ReadAsStringAsync().ConfigureAwait(false);
                var solution = ParseSolution(resultText);
                if (solution != null)
                    return solution;
                if (resultText != null && resultText.IndexOf("processing", StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;
                return null;
            }
            return null;
        }

        private static string EscapeJson(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
        }

        private static string ParseTaskId(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            var key = "\"taskId\":";
            var idx = json.IndexOf(key, StringComparison.Ordinal);
            if (idx < 0) return null;
            idx += key.Length;
            var end = json.IndexOfAny(new[] { ',', '}' }, idx);
            if (end < 0) end = json.Length;
            return json.Substring(idx, end - idx).Trim();
        }

        private static string ParseSolution(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            if (json.IndexOf("\"status\":\"ready\"", StringComparison.OrdinalIgnoreCase) < 0) return null;
            var key = "\"text\":\"";
            var idx = json.IndexOf(key, StringComparison.Ordinal);
            if (idx < 0) return null;
            idx += key.Length;
            var end = idx;
            while (end < json.Length && json[end] != '"' && (json[end] != '\\' || end + 1 < json.Length))
                end++;
            return json.Substring(idx, end - idx).Replace("\\\"", "\"");
        }
    }
}
