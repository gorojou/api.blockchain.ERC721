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

        [HttpGet("createWallet/{userId}")]
        public string  createWallet( string userId)
        {
            
       /*     string Words = "ripple scissors kick mammal hire column oak again sun offer wealth tomorrow wagon turn fatal";
            string Password1 = "password";
           
            var wallet1 = new Nethereum.HdWallet.Wallet(Words, Password1);
            for (int i = 0; i < 10; i++)
            {
                  var account = wallet1.GetAccount(0); 
             //    Console.WriteLine("Account index : "+ i +" - Address : "+ account.Address +" - Private key : "+ account.PrivateKey);
            }
           
           return wallet1.GetAccount(0).ToString();
*/
         //  KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));
           

            
          // AzureKeyVaultExternalSigner keyVault = new AzureKeyVaultExternalSigner(keyVaultClient, BASESECRETURI);


          // DefaultAzureCredential defaultAzure = new DefaultAzureCredential();
             
           
           string password = "strongpassword";

            EthECKey key = EthECKey.GenerateKey();
            byte[] privateKey = key.GetPrivateKeyAsBytes();
            string address = key.GetPublicAddress();
            var keyStore = new KeyStoreScryptService();


            var credential = new DefaultAzureCredential();
        
      /*      
           
         var client = new KeyClient(new Uri(BASESECRETURI), new DefaultAzureCredential());
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
            
             //keyVault.SignAsync(privateKey).ConfigureAwait(false);

             

            return address;
        
        
        }

      [HttpPost("addNFT/{cardId}")]
        public IActionResult addNFT(string cardId)
        {

            PolygonClientIntegrationFixture polygon = new PolygonClientIntegrationFixture();
           
            
            
            var eRC = new ERC721PresetMinterPauserAutoIdImpl(polygon);

            eRC.ShouldDeployAndMintNFTToken();
         
            if (ModelState.IsValid)
            {
                return StatusCode(200);
            }

            return StatusCode(400);
        }

       [HttpPost("transferNFT/{userId}/{addressfrom}/{addressto}")]
        public IActionResult transferNFT(string userId, string addressfrom, string addressto)
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
             To = "0xEd84a6761ab9da64316D28D01C2b8e4f19eB1049"//,
             //TokenId = model.Idnft
             
             };

            

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
