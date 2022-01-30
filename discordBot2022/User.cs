using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
namespace discordBot2022
{
    class User
    {

        public ulong ID;
        public string name, role;
        public int activityPoints;
        public List<User> allUsers = new List<User>();

        public async Task GetUsersFromDb()
        {
            Console.WriteLine("Getting users from database");
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["usersDb"].ConnectionString);
            connection.Open();
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM Users", connection);
            reader = command.ExecuteReader();
            User temp_user = new User();
            while (reader.Read())
            {
                temp_user.ID = Convert.ToUInt64(reader["discord_id"].ToString());
                temp_user.name = reader["name"].ToString();
                temp_user.activityPoints = Convert.ToInt32(reader["activityPoints"]);
                allUsers.Add(temp_user);
            }
            Console.WriteLine("Users are ready");

        }

        public void CreateNewUser(SocketUser newUser)
        {
            User temp = new User();
            temp.ID = newUser.Id;
            temp.name = newUser.Username;
            temp.activityPoints = 0;

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["usersDb"].ConnectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("INSERT INTO [Users](discord_id, name, activityPoints)" +
                "VALUES(@discord_id, @name, @activityPoints)", connection);
            command.Parameters.AddWithValue("discord_id", temp.ID);
            command.Parameters.AddWithValue("name", temp.name);
            command.Parameters.AddWithValue("activityPoints", temp.activityPoints);
            allUsers.Add(temp);
            Console.WriteLine("User " + temp.ID + " successfully added");
        }


    }
    
}
