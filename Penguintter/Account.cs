using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using CoreTweet;
using Penguintter.Setup;
using System.Windows;

namespace Penguintter
{
    [Serializable()]
    public class Account
    {
        public string AccessToken { get; set; }
        public string AccessSecret { get; set; }

        static public void CreateAccount()
        {
            var dialog = new AccountSetupWindow();
            dialog.ShowDialog();
            if (dialog.Token == null)
            {
                Environment.Exit(0);
            }
            new Account()
            {
                AccessToken = dialog.Token.AccessToken,
                AccessSecret = dialog.Token.AccessTokenSecret
            }.SaveAccount();
        }

        static public string AccountFileName { get; } = @"penguintter.account";
        public void SaveAccount()
        {
            using (var st = new System.IO.FileStream(AccountFileName, System.IO.FileMode.Create))
            {
                new BinaryFormatter().Serialize(st, this);
            }
        }

        static public Tokens LoadAccount()
        {
            Account account;
            try
            {
                using (var st = new System.IO.FileStream(AccountFileName, System.IO.FileMode.Open))
                {
                    account = (Account)new BinaryFormatter().Deserialize(st);
                }
                return Tokens.Create(
                    "*****",
                    "*****",
                    account.AccessToken,
                    account.AccessSecret);
            }
            catch
            {
                return null;
            }
        }
    }
}
