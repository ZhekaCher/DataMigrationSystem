using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class KazpatentMigrationService : MigrationService
    {
        private int _total;

        private readonly object _lock = new object();

        public KazpatentMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
        }
        public override async Task StartMigratingAsync()
        {
            await using var webKazpatentContext = new WebKazpatentContext();
            await using var parsedKazpatentContext = new ParsedKazpatentContext();
            _total = parsedKazpatentContext.KazpatentDtos.Count();
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            await Migrate();

            await parsedKazpatentContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.kazpatents restart identity cascade;");
            await parsedKazpatentContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.kazpatent_iins restart identity cascade;");
        }

        private async Task Migrate()
        {
            Logger.Info("started thread");
            await using var parsedKazpatentContext = new ParsedKazpatentContext();
            var kazpatentDtos = parsedKazpatentContext.KazpatentDtos.AsNoTracking()
                .Include(x => x.KazPatentOwnerDtos);
            var tasks = new List<Task>();
            foreach (var dto in kazpatentDtos)
            {
                tasks.Add(Insert(dto));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task Insert(KazpatentDto dto)
        {
            await using var webKazpatentContext = new WebKazpatentContext();
            webKazpatentContext.ChangeTracker.AutoDetectChangesEnabled = false;
            var kazpatent = await DtoToWeb(dto);
            var found = webKazpatentContext.Kazpatents.Include(x=>x.KazPatentOwners)
                .FirstOrDefault(x =>
                    x.CertificateNumber == kazpatent.CertificateNumber && x.RegDate == kazpatent.RegDate && x.ReceiptDate== kazpatent.ReceiptDate);
            try
            {
                if (found!=null)
                {
                    await webKazpatentContext.Kazpatents.Upsert(kazpatent).On(x =>new {x.CertificateNumber,x.RegDate,x.ReceiptDate}).UpdateIf((x,y)=>x.Status != y.Status).RunAsync();
                }
                else
                {
                    await webKazpatentContext.Kazpatents.AddAsync(kazpatent);
                    await webKazpatentContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            lock (_lock)
                Logger.Trace($"Left {--_total}");
        }

        private static async Task<Kazpatent> DtoToWeb(KazpatentDto dto)
        {
            var kazpatent = new Kazpatent
            {
                CertificateNumber = dto.CertificateNumber,
                RegDate = dto.RegDate,
                ReceiptDate = dto.ReceiptDate,
                FullName = dto.FullName,
                PatentType = dto.PatentType,
                PatentName = dto.PatentName,
                CreateDate = dto.CreateDate,
                Status = dto.Status,
                RelevanceDate = dto.RelevanceDate
            };
            
            if (dto.KazPatentOwnerDtos != null)
            {
                kazpatent.KazPatentOwners = new List<KazPatentOwner>();
                foreach (var ownerDto in dto.KazPatentOwnerDtos)
                {
                    var owner = new KazPatentOwner
                    {
                        Iin = ownerDto.Iin,
                        PatentId = kazpatent.Id
                    };
                    kazpatent.KazPatentOwners.Add(owner);
                }
            }
            return kazpatent;
        }
    }
}