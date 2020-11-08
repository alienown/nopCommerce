using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Misc.IssueManagement.Factories;
using Nop.Plugin.Misc.IssueManagement.Services;

namespace Nop.Plugin.Misc.IssueManagement.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, AppSettings appSettings)
        {
            builder.RegisterType<IssueService>().As<IIssueService>().InstancePerLifetimeScope();
            builder.RegisterType<IssueModelFactory>().As<IIssueModelFactory>().InstancePerLifetimeScope();
        }

        public int Order => 2;
    }
}