using Models;
using System.Net;
using System.Text.Json;

HttpListener httpListener = new HttpListener();

const int port = 80;

httpListener.Prefixes.Add($"http://*:{port}/");

httpListener.Start();

const string connectionString = $"Server=localhost;Database=Network;TrustServerCertificate=True;Trusted_Connection=True;";
var usersRepository = new UserRepository(connectionString);

while (true)
{
    HttpListenerContext context = await httpListener.GetContextAsync();

    string requestPath = context.Request.Url?.AbsolutePath!;

    var requestPathItems = requestPath.Split('/', StringSplitOptions.RemoveEmptyEntries).Take(2);

    string responseJson = string.Empty;
    try
    {
        requestPathItems.First().ToLower();
    }
    catch
    {
        continue;
    }
    switch (requestPathItems.First().ToLower())
    {
        case "users":
            {
                switch (requestPathItems.Last().ToLower())
                {
                    case "getall":
                        if (context.Request.HttpMethod.ToLower() == "get")
                        {
                            var allUsers = await usersRepository.FindAllAsync();
                            responseJson = JsonSerializer.Serialize(allUsers);

                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = 200;
                        }
                        else
                        {
                            responseJson = "Error: Invalid method type!";
                            context.Response.ContentType = "plain/text";
                            context.Response.StatusCode = 400;
                        }
                        break;
                    case "get":
                        if (context.Request.HttpMethod.ToLower() == "get")
                        {
                            try
                            {
                                var id = int.Parse(context.Request.QueryString["id"]);
                                var user = await usersRepository.FindAsync(id);
                                responseJson = JsonSerializer.Serialize(user);

                                context.Response.ContentType = "application/json";
                                context.Response.StatusCode = 200;
                            }
                            catch
                            {
                                responseJson = "Error!";
                                context.Response.ContentType = "plain/text";
                                context.Response.StatusCode = 400;
                            }
                           
                        }
                        else
                        {
                            responseJson = "Error: Invalid method type!";
                            context.Response.ContentType = "plain/text";
                            context.Response.StatusCode = 400;
                        }
                        break;

                    case "create":
                        if (context.Request.HttpMethod.ToLower() == "post")
                        {
                            try
                            {
                                using var reader = new StreamReader(context.Request.InputStream);
                                var requestJson = await reader.ReadToEndAsync();
                                var user = JsonSerializer.Deserialize<User>(requestJson);

                                if (user == null)
                                {
                                    context.Response.StatusCode = 400;
                                    break;
                                }

                                await usersRepository.CreateAsync(user);
                                context.Response.StatusCode = 201;
                                responseJson = $"User '{user.login} {user.password}' created successfully!";
                                context.Response.ContentType = "plain/text";
                            }
                            catch
                            {
                                responseJson = "Error";
                                context.Response.ContentType = "plain/text";
                                context.Response.StatusCode = 400;
                            }
                        }
                        else
                        {
                            responseJson = "Error: Invalid method type!";
                            context.Response.ContentType = "plain/text";
                            context.Response.StatusCode = 400;
                        }
                        break;

                    case "delete":
                        if (context.Request.HttpMethod.ToLower() == "delete")
                        {
                            using var reader = new StreamReader(context.Request.InputStream);
                            var id = int.Parse(context.Request.QueryString["id"]);


                            await usersRepository.DeleteAsync(id);
                            context.Response.StatusCode = 201;
                            responseJson = $"User '{id}' deleted successfully!";
                            context.Response.ContentType = "plain/text";
                        }
                        else
                        {
                            responseJson = "Error: Invalid method type!";
                            context.Response.ContentType = "plain/text";
                            context.Response.StatusCode = 400;
                        }
                        break;
                    case "update":
                        if (context.Request.HttpMethod.ToLower() == "put")
                        {
                            using var reader = new StreamReader(context.Request.InputStream);
                            var id = int.Parse(context.Request.QueryString["id"]);
                            var requestJson = await reader.ReadToEndAsync();
                            var user = JsonSerializer.Deserialize<User>(requestJson);

                            if (user == null)
                            {
                                context.Response.StatusCode = 400;
                                break;
                            }
                            await usersRepository.UpdateAsync(id,user);
                            context.Response.StatusCode = 201;
                            responseJson = $"User '{user.login} {user.password}' updated successfully!";
                            context.Response.ContentType = "plain/text";

                        }
                        else
                        {
                            responseJson = "Error: Invalid method type!";
                            context.Response.ContentType = "plain/text";
                            context.Response.StatusCode = 400;
                        }
                        break;
                    case "login":
                        if (context.Request.HttpMethod.ToLower() == "get")
                        {
                            using var reader = new StreamReader(context.Request.InputStream);
                            var requestJson = await reader.ReadToEndAsync();
                            var user = JsonSerializer.Deserialize<User>(requestJson);

                            if (user == null)
                            {
                                context.Response.StatusCode = 400;
                                break;
                            }
                            responseJson = JsonSerializer.Serialize(await usersRepository.LoginAsync(user));
                            context.Response.StatusCode = 201;
                            context.Response.ContentType = "plain/text";

                        }
                        else
                        {
                            responseJson = "Error: Invalid method type!";
                            context.Response.ContentType = "plain/text";
                            context.Response.StatusCode = 400;
                        }
                        break;
                }
                break;
            }
        default:
            break;
    }
    using var writer = new StreamWriter(context.Response.OutputStream);
    await writer.WriteLineAsync(responseJson);
    await writer.FlushAsync();
}