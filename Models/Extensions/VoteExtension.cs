using Models.Enums;

namespace Models.Extensions
{
    public static class VoteExtension
    {
        public static int IntValue(this Vote vote)
        {
            return vote switch
            {
                Vote.Up => 1,
                Vote.Down => -1,
                _ => 0
            };
        }
    }
}