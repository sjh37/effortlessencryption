namespace Effortless.Net.Encryption
{
    using System;

    public class Digest
    {
        private readonly string _data;
        private readonly string _hash;
        private readonly HashType _hashType;

        public Digest(string data, string hash, HashType hashType)
        {
            _data = data;
            _hash = hash;
            _hashType = hashType;
        }

        public string Data
        {
            get { return _data; }
        }

        public string Hash
        {
            get { return _hash; }
        }

        public static Digest Create(HashType hashType, string data, string sharedKey)
        {
            string hash = Encryption.Hash.Create(hashType, data, sharedKey, true);
            return new Digest(data, hash, hashType);
        }

        public override string ToString()
        {
            string hashType = ((int)_hashType).ToString("d2");
            string hashLength = _hash.Length.ToString("d3");
            return hashType + hashLength + _hash + _data;
        }

        /// <summary>
        /// This is the opposite of ToString(). It takes the data and re-creates the Digest.
        /// </summary>
        /// <param name="hashedData">The data obtained from ToString()</param>
        /// <param name="sharedKey">The sharedKey is shared by the two parties who independently calculate the hash. The data is passed between parties
        /// together with the hash. The hash will be identical if the data is unmodified. Use a sharedKey that is sufficiently
        /// long and complex for the application - https://www.grc.com/passwords.htm - and share the sharedKey once over a secure
        /// channel. See http://en.wikipedia.org/wiki/Cryptographic_hash_function for more information.</param>
        /// <returns>Returns a Digest if succesfully verified, otherwise returns null</returns>
        public static Digest CreateFromString(string hashedData, string sharedKey)
        {
            if (string.IsNullOrEmpty(hashedData))
                throw new ArgumentNullException("hashedData");
            if (sharedKey == null)
                throw new ArgumentNullException("sharedKey");

            if (hashedData.Length < 12)
                return null; // Not long enough to cover even the smallest

            var hashType = (HashType)int.Parse(hashedData.Substring(0, 2));

            int hashLength;
            int.TryParse(hashedData.Substring(2, 3), out hashLength);
            if (hashLength < 0)
                return null;

            if (hashedData.Length < hashLength + 5)
                return null;

            string hash = hashedData.Substring(5, hashLength);
            string data = hashedData.Substring(5 + hashLength);

            // Validate
            if (hash != Encryption.Hash.Create(hashType, data, sharedKey, true))
                return null;

            return new Digest(data, hash, hashType);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (_hash == null)
                throw new ArgumentException("_hash");
            if (!(obj is Digest))
                throw new ArgumentException("obj is not a Digest");

            var other = obj as Digest;
            string toString = other.ToString();
            if (toString == null)
                throw new ArgumentNullException("obj");
            return toString.Equals(ToString());
        }

        public override int GetHashCode()
        {
            if (_data == null)
                throw new ArgumentException("_data");
            return _data.GetHashCode();
        }

        #region Operator overloads

        public static bool operator ==(Digest a, Digest b)
        {
            if ((object)a == null)
                throw new ArgumentNullException("a");
            if ((object)b == null)
                throw new ArgumentNullException("b");
            return a.Equals(b);
        }

        public static bool operator ==(Digest a, string b)
        {
            if ((object)a == null)
                throw new ArgumentNullException("a");
            if (b == null)
                throw new ArgumentNullException("b");
            string toString = a.ToString();
            if (toString == null)
                throw new ArgumentNullException("a");
            return toString.Equals(b);
        }

        public static bool operator ==(string a, Digest b)
        {
            return (b == a);
        }

        public static bool operator !=(Digest a, string b)
        {
            return !(a == b);
        }

        public static bool operator !=(Digest a, Digest b)
        {
            return !(a == b);
        }

        public static bool operator !=(string a, Digest b)
        {
            return !(b == a);
        }

        #endregion
    }
}