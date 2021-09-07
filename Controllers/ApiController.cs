using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.HdWallet;
using Nethereum.JsonRpc.Client;
using Nethereum.StandardTokenEIP20;
using Nethereum.Signer;
using Nethereum.KeyStore;
using WebApiMyGalleryPolygon.Interfaces;
using Nethereum.Signer.AzureKeyVault;
using Newtonsoft.Json;
using Microsoft.Azure.KeyVault;
using System.Net.Http;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xunit;
using WebApiMyGalleryPolygon.ContractsLibrary.ContractsImpl;
using WebApiMyGalleryPolygon.ContractsLibrary.ContractService;
using System.Threading;
using WebApiMyGalleryPolygon.ContractsLibrary.ContractDefinition;
using System.Net;
using System.IO;
using Nethereum.RPC;
using Nethereum.Web3.Accounts.Managed;

namespace WebApiMyGalleryPolygon.Controllers
{

    
    [ApiController]
    [Route("[controller]")]
    [Collection(PolygonClientIntegrationFixture.POLYGON_CLIENT_COLLECTION_DEFAULT)]
    public class ApiController : ControllerBase
    {
      
       

      // const string BASESECRETURI = "https://mygallerykeyvault.vault.azure.net/keys/myGalleryKeyTes/c5b1c3b7f69346698d9da5b43fecd818";
       private readonly ILogger<ApiController> _logger;
        

       //Account DefaultAccount => walletManager.Wallet?.GetAccount(0);
       Account DefaultAccount;

       //public string DefaultAccountAddress => DefaultAccount?.Address;

        readonly IWalletManager walletManager;
        StandardTokenService standardTokenService;
        Web3 web3;

        public static string APP_ID= "37a24f97-0b68-4d78-baf9-038224b644ec";
        public static string APP_PASSWORD = "P3RpF4Ux2KHX9aoMAk4tUJtn8A5bAECCo/OmnwyeIW8=";
        public static string TENANTID = "6a3ee749-b708-41fa-8aba-46beca6fa849"; 
        public static string BASESECRETURI = "https://mygallerykeyvault.vault.azure.net/"; 
         

        public ApiController(ILogger<ApiController> logger)
        {
            _logger = logger;

             //Initialize();

        }
        void Initialize()
        {
            const string CONTRACT_ADDRESS="0xefFa48c77D2E5a44Cc183D6A5D256FA47fCcDb25";

         
            var client = new  RpcClient(new Uri("https://polygon-mumbai.infura.io/v3/46d94a2d40114cbaad5a51ab07d02737"));
            

            //web3 = new Web3(DefaultAccount, client);
            web3 = new Web3(DefaultAccount, client);
            
            standardTokenService = new StandardTokenService(web3, CONTRACT_ADDRESS);
        }
        
       public static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);

            var  clientCred = new ClientCredential(APP_ID, APP_PASSWORD);
            
            var result = await authContext.AcquireTokenAsync(resource,clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }

        [HttpGet("createWallet")]
        public string  createWallet()
        {
            
            //string Words = "ripple scissors kick mammal hire column oak again sun offer wealth tomorrow wagon turn fatal";
            //string Password1 = "password";
           
           // var wallet1 = new Wallet(Words, Password1);
            //for (int i = 0; i < 10; i++)
           // {
                  //var account = wallet1.GetAccount(0); 
             //    Console.WriteLine("Account index : "+ i +" - Address : "+ account.Address +" - Private key : "+ account.PrivateKey);
            //}
           
           //return wallet1.GetAccount(0).ToString();

           KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));
           

            
           AzureKeyVaultExternalSigner keyVault = new AzureKeyVaultExternalSigner(keyVaultClient, BASESECRETURI);


           DefaultAzureCredential defaultAzure = new DefaultAzureCredential();
           
           
           string password = "strongpassword";

            EthECKey key = EthECKey.GenerateKey();
            byte[] privateKey = key.GetPrivateKeyAsBytes();
            string address = key.GetPublicAddress();
            var keyStore = new KeyStoreScryptService();


            var credential = new DefaultAzureCredential();
        
            
           
        /*    var client = new KeyClient(new Uri(BASESECRETURI), new DefaultAzureCredential());
            string rsaKeyName = $"MyGalleryKey-{Guid.NewGuid()}";
            
            var rsaKey = new CreateRsaKeyOptions(rsaKeyName, hardwareProtected: false)
            {
                KeySize = 2048,
                ExpiresOn = DateTimeOffset.Now.AddYears(1)
            };

            client.CreateRsaKey(rsaKey);
     */

            string json = keyStore.EncryptAndGenerateKeyStoreAsJson(
                password: password,
                privateKey: privateKey,
                addresss: address);
            
             
;
            
             keyVault.SignAsync(privateKey).ConfigureAwait(false);

             

            return address;
        
        
        }

        [HttpPost("addNFT")]
        
        public async Task<IActionResult> addNFT()
        {
            var url = $"https://polygon-mumbai.infura.io/v3/46d94a2d40114cbaad5a51ab07d02737";
               
              var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("igor.miquilena@gmail.com:imiquilena")));
                request.PreAuthenticate = true;
                request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml");
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("x-nhia-apikey","b4dfa941a2c84793b22935183f0f01a4");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                request.Headers.Add("Accept-Charset", "ISO-8859-1");
                request.Credentials = new NetworkCredential() {
                UserName = "igor.miquilena@gmail.com",
                Password = "imiquilena"
               
                };
                
             // request.UserAgent = "curl";
request.Accept = "*/*";
//System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

//System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
  //System.Net.ServicePointManager.SecurityProtocol |= 
    //SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;   
    ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 |
(SecurityProtocolType)768 | (SecurityProtocolType)3072;     

               
                 try
                    {
                        var handler = new HttpClientHandler();

                        handler.ServerCertificateCustomValidationCallback += 
                                       (sender, certificate, chain, errors) =>
                                        {
                                            return true;
                                        };
                        var web3 = new Web3("https://polygon-mumbai.infura.io/v3/46d94a2d40114cbaad5a51ab07d02737");
                        
                       // var balance = await web3.Eth.GetBalance.SendRequestAsync("0x00BDa32084AB0190897b3B71f07BCd649f906bDb").ConfigureAwait(false);
                      //  var privateKey = "bb0dd811e80a5a6fbd996020cecacbdb810b20b3839a7a372112378a95ae6dff";
                    //    var account = new Account(privateKey);    
                   //    var networkId = await web3.Net.Version.SendRequestAsync();
                 //  var senderAddress = "0x12890d2cce102216644c59daE5baed380d84830c";
                 var password = "imiquilena";

                //var accountman = new ManagedAccount(senderAddress, password);
               // var web3Account = new Web3(account);
                       var toAddress = "0xEd84a6761ab9da64316D28D01C2b8e4f19eB1049";
var senderAddress = "0x00BDa32084AB0190897b3B71f07BCd649f906bDb";

var account = new ManagedAccount(senderAddress, password);
 //web3 = new Nethereum.Web3.Web3(account, "http://127.0.0.1:8545");

                       //var transaction = await web3.TransactionManager.SendTransactionAsync(account.Address, toAddress, new Nethereum.Hex.HexTypes.HexBigInteger(1));
              
              var unlocked = web3.Personal.UnlockAccount;
             // var prueba =  await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress,password,new Nethereum.Hex.HexTypes.HexBigInteger(1)); 
               var receipt =  web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync("0x2e8983b5225a9c01464d75db65cab9dd0f4b28b15d093938ef4851d90183ed9b").Result;
        Console.WriteLine($"Status: {receipt.Status}");
               var prueba = await web3.Eth.ProtocolVersion.SendRequestAsync();
               
               
                        using (WebResponse response = request.GetResponse())
                        {
                            PolygonClientIntegrationFixture polygon = new PolygonClientIntegrationFixture();
           
                            var eRC = new ERC721PresetMinterPauserAutoIdImpl(polygon);

                            eRC.ShouldDeployAndMintNFTToken();

                            using (Stream strReader = response.GetResponseStream())
                            {
                                if (strReader == null) //return;
                                using (StreamReader objReader = new StreamReader(strReader))
                                {
                                string responseBody = objReader.ReadToEnd();
                                // Do something with responseBody
                                
                                Console.WriteLine(responseBody);
                                
                               }
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        return StatusCode(400);
                    }
         
         
            if (ModelState.IsValid)
            {
                return StatusCode(200);
            }
           
           return StatusCode(400);
            
        }

       [HttpPost("transferNFT/{model}")]
        public IActionResult transferNFT(Cards model)
        {


            PolygonClientIntegrationFixture polygon = new PolygonClientIntegrationFixture();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var web3 = polygon.GetWeb3();
        
            var contract = polygon.GetAccount();

            var eRC = new ERC721PresetMinterPauserAutoIdService(web3, contract.ToString());

            var transfer = new TransferFromFunction()
            {FromAddress = "0x00BDa32084AB0190897b3B71f07BCd649f906bDb",
             Gas = 2100,
             Nonce = 2,
             GasPrice = Nethereum.Web3.Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei),
             To = "0xEd84a6761ab9da64316D28D01C2b8e4f19eB1049",
             TokenId = model.Idnft};

            

            eRC.TransferFromRequestAndWaitForReceiptAsync ( transfer, cancellationTokenSource=null);

        
         
            if (ModelState.IsValid)
            {
                return StatusCode(200);
            }

            return StatusCode(400);
        }

        [HttpGet("getOpenSea/{address}/{id}")]
        public ActionResult  getOpenSea(string address, string id)
        {

             //Url format  https://testnets.opensea.io/assets/[nftAddress]/[id]

            
        if (ModelState.IsValid)
            {
                return StatusCode(200);
            }

            return StatusCode(400);
        
        }
       
    }
}
