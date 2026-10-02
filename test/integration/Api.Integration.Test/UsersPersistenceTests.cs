using System.Net;
using System.Net.Http.Json;

namespace Api.Integration.Test;

// Vérifie l'intégration EF Core + Npgsql + migrations avec un vrai PostgreSQL :
// mapping des colonnes, contrainte d'unicité, ExecuteUpdate / ExecuteDelete.
public class UsersPersistenceTests(AspireFixture fixture)
{
    private record UserResponse(Guid Id, string Email, string FirstName, string LastName, DateTime CreatedAt);

    private record CreateUserRequest(string Email, string FirstName, string LastName);

    private HttpClient Client => fixture.ApiClient;

    private static CreateUserRequest NewUser() =>
        new($"{Guid.NewGuid():N}@example.com", "Ada", "Lovelace");

    private async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        HttpResponseMessage response = await Client.PostAsJsonAsync("/user", request, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<UserResponse>(TestContext.Current.CancellationToken))!;
    }

    [Fact]
    public async Task Created_user_is_persisted_and_can_be_read_back()
    {
        CreateUserRequest request = NewUser();

        UserResponse created = await CreateAsync(request);
        HttpResponseMessage response = await Client.GetAsync($"/user/{created.Id}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var fetched = await response.Content.ReadFromJsonAsync<UserResponse>(TestContext.Current.CancellationToken);
        Assert.Equal(created, fetched);
        Assert.Equal(request.Email, fetched!.Email);
        Assert.Equal(request.FirstName, fetched.FirstName);
        Assert.Equal(request.LastName, fetched.LastName);
    }

    [Fact]
    public async Task Creating_two_users_with_the_same_email_returns_conflict()
    {
        CreateUserRequest request = NewUser();
        await CreateAsync(request);

        HttpResponseMessage response = await Client.PostAsJsonAsync("/user", request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Getting_an_unknown_user_returns_not_found()
    {
        HttpResponseMessage response = await Client.GetAsync($"/user/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Updated_user_is_persisted()
    {
        UserResponse created = await CreateAsync(NewUser());
        var update = new CreateUserRequest($"{Guid.NewGuid():N}@example.com", "Grace", "Hopper");

        HttpResponseMessage response = await Client.PutAsJsonAsync($"/user?id={created.Id}", update, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var fetched = await Client.GetFromJsonAsync<UserResponse>($"/user/{created.Id}", TestContext.Current.CancellationToken);
        Assert.Equal(update.Email, fetched!.Email);
        Assert.Equal(update.FirstName, fetched.FirstName);
        Assert.Equal(update.LastName, fetched.LastName);
    }

    [Fact]
    public async Task Deleted_user_is_no_longer_found()
    {
        UserResponse created = await CreateAsync(NewUser());

        HttpResponseMessage deleteResponse = await Client.DeleteAsync($"/user/{created.Id}", TestContext.Current.CancellationToken);
        HttpResponseMessage getResponse = await Client.GetAsync($"/user/{created.Id}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
