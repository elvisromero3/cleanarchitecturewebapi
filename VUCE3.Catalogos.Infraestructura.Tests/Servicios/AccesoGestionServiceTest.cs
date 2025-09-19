using Microsoft.Identity.Web;
using Moq;
using RichardSzalay.MockHttp;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Infraestructura.Servicios;
using VUCE3.Catalogos.Aplicacion.Servicios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Servicios
{
    public class AccesoGestionServiceTest
    {
        [Fact]
        public async Task ObtenerInstituciones_DevuelveOk()
        {
            //Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("*")
                    .Respond("application/json", @"{
  ""@odata.context"": ""http://accesogestionusuarios.whitebush-ee0e3964.brazilsouth.azurecontainerapps.io/odata/$metadata#Instituciones"",
  ""value"": [
        {""Id"":1,""Nombre"":""MINCEX""},
        {""Id"":2,""Nombre"":""PROCOMER""},
        {""Id"":3,""Nombre"":""Institucion 3""},
        {""Id"":4,""Nombre"":""Institucion 4""},
        {""Id"":5,""Nombre"":""Institucion 5""}
      ]
    }");
                HttpClient client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://test");

            Mock<ITokenAcquisition> mockToken = new Mock<ITokenAcquisition>();
            mockToken.Setup(s => s.GetAccessTokenForAppAsync(It.IsAny<string>(), It.IsAny<string>(), null, null)).ReturnsAsync("");

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();

            var servicio = new AccesoGestionUsuariosService(client, mockToken.Object, mockConfig.Object);

            //Act
            var instituciones = await servicio.ObtenerInstituciones();

            //Assert
            Assert.False(instituciones.IsError);
            Assert.NotEmpty(instituciones.Value);
          //  Assert.IsType<Dictionary<int, string>>(divisiones.Value);
        }

        [Fact]
        public async Task ObtenerInstituciones_DevuelveError()
        {
            //Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("*").Throw(new Exception());

            HttpClient client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://test");

            Mock<ITokenAcquisition> mockToken = new Mock<ITokenAcquisition>();
            mockToken.Setup(s => s.GetAccessTokenForAppAsync(It.IsAny<string>(), It.IsAny<string>(), null, null)).ReturnsAsync("");

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();

            var servicio = new AccesoGestionUsuariosService(client, mockToken.Object, mockConfig.Object);

            //Act
            var instituciones = await servicio.ObtenerInstituciones();

            //Assert
            Assert.True(instituciones.IsError);
        }
        [Fact]
        public async Task ObtenerExisteRelacion_DevuelveOk()
        {
            //Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("*")
                    .Respond("application/json", @"{
  ""@odata.context"": ""http://accesogestionusuarios.whitebush-ee0e3964.brazilsouth.azurecontainerapps.io/odata/$metadata#InstitucionesAutorizadas"",
  ""value"": [
         {""Id"":39,""IdInstitucion"":4,""IdUsuario"":659,""IdAduana"":12,""IdTipoIdentificacion"":1,""NumeroIdentificacion"":""112490335"",""NombreEmpresa"":""HANS GUTIERREZ ARTAVIA""},
         {""Id"":59,""IdInstitucion"":2,""IdUsuario"":719,""IdAduana"":12,""IdTipoIdentificacion"":4,""NumeroIdentificacion"":""sdfsfs"",""NombreEmpresa"":""fsddssdf""},
         {""Id"":63,""IdInstitucion"":2,""IdUsuario"":722,""IdAduana"":12,""IdTipoIdentificacion"":1,""NumeroIdentificacion"":null,""NombreEmpresa"":null},
         {""Id"":86,""IdInstitucion"":2,""IdUsuario"":655,""IdAduana"":12,""IdTipoIdentificacion"":4,""NumeroIdentificacion"":""234242428"",""NombreEmpresa"":""Empresa XYZ""}
      ]
    }");
            HttpClient client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://test");

            Mock<ITokenAcquisition> mockToken = new Mock<ITokenAcquisition>();
            mockToken.Setup(s => s.GetAccessTokenForAppAsync(It.IsAny<string>(), It.IsAny<string>(), null, null)).ReturnsAsync("");

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();

            var servicio = new AccesoGestionUsuariosService(client, mockToken.Object, mockConfig.Object);

            //Act
            var institucionesAutorizadas = await servicio.ExisteRelacionAduanaInstitucionesAutorizadas(1);

            //Assert
            Assert.False(institucionesAutorizadas.IsError);
 
        }
        [Fact]
        public async Task ObtenerExisteRelacion_DevuelveFalse()
        {
            //Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("*")
                    .Respond("application/json", @"{
  ""@odata.context"": ""http://accesogestionusuarios.whitebush-ee0e3964.brazilsouth.azurecontainerapps.io/odata/$metadata#InstitucionesAutorizadas"",
  ""value"": []
    }");
            HttpClient client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://test");

            Mock<ITokenAcquisition> mockToken = new Mock<ITokenAcquisition>();
            mockToken.Setup(s => s.GetAccessTokenForAppAsync(It.IsAny<string>(), It.IsAny<string>(), null, null)).ReturnsAsync("");

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();

            var servicio = new AccesoGestionUsuariosService(client, mockToken.Object, mockConfig.Object);

            //Act
            var institucionesAutorizadas = await servicio.ExisteRelacionAduanaInstitucionesAutorizadas(1);

            //Assert
            Assert.False(institucionesAutorizadas.IsError);

        }
        [Fact]
        public async Task ObtenerExisteRelacion_DevuelveError()
        {
            //Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("*").Throw(new Exception());

            HttpClient client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://test");

            Mock<ITokenAcquisition> mockToken = new Mock<ITokenAcquisition>();
            mockToken.Setup(s => s.GetAccessTokenForAppAsync(It.IsAny<string>(), It.IsAny<string>(), null, null)).ReturnsAsync("");

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();

            var servicio = new AccesoGestionUsuariosService(client, mockToken.Object, mockConfig.Object);

            //Act
            var institucionesAutorizadas = await servicio.ExisteRelacionAduanaInstitucionesAutorizadas(1);

            //Assert
            Assert.True(institucionesAutorizadas.IsError);
        }


    }
}
