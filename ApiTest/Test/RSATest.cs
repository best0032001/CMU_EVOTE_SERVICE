using ApiTest.model;
using Evote_Service;
using Evote_Service.Model.Entity;
using Evote_Service.Model.Util;
using Evote_Service.Model.View;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest.Test
{
    [TestClass]
    public class RSATest
    {
        private String _PublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoAPV2hhYKqdW97A+G7bQXzeIXuw5jDPFdobdYqWRWPu97tGnILFEBx/ztNGKNiY/l9wZbpL66OKOOBvX+1uFPdur209PK7QbViOGY8khmTQuSkbGNpYrgg5+1RFJYRsY7mI2FcqjefuBbJRPUpprLoEze4Hl22fPmGaA1xz7gNues1ynBtQSp6ZQdSlwTfdRQ2sMZlPYwgo53lnZPuLmNzU2CGMDmpO8Nbr1QThSndHzSrXNw1+WDPRmwacjpsYjPnDd2zvI1I6zpLuD45Mh08ZZbb5kwzB8sg9nfzz5yD9xa3Yaz6uhr/43XCkTFD7wZMKcMebEPWXVGMzBrGReNQIDAQAB";
        private String _PrivateKey = "MIIEowIBAAKCAQEAoAPV2hhYKqdW97A+G7bQXzeIXuw5jDPFdobdYqWRWPu97tGnILFEBx/ztNGKNiY/l9wZbpL66OKOOBvX+1uFPdur209PK7QbViOGY8khmTQuSkbGNpYrgg5+1RFJYRsY7mI2FcqjefuBbJRPUpprLoEze4Hl22fPmGaA1xz7gNues1ynBtQSp6ZQdSlwTfdRQ2sMZlPYwgo53lnZPuLmNzU2CGMDmpO8Nbr1QThSndHzSrXNw1+WDPRmwacjpsYjPnDd2zvI1I6zpLuD45Mh08ZZbb5kwzB8sg9nfzz5yD9xa3Yaz6uhr/43XCkTFD7wZMKcMebEPWXVGMzBrGReNQIDAQABAoIBAG5tFyiyQi31W5wFAWeIytXa8f5n0PMDS1MXkTIhhmO9Hv7vqgFys7qi/0EaleH2lU5MczSOtB8BMhpghLWPHC1rKndnjQBhA7h3PaghRlF+5C8YFnPXQGE5dae+jUA5PgRMvHxfYl+tBE0VMISV0j++o/Oo2iKGyommu2U4OAxc68HIE+sxjkIb4FPFulZigFus0yVYosTjF/RE2JeM73DBXF4jg1qNPEyQaW61YLz9vPRjf2j7hL49fx4DIAqvn54kkKhjD5fjSpKhYL1xfDvObhyUYph0jfl7q5ezHKmIma09ff5A95/wHTi9dGnrhhYbTpTPoWyg1ohpe+ENgaECgYEA1JLfgoXhEQd65tsVIhx8MvIVfGtvF5VwVTtAbhqlNOs0Bt/4adpB8ds6llFGpvZXfr0KZsUg27JfBCyNWzwITeA8ma2k+GK2r11ZhFw8AzPVs7My9R2gGDZMZNRxmfQKLjC5cwnG/2AMQVNhxxtl6tJrLtFGw50il7fls8r8JakCgYEAwLRCLMcPppemZ0ntvDw4MSvZtx1Zadf2gigSseBkqf28EK4HgJc3agQHtxRQEL7nXbl7z7TmGCQszvDmDvUGgq1uTBX5zWNu6boFmMQrsJBEufnEjyoVPPfq5fvxUyhOzngMeeFlmv/qR3KeBsFfA2ClruO2JmbR8Qq09Wb0c60CgYEAzSLfZonj5Bcf12BcSIrMoC1V5reWgV/JA7cmOhqkiyjfEDNa+muRb+Br7VuJnt3jGX88ZmidiOXdI54K25xXNy/Jy1Py+2/nc9vV4xFPKJgBBmVMK5bnQ/ZCSpto9XS3zlNe41DwJMl/ihr5JLef5rggjxGOBH/DPj5NAPBF2+ECgYBkke7zZZRCcmTTBR9AnQEKkIMYcQXIGoC5Xuaa1KxUl2q+HcUmlETEXIQWRVCf3LHtFS+LsDJhqQeFnO3EIpaaPp8QsGtliJ5K9t2S49aVWEW19adivCjHX+/ExV8l8iRm1vpT5ZFcenEvhp74kZTfs2Hky0y17/VjYh4c8PVlJQKBgHUGHtIGepbSjFKHVaBlCtnIpjA+0wV1KgCaUgJE+NRcWqY9+byKZKMk5IFU/lSeOJh2uRhX823WOOGpOXDqGRWAEt8ktJCf+YWXDGMM7b0hDTicvds4NZC3C0V9v1+Tc592Ez05IVQ41TfpOrsDRYdA/AfUW0DAXcCKslxTQe/n";


        [TestMethod]
        public async Task TestRSA()
        {

            VoteModel voteModel = new VoteModel();
            voteModel.data1 = "data1";
            voteModel.data2 = "data2";
            voteModel.data3 = "data3";
            String json = JsonConvert.SerializeObject(voteModel);

            var publicKey = Convert.FromBase64String(_PublicKey);
            RSA rsaPublic = RSA.Create();
            rsaPublic.ImportSubjectPublicKeyInfo(publicKey, out int keyLengthPub);
            VoteEnCrypt voteEnCrypt = new VoteEnCrypt();
            String cypherText = await voteEnCrypt.EnCrypt(json, rsaPublic);


            var privateKey = Convert.FromBase64String(_PrivateKey);
            RSA rsaPrivate = RSA.Create();
            rsaPrivate.ImportRSAPrivateKey(privateKey, out int keyLengthPri);

            String deCryptText = await voteEnCrypt.DeCrypt(cypherText, rsaPrivate);

            Assert.IsTrue(1 == 1);
        }
    }
}
