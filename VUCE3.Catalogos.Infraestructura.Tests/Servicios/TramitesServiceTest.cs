using Microsoft.Identity.Web;
using Moq;
using RichardSzalay.MockHttp;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using VUCE3.Catalogos.Infraestructura.Servicios;
using Xunit;

namespace VUCE3.Catalogos.Infraestructura.Tests.Servicios
{
    public class TramitesServiceTest
    {
        [Fact]
        public async Task ExisteRelacionTipoTramiteSubtipo_DevuelveTrue()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("*")
                .Respond("application/json", @"{
                    ""@odata.context"": ""http://test/odata/$metadata#SubtiposTramites"",
                    ""value"": [
                        { ""Id"": 5, ""Nombre"": ""Subtipo X"", ""IdTipoTramite"": 3 }
                    ]
                }");

            HttpClient client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://test");

            var mockToken = new Mock<ITokenAcquisition>();
            mockToken.Setup(t => t.GetAccessTokenForAppAsync(It.IsAny<string>(), It.IsAny<string>(), null, null))
                     .ReturnsAsync("fake-token");

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Apis:Tramites:Scope"]).Returns("scope-uri");

            var servicio = new TramitesService(client, mockToken.Object, mockConfig.Object);

            // Act
            var resultado = await servicio.ExisteRelacionTipoTramiteSubtipo(3, 5);

            // Assert
            Assert.False(resultado.IsError);
            Assert.True(resultado.Value);
        }

        [Fact]
        public async Task ExisteRelacionTipoTramiteSubtipo_DevuelveFalse_SinRelacion()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("*")
                .Respond("application/json", @"{
                    ""@odata.context"": ""http://test/odata/$metadata#SubtiposTramites"",
                    ""value"": []
                }");

            HttpClient client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://test");

            var mockToken = new Mock<ITokenAcquisition>();
            mockToken.Setup(t => t.GetAccessTokenForAppAsync(It.IsAny<string>(), It.IsAny<string>(), null, null))
                     .ReturnsAsync("fake-token");

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Apis:Tramites:Scope"]).Returns("scope-uri");

            var servicio = new TramitesService(client, mockToken.Object, mockConfig.Object);

            // Act
            var resultado = await servicio.ExisteRelacionTipoTramiteSubtipo(3, 999);

            // Assert
            Assert.False(resultado.IsError);
            Assert.False(resultado.Value);
        }

        [Fact]
        public async Task ExisteRelacionTipoTramiteSubtipo_DevuelveError_EnExcepcion()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("*").Throw(new Exception("Fallo HTTP"));

            HttpClient client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://test");

            var mockToken = new Mock<ITokenAcquisition>();
            mockToken.Setup(t => t.GetAccessTokenForAppAsync(It.IsAny<string>(), It.IsAny<string>(), null, null))
                     .ReturnsAsync("fake-token");

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Apis:Tramites:Scope"]).Returns("scope-uri");

            var servicio = new TramitesService(client, mockToken.Object, mockConfig.Object);

            // Act
            var resultado = await servicio.ExisteRelacionTipoTramiteSubtipo(3, 5);

            // Assert
            Assert.True(resultado.IsError);
        }
    }
}
