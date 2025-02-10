namespace RockPaperScissors.Controllers
{
    public static class GameActions
    {


        /// <summary>
        /// Вспомогательный метод для проверки корректности хода.
        /// </summary>
        public static bool IsValidMove(char? move)
        {
            if (move == null)
                return false;

            return move == 'К' || move == 'Н' || move == 'Б';
        }

        /// <summary>
        /// Логика определения победителя для "Камень, ножницы, бумага".
        /// Возвращает Id победителя или 0 в случае ничьей.
        /// </summary>

        public static int DetermineWinner(int player1Id, int player2Id, char move1, char move2)
        {
            // Если ходы равны, возвращаем 0 – ничья
            if (move1 == move2) return 0;

            // Логика выигрыша:
            // Камень ("К") побеждает Ножницы ("Н"),
            // Ножницы ("Н") побеждают Бумагу ("Б"),
            // Бумага ("Б") побеждает Камень ("К").
            if ((move1 == 'К' && move2 == 'Н') ||
                (move1 == 'Н' && move2 == 'Б') ||
                (move1 == 'Б' && move2 == 'К'))

                return player1Id;
            else
                return player2Id;
        }
    }
}
