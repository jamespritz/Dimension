using intrinsic.security.encryption;
using Xunit;


namespace test.security {

    public class EncryptionServiceTest {

        [Fact]
        public void EncryptIsRandom() {

            EncryptionService svc = new EncryptionService();
            EncryptedValue a = svc.Encrypt("samepasscode", "somerandomdata");
            EncryptedValue b = svc.Encrypt("samepasscode", "somerandomdata");

            Assert.NotEqual(a.Base64EncryptedValue, b.Base64EncryptedValue);
        }

        [Fact]
        public void DecryptSuccess() {

            EncryptionService svc = new EncryptionService();
            EncryptedValue a = svc.Encrypt("C0mpl3xPa55w04d", "some random payload");

            EncryptedValue b = svc.Encrypt("C0mpl3xPa55w04d", "some random payload", a.Base64Salt, a.Base64IV);
            Assert.Equal(a.Base64EncryptedValue, b.Base64EncryptedValue);


        }

    }
}
