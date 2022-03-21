using System.Numerics;
using Nethereum.Signer;
using Nethereum.Util;
using NBitcoin;
using Nethereum.Web3;
using Nethereum.Web3.Accounts; 
using Nethereum.Hex.HexConvertors.Extensions; 
using Nethereum.HdWallet;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using WebApiMyGalleryPolygon.ContractsLibrary.ContractDefinition;
using WebApiMyGalleryPolygon.ContractsLibrary.ContractService;

namespace WebApiMyGalleryPolygon.ContractsLibrary.ContractsImpl
{
    [Collection(PolygonClientIntegrationFixture.POLYGON_CLIENT_COLLECTION_DEFAULT)]
    public class ERC721PresetMinterPauserAutoIdImpl
    {
        private readonly PolygonClientIntegrationFixture _polygonClientIntegrationFixture;

        public ERC721PresetMinterPauserAutoIdImpl(PolygonClientIntegrationFixture polygonClientIntegrationFixture)
        {
            _polygonClientIntegrationFixture = polygonClientIntegrationFixture;
        }

       
        [Fact]
        public async void ShouldDeployAndMintNFTToken()
        {

            //Using rinkeby to demo opensea, if we dont want to use the configured client
            var web3 = _polygonClientIntegrationFixture.GetInfuraWeb3(InfuraPolygon.Polygon, InfuraNetwork.Mumbai);
            //var web3 = _ethereumClientIntegrationFixture.GetWeb3();

/*
Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
string Password1 = "password";
var wallet1 = new Nethereum.HdWallet.Wallet(mnemo.ToString(), Password1);
for (int i = 0; i < 10; i++)
{
    var account = wallet1.GetAccount(i,null); 

}
 */   
            
            var erc721PresetMinter = new ERC721PresetMinterPauserAutoIdDeployment() {
                BaseURI = "gorojou/demo",
                //BaseURI = "https://my-json-server.typicode.com/juanfranblanco/samplenftdb/tokens/", 
                Name = "NFTArt", 
                Symbol = "NFA",
                Gas = 5100,
                Nonce = 2,
                GasPrice = Nethereum.Web3.Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei),
                FromAddress ="0x00BDa32084AB0190897b3B71f07BCd649f906bDb",
                AmountToSend = 00001};
                
                

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
             
            //Deploy the erc721Minter
        var deploymentReceipt = await ERC721PresetMinterPauserAutoIdService.DeployContractAndWaitForReceiptAsync(web3, erc721PresetMinter, cancellationTokenSource=null);
            

            var erc721PresetMinterService = new ERC721PresetMinterPauserAutoIdService(web3, deploymentReceipt.ContractAddress);

            var addressToGiveFirstToken = "0x00BDa32084AB0190897b3B71f07BCd649f906bDb";

            var mintReceipt = await erc721PresetMinterService.MintRequestAndWaitForReceiptAsync(
                addressToGiveFirstToken);

            //we have just minted our first nft so the nft will have the id of 0. 
            var ownerOfTokem = await erc721PresetMinterService.OwnerOfQueryAsync(0);

            
            Assert.True(ownerOfTokem.IsTheSameAddress(addressToGiveFirstToken));

            
            //Url format  https://testnets.opensea.io/assets/[nftAddress]/[id]

        }


    }
}