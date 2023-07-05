using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Test;

public class TestSuite : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _output;

    public TestSuite(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
    }

    [Fact]
    public async Task This_Test_Will_Hit_Middleware_Instead_Of_Filter_Resulting_In_InternalServerError_Because_It_Cant_Bind_The_Model()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            CategoryIdentification = "",
            UserIdentification = "",
            Topic = "",
            Content = ""
        };

        // Act
        var response = await client.PostAsync("/posts",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        _output.WriteLine(await response.Content.ReadAsStringAsync());
        
        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task This_Test_Will_Hit_Filter_As_I_Would_Expect_Because_Properties_Are_Missing()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            Topic = "",
            Content = ""
        };

        // Act
        var response = await client.PostAsync("/posts",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        _output.WriteLine(await response.Content.ReadAsStringAsync());
        
        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
    
    [Fact]
    public async Task This_Test_Will_Hit_Filter_Because_Identifications_Are_Guids_But_Topic_And_Content_Are_Invalid()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            CategoryIdentification = "8fbd7f90-dddc-479d-8211-0abd4815c0c7",
            UserIdentification = "16dc0c03-94db-4445-b94b-273006257006",
            Topic = "",
            Content = ""
        };

        // Act
        var response = await client.PostAsync("/posts",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
        
        _output.WriteLine(await response.Content.ReadAsStringAsync());

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
    
    [Fact]
    public async Task This_Test_Will_Get_A_Successful_Response_Because_The_Model_Is_Valid()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            CategoryIdentification = "8fbd7f90-dddc-479d-8211-0abd4815c0c7",
            UserIdentification = "16dc0c03-94db-4445-b94b-273006257006",
            Topic = "My amazing topic",
            Content = "Seriously good content"
        };

        // Act
        var response = await client.PostAsync("/posts",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        _output.WriteLine(await response.Content.ReadAsStringAsync());
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task This_Test_Will_Get_Internal_Server_Error_Because_The_Guid_In_CategoryIdentification_Is_Not_A_Valid_Guid()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            CategoryIdentification = "8fbd7f90-dddc-479d-8211-0abd4815c0c",
            UserIdentification = "16dc0c03-94db-4445-b94b-273006257006",
            Topic = "My amazing topic",
            Content = "Seriously good content"
        };

        // Act
        var response = await client.PostAsync("/posts",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        _output.WriteLine(await response.Content.ReadAsStringAsync());
        
        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    
    [Fact]
    public async Task This_Test_Will_Hit_Middleware_Because_Integer_Wont_Bind_To_String()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            CategoryIdentification = "",
            UserIdentification = "",
            Topic = 1,
            Content = ""
        };

        // Act
        var response = await client.PostAsync("/posts",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        _output.WriteLine(await response.Content.ReadAsStringAsync());
        
        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}