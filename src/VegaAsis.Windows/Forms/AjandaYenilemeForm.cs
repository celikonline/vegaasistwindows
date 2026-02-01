using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;

namespace VegaAsis.Windows.Forms
{
    public class AjandaYenilemeForm : Form
    {
        private readonly IAppointmentService _appointmentService;
        private DataGridView _grid;
        private Button _btnYenile;

        public AjandaYenilemeForm(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
            Text = "Ajanda";
            Size = new Size(900, 500);
            StartPosition = FormStartPosition.CenterParent;

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            _btnYenile = new Button { Text = "Yenile", Dock = DockStyle.Top, Height = 36 };
            _btnYenile.Click += (s, e) => _ = LoadAppointmentsAsync();

            var panel = new Panel { Dock = DockStyle.Fill };
            panel.Controls.Add(_grid);
            panel.Controls.Add(_btnYenile);
            Controls.Add(panel);
            Load += (s, e) => _ = LoadAppointmentsAsync();
        }

        private async Task LoadAppointmentsAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var list = await _appointmentService.GetAllAsync(null).ConfigureAwait(true);
                _grid.DataSource = null;
                _grid.Columns.Clear();
                if (list != null && list.Count > 0)
                {
                    _grid.DataSource = list;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Randevular yüklenirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
