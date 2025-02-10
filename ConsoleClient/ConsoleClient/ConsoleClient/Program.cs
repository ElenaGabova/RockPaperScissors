using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using Grpc.Core;
using Grpc.Net.Client;
using RockPaperScissors;

namespace GameClient
{
    class Program
    {
       
        static async Task Main(string[] args)
        {
            // Разрешаем поддержку нешифрованного HTTP/2
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var channel = GrpcChannel.ForAddress("http://localhost:5002");
            var client = new RockPaperScissorsService.RockPaperScissorsServiceClient(channel);


            Console.WriteLine("Добро пожаловать. Введите свой ИД");
            var userId = int.Parse(Console.ReadLine());

                Console.WriteLine(@"1 - посмотреть баланс;
2 - получение списка игр, ставок;
3 {Ид игры} {ваш ход}- подключение к игре.
exit - выход из игры.");
            while (true)
            {
                Console.WriteLine("Введите команду:");
                var input = Console.ReadLine().Split(' ');

                try
                {
                    switch (input[0])
                    {
                        case "1":
                            var balanceResponse = client.GetBalance(new UserRequest { UserId = userId });
                            Console.WriteLine($"Ваш баланс: {balanceResponse.Balance}");
                            break;
                        case "2":

                            var listMatchesResponse = await client.ListMatchesAsync(new UserRequest{ UserId = userId });
                            foreach (var match in listMatchesResponse.Matches)
                            {
                                Console.WriteLine($"{match.Result}");
                            }
                            break;
                        case "3":
                            var joinMatchResponse = await client.JoinMatchAsync(new JoinMatchRequest { UserId = userId, MatchId = int.Parse(input[1]), Move = input[2] });
                            Console.WriteLine($"{joinMatchResponse.Message}");
                            break;
                        case "exit":
                            {
                                Console.WriteLine("Добро пожаловать. Введите свой ИД");
                                userId = int.Parse(Console.ReadLine());
                                break;
                            }
                        default:
                            Console.WriteLine("Неизвестная команда.");
                            break;
                    }
                }

                catch (RpcException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Status.StatusCode} - {ex.Status.Detail}");
                }
            }
        }
    }
}
