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
            builder.RegisterType<UserGroupService>().As<IUserGroupService>().InstancePerLifetimeScope();
            builder.RegisterType<QuotaSettingsService>().As<IQuotaSettingsService>().InstancePerLifetimeScope();
            var container = builder.Build();

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
