using Models;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using System.Formats.Asn1;

public class UserRepository
{
    private readonly IDbConnection connection;

    public UserRepository(string connectionString)
    {
        this.connection = new SqlConnection(connectionString);
    }

    public async Task<IEnumerable<User>> FindAllAsync()
    {
        var users = await this.connection.QueryAsync<User>($@"select * from Users");

        return users;
    }

    public async Task<User> FindAsync(int id)
    {
            var user = await this.connection.QueryFirstAsync<User>(
                sql: $@"select * from Users u
                    where u.Id = @id",
                param: new { id });
            return user;
    }

    public async Task CreateAsync(User user)
    {
        await this.connection.ExecuteAsync(
            sql: @"insert into Users([Login], [Password],[Email])
                    values(@Login, @Password,@Email)",
            param: new { user.login, user.password,user.email });
    }

    public async Task DeleteAsync(int id)
    {
        await this.connection.ExecuteAsync(
           sql: @"delete from Users
                  where Users.Id = @id",
           param: new { id });
    }

    public async Task UpdateAsync(int id,User user)
    {
        await this.connection.ExecuteAsync(
           sql: @"update Users
                    Set Login = @login,Password = @password,Email = @email
                    where Id = @id",
           param: new { id,user.login,user.password,user.email });
    }

    public async Task<User> LoginAsync(User user)
    {
        var findeduser = await this.connection.QueryFirstAsync<User>(
           sql: @"select *
                    from Users
                    where [Login] = @login and Password = @password and Email = @email",
           param: new {user.login, user.password, user.email });
        return findeduser;
    }
}