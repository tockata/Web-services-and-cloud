namespace Battleships.ConsoleApplication
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Battleships.ConsoleApplication.DTO;

    public static class GameEngine
    {
        private const string BaseUrl = "http://localhost:62858/";
        private static PlayerTokenDto playerToken;
        private static IList<string> joinedGamesId = new List<string>();

        public static void ParseCommand(string line)
        {
            var parameters = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var command = parameters[0];

            switch (command)
            {
                case "register":
                    RegisterUser(parameters);
                    break;

                case "login":
                    LoginUser(parameters);
                    break;

                case "create-game":
                    CreateGame();
                    break;

                case "join-game":
                    JoinGame(parameters);
                    break;

                case "play":
                    PlayTurn(parameters);
                    break;
            }
        }

        private static async void RegisterUser(string[] parameters)
        {
            if (parameters.Length != 4)
            {
                Console.WriteLine("Invalid count of parameters for this command.");
                return;
            }

            string email = parameters[1];
            string password = parameters[2];
            string confirmPassword = parameters[3];

            using (var httpClient = new HttpClient())
            {
                string endpoint = BaseUrl + "api/account/register";
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Email", email),
                    new KeyValuePair<string, string>("Password", password),
                    new KeyValuePair<string, string>("ConfirmPassword", confirmPassword),
                });

                var response = await httpClient.PostAsync(endpoint, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    Console.WriteLine("User successfully registered!");
                }
            }
        }

        private static async void LoginUser(string[] parameters)
        {
            if (parameters.Length != 3)
            {
                Console.WriteLine("Invalid count of parameters for this command.");
                return;
            }

            string username = parameters[1];
            string password = parameters[2];

            using (var httpClient = new HttpClient())
            {
                string endpoint = BaseUrl + "token";
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Username", username),
                    new KeyValuePair<string, string>("Password", password),
                    new KeyValuePair<string, string>("grant_type", "password")
                });

                var response = await httpClient.PostAsync(endpoint, content);
                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(error);
                }
                else
                {
                    playerToken = await response.Content.ReadAsAsync<PlayerTokenDto>();
                    Console.WriteLine("Login successfull!");
                }
            }
        }

        private static async void CreateGame()
        {
            using (var httpClient = new HttpClient())
            {
                string endpoint = BaseUrl + "api/games/create";
                httpClient.DefaultRequestHeaders.Add("Authorization", playerToken.TokenType + " " + playerToken.Token);

                var response = await httpClient.PostAsync(endpoint, null);
                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(error);
                }
                else
                {
                    string gameId = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Created game with id: {0}!", gameId);
                }
            }
        }

        private static async void JoinGame(string[] parameters)
        {
            if (parameters.Length != 2)
            {
                Console.WriteLine("Invalid count of parameters for this command.");
                return;
            }

            if (playerToken == null)
            {
                Console.WriteLine("Please login before join a game!");
                return;
            }

            string gameId = parameters[1];

            try
            {
                Guid guid = new Guid(gameId);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid game Id, try again!");
                return;
            }

            using (var httpClient = new HttpClient())
            {
                string endpoint = BaseUrl + "api/games/join";
                httpClient.DefaultRequestHeaders.Add("Authorization", playerToken.TokenType + " " + playerToken.Token);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("GameId", gameId)
                });

                var response = await httpClient.PostAsync(endpoint, content);
                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(error);
                }
                else
                {
                    joinedGamesId.Add(await response.Content.ReadAsStringAsync());
                    Console.WriteLine("Joined to game {0}!", gameId);
                }
            }
        }

        private static async void PlayTurn(string[] parameters)
        {
            if (parameters.Length != 4)
            {
                Console.WriteLine("Invalid count of parameters for this command.");
                return;
            }

            if (playerToken == null)
            {
                Console.WriteLine("Please login before join a game!");
                return;
            }

            string gameId = parameters[1];
            string positionX = parameters[2];
            string positionY = parameters[2];

            try
            {
                Guid guid = new Guid(gameId);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid game Id, try again!");
                return;
            }

            using (var httpClient = new HttpClient())
            {
                string endpoint = BaseUrl + "api/games/play";
                httpClient.DefaultRequestHeaders.Add("Authorization", playerToken.TokenType + " " + playerToken.Token);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("GameId", gameId),
                    new KeyValuePair<string, string>("PositionX", positionX),
                    new KeyValuePair<string, string>("PositionY", positionY)
                });

                var response = await httpClient.PostAsync(endpoint, content);
                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(error);
                }
                else
                {
                    Console.WriteLine("Made turn to game {0}, X = {1}, Y = {2}!", gameId, positionX, positionY);
                }
            }
        }
    }
}