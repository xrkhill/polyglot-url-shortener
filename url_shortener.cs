using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace polyglot_url_shortener
{
    class Shortener
    {
        static void Main(string[] args)
        {
            var urls = new List<string> {
                "http://www.google.com",
                "http://www.amazon.com",
                "http://www.netflix.com",
                "http://www.hulu.com",
                "http://www.linkedin.com",
                "http://www.facebook.com",
                "http://www.apple.com"
            };

            var shortener = new Shortener();

            foreach (var url in urls)
            {
                Console.WriteLine("{0} => http://cor.to/{1}", url, shortener.Shorten(url));
            }
        }

        private int shortStringLength;
        private int ShortStringLength
        {
            get { return shortStringLength; }
            set
            {
                shortStringLength = value;
                if (value >= 1 && value <= 62)
                {
                    shortStringLength = value;
                }
                else
                {
                    shortStringLength = 7;
                }
            }
        }

        private int randomBytesLength;
        private int RandomBytesLength
        {
            get { return randomBytesLength; }
            set
            {
                randomBytesLength = value;
                if (value >= 32 && value <= 4096)
                {
                    randomBytesLength = value;
                }
                else
                {
                    randomBytesLength = 1024;
                }
            }
        }

        public Shortener(int shortStringLength = 7, int randomBytesLength = 1024)
        {
            ShortStringLength = shortStringLength;
            RandomBytesLength = randomBytesLength;
        }

        public string Shorten(string url)
        {
            // generate random bytes, and add process id and current time for more entropy
            var randomBytes = RandomBytes();
            var pid = Pid();
            var time = Time();

            // combine url with random data
            var combined = String.Format("{0}:{1}:{2}:{3}", url, randomBytes, pid, time);

            // apply sha1 hash to combined data to get fixed number of bytes
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(combined));

            // convert sha1 bytes to integer
            var number = BytesToBigInteger(hashBytes);

            // convert to base 62 so we only return alpha numeric chars
            var result = BigIntegerToBase62(number);

            // truncate value to seven chars so it will be short
            // 62^7 combinations (3,521,614,606,208)
            return result.Substring(0, ShortStringLength);
        }

        private string RandomBytes()
        {
            var bytes = new byte[RandomBytesLength];
            var rand = new Random();
            rand.NextBytes(bytes);

            return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        private int Pid()
        {
            var currentProcess = Process.GetCurrentProcess();

            return currentProcess.Id;
        }

        private long Time()
        {
            DateTime currentDate = DateTime.Now;

            return currentDate.Ticks;
        }

        private BigInteger BytesToBigInteger(byte[] bytes)
        {
            return new BigInteger(bytes);
        }

        private const string Characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int Base62 = 62;

        // Convert integer to base 62 (0-9, a-z, A-Z)
        private string BigIntegerToBase62(BigInteger number)
        {
            var result = "";

            // one's complement (invert sign), if needed
            if (number.Sign == -1)
            {
                number = ~number;
            }

            while (number > 0)
            {
                var remainder = BigInteger.Remainder(number, Base62);
                result = Characters[(int)remainder] + result;
                number /= Base62;
            }

            return result;
        }
    }
}