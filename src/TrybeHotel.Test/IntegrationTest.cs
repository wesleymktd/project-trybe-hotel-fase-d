namespace TrybeHotel.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Net.Http.Headers;

public class TokenLogin
{
    public string? Token { get; set; }
}


public class IntegrationTest: IClassFixture<WebApplicationFactory<Program>>
{
     public HttpClient _clientTest;

     public IntegrationTest(WebApplicationFactory<Program> factory)
    {
        //_factory = factory;
        _clientTest = factory.WithWebHostBuilder(builder => {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TrybeHotelContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ContextTest>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDatabase");
                });
                services.AddScoped<ITrybeHotelContext, ContextTest>();
                services.AddScoped<ICityRepository, CityRepository>();
                services.AddScoped<IHotelRepository, HotelRepository>();
                services.AddScoped<IRoomRepository, RoomRepository>();
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ContextTest>())
                {
                    appContext.Database.EnsureCreated();
                    appContext.Database.EnsureDeleted();
                    appContext.Database.EnsureCreated();
                    appContext.Cities.Add(new City {CityId = 1, Name = "Manaus", State = "AM"});
                    appContext.Cities.Add(new City {CityId = 2, Name = "Palmas", State = "TO"});
                    appContext.SaveChanges();
                    appContext.Hotels.Add(new Hotel {HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1});
                    appContext.Hotels.Add(new Hotel {HotelId = 2, Name = "Trybe Hotel Palmas", Address = "Address 2", CityId = 2});
                    appContext.Hotels.Add(new Hotel {HotelId = 3, Name = "Trybe Hotel Ponta Negra", Address = "Addres 3", CityId = 1});
                    appContext.SaveChanges();
                    appContext.Rooms.Add(new Room { RoomId = 1, Name = "Room 1", Capacity = 2, Image = "Image 1", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 2, Name = "Room 2", Capacity = 3, Image = "Image 2", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 3, Name = "Room 3", Capacity = 4, Image = "Image 3", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 4, Name = "Room 4", Capacity = 2, Image = "Image 4", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 5, Name = "Room 5", Capacity = 3, Image = "Image 5", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 6, Name = "Room 6", Capacity = 4, Image = "Image 6", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 7, Name = "Room 7", Capacity = 2, Image = "Image 7", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 8, Name = "Room 8", Capacity = 3, Image = "Image 8", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 9, Name = "Room 9", Capacity = 4, Image = "Image 9", HotelId = 3 });
                    appContext.SaveChanges();
                    appContext.Users.Add(new User { UserId = 1, Name = "Ana", Email = "ana@trybehotel.com", Password = "Senha1", UserType = "admin" });
                    appContext.Users.Add(new User { UserId = 2, Name = "Beatriz", Email = "beatriz@trybehotel.com", Password = "Senha2", UserType = "client" });
                    appContext.Users.Add(new User { UserId = 3, Name = "Laura", Email = "laura@trybehotel.com", Password = "Senha3", UserType = "client" });
                    appContext.SaveChanges();
                    appContext.Bookings.Add(new Booking { BookingId = 1, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 2, RoomId = 1});
                    appContext.Bookings.Add(new Booking { BookingId = 2, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 3, RoomId = 4});
                    appContext.SaveChanges();
                }
            });
        }).CreateClient();
    }
 
    [Trait("Category", "1(1) testando a rota GET /city")]
    [Theory(DisplayName = "Será validado que a resposta é status 200")]
    [InlineData("/city")]
    public async Task TestGetCity(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    [Trait("Category", "1(2) testando a rota POST /city")]
    [Theory(DisplayName = "Será validado que a resposta será um status http 201")]
    [InlineData("/city")]
    public async Task TestPostCity(string url)
    {
        var inputObj = new {
            Name = "Rio de Janeiro"
        };
        var response = await _clientTest.PostAsync(url,new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }
// --------------------GET-------------------------------------------------
    [Trait("Category", "2(1) testando a rota GET /hotel")]
    [Theory(DisplayName = "Será validado que a resposta é status 200")]
    [InlineData("/hotel")]
    public async Task TestGetHotel(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }
// -------------------POST----------------------
    [Trait("Category", "2(2) testando a rota POST /hotel")]
    [Theory(DisplayName = "request com user autenticado status volta 201")]
    [InlineData("/hotel")]
    public async Task TestPostHotelOk(string url)
    {
        
        var userAdmin = new { email = "ana@trybehotel.com", password = "Senha1" };
        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await responseLogin.Content.ReadAsStringAsync();
        TokenLogin tok = JsonConvert.DeserializeObject<TokenLogin>(responseString);                           
        

        var inputObj = new {
            Name = "Trybe Hotel RJ",
            Address = "Avenida Atlântica, 1400",
            CityId = 1
        };

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.Token);
        var response = await _clientTest.PostAsync(url,new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }

    [Trait("Category", "2(3) testando a rota POST /hotel")]
    [Theory(DisplayName = "request com user não autenticado status volta 401")]
    [InlineData("/hotel")]
    public async Task TestPostHotelNotAutorized(string url)
    {
        
        var userAdmin = new { email = "beatriz@trybehotel.com", password = "Sa2" };
        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await responseLogin.Content.ReadAsStringAsync();
        TokenLogin tok = JsonConvert.DeserializeObject<TokenLogin>(responseString);                           
        

        var inputObj = new {
            Name = "Trybe Hotel RJ",
            Address = "Avenida Atlântica, 1400",
            CityId = 1
        };

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.Token);
        var response = await _clientTest.PostAsync(url,new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response?.StatusCode);
    }
//----------------------------GET------------------------------------------------
    [Trait("Category", "3(1) testando a rota GET /room/:hotelId")]
    [Theory(DisplayName = "Será validado que a resposta é status 200")]
    [InlineData("/room/1")]
    public async Task TestGetRoomById(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }
//----------------------------POST------------------------------------------------
    [Trait("Category", "3(2) testando a rota POST /room")]
    [Theory(DisplayName = "request com user autenticado status volta 201")]
    [InlineData("/room")]
    public async Task TestPostRoom(string url)
    {
        var userAdmin = new { email = "ana@trybehotel.com", password = "Senha1" };
        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await responseLogin.Content.ReadAsStringAsync();
        TokenLogin tok = JsonConvert.DeserializeObject<TokenLogin>(responseString);

        var inputObj = new {
            Name = "Suite Básica",
            Capacity = 2,
            Image = "image suite",
            HotelId = 1
        };

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.Token);
        var response = await _clientTest.PostAsync(url,new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }
//-----------------DELETE-------------------------
    [Trait("Category", "3(3) testando a rota DELETE /room")]
    [Theory(DisplayName = "request com user autenticado status volta 204")]
    [InlineData("/room/1")]
    public async Task TestDeleteRoom(string url)
    {
        var userAdmin = new { email = "ana@trybehotel.com", password = "Senha1" };
        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await responseLogin.Content.ReadAsStringAsync();
        TokenLogin tok = JsonConvert.DeserializeObject<TokenLogin>(responseString);

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.Token);
        var response = await _clientTest.DeleteAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response?.StatusCode);
    }
 //-------------POST--------------------------
    [Trait("Category", "4(1) testando a rota POST /booking")]
    [Theory(DisplayName = "request com user autenticado status volta 201")]
    [InlineData("/booking")]
    public async Task TestPostBooking(string url)
    {
        var userAdmin = new { Email = "beatriz@trybehotel.com", Password = "Senha2" };
        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await responseLogin.Content.ReadAsStringAsync();
        TokenLogin tok = JsonConvert.DeserializeObject<TokenLogin>(responseString);

        var inputObj = new {
            CheckIn = "2030-08-27",
            CheckOut = "2030-08-28",
            GuestQuant = "1",
            RoomId = 1
        };

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.Token);
        var response = await _clientTest.PostAsync(url,new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }

    [Trait("Category", "4(2) testando a rota POST /booking")]
    [Theory(DisplayName = "request com user autenticado GuestQuant > Capacity retorna 400")]
    [InlineData("/booking")]
    public async Task TestPostBookingBadRequest(string url)
    {
        var userAdmin = new { Email = "beatriz@trybehotel.com", Password = "Senha2" };
        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await responseLogin.Content.ReadAsStringAsync();
        TokenLogin tok = JsonConvert.DeserializeObject<TokenLogin>(responseString);

        var inputObj = new {
            CheckIn = "2030-08-27",
            CheckOut = "2030-08-28",
            GuestQuant = "10",
            RoomId = 1
        };

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.Token);
        var response = await _clientTest.PostAsync(url,new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response?.StatusCode);
    }
// ----------------------GET-------------------------
    [Trait("Category", "4(3) testando a rota GET /booking")]
    [Theory(DisplayName = "request com user que queira visualizar uma reserva que não corresponde com seu Id retorna 401")]
    [InlineData("/booking/1")]
    public async Task TestGetBookingByIdUnautorized(string url)
    {
        var userAdmin = new { Email = "laura@trybehotel.com", Password = "Senha3" };
        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await responseLogin.Content.ReadAsStringAsync();
        TokenLogin tok = JsonConvert.DeserializeObject<TokenLogin>(responseString);

        

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.Token);
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response?.StatusCode);
    }

    [Trait("Category", "4(4) testando a rota GET /booking")]
    [Theory(DisplayName = "request com user que queira visualizar sua reserva retorna 200")]
    [InlineData("/booking/1")]
    public async Task TestGetBookingByIdOk(string url)
    {
        var userAdmin = new { Email = "beatriz@trybehotel.com", Password = "Senha2" };
        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await responseLogin.Content.ReadAsStringAsync();
        TokenLogin tok = JsonConvert.DeserializeObject<TokenLogin>(responseString);

        

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.Token);
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }
// ------------------POST----------------------------
    [Trait("Category", "5(1) testando a rota POST /login")]
    [Theory(DisplayName = "request com user login e senha ok retorna 200")]
    [InlineData("/login")]
    public async Task TestPostLoginOk(string url)
    {
        var userAdmin = new { Email = "beatriz@trybehotel.com", Password = "Senha2" };
        var responseLogin = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
       
        Assert.Equal(System.Net.HttpStatusCode.OK, responseLogin?.StatusCode);
    }

    [Trait("Category", "5(2) testando a rota POST /login")]
    [Theory(DisplayName = "request com user login e senha errado retorna 401")]
    [InlineData("/login")]
    public async Task TestPostLoginUnautorized(string url)
    {
        var userAdmin = new { Email = "beatriz@trybehotel.com", Password = "Senh" };
        var responseLogin = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
       
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, responseLogin?.StatusCode);
    }
// ------------------GET----------------------------
    [Trait("Category", "6(1) testando a rota GET /user")]
    [Theory(DisplayName = "request com user autenticado retorna 200")]
    [InlineData("/user")]
    public async Task TestGetUserOk(string url)
    {
        var userAdmin = new { Email = "ana@trybehotel.com", Password = "Senha1" };
        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await responseLogin.Content.ReadAsStringAsync();
        TokenLogin tok = JsonConvert.DeserializeObject<TokenLogin>(responseString);

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.Token);
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }
// ------------------POST----------------------------
    [Trait("Category", "6(2) testando a rota POST /user")]
    [Theory(DisplayName = "request com user inexistente no banco retorna 201")]
    [InlineData("/user")]
    public async Task TestPostUserOk(string url)
    {
        var userAdmin = new { Email = "xablau@trybehotel.com", Password = "XablauSenha" };
        var responseLogin = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
       
        Assert.Equal(System.Net.HttpStatusCode.Created, responseLogin?.StatusCode);
    }

    [Trait("Category", "6(3) testando a rota POST /user")]
    [Theory(DisplayName = "request com user que já no banco retorna 409")]
    [InlineData("/user")]
    public async Task TestPostUserConflict(string url)
    {
        var userAdmin = new { Email = "ana@trybehotel.com", Password = "Senha1" };
        var responseLogin = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(userAdmin), System.Text.Encoding.UTF8, "application/json"));
       
        Assert.Equal(System.Net.HttpStatusCode.Conflict, responseLogin?.StatusCode);
    }

    
}