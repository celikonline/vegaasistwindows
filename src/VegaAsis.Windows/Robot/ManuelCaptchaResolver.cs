using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Windows.Forms;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Manuel captcha çözümü: kullanıcıya ManuelCaptchaForm gösterir; girilen metni döndürür. ICaptchaResolver implementasyonu.
    /// </summary>
    public class ManuelCaptchaResolver : ICaptchaResolver
    {
        private readonly IWin32Window _owner;

        public ManuelCaptchaResolver(IWin32Window owner = null)
        {
            _owner = owner;
        }

        public Task<string> SolveAsync(byte[] captchaImageBytes)
        {
            if (captchaImageBytes == null || captchaImageBytes.Length == 0)
                return Task.FromResult<string>(null);
            return SolveWithFormAsync(captchaImageBytes);
        }

        public Task<string> SolveAsync(string imageBase64)
        {
            if (string.IsNullOrWhiteSpace(imageBase64))
                return Task.FromResult<string>(null);
            try
            {
                var bytes = Convert.FromBase64String(imageBase64.Trim());
                return SolveWithFormAsync(bytes);
            }
            catch
            {
                return Task.FromResult<string>(null);
            }
        }

        private Task<string> SolveWithFormAsync(byte[] imageBytes)
        {
            var ctx = SynchronizationContext.Current;
            var tcs = new TaskCompletionSource<string>();
            if (ctx != null)
            {
                ctx.Post(_ =>
                {
                    try
                    {
                        string result = null;
                        using (var form = new ManuelCaptchaForm())
                        {
                            form.SetImage(imageBytes);
                            if ((_owner != null ? form.ShowDialog(_owner as Form) : form.ShowDialog()) == DialogResult.OK)
                                result = form.SolutionText;
                        }
                        tcs.TrySetResult(result);
                    }
                    catch
                    {
                        tcs.TrySetResult(null);
                    }
                }, null);
            }
            else
            {
                try
                {
                    string result = null;
                    using (var form = new ManuelCaptchaForm())
                    {
                        form.SetImage(imageBytes);
                        if ((_owner != null ? form.ShowDialog(_owner as Form) : form.ShowDialog()) == DialogResult.OK)
                            result = form.SolutionText;
                    }
                    tcs.TrySetResult(result);
                }
                catch
                {
                    tcs.TrySetResult(null);
                }
            }
            return tcs.Task;
        }
    }
}
