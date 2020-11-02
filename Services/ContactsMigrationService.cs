using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class ContactsMigrationService : MigrationService
    {
        private int _total;

        private readonly object _lock = new object();

        public ContactsMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
        }
        public override async Task StartMigratingAsync()
        {
            await using var webContactContext = new WebContactContext();
            await using var parsedContactContext = new ParsedContactContext();
            _total = parsedContactContext.ContactsDtos.Count();
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();

            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(Migrate(i));
            }

            await Task.WhenAll(tasks);
            await parsedContactContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.contacts restart identity cascade;");
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("started thread");
            await using var webContactContext = new WebContactContext();
            await using var parsedContactContext = new ParsedContactContext();

            foreach (var dto in parsedContactContext.ContactsDtos.Where(x => x.Id % NumOfThreads == threadNum))
            {
                if (dto.Bin != null)
                {
                    if (dto.Telephone != null)
                        await webContactContext.ContactTelephones
                            .UpsertRange(AddTel(dto.Telephone, dto.Bin, dto.Source))
                            .On(x => new {x.Bin, x.Telephone}).RunAsync();
                    if (dto.Email != null)
                        await webContactContext.ContactEmails.UpsertRange(AddEmail(dto.Email, dto.Bin, dto.Source))
                            .On(x => new {x.Bin, x.Email}).RunAsync();
                    if (dto.Website != null)
                        await webContactContext.ContactWebsites
                            .UpsertRange(AddWebsite(dto.Website, dto.Bin, dto.Source))
                            .On(x => new {x.Bin, x.Website}).RunAsync();
                    lock (_lock)
                    {
                        Logger.Trace($"Left {--_total}");
                    }
                }
            }
        }

        private static List<ContactTelephone> AddTel(string telephone, long? bin, string source)
        {
            var contactTelephones = new List<ContactTelephone>();
            var telephones = ValidateTel(telephone);
            foreach (var tel in telephones)
            {
                var contactTelephone = new ContactTelephone
                {
                    Bin = bin,
                    Telephone = tel,
                    Source = source,
                    RelevanceDate = DateTime.Now
                };
                contactTelephones.Add(contactTelephone);
            }

            return contactTelephones;
        }

        private static List<ContactEmail> AddEmail(string email, long? bin, string source)
        {
            var contactEmails = new List<ContactEmail>();
            var emails = ValidateEmail(email.ToLower());
            foreach (var mail in emails)
            {
                var contactEmail = new ContactEmail
                {
                    Bin = bin,
                    Email = mail,
                    Source = source,
                    RelevanceDate = DateTime.Now
                };
                contactEmails.Add(contactEmail);
            }

            return contactEmails;
        }

        private static List<ContactWebsite> AddWebsite(string website, long? bin, string source)
        {
            var contactWebsites = new List<ContactWebsite>();
            var websites = ValidateWebsite(website.ToLower());
            foreach (var web in websites)
            {
                var contactWebsite = new ContactWebsite
                {
                    Bin = bin,
                    Website = web,
                    Source = source,
                    RelevanceDate = DateTime.Now
                };
                contactWebsites.Add(contactWebsite);
            }

            return contactWebsites;
        }

        private static List<string> ValidateTel(string telephone)
        {
            var hashSet = new HashSet<string>();
            if (telephone.Contains(',') || telephone.Contains(';'))
            {
                var b = telephone.Split(',', ';').ToList();
                b = b.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

                foreach (var word in b)
                {
                    if (OnlyNum(word) != null)
                    {
                        hashSet.Add(OnlyNum(word.Trim()));
                    }
                }
            }
            else
            {
                if (OnlyNum(telephone) != null && OnlyNum(telephone).Length < 13)
                {
                    hashSet.Add(OnlyNum(telephone.Trim()));
                }
                else
                {
                    if (OnlyNum(telephone) != null)
                        hashSet.Add(telephone.Trim());
                }
            }

            return hashSet.ToList();
        }

        private static string OnlyNum(string a)
        {
            a = a.Replace("+7", "8");
            a = Regex.Replace(a, @"[^\d]", string.Empty);
            if (a.Length == 10 && a[0] == '7')
            {
                a = "8" + a;
            }

            if (a.Length > 11 && a[0] == '8' || a.Length > 11 && a[0] == '7') //update
            {
                a = a.Substring(0, 11);
            }

            if (a.Length != 11)
            {
                return null;
            }

            return a;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static List<string> ValidateEmail(string email)
        {
            var hashSet = new HashSet<string>();
            if (email.Contains(',') || email.Contains(';') || email.Contains(' '))
            {
                var b = email.Split(',', ';', ' ').ToList();
                b = b.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

                foreach (var word in b)
                {
                    if (IsValidEmail(word))
                    {
                        hashSet.Add(word.Trim());
                    }
                }
            }
            else
            {
                if (IsValidEmail(email))
                {
                    hashSet.Add(email.Trim());
                }
            }

            return hashSet.ToList();
        }

        private static Boolean IsValidWebsite(string web)
        {
            if (web.Contains('@'))
            {
                return false;
            }

            Regex regex = new Regex(
                @"((http|ftp|https|www)://)?([\d\w-.]+?\.(a[cdefgilmnoqrstuwz]|b[abdefghijmnorstvwyz]|c[acdfghiklmnoruvxyz]|d[ejkmnoz]|e[ceghrst]|f[ijkmnor]|g[abdefghilmnpqrstuwy]|h[kmnrtu]|i[delmnoqrst]|j[emop]|k[eghimnprwyz]|l[abcikrstuvy]|m[acdghklmnopqrstuvwxyz]|n[acefgilopruz]|om|p[aefghklmnrstwy]|qa|r[eouw]|s[abcdeghijklmnortuvyz]|t[cdfghjkmnoprtvwz]|u[augkmsyz]|v[aceginu]|w[fs]|y[etu]|z[amw]|aero|arpa|biz|com|coop|edu|info|int|gov|mil|museum|name|net|org|pro)(\b|\W(?<!&|=)(?!\.\s|\.{3}).*?))(\s|$)");
            var check = regex.IsMatch(web);
            return check;
        }

        private static List<string> ValidateWebsite(string a)
        {
            HashSet<string> hashSet = new HashSet<string>();
            if (a.Contains(',') || a.Contains(';') || a.Contains(' '))
            {
                var b = a.Split(',', ';', ' ').ToList();
                b = b.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

                foreach (var word in b)
                {
                    if (IsValidWebsite(word))
                    {
                        hashSet.Add(BeautifyWebsite(word.Trim()));
                    }
                }
            }
            else
            {
                if (IsValidWebsite(a))
                {
                    hashSet.Add(BeautifyWebsite(a.Trim()));
                }
            }

            return hashSet.ToList();
        }

        private static string BeautifyWebsite(string website)
        {
            website = website.Replace("http://", string.Empty).Replace("https://", string.Empty);
            website = website.TrimStart('/').TrimEnd('/');
            if (!website.Contains("www") && !website.Contains('@'))
            {
                website = "www." + website;
            }

            return website;
        }
    }
}