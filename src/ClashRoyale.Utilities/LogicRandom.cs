namespace ClashRoyale.Utilities
{
    public class LogicRandom
    {
        private int _seed = -237100689;

        public int Rand(int a2)
        {
            int result; 

            if (a2 >= 1)
            {
                var v3 = _seed; 
                var v4 = v3 ^ (v3 << 13) ^ ((v3 ^ (v3 << 13)) >> 17);
                var v5 = v4 ^ (32 * v4); 
                _seed = v5;
                int v6; 
                if (v5 <= -1) v6 = -v5;
                else v6 = v5;
                result = v6 % a2;
            }
            else
                result = 0;

            return result;
        }

        public void SetIteratedRandomSeed(int rndSeed)
        {
            _seed = rndSeed;
        }
    }
}