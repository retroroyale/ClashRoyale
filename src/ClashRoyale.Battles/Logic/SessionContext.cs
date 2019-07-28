using System;
using ClashRoyale.Utilities.Crypto;

namespace ClashRoyale.Battles.Logic
{
    public class SessionContext
    {
        public Rc4Core Rc4 = new Rc4Core("fhsd6f86f67rt8fw78fw789we78r9789wer6re", "scroll");
        public long PlayerId { get; set; }

        private DateTime _lastMessage = DateTime.UtcNow;

        public bool Active
        {
            get => DateTime.UtcNow.Subtract(_lastMessage).TotalSeconds < 10;
            set
            {
                if (value)
                {
                    _lastMessage = DateTime.UtcNow;
                }
            }
        }
    }
}