namespace EasyBank.Migrations
{
    using EasyBank.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EasyBank.Models.ConnectionContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EasyBank.Models.ConnectionContext context)
        {
            var clients = new List<Client>
            {
                new Client{Name="Borys", Surname="Symonenko", PIdNumber="1234567895", Email="symonenkob@gmail.com", BirthDate=DateTime.Parse("15.07.1995"), RegistrationDate=DateTime.Now, Accounts=new List<Account>()},
                new Client{Name="Ivan", Surname="Dontknow", PIdNumber="2234567895",Email="some@mail.com", BirthDate=DateTime.Parse("15.03.2000"), RegistrationDate=DateTime.Now, Accounts=new List<Account>()},
                new Client{Name="Aleksey", Surname="Nikonov", PIdNumber="3234567895",Email="some@gmail.com", BirthDate=DateTime.Parse("19.05.1989"), RegistrationDate=DateTime.Now, Accounts=new List<Account>()}
            };
            clients.ForEach(c => context.Clients.AddOrUpdate(p => p.Email, c));
            context.SaveChanges();

            var currencies = new List<Currency>
            {
                //new Currency {CurrencyId = 0, CurrencyName = "None"},
                new Currency {CurrencyId = 1, CurrencyName = "UAH"},
                new Currency {CurrencyId = 2, CurrencyName = "USD"},
                new Currency {CurrencyId = 3, CurrencyName = "EUR"}
            };
            currencies.ForEach((c => context.Currencies.AddOrUpdate(p => p.CurrencyName, c)));
            context.SaveChanges();

            var accoutStatuses = new List<AccountStatus>
            {
                //new AccountStatus {StatusId = 0, StatusName = "None"},
                new AccountStatus {StatusId = 1, StatusName = "Normal"},
                new AccountStatus {StatusId = 2, StatusName = "Blocked"},
                new AccountStatus {StatusId = 3, StatusName = "Frozen"},
                new AccountStatus {StatusId = 4, StatusName = "Expired"}
            };
            accoutStatuses.ForEach(s => context.AccountStatuses.AddOrUpdate(i => i.StatusName, s));
            context.SaveChanges();

            var accoutTypes = new List<AccountType>
            {
                //new AccountType() {TypeId = 0, TypeName = "None"},
                new AccountType {TypeId = 1, TypeName = "Normal"},
                new AccountType {TypeId = 2, TypeName = "Deposit"},
                new AccountType {TypeId = 3, TypeName = "Credit"}
            };
            accoutTypes.ForEach(t => context.AccountTypes.AddOrUpdate(i => i.TypeName, t));
            context.SaveChanges();

            var accounts = new List<Account>
            {
                new Account{ 
                    ExpirationDate=DateTime.MaxValue.Date, Amount=23444, ClientId=clients.First(c=>c.Surname=="Symonenko").ClientId,
                    TypeId=accoutTypes.First(t=>t.TypeName=="Normal").TypeId, CurrencyId=currencies.First(c=>c.CurrencyName=="UAH").CurrencyId, 
                    StatusId=accoutStatuses.First(s=>s.StatusName=="Normal").StatusId
                },
                new Account{
                    ExpirationDate=DateTime.MaxValue.Date, Amount=3453, ClientId=clients.First(c=>c.Surname=="Symonenko").ClientId,
                    TypeId=accoutTypes.First(t=>t.TypeName=="Deposit").TypeId, CurrencyId=currencies.First(c=>c.CurrencyName=="USD").CurrencyId, 
                    StatusId=accoutStatuses.First(s=>s.StatusName=="Normal").StatusId
                },
                new Account{
                    ExpirationDate=DateTime.MaxValue.Date, Amount=2323, ClientId=clients.First(c=>c.Surname=="Dontknow").ClientId,
                    TypeId=accoutTypes.First(t=>t.TypeName=="Normal").TypeId, CurrencyId=currencies.First(c=>c.CurrencyName=="EUR").CurrencyId, 
                    StatusId=accoutStatuses.First(s=>s.StatusName=="Frozen").StatusId
                },
                new Account{
                    ExpirationDate=DateTime.MaxValue.Date, Amount=6654, ClientId=clients.First(c=>c.Surname=="Nikonov").ClientId,
                    TypeId=accoutTypes.First(t=>t.TypeName=="Credit").TypeId, CurrencyId=currencies.First(c=>c.CurrencyName=="USD").CurrencyId, 
                    StatusId=accoutStatuses.First(s=>s.StatusName=="Normal").StatusId
                }
            };
            accounts.ForEach(a=>context.Accounts.AddOrUpdate(i=>i.AccountNumber, a));
            context.SaveChanges();
        }
    }
}
