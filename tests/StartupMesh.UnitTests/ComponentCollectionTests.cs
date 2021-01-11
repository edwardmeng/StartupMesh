using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace StartupMesh.UnitTests
{
    public class ComponentCollectionTests
    {
        [Fact]
        public void CanRegisterComponent_WithoutServiceProvider()
        {
            var components = new ComponentCollection();

            components.Add(typeof(IFormatProvider), CultureInfo.CurrentCulture);
            var component = components.Get(typeof(IFormatProvider));
            Assert.NotNull(component);
            Assert.Equal(CultureInfo.CurrentCulture, component);
        }

        [Fact]
        public void CanReplaceComponent_WithoutServiceProvider()
        {
            var components = new ComponentCollection();
            components.Add(typeof(IFormatProvider), CultureInfo.CurrentCulture);
            components.Add(typeof(IFormatProvider), CultureInfo.CurrentCulture.DateTimeFormat);
            var component = components.Get(typeof(IFormatProvider));
            Assert.NotNull(component);
            Assert.NotEqual(CultureInfo.CurrentCulture, component);
            Assert.Equal(CultureInfo.CurrentCulture.DateTimeFormat, component);
        }

        [Fact]
        public void CanGetNotRegisteredComponent_WithoutServiceProvider()
        {
            var components = new ComponentCollection();
            components.Add(typeof(IFormatProvider), CultureInfo.CurrentCulture);

            Assert.NotNull(components.Get(typeof(IFormatProvider)));
            Assert.Null(components.Get(typeof(ICustomFormatter)));
        }

        [Fact]
        public void CanRegisterMultipleComponent_WithoutServiceProvider()
        {
            var components = new ComponentCollection();
            components.Add(typeof(IFormatProvider), CultureInfo.CurrentCulture);
            components.Add(typeof(IFormatProvider), CultureInfo.CurrentCulture.DateTimeFormat);

            var providers = (IFormatProvider[])components.GetAll(typeof(IFormatProvider));
            Assert.NotNull(providers);
            Assert.Equal(2, providers.Length);
        }

        [Fact]
        public void CanGetEmptyArrayComponents_WithoutServiceProvider()
        {
            var components = new ComponentCollection();
            var providers = (IFormatProvider[])components.GetAll(typeof(IFormatProvider));
            Assert.NotNull(providers);
            Assert.Empty(providers);
        }

        [Fact]
        public void CanGetComponent_RegisteredInServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IFormatProvider>(CultureInfo.CurrentCulture);
            var components = new ComponentCollection();
            components.Add(typeof(IServiceProvider), services.BuildServiceProvider());

            var component = components.Get(typeof(IFormatProvider));
            Assert.Equal(CultureInfo.CurrentCulture, component);
        }

        [Fact]
        public void CanOverrideComponent_RegisteredInServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IFormatProvider>(CultureInfo.CurrentCulture);
            var components = new ComponentCollection();
            components.Add(typeof(IServiceProvider), services.BuildServiceProvider());
            components.Add(typeof(IFormatProvider), CultureInfo.CurrentCulture.DateTimeFormat);

            var component = components.Get(typeof(IFormatProvider));
            Assert.Equal(CultureInfo.CurrentCulture.DateTimeFormat, component);
        }

        [Fact]
        public void GetMultipleComponents_RegisteredInServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IFormatProvider>(CultureInfo.CurrentCulture);
            services.AddSingleton<IFormatProvider>(CultureInfo.CurrentCulture.DateTimeFormat);
            var components = new ComponentCollection();
            components.Add(typeof(IServiceProvider), services.BuildServiceProvider());
            var array = components.GetAll(typeof(IFormatProvider));
            Assert.NotNull(array);
            Assert.Equal(2, array.Length);
        }

        [Fact]
        public void GetMultipleComponents_RegisteredInBothWays()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IFormatProvider>(CultureInfo.CurrentCulture);
            var components = new ComponentCollection();
            components.Add(typeof(IServiceProvider), services.BuildServiceProvider());
            components.Add(typeof(IFormatProvider), CultureInfo.CurrentCulture.DateTimeFormat);
            var array = components.GetAll(typeof(IFormatProvider));
            Assert.NotNull(array);
            Assert.Equal(2, array.Length);
        }
    }
}
