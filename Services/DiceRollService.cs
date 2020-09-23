using Dice;

namespace DiscordOregonTrail.Services
{

    // TODO - if everything is static, is it a service?
    public class DiceRollService
    {
        public DiceRollService()
        {
        }

        public static int Roll(string expression)
        {
            RollResult result = Roller.Roll(expression);
            return (int)result.Value;
        }

        /// <summary>
        /// Rolls 1d<paramref name="maxValue"/>
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Roll(int maxValue)
        {
            string expression = string.Format("1d{0}", maxValue);

            RollResult result = Roller.Roll(expression);
            return (int)result.Value;
        }


    }
}
