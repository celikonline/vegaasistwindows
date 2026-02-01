using System;
using System.Windows.Forms;
using VegaAsis.Windows;

namespace VegaAsis.Windows.Forms
{
    /// <summary>
    /// Base form class with common functionality for service resolution and error handling.
    /// </summary>
    public class BaseForm : Form
    {
        protected BaseForm()
        {
        }

        /// <summary>
        /// Resolves a service from the DI container.
        /// </summary>
        protected T Resolve<T>()
        {
            if (!ServiceLocator.IsInitialized)
            {
                throw new InvalidOperationException("ServiceLocator has not been initialized.");
            }
            return ServiceLocator.Resolve<T>();
        }

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        protected void ShowError(string message, string title = "Hata")
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows an information message to the user.
        /// </summary>
        protected void ShowInfo(string message, string title = "Bilgi")
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows a confirmation dialog.
        /// </summary>
        protected bool Confirm(string message, string title = "Onay")
        {
            return MessageBox.Show(this, message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// Executes an async action with loading state and error handling.
        /// </summary>
        protected void ExecuteAsync(System.Func<System.Threading.Tasks.Task> action)
        {
            if (action == null) return;

            Cursor = Cursors.WaitCursor;
            try
            {
                action().ContinueWith(t =>
                {
                    try
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                Cursor = Cursors.Default;
                                if (t.IsFaulted && t.Exception != null)
                                {
                                    ShowError(t.Exception.GetBaseException().Message);
                                }
                            }));
                        }
                        else
                        {
                            Cursor = Cursors.Default;
                            if (t.IsFaulted && t.Exception != null)
                            {
                                ShowError(t.Exception.GetBaseException().Message);
                            }
                        }
                    }
                    catch
                    {
                        if (InvokeRequired)
                        {
                            try { Invoke(new Action(() => Cursor = Cursors.Default)); } catch { }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                ShowError(ex.Message);
            }
        }
    }
}
