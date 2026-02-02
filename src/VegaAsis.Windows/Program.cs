using System;
using System.Windows.Forms;
using Autofac;
using VegaAsis.Core.Contracts;
using VegaAsis.Data;
using VegaAsis.Data.Services;

namespace VegaAsis.Windows
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += (s, e) =>
            {
                try
                {
                    MessageBox.Show(e.Exception?.Message ?? "Beklenmeyen hata.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch { }
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                try
                {
                    var ex = e.ExceptionObject as Exception;
                    MessageBox.Show(ex?.Message ?? "Beklenmeyen hata.", "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch { }
            };

            var builder = new ContainerBuilder();
            builder.RegisterType<VegaAsisDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<AuthService>().As<IAuthService>().SingleInstance();
            builder.RegisterType<UserManagementService>().As<IUserManagementService>().InstancePerLifetimeScope();
            builder.RegisterType<CompanySettingsService>().As<ICompanySettingsService>().InstancePerLifetimeScope();
            builder.RegisterType<OfferService>().As<IOfferService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyService>().As<IPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<AppointmentService>().As<IAppointmentService>().InstancePerLifetimeScope();
            builder.RegisterType<SharedCompanyService>().As<ISharedCompanyService>().InstancePerLifetimeScope();
            builder.RegisterType<WebUserService>().As<IWebUserService>().InstancePerLifetimeScope();
            builder.RegisterType<AppSettingsService>().As<IAppSettingsService>().InstancePerLifetimeScope();
            builder.RegisterType<AnnouncementService>().As<IAnnouncementService>().InstancePerLifetimeScope();
            builder.RegisterType<DaskService>().As<IDaskService>().InstancePerLifetimeScope();
            builder.RegisterType<KaskoService>().As<IKaskoService>().InstancePerLifetimeScope();
            builder.RegisterType<ImmService>().As<IImmService>().InstancePerLifetimeScope();
            builder.RegisterType<KonutService>().As<IKonutService>().InstancePerLifetimeScope();
            builder.RegisterType<TssService>().As<ITssService>().InstancePerLifetimeScope();
            builder.RegisterType<TramerService>().As<ITramerService>().InstancePerLifetimeScope();
            builder.RegisterType<UavtService>().As<IUavtService>().InstancePerLifetimeScope();
            builder.RegisterType<SablonService>().As<ISablonService>().InstancePerLifetimeScope();
            builder.RegisterType<WsSorguService>().As<IWsSorguService>().InstancePerLifetimeScope();
            builder.RegisterType<BildirimService>().As<IBildirimService>().InstancePerLifetimeScope();
            builder.RegisterType<UserGroupService>().As<IUserGroupService>().InstancePerLifetimeScope();
            builder.RegisterType<QuotaSettingsService>().As<IQuotaSettingsService>().InstancePerLifetimeScope();
            builder.RegisterType<CompanyCredentialService>().As<ICompanyCredentialService>().InstancePerLifetimeScope();
            var container = builder.Build();

            // company_credentials tablosu yoksa oluştur
            using (var migrationScope = container.BeginLifetimeScope())
            {
                try
                {
                    var db = migrationScope.Resolve<VegaAsisDbContext>();
                    CompanyCredentialService.EnsureCompanyCredentialsTable(db);
                }
                catch { /* veritabanı yok veya yetki hatası */ }
            }

            var scope = container.BeginLifetimeScope();
            ServiceLocator.Initialize(scope);
            
            var authService = scope.Resolve<IAuthService>();
            var authForm = new Forms.AuthForm(authService);
            if (authForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var offerService = scope.Resolve<IOfferService>();
            var policyService = scope.Resolve<IPolicyService>();
            var appointmentService = scope.Resolve<IAppointmentService>();
            var companySettingsService = scope.Resolve<ICompanySettingsService>();
            var userManagementService = scope.Resolve<IUserManagementService>();
            Application.Run(new MainForm(authService, offerService, policyService, appointmentService, companySettingsService, userManagementService));
        }
    }
}
