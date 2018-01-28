﻿using Coinbase;
using Coinbase.Models;
using Coinbase.Wallet;
using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voicecoin.Core.Tables;

namespace Voicecoin.Core
{
    public class Contribution
    {
        private Client coinbase;
        private Database dc;
        private String userId;

        public Contribution(String userId, Database dc, IConfiguration config)
        {
            coinbase = new Client(config.GetSection("Coinbase:Key").Value, config.GetSection("Coinbase:Secret").Value);
            this.dc = dc;
            this.userId = userId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currency">BTC</param>
        /// <param name="createIfNotExist">Create a new address if no address found</param>
        /// <returns></returns>
        public String GetAddress(CurrencyType currency, bool createIfNotExist)
        {
            // check if address exist
            var address = dc.Table<ContributorCurrencyAddress>().FirstOrDefault(x => x.UserId == userId && x.Currency == currency);

            if (address == null)
            {
                var accounts = coinbase.GetAccounts();
                var account = accounts.Data.FirstOrDefault(x => x.Currency.Code == currency.ToString());
                var addr = coinbase.CreateAddress(account.Id);

                dc.Table<ContributorCurrencyAddress>().Add(new ContributorCurrencyAddress
                {
                    UserId = userId,
                    Currency = currency,
                    Address = addr.Address
                });

                return addr.Address;
            }
            else
            {
                return address.Address;
            }
        }
    }
}
