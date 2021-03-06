﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using EntityFrameworkCore.BootKit;
using Voicecoin.Core.Coupons;

namespace Voicecoin.Core
{
    public class HookDbInitializer : IHookDbInitializer
    {
        public int Priority => 1000;

        public void Load(IConfiguration config, Database dc)
        {
            InitCurrency(config, dc);
            InitTokenPrice(config, dc);
            InitReceiveAddress(config, dc);
            InitCoupon(config, dc);
            InitCommonDataCountry(config, dc);
            InitCommonDataUsStates(config, dc);
        }

        private void InitCurrency(IConfiguration config, Database dc)
        {
            if (dc.Table<Cryptocurrency>().Any()) return;

            string json = File.ReadAllText(Database.ContentRootPath + $"{Path.DirectorySeparatorChar}App_Data{Path.DirectorySeparatorChar}DbInitializer{Path.DirectorySeparatorChar}Voicecoin.Cryptocurrency.json");
            var cryptocurrency = JsonConvert.DeserializeObject<Cryptocurrency>(json);
            dc.Table<Cryptocurrency>().Add(cryptocurrency);
        }

        private void InitTokenPrice(IConfiguration config, Database dc)
        {
            if (dc.Table<TokenPrice>().Any()) return;

            string json = File.ReadAllText(Database.ContentRootPath + $"{Path.DirectorySeparatorChar}App_Data{Path.DirectorySeparatorChar}DbInitializer{Path.DirectorySeparatorChar}Voicecoin.TokenPrice.json");
            var icoinfos = JsonConvert.DeserializeObject<List<TokenPrice>>(json);
            dc.Table<TokenPrice>().AddRange(icoinfos);
        }

        private void InitReceiveAddress(IConfiguration config, Database dc)
        {
            if (dc.Table<TokenWalletAddress>().Any()) return;

            dc.Table<TokenWalletAddress>().Add(new TokenWalletAddress
            {
                UserId = IdConstants.RootAccountId,
                Address = "0x7eb040bD9b4C2aE6c0E7D8abF75c17dA93A70990",
                Currency = "ETH"
            });

            dc.Table<TokenWalletAddress>().Add(new TokenWalletAddress
            {
                UserId = IdConstants.RootAccountId,
                Address = "3EvJEoembUezSzafphNCoHerT4DrBziUAU",
                Currency = "BTC"
            });
        }

        private void InitCoupon(IConfiguration config, Database dc)
        {
            if (dc.Table<Coupon>().Any()) return;
            string json = File.ReadAllText(Database.ContentRootPath + $"{Path.DirectorySeparatorChar}App_Data{Path.DirectorySeparatorChar}DbInitializer{Path.DirectorySeparatorChar}Voicecoin.Coupon.json");
            var coupnons = JsonConvert.DeserializeObject<List<Coupon>>(json);
            dc.Table<Coupon>().AddRange(coupnons);
        }


        /// <summary>
        /// https://github.com/OpenBookPrices/country-data/tree/master/data
        /// </summary>
        /// <param name="config"></param>
        /// <param name="dc"></param>
        private void InitCommonDataCountry(IConfiguration config, Database dc)
        {
            if (dc.Table<Country>().Any()) return;

            string json = File.ReadAllText(Database.ContentRootPath + $"{Path.DirectorySeparatorChar}App_Data{Path.DirectorySeparatorChar}DbInitializer{Path.DirectorySeparatorChar}Common.Countries.json");
            var countries = JsonConvert.DeserializeObject<List<JObject>>(json);

            countries.ForEach(country => {
                dc.Table<Country>().Add(new Country
                {
                    Name = country["name"].ToString(),
                    Code2 = country["alpha_2_code"].ToString(),
                    Code3 = country["alpha_3_code"].ToString(),
                    Nationality = country["nationality"].ToString()
                });
            });
        }

        private void InitCommonDataUsStates(IConfiguration config, Database dc)
        {
            if (dc.Table<State>().Any()) return;

            string json = File.ReadAllText(Database.ContentRootPath + $"{Path.DirectorySeparatorChar}App_Data{Path.DirectorySeparatorChar}DbInitializer{Path.DirectorySeparatorChar}Common.States-US.json");
            var states = JsonConvert.DeserializeObject<List<JObject>>(json);

            states.ForEach(state => {
                dc.Table<State>().Add(new State
                {
                    Name = state["name"].ToString(),
                    Abbr = state["abbr"].ToString(),
                    CountryCode = "US"
                });
            });

            json = File.ReadAllText(Database.ContentRootPath + $"{Path.DirectorySeparatorChar}App_Data{Path.DirectorySeparatorChar}DbInitializer{Path.DirectorySeparatorChar}Common.States-CN.json");
            states = JsonConvert.DeserializeObject<List<JObject>>(json);

            states.ForEach(state => {
                dc.Table<State>().Add(new State
                {
                    Name = state["name"].ToString(),
                    Abbr = state["abbr"].ToString(),
                    CountryCode = "CN"
                });
            });
        }
    }
}
