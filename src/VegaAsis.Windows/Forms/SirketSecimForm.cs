using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;

namespace VegaAsis.Windows.Forms
{
    public class SirketSecimForm : Form
    {
        private Panel _topPanel;
        private TextBox _txtArama;
        private Label _lblPlaceholder;
        private ListBox _lstSirketler;
        private Panel _bottomPanel;
        private Button _btnSec;
        private Button _btnIptal;

        private readonly List<string> _allCompanies;
        private List<string> _filteredCompanies;
        private readonly ICompanySettingsService _companySettingsService;

        public string SelectedCompany
        {
            get
            {
                if (_lstSirketler.SelectedItem != null)
                {
                    return _lstSirketler.SelectedItem.ToString();
                }
                return null;
            }
        }

        public SirketSecimForm(ICompanySettingsService companySettingsService = null)
        {
            _companySettingsService = companySettingsService;

            _allCompanies = new List<string>
            {
                "ACIBADEM", "ACNTURK", "ANA", "ANKARA", "AREX", "ATLAS", "ATLAS_MUTUEL",
                "AVEON", "BEREKET", "BEREKET_KAPSUL", "CORPUS_PORTAL", "DEMIROFFICE",
                "EMAA", "EMAA_TALEP", "ETHICA", "EUREKO", "FIBA", "FIBA_EMEKLILIK",
                "GIG", "GIG_SFS", "GROUPAMA", "HDI", "HDI_KATILIM", "HDI_PLUS",
                "KATILIM_SAGLIK", "KORU", "MAGDEBURGERPORTAL", "MAGDEBURGERSFS",
                "NEOVA", "QUICK", "RAY", "SOMPO", "TURK_NIPPON", "UNICO", "ZURICH"
            };

            _filteredCompanies = new List<string>(_allCompanies);

            InitializeComponent();
            Load += SirketSecimForm_Load;
        }

        private async void SirketSecimForm_Load(object sender, EventArgs e)
        {
            if (_companySettingsService != null)
            {
                await LoadCompaniesFromService();
            }
        }

        private async Task LoadCompaniesFromService()
        {
            try
            {
                var companies = await _companySettingsService.GetSelectedCompaniesAsync();
                if (companies != null && companies.Count > 0)
                {
                    var distinct = companies.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(c => c).ToList();
                    _allCompanies.Clear();
                    _allCompanies.AddRange(distinct);
                    _filteredCompanies = new List<string>(_allCompanies);
                    UpdateCompanyList();
                }
            }
            catch
            {
            }
        }

        private void InitializeComponent()
        {
            Text = "Fiyat için Şirket Seçiniz";
            Size = new Size(400, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Üst Panel (Arama)
            _topPanel = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                Padding = new Padding(10, 10, 10, 5)
            };

            var searchPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 22
            };

            _txtArama = new TextBox
            {
                Dock = DockStyle.Fill,
                Height = 22
            };
            _txtArama.TextChanged += TxtArama_TextChanged;
            _txtArama.GotFocus += TxtArama_GotFocus;
            _txtArama.LostFocus += TxtArama_LostFocus;

            _lblPlaceholder = new Label
            {
                Text = "Şirket ara...",
                ForeColor = Color.Gray,
                BackColor = SystemColors.Window,
                AutoSize = false,
                Location = new Point(3, 3),
                Size = new Size(200, 20),
                Cursor = Cursors.IBeam
            };
            _lblPlaceholder.Click += (s, e) => _txtArama.Focus();

            searchPanel.Controls.Add(_txtArama);
            searchPanel.Controls.Add(_lblPlaceholder);
            searchPanel.Controls.SetChildIndex(_lblPlaceholder, 0);

            _topPanel.Controls.Add(searchPanel);

            // ListBox
            _lstSirketler = new ListBox
            {
                Dock = DockStyle.Fill,
                SelectionMode = SelectionMode.One
            };
            UpdateCompanyList();

            // Alt Panel (Butonlar)
            _bottomPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom,
                Padding = new Padding(10)
            };

            _btnSec = new Button
            {
                Text = "Seç",
                DialogResult = DialogResult.OK,
                Width = 80,
                Height = 30
            };
            _btnSec.Click += BtnSec_Click;

            _btnIptal = new Button
            {
                Text = "İptal",
                DialogResult = DialogResult.Cancel,
                Width = 80,
                Height = 30
            };

            _bottomPanel.Controls.Add(_btnSec);
            _bottomPanel.Controls.Add(_btnIptal);
            
            // Butonları konumlandır
            void PositionButtons(object s, EventArgs e)
            {
                _btnSec.Location = new Point(_bottomPanel.Width - _btnSec.Width - 10, (_bottomPanel.Height - _btnSec.Height) / 2);
                _btnIptal.Location = new Point(_btnSec.Left - _btnIptal.Width - 10, (_bottomPanel.Height - _btnIptal.Height) / 2);
            }
            
            _bottomPanel.Layout += PositionButtons;
            PositionButtons(null, EventArgs.Empty);

            // Ana Panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            mainPanel.Controls.Add(_lstSirketler);
            mainPanel.Controls.Add(_topPanel);
            mainPanel.Controls.Add(_bottomPanel);
            mainPanel.Controls.SetChildIndex(_topPanel, 0);
            mainPanel.Controls.SetChildIndex(_bottomPanel, 1);
            mainPanel.Controls.SetChildIndex(_lstSirketler, 2);

            Controls.Add(mainPanel);

            AcceptButton = _btnSec;
            CancelButton = _btnIptal;
        }

        private void TxtArama_GotFocus(object sender, EventArgs e)
        {
            _lblPlaceholder.Visible = false;
        }

        private void TxtArama_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_txtArama.Text))
            {
                _lblPlaceholder.Visible = true;
            }
        }

        private void TxtArama_TextChanged(object sender, EventArgs e)
        {
            // Placeholder görünürlüğünü güncelle
            _lblPlaceholder.Visible = string.IsNullOrEmpty(_txtArama.Text);

            var searchText = _txtArama.Text?.Trim().ToUpperInvariant();
            
            if (string.IsNullOrEmpty(searchText))
            {
                _filteredCompanies = new List<string>(_allCompanies);
            }
            else
            {
                _filteredCompanies = _allCompanies
                    .Where(c => c.ToUpperInvariant().Contains(searchText))
                    .ToList();
            }

            UpdateCompanyList();
        }

        private void UpdateCompanyList()
        {
            _lstSirketler.BeginUpdate();
            _lstSirketler.Items.Clear();
            foreach (var company in _filteredCompanies)
            {
                _lstSirketler.Items.Add(company);
            }
            _lstSirketler.EndUpdate();
        }

        private void BtnSec_Click(object sender, EventArgs e)
        {
            if (_lstSirketler.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir şirket seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
            }
        }
    }
}
