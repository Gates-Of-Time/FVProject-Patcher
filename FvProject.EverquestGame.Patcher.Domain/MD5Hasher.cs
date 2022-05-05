using System.Security.Cryptography;
using System.Text;

namespace FvProject.EverquestGame.Patcher.Domain {
    public class MD5Hasher 
    {
        // https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file
        public static string CreateHash(Stream dataStream) {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(dataStream);
            var sb = new StringBuilder();
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
