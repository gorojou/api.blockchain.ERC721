using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Web3.Accounts.Managed;

namespace WebApiMyGalleryPolygon
{
    public enum PolygonClient
    {
        Geth,
        OpenEthereum,
        Ganache,
        Infura,
        External
    }

     public enum InfuraPolygon
    {
        Polygon
    }

    public enum InfuraNetwork
    {
        Mumbai,

        Rinkeby,
        Kovan,
        Mainnet
    }

    public class PolygonClientIntegrationFixture : IDisposable
    {
        public const string POLYGON_CLIENT_COLLECTION_DEFAULT = "Polygon client Test";
        public static string GethClientPath { get; set; } = @"..\..\..\..\testchain\gethclique\geth.exe";
        public static string ParityClientPath { get; set; } = @"..\..\..\..\testchain\openethereumpoa\openethereum.exe";
        public static string AccountPrivateKey { get; set; } = "bb0dd811e80a5a6fbd996020cecacbdb810b20b3839a7a372112378a95ae6dff";
        public static string AccountAddress { get; set; } = "0x00BDa32084AB0190897b3B71f07BCd649f906bDb";
        public static string ManagedAccountPassword { get; set; } = "imiquilena";
        public static string InfuraId { get; set; } = "46d94a2d40114cbaad5a51ab07d02737";

        public static InfuraPolygon InfuraPolygon { get; set; } = InfuraPolygon.Polygon;
        public static InfuraNetwork InfuraNetwork { get; set; } = InfuraNetwork.Mumbai;

       
        public static string HttpUrl { get; set; } = "http://localhost:3000";
        public static System.Numerics.BigInteger ChainId { get; set; } = 80001;//444444444500;

        public  Account GetAccount()
        {
            return new Account(AccountPrivateKey, ChainId);
        }

        public static ManagedAccount GetManagedAccount()
        {
            return new ManagedAccount(AccountAddress, ManagedAccountPassword);
        }

        private readonly Process _process;
        private readonly string _exePath;

        public string GetInfuraUrl(InfuraPolygon infuraPolygon, InfuraNetwork infuraNetwork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
 
            return "https://" + Enum.GetName(typeof(InfuraPolygon), infuraPolygon).ToLower()+"-"+ Enum.GetName(typeof(InfuraNetwork), infuraNetwork).ToLower()+ ".infura.io/v3/" + InfuraId;
        }

        public Web3 GetInfuraWeb3(InfuraPolygon infuraPolygon, InfuraNetwork infuraNetwork)
        {
            return new Web3 (new Account(AccountPrivateKey), GetInfuraUrl(infuraPolygon,infuraNetwork));
        }

        private string GetHttpUrl()
        {
            if (PolygonClient == PolygonClient.Infura)
            {
                return GetInfuraUrl(InfuraPolygon,InfuraNetwork);
            }
            else
            {
                return HttpUrl;
            }
        }

        private Web3 _web3;
        public Web3 GetWeb3()
        {
            if (_web3 == null)
            {
                _web3 = new Web3 (GetAccount(), GetHttpUrl());
            }

            return _web3;
        }

        public PolygonClient PolygonClient { get; private set; } = PolygonClient.Infura;

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.test.json", true)
                .Build();
            return config;
        }

        public class PolygonTestSettings
        {
            public string GethPath { get; set; }
            public string ParityPath { get; set; }

            public string AccountAddress { get; set; }

            public string AccountPrivateKey { get; set; }
            public string ManagedAccountPassword { get; set; }

            public string ChainId { get; set; }

            public string Client { get; set; }

            public string InfuraNetwork { get; set; }
            public string InfuraId { get; set; }

            public string HttpUrl { get; set; }
        }

        public PolygonClientIntegrationFixture()
        {

            var config = InitConfiguration();
            if (config != null)
            {

                var polygonTestSection = config.GetSection("PolygonTestSettings");

                if (polygonTestSection != null)
                {
                    var polygonTestSettings = new PolygonTestSettings();
                    polygonTestSection.Bind(polygonTestSettings);
                    if (!string.IsNullOrEmpty(polygonTestSettings.GethPath)) GethClientPath = polygonTestSettings.GethPath;
                    if (!string.IsNullOrEmpty(polygonTestSettings.ParityPath)) ParityClientPath = polygonTestSettings.ParityPath;
                    if (!string.IsNullOrEmpty(polygonTestSettings.AccountAddress)) AccountAddress = polygonTestSettings.AccountAddress;
                    if (!string.IsNullOrEmpty(polygonTestSettings.AccountPrivateKey)) AccountPrivateKey = polygonTestSettings.AccountPrivateKey;
                    if (!string.IsNullOrEmpty(polygonTestSettings.ChainId)) ChainId = BigInteger.Parse(polygonTestSettings.ChainId);
                    if (!string.IsNullOrEmpty(polygonTestSettings.ManagedAccountPassword)) ManagedAccountPassword = polygonTestSettings.ManagedAccountPassword;
                    if (!string.IsNullOrEmpty(polygonTestSettings.Client)) PolygonClient = (PolygonClient)Enum.Parse(typeof(PolygonClient), polygonTestSettings.Client);
                    if (!string.IsNullOrEmpty(polygonTestSettings.InfuraNetwork)) InfuraNetwork = (InfuraNetwork)Enum.Parse(typeof(InfuraNetwork), polygonTestSettings.InfuraNetwork); ;
                    if (!string.IsNullOrEmpty(polygonTestSettings.InfuraId)) InfuraId = polygonTestSettings.InfuraId;
                    if (!string.IsNullOrEmpty(polygonTestSettings.HttpUrl)) HttpUrl = polygonTestSettings.HttpUrl;
                }
            }

            var client = Environment.GetEnvironmentVariable("POLYGON_CLIENT");

            if (client == null)
            {
                Console.WriteLine("**************TEST CLIENT NOT CONFIGURED IN ENVIRONMENT USING DEFAULT");
            }
            else
            {
                Console.WriteLine("************ENVIRONMENT CONFIGURED WITH CLIENT: " + client.ToString());
            }

            if (string.IsNullOrEmpty(client))
            {

            }
           // else if (client == "geth")
           // {
            //    PolygonClient = PolygonClient.Geth;
            //    Console.WriteLine("********TESTING WITH GETH****************");
           // }
           // else if (client == "parity")
           // {
           //     PolygonClient = PolygonClient.OpenEthereum;
           //     Console.WriteLine("******* TESTING WITH PARITY ****************");
           // }
           // else if (client == "ganache")
           // {
             //   PolygonClient = PolygonClient.Ganache;
               // Console.WriteLine("******* TESTING WITH GANACHE ****************");
            //}

            // if (PolygonClient == PolygonClient.Geth)
            // {

            //     var location = typeof(PolygonClientIntegrationFixture).GetTypeInfo().Assembly.Location;
            //     var dirPath = Path.GetDirectoryName(location);
            //     _exePath = Path.GetFullPath(Path.Combine(dirPath, GethClientPath));

            //     DeleteData();

            //     var psiSetup = new ProcessStartInfo(_exePath,
            //         @" --datadir=devChain init genesis_clique.json ")
            //     {
            //         CreateNoWindow = false,
            //         WindowStyle = ProcessWindowStyle.Normal,
            //         UseShellExecute = true,
            //         WorkingDirectory = Path.GetDirectoryName(_exePath)

            //     };

            //     Process.Start(psiSetup);
            //     Thread.Sleep(3000);

            //     var psi = new ProcessStartInfo(_exePath,
            //         @" --nodiscover --rpc --datadir=devChain  --rpccorsdomain "" * "" --mine --rpcapi ""eth, web3, personal, net, miner, admin, debug"" --rpcaddr ""0.0.0.0"" --allow-insecure-unlock --unlock 0x12890d2cce102216644c59daE5baed380d84830c --password ""pass.txt""  --ws  --wsaddr ""0.0.0.0"" --wsapi ""eth, web3, personal, net, miner, admin, debug"" --wsorigins "" * "" --verbosity 0 console  ")
            //     {
            //         CreateNoWindow = false,
            //         WindowStyle = ProcessWindowStyle.Normal,
            //         UseShellExecute = true,
            //         WorkingDirectory = Path.GetDirectoryName(_exePath)

            //     };
            //     _process = Process.Start(psi);
            // }
            // else if (PolygonClient == PolygonClient.OpenEthereum)
            // {

            //     var location = typeof(PolygonClientIntegrationFixture).GetTypeInfo().Assembly.Location;
            //     var dirPath = Path.GetDirectoryName(location);
            //     _exePath = Path.GetFullPath(Path.Combine(dirPath, ParityClientPath));

            //     DeleteData();

            //     var psi = new ProcessStartInfo(_exePath,
            //         @" --config node0.toml") // --logging debug")
            //     {
            //         CreateNoWindow = false,
            //         WindowStyle = ProcessWindowStyle.Normal,
            //         UseShellExecute = true,
            //         WorkingDirectory = Path.GetDirectoryName(_exePath)

            //     };
            //     _process = Process.Start(psi);
            //     Thread.Sleep(10000);
            // }
            // else if (PolygonClient == PolygonClient.Ganache)
            // {
            //     var psi = new ProcessStartInfo("ganache-cli")
            //     {
            //         CreateNoWindow = false,
            //         WindowStyle = ProcessWindowStyle.Normal,
            //         UseShellExecute = true,
            //         WorkingDirectory = _exePath,
            //         Arguments = " --account=" + AccountPrivateKey + ",10000000000000000000000"

            //     };
            //     _process = Process.Start(psi);
            //     Thread.Sleep(10000);
            // }


            Thread.Sleep(3000);
        }

        public void Dispose()
        {
            if (!_process.HasExited)
            {
                _process.Kill();
            }

            Thread.Sleep(2000);
            DeleteData();
        }

        private void DeleteData()
        {
            var attempts = 0;
            var success = false;

            while (!success && attempts < 2)
            {
                try
                {
                    InnerDeleteData();
                    success = true;
                }
                catch
                {
                    Thread.Sleep(1000);
                    attempts = attempts + 1;
                }
            }
        }

        private void InnerDeleteData()
        {
            if (PolygonClient == PolygonClient.Geth)
            {
                var pathData = Path.Combine(Path.GetDirectoryName(_exePath), @"devChain\geth");

                if (Directory.Exists(pathData))
                {
                    Directory.Delete(pathData, true);
                }

            }
            else if (PolygonClient == PolygonClient.OpenEthereum)
            {
                var pathData = Path.Combine(Path.GetDirectoryName(_exePath), @"parity0\chains");

                if (Directory.Exists(pathData))
                {
                    Directory.Delete(pathData, true);
                }
            }

        }
    }
}