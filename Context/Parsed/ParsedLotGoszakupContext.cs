﻿using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{

    /// @author Yevgeniy Cherdantsev
    /// @date 19.02.2020 19:49:36
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'lots_goszakup'
    /// </summary>

    public class ParsedLotGoszakupContext : DbContext
    {
        
        public DbSet<LotGoszakupDto> LotGoszakupDtos { get; set; }    
        public ParsedLotGoszakupContext(DbContextOptions<ParsedLotGoszakupContext> options)
            : base(options)
        {

        }

        public ParsedLotGoszakupContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = '192.168.2.37'; Database = 'development'; Port='5432'; User ID = 'administrator'; Password = 'administrator'; Search Path = 'evgeniy'; Integrated Security=true; Pooling=true;");
        }
    }
}